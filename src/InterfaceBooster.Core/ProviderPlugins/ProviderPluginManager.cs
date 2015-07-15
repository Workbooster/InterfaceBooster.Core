using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Core.ProviderPlugins.Information;
using InterfaceBooster.Common.Interfaces.ErrorHandling;
using InterfaceBooster.Common.Interfaces.InterfaceDefinition.Data;
using InterfaceBooster.Common.Interfaces.ProviderPlugin.Interfaces.Information.Data;
using InterfaceBooster.Common.Interfaces.ProviderPlugin.Interfaces;
using InterfaceBooster.Common.Interfaces.ProviderPlugin.Control;
using InterfaceBooster.ProviderPluginApi;
using InterfaceBooster.Database.Interfaces.Structure;
using InterfaceBooster.Database.Interfaces;
using InterfaceBooster.ProviderPluginApi.Data;
using InterfaceBooster.ProviderPluginApi.Service;
using InterfaceBooster.Core.ProviderPlugins.Communication;
using InterfaceBooster.ProviderPluginApi.Communication;
using InterfaceBooster.Common.Tools.Data.Array;

namespace InterfaceBooster.Core.ProviderPlugins
{
    /// <summary>
    /// handels the loading of provider plugins and the communication with them
    /// </summary>
    public class ProviderPluginManager : IProviderPluginManager
    {
        #region MEMBERS

        private Dictionary<IProviderPluginData, IProviderPlugin> _AvailablePlugins = new Dictionary<IProviderPluginData, IProviderPlugin>();
        private Dictionary<ProviderPluginInstanceReference, IProviderPluginInstance> _ProviderPluginInstances = new Dictionary<ProviderPluginInstanceReference, IProviderPluginInstance>();
        private Dictionary<string[], IProviderConnection> _Connections = new Dictionary<string[], IProviderConnection>(new ArrayEqualityComparer<string>());

        #endregion

        #region PROPERTIES

        /// <summary>
        /// the absolute path to the main directory of the plugins
        /// </summary>
        public string PluginMainDirectoryPath { get; private set; }

        /// <summary>
        /// a list of all provider plugins already loaded
        /// </summary>
        public IReadOnlyDictionary<IProviderPluginData, IProviderPlugin> AvailablePlugins { get { return _AvailablePlugins; } }

        /// <summary>
        /// a list of all provider plugin instances already loaded
        /// </summary>
        public IReadOnlyDictionary<ProviderPluginInstanceReference, IProviderPluginInstance> ProviderPluginInstances { get { return _ProviderPluginInstances; } }

        /// <summary>
        /// a list of all open connections
        /// </summary>
        public IReadOnlyDictionary<string[], IProviderConnection> Connections { get { return _Connections; } }

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// handels the loading of provider plugins and the communication with them
        /// </summary>
        /// <param name="pluginMainDirectoryPath">the absolute path to the main directory of the plugins</param>
        public ProviderPluginManager(string pluginMainDirectoryPath)
        {
            PluginMainDirectoryPath = pluginMainDirectoryPath;
        }

        /// <summary>
        /// activate a provider plugin instance to be ready to open a connection using that instance
        /// </summary>
        /// <param name="reference"></param>
        public void Activate(ProviderPluginInstanceReference reference)
        {
            if (string.IsNullOrEmpty(PluginMainDirectoryPath))
            {
                throw new ProviderPluginManagerException(this, String.Format(
                    "A valid PluginMainDirectoryPath must be set before using the ProviderPluginManager. Path given: {0}",
                    PluginMainDirectoryPath));
            }

            IProviderPluginInstance instance = (from i in _ProviderPluginInstances
                                                where i.Key.IdPluginInstance == reference.IdPluginInstance
                                                select i.Value).FirstOrDefault();

            // If an instance was found the plugin instance has already been initialized. So nothing must be done.
            // Otherwise we have to find out whether the plugin already is loaded. If so we can just create the new instance.
            // If neither the plugin and the plugin instance are available we have to activate the plugin before we can create the instance.
            if (instance == null)
            {
                IProviderPlugin plugin = GetProviderPlugin(reference);

                if (plugin != null)
                {
                    // request a new ProviderPluginInstance from the plugin
                    instance = plugin.CreateProviderPluginInstance(reference.IdPluginInstance, new ProviderPluginHost());

                    if (instance != null)
                    {
                        _ProviderPluginInstances.Add(reference, instance);
                    }
                    else
                    {
                        throw new ProviderPluginManagerException(this, String.Format(
                            "Plugin activation for SyneryIdentifier='{4}' failed. The Plugin with Name='{0}' and Id='{1}' doesn't return the requested ProviderPluginInstance with Name='{2}' and Id='{3}'.",
                            reference.PluginName, reference.IdPlugin, reference.PluginInstanceName, reference.IdPluginInstance, reference.SyneryIdentifier));
                    }
                }
                else
                {
                    throw new ProviderPluginManagerException(this, String.Format(
                        "Plugin activation for SyneryIdentifier='{0}' failed. The requested Plugin with Name='{1}' and Id='{2}' was not found.",
                        reference.SyneryIdentifier, reference.PluginName, reference.IdPlugin));
                }
            }
        }

        /// <summary>
        /// Activates many provider plugin instance to be ready to open a connection using those instances
        /// </summary>
        /// <param name="references"></param>
        public void Activate(IList<ProviderPluginInstanceReference> references)
        {
            foreach (var item in references)
            {
                Activate(item);
            }
        }

        /// <summary>
        /// Runs a task and updates the tasks state. A task is used to communicate with a provider plugin (e.g. opening connections, reading and writing data).
        /// A data exchange task must contain a FullPath. The connection, endpoint and resource paths will be extracted from the FullPath.
        /// The tasks may contain sub-tasks. These tasks are also executed.
        /// Exceptions are not handled! They are just enriched with some information about the provider plugin manager.
        /// There is no return value. You can use the events of the task to track the execution.
        /// </summary>
        /// <param name="task">These types are currently supported: Connect, Read, Create, Update, Save, Delete, Execute</param>
        public void RunTask(ProviderPluginTask task)
        {
            task.SetNewState(ProviderPluginTaskStateEnum.Started);

            try
            {
                // handle the task execution by its type

                switch (task.Type)
                {
                    case ProviderPluginTaskTypeEnum.Connect:
                        RunConnectTask((ProviderPluginConnectTask)task);
                        break;
                    case ProviderPluginTaskTypeEnum.Read:
                        {
                            ProviderPluginReadTask dataTask = (ProviderPluginReadTask)task;
                            ExtractPaths(dataTask);
                            IProviderConnection connection = connection = GetConnection(dataTask);
                            IReadEndpoint endpoint = GetEndpoint<IReadEndpoint>(connection, dataTask);
                            ReadResource resource = endpoint.GetReadResource();

                            // prepare the request

                            ReadRequest request = PrepareReadRequest(dataTask, resource);
                            request.SubRequests = PrepareSubRequests(dataTask.NestedTasks, resource.SubResources);

                            // send the request

                            task.SetNewState(ProviderPluginTaskStateEnum.RequestSent);
                            ReadResponse response = endpoint.RunReadRequest(request);

                            // handle the response

                            task.SetNewState(ProviderPluginTaskStateEnum.ResponseReceived);
                            ImportRecordSetToDatabase(dataTask.Memory.Database, dataTask.TargetTableName, response.RecordSet, dataTask.FieldNames);
                            HandleSubResponses(task.NestedTasks, response.SubResponses);
                        }
                        break;
                    case ProviderPluginTaskTypeEnum.Create:
                        {
                            ProviderPluginCreateTask dataTask = (ProviderPluginCreateTask)task;
                            ExtractPaths(dataTask);
                            IProviderConnection connection = connection = GetConnection(dataTask);
                            ICreateEndpoint endpoint = GetEndpoint<ICreateEndpoint>(connection, dataTask);
                            CreateResource resource = endpoint.GetCreateResource();

                            // prepare the request

                            CreateRequest request = PrepareCreateRequest(dataTask, resource);
                            request.SubRequests = PrepareSubRequests(dataTask.NestedTasks, resource.SubResources);

                            // send the request

                            task.SetNewState(ProviderPluginTaskStateEnum.RequestSent);
                            CreateResponse response = endpoint.RunCreateRequest(request);

                            // handle the response

                            task.SetNewState(ProviderPluginTaskStateEnum.ResponseReceived);
                            HandleSubResponses(task.NestedTasks, response.SubResponses);
                        }
                        break;
                    case ProviderPluginTaskTypeEnum.Update:
                        {
                            ProviderPluginUpdateTask dataTask = (ProviderPluginUpdateTask)task;
                            ExtractPaths(dataTask);
                            IProviderConnection connection = connection = GetConnection(dataTask);
                            IUpdateEndpoint endpoint = GetEndpoint<IUpdateEndpoint>(connection, dataTask);
                            UpdateResource resource = endpoint.GetUpdateResource();

                            // prepare the request

                            UpdateRequest request = PrepareUpdateRequest(dataTask, resource);
                            request.SubRequests = PrepareSubRequests(dataTask.NestedTasks, resource.SubResources);

                            // send the request

                            task.SetNewState(ProviderPluginTaskStateEnum.RequestSent);
                            UpdateResponse response = endpoint.RunUpdateRequest(request);

                            // handle the response

                            task.SetNewState(ProviderPluginTaskStateEnum.ResponseReceived);
                            HandleSubResponses(task.NestedTasks, response.SubResponses);
                        }
                        break;
                    case ProviderPluginTaskTypeEnum.Save:
                        {
                            ProviderPluginSaveTask dataTask = (ProviderPluginSaveTask)task;
                            ExtractPaths(dataTask);
                            IProviderConnection connection = connection = GetConnection(dataTask);
                            ISaveEndpoint endpoint = GetEndpoint<ISaveEndpoint>(connection, dataTask);
                            SaveResource resource = endpoint.GetSaveResource();

                            // prepare the request

                            SaveRequest request = PrepareSaveRequest(dataTask, resource);
                            request.SubRequests = PrepareSubRequests(dataTask.NestedTasks, resource.SubResources);

                            // send the request

                            task.SetNewState(ProviderPluginTaskStateEnum.RequestSent);
                            SaveResponse response = endpoint.RunSaveRequest(request);

                            // handle the response

                            task.SetNewState(ProviderPluginTaskStateEnum.ResponseReceived);
                            HandleSubResponses(task.NestedTasks, response.SubResponses);
                        }
                        break;
                    case ProviderPluginTaskTypeEnum.Delete:
                        {
                            ProviderPluginDeleteTask dataTask = (ProviderPluginDeleteTask)task;
                            ExtractPaths(dataTask);
                            IProviderConnection connection = connection = GetConnection(dataTask);
                            IDeleteEndpoint endpoint = GetEndpoint<IDeleteEndpoint>(connection, dataTask);
                            DeleteResource resource = endpoint.GetDeleteResource();

                            // prepare the request

                            DeleteRequest request = PrepareDeleteRequest(dataTask, resource);
                            request.SubRequests = PrepareSubRequests(dataTask.NestedTasks, resource.SubResources);

                            // send the request

                            task.SetNewState(ProviderPluginTaskStateEnum.RequestSent);
                            DeleteResponse response = endpoint.RunDeleteRequest(request);

                            // handle the response

                            task.SetNewState(ProviderPluginTaskStateEnum.ResponseReceived);
                            HandleSubResponses(task.NestedTasks, response.SubResponses);
                        }
                        break;
                    case ProviderPluginTaskTypeEnum.Execute:
                        {
                            ProviderPluginExecuteTask dataTask = (ProviderPluginExecuteTask)task;
                            ExtractPaths(dataTask);
                            IProviderConnection connection = connection = GetConnection(dataTask);
                            IExecuteEndpoint endpoint = GetEndpoint<IExecuteEndpoint>(connection, dataTask);
                            ExecuteResource resource = endpoint.GetExecuteResource();

                            // prepare the request

                            ExecuteRequest request = PrepareExecuteRequest(dataTask, resource);
                            request.SubRequests = PrepareSubRequests(dataTask.NestedTasks, resource.SubResources);

                            // send the request

                            task.SetNewState(ProviderPluginTaskStateEnum.RequestSent);
                            ExecuteResponse response = endpoint.RunExecuteRequest(request);

                            // handle the response

                            task.SetNewState(ProviderPluginTaskStateEnum.ResponseReceived);
                            ImportGetValues(dataTask.GetValues, response.Values);
                            HandleSubResponses(task.NestedTasks, response.SubResponses);
                        }
                        break;
                    default:
                        throw new ProviderPluginManagerException(this, String.Format(
                            "The task type '{0}' is not supported.",
                            Enum.GetName(typeof(ProviderPluginTaskTypeEnum), task.Type)));
                }
            }
            catch (Exception)
            {
                // update the tasks state
                task.SetNewState(ProviderPluginTaskStateEnum.FinishedWithError);

                // re-throw the exception
                throw;
            }

            task.SetNewState(ProviderPluginTaskStateEnum.FinishedSuccessfully);
        }

        private void HandleSubResponses(IEnumerable<ProviderPluginTask> listOfNestedTasks, IEnumerable<Response> listOfNestedResponses)
        {
            if (listOfNestedResponses != null)
            {
                foreach (var task in listOfNestedTasks)
                {
                    switch (task.Type)
                    {
                        case ProviderPluginTaskTypeEnum.Connect:
                            break;
                        case ProviderPluginTaskTypeEnum.Read:
                            ProviderPluginReadTask readTask = (ProviderPluginReadTask)task;
                            ReadResponse readResponse = (from r in listOfNestedResponses
                                                         where r.RequestType == RequestTypeEnum.Read
                                                         && ((ReadResponse)r).Request.Resource.Name == readTask.ResourceName
                                                         select (ReadResponse)r).FirstOrDefault();

                            if (readResponse == null)
                                throw new ProviderPluginManagerException(this, String.Format("No read response from provider plugin endpoint '{0}'.", readTask.FullSyneryPath));

                            // TODO: check schema (compare task and response schema)

                            ImportRecordSetToDatabase(readTask.Memory.Database, readTask.TargetTableName, readResponse.RecordSet, readTask.FieldNames);

                            HandleSubResponses(task.NestedTasks, readResponse.SubResponses);
                            break;
                        case ProviderPluginTaskTypeEnum.Create:
                            ProviderPluginCreateTask createTask = (ProviderPluginCreateTask)task;

                            CreateResponse createResponse = (from r in listOfNestedResponses
                                                             where r.RequestType == RequestTypeEnum.Create
                                                             && ((CreateResponse)r).Request.Resource.Name == createTask.ResourceName
                                                             select (CreateResponse)r).FirstOrDefault();

                            if (createResponse == null)
                                throw new ProviderPluginManagerException(this, String.Format("No create response from provider plugin endpoint '{0}'.", createTask.FullSyneryPath));

                            HandleSubResponses(task.NestedTasks, createResponse.SubResponses);
                            break;
                        case ProviderPluginTaskTypeEnum.Update:
                            ProviderPluginUpdateTask updateTask = (ProviderPluginUpdateTask)task;

                            UpdateResponse updateResponse = (from r in listOfNestedResponses
                                                             where r.RequestType == RequestTypeEnum.Update
                                                             && ((UpdateResponse)r).Request.Resource.Name == updateTask.ResourceName
                                                             select (UpdateResponse)r).FirstOrDefault();

                            if (updateResponse == null)
                                throw new ProviderPluginManagerException(this, String.Format("No update response from provider plugin endpoint '{0}'.", updateTask.FullSyneryPath));

                            HandleSubResponses(task.NestedTasks, updateResponse.SubResponses);
                            break;
                        case ProviderPluginTaskTypeEnum.Save:
                            ProviderPluginSaveTask saveTask = (ProviderPluginSaveTask)task;

                            SaveResponse saveResponse = (from r in listOfNestedResponses
                                                         where r.RequestType == RequestTypeEnum.Save
                                                         && ((SaveResponse)r).Request.Resource.Name == saveTask.ResourceName
                                                         select (SaveResponse)r).FirstOrDefault();

                            if (saveResponse == null)
                                throw new ProviderPluginManagerException(this, String.Format("No save response from provider plugin endpoint '{0}'.", saveTask.FullSyneryPath));

                            HandleSubResponses(task.NestedTasks, saveResponse.SubResponses);
                            break;
                        case ProviderPluginTaskTypeEnum.Delete:
                            ProviderPluginDeleteTask deleteTask = (ProviderPluginDeleteTask)task;

                            DeleteResponse deleteResponse = (from r in listOfNestedResponses
                                                             where r.RequestType == RequestTypeEnum.Delete
                                                             && ((DeleteResponse)r).Request.Resource.Name == deleteTask.ResourceName
                                                             select (DeleteResponse)r).FirstOrDefault();

                            if (deleteResponse == null)
                                throw new ProviderPluginManagerException(this, String.Format("No delete response from provider plugin endpoint '{0}'.", deleteTask.FullSyneryPath));

                            HandleSubResponses(task.NestedTasks, deleteResponse.SubResponses);
                            break;
                        case ProviderPluginTaskTypeEnum.Execute:
                            ProviderPluginExecuteTask executeTask = (ProviderPluginExecuteTask)task;

                            ExecuteResponse executeResponse = (from r in listOfNestedResponses
                                                               where r.RequestType == RequestTypeEnum.Execute
                                                               && ((ExecuteResponse)r).Request.Resource.Name == executeTask.ResourceName
                                                               select (ExecuteResponse)r).FirstOrDefault();

                            if (executeResponse == null)
                                throw new ProviderPluginManagerException(this, String.Format("No execute response from provider plugin endpoint '{0}'.", executeTask.FullSyneryPath));

                            ImportGetValues(executeTask.GetValues, executeResponse.Values);
                            HandleSubResponses(task.NestedTasks, executeResponse.SubResponses);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        #endregion

        #region INTERNAL METHODS

        #region PLUGIN ACTIVATION

        /// <summary>
        /// activates the provider plugin and the instance given by the referene.
        /// uses the plugin.xml file of the plugin to find  the referenced provider plugin instance.
        /// </summary>
        /// <param name="reference"></param>
        /// <returns></returns>
        private IProviderPlugin GetProviderPlugin(ProviderPluginInstanceReference reference)
        {
            IProviderPlugin plugin = (from p in _AvailablePlugins
                                      where p.Key.Id == reference.IdPlugin
                                      select p.Value).FirstOrDefault();

            if (plugin == null)
            {

                string pluginDirectoryPath = Path.Combine(PluginMainDirectoryPath, reference.IdPlugin.ToString());

                if (Directory.Exists(pluginDirectoryPath))
                {
                    string pluginXmlFilePath = Path.Combine(pluginDirectoryPath, "plugin.xml");
                    IProviderPluginData pluginData;

                    try
                    {
                        // load plugin.xml
                        pluginData = ProviderPluginDataController.Load(pluginXmlFilePath);
                    }
                    catch (Exception ex)
                    {
                        throw new ProviderPluginManagerException(this, String.Format(
                            "Error while loading the plugin.xml file from path='{0}'.", pluginXmlFilePath), ex);
                    }

                    // load assembly and create IProviderPlugin object
                    plugin = LoadProviderPlugin(reference, pluginData);

                    // add the loaded plugin to the list of available plugins
                    _AvailablePlugins.Add(pluginData, plugin);

                    return plugin;
                }
                else
                {
                    throw new ProviderPluginManagerException(this, String.Format(
                        "The requested plugin with Name='{0}' and Id='{1}' wasn't found. The plugin directory is missing: {2}",
                        reference.PluginName, reference.IdPlugin, pluginDirectoryPath));
                }
            }

            return plugin;
        }

        /// <summary>
        /// creates a provider plugin object by loading the external plugin assembly.
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="pluginData"></param>
        /// <returns></returns>
        private IProviderPlugin LoadProviderPlugin(ProviderPluginInstanceReference reference, IProviderPluginData pluginData)
        {

            IProviderPluginAssemblyData assemblyData = (from a in pluginData.Assemblies
                                                        where a.Instances.Where(i => i.Id == reference.IdPluginInstance).Count() > 0
                                                        select a).FirstOrDefault();


            if (assemblyData != null)
            {
                Assembly assembly;
                IProviderPluginInstanceData instanceData = (from i in assemblyData.Instances
                                                            where i.Id == reference.IdPluginInstance
                                                            select i).FirstOrDefault();

                // check for the correct interface version
                // if the versions don't match the ProviderPlugin bases on another interface .dll-file than provided by the current runtime
                Version availableInterfaceVersion = GetInterfaceAssemblyVersion();

                if (availableInterfaceVersion != assemblyData.RequiredInterfaceVersion)
                {
                    throw new ProviderPluginManagerException(this, String.Format(
                        "The RequiredInterfaceVersion='{0}' doesn't match the available interface version ({1}). Error while loading instance with name='{2}' and id='{3}'.",
                        assemblyData.RequiredInterfaceVersion, availableInterfaceVersion, instanceData.Name, instanceData.Id));
                }

                string assemblyFilePath = Path.Combine(pluginData.PluginDirectoryPath, assemblyData.Path);

                if (!File.Exists(assemblyFilePath))
                {
                    throw new ProviderPluginManagerException(this, String.Format(
                        "The given assembly of ProviderPlugin with name='{0}' and id='{1}' cannot be found. Assembly file path='{2}'.",
                        pluginData.Name, pluginData.Id, assemblyFilePath));
                }

                try
                {
                    // now we are ready to load the IProviderPlugin object from the foreign assembly

                    assembly = Assembly.LoadFrom(assemblyFilePath);
                }
                catch (Exception ex)
                {
                    // catch unexpected exceptions while loading the assembly to enrich the thrown exception with some additional information

                    throw new ProviderPluginManagerException(this, String.Format(
                        "An unexpected error occured while loading the ProviderPlugin with name='{0}' and id='{1}'. Assembly file path='{2}'.",
                        pluginData.Name, pluginData.Id, assemblyFilePath),
                        ex);
                }

                // try to find classes that implement the interface "IProviderPlugin"

                Type providerPluginInterfaceType = typeof(IProviderPlugin);
                IEnumerable<Type> providerPluginTypes = from t in assembly.GetTypes()
                                                        where providerPluginInterfaceType.IsAssignableFrom(t)
                                                        select t;

                if (providerPluginTypes.Count() == 1)
                {
                    // everything ok: activate the plugin
                    Type providerPluginType = providerPluginTypes.First();
                    IProviderPlugin plugin = (IProviderPlugin)Activator.CreateInstance(providerPluginType);

                    return plugin;
                }
                else if (providerPluginTypes.Count() > 1)
                {
                    throw new ProviderPluginManagerException(this, String.Format(
                        "The given assembly of ProviderPlugin with name='{0}' and id='{1}' has more than one class that implements the interface IProviderPlugin. Assembly file path='{2}'.",
                        pluginData.Name, pluginData.Id, assemblyFilePath));
                }
                else
                {
                    throw new ProviderPluginManagerException(this, String.Format(
                        "The given assembly of ProviderPlugin with name='{0}' and id='{1}' has has no class that implements the interface IProviderPlugin. Assembly file path='{2}'.",
                        pluginData.Name, pluginData.Id, assemblyFilePath));
                }
            }
            else
            {
                throw new ProviderPluginManagerException(this, String.Format(
                    "No assembly found that contains an ProviderPlugin instance with name='{0}' and id='{1}'",
                    reference.PluginInstanceName, reference.IdPluginInstance
                    ));
            }
        }

        private Version GetInterfaceAssemblyVersion()
        {
            Assembly assembly = Assembly.GetAssembly(typeof(IProviderPlugin));
            return assembly.GetName().Version;
        }

        #endregion

        #region QUESTIONS / ANSWERS

        /// <summary>
        /// translates some parameters into setting answers used to create a provider plugin connection
        /// </summary>
        /// <param name="listOfParameters"></param>
        /// <param name="pluginInstance"></param>
        /// <returns></returns>
        private AnswerList GetSettingAnswersFromParameters(IDictionary<string[], object> listOfParameters, IEnumerable<Question> listOfQuestions)
        {
            AnswerList listOfAnswers = new AnswerList();

            foreach (var item in listOfParameters)
            {
                string[] path = item.Key.Take(item.Key.Length - 1).ToArray();
                string name = item.Key.LastOrDefault();

                // try to find the question with the matching path and name
                Question question = (from q in listOfQuestions
                                     where q.Name == name
                                     && (((q.Path == null || q.Path.Length == 0) && path.Length == 0)
                                        || (q.Path != null && q.Path.SequenceEqual(path)))
                                     select q).FirstOrDefault();

                if (question != null)
                {
                    // check whether the requested type and the given type are equal or the value is NULL
                    if ((item.Value == null && question.IsRequired == false)
                        || item.Value.GetType() == question.ExpectedType)
                    {
                        Answer answer = new Answer(question, item.Value);

                        listOfAnswers.Add(answer);
                    }
                    // TODO: write warning to log
                }
                // TODO: write warning to log
            }

            return listOfAnswers;
        }

        /// <summary>
        /// Checks whether all required answers are available. Otherwise an exception containing the names of all missing answers is thrown.
        /// </summary>
        /// <param name="listOfQuestions"></param>
        /// <param name="listOfAnswers"></param>
        private void CheckRequiredAnswers(IEnumerable<Question> listOfQuestions, IEnumerable<Answer> listOfAnswers)
        {
            List<string> listOfMissingAnswers = new List<string>();

            if (listOfQuestions != null)
            {
                foreach (var question in listOfQuestions)
                {
                    if (question.IsRequired)
                    {
                        if (listOfAnswers.Count(a => a.Question == question) == 0)
                        {
                            if (question.Path != null && question.Path.Length > 0)
                            {
                                listOfMissingAnswers.Add(String.Format(
                                    "{0}.{1} ({2})",
                                    string.Join(".", question.Path),
                                    question.Name,
                                    question.ExpectedType.Name.ToUpper()));
                            }
                            else
                            {
                                listOfMissingAnswers.Add(String.Format(
                                    "{0} ({1})",
                                    question.Name,
                                    question.ExpectedType.Name.ToUpper()));
                            }
                        }
                    }
                }
            }

            if (listOfMissingAnswers.Count != 0)
            {
                throw new ProviderPluginManagerException(this, String.Format(
                    "Not all required set-parameters are available. Missing parameters: {0}",
                    String.Join(", ", listOfMissingAnswers)));
            }
        }

        #endregion

        #region CONNECTION TASK

        /// <summary>
        /// creates a connection to a provider plugin to be able to run other commands
        /// </summary>
        /// <param name="task"></param>
        private void RunConnectTask(ProviderPluginConnectTask task)
        {
            IEnumerable<IProviderPluginInstance> pluginInstances = from i in _ProviderPluginInstances
                                                                   where i.Key.SyneryIdentifier == task.InstanceReferenceSyneryIdentifier
                                                                   select i.Value;

            if (pluginInstances.Count() == 1)
            {
                IProviderPluginInstance pluginInstance = pluginInstances.First();

                // get the answers on the questions needed by the provider plugin instance
                IList<Answer> listOfAnswers = GetSettingAnswersFromParameters(task.Parameters, pluginInstance.ConnectionSettingQuestions);

                // check whether all required questions were answered
                CheckRequiredAnswers(pluginInstance.ConnectionSettingQuestions, listOfAnswers);

                ConnectionSettings settings = new ConnectionSettings(listOfAnswers);

                // connect
                IProviderConnection connection = pluginInstance.CreateProviderConnection(settings);

                _Connections.Add(task.ConnectionPath, connection);
            }
            else if (pluginInstances.Count() > 1)
            {
                throw new ProviderPluginManagerException(this, String.Format(
                    "Error while resolving plugin instance with name='{0}' because there were more than 1 instances with the same name.",
                    task.InstanceReferenceSyneryIdentifier));
            }
            else
            {
                throw new ProviderPluginManagerException(this, String.Format(
                    "Error while resolving plugin instance because there is no instance available with the name='{0}'.",
                    task.InstanceReferenceSyneryIdentifier));
            }
        }

        #endregion

        #region DATA EXCHANGE REQUEST PREPARATION

        private IList<IRequest> PrepareSubRequests(IEnumerable<ProviderPluginTask> listOfNestedTasks, IEnumerable<Resource> listOfNestedResources)
        {
            List<IRequest> listOfRequests = new List<IRequest>();

            if (listOfNestedResources != null)
            {
                foreach (var task in listOfNestedTasks)
                {
                    Request request = null;

                    switch (task.Type)
                    {
                        case ProviderPluginTaskTypeEnum.Connect:
                            break;
                        case ProviderPluginTaskTypeEnum.Read:
                            ProviderPluginReadTask readTask = (ProviderPluginReadTask)task;

                            // use the synery path as resource name
                            // because a sub ressource doesn't have a identifier path like primary ressources
                            readTask.ResourceName = readTask.FullSyneryPath;

                            ReadResource readResource = (from r in listOfNestedResources
                                                         where r.RequestType == RequestTypeEnum.Read
                                                         && r.Name == readTask.ResourceName
                                                         select (ReadResource)r).FirstOrDefault();

                            if (readResource == null)
                                throw new ProviderPluginManagerException(this, String.Format(
                                    "A read subresource with name '{0}' wasn't found.", readTask.ResourceName));

                            request = PrepareReadRequest(readTask, readResource);
                            request.SubRequests = PrepareSubRequests(readTask.NestedTasks, readResource.SubResources);
                            break;
                        case ProviderPluginTaskTypeEnum.Create:
                            ProviderPluginCreateTask createTask = (ProviderPluginCreateTask)task;

                            // use the synery path as resource name
                            // because a sub ressource doesn't have a identifier path like primary ressources
                            createTask.ResourceName = createTask.FullSyneryPath;

                            CreateResource createResource = (from r in listOfNestedResources
                                                             where r.RequestType == RequestTypeEnum.Create
                                                             && r.Name == createTask.ResourceName
                                                             select (CreateResource)r).FirstOrDefault();

                            if (createResource == null)
                                throw new ProviderPluginManagerException(this, String.Format(
                                    "A create subresource with name '{0}' wasn't found.", createTask.ResourceName));

                            request = PrepareCreateRequest(createTask, createResource);
                            request.SubRequests = PrepareSubRequests(createTask.NestedTasks, createResource.SubResources);
                            break;
                        case ProviderPluginTaskTypeEnum.Update:
                            ProviderPluginUpdateTask updateTask = (ProviderPluginUpdateTask)task;

                            // use the synery path as resource name
                            // because a sub ressource doesn't have a identifier path like primary ressources
                            updateTask.ResourceName = updateTask.FullSyneryPath;

                            UpdateResource updateResource = (from r in listOfNestedResources
                                                             where r.RequestType == RequestTypeEnum.Update
                                                             && r.Name == updateTask.ResourceName
                                                             select (UpdateResource)r).FirstOrDefault();

                            if (updateResource == null)
                                throw new ProviderPluginManagerException(this, String.Format(
                                    "An update subresource with name '{0}' wasn't found.", updateTask.ResourceName));

                            request = PrepareUpdateRequest(updateTask, updateResource);
                            request.SubRequests = PrepareSubRequests(updateTask.NestedTasks, updateResource.SubResources);
                            break;
                        case ProviderPluginTaskTypeEnum.Save:
                            ProviderPluginSaveTask saveTask = (ProviderPluginSaveTask)task;

                            // use the synery path as resource name
                            // because a sub ressource doesn't have a identifier path like primary ressources
                            saveTask.ResourceName = saveTask.FullSyneryPath;

                            SaveResource saveResource = (from r in listOfNestedResources
                                                         where r.RequestType == RequestTypeEnum.Save
                                                         && r.Name == saveTask.ResourceName
                                                         select (SaveResource)r).FirstOrDefault();

                            if (saveResource == null)
                                throw new ProviderPluginManagerException(this, String.Format(
                                    "A save subresource with name '{0}' wasn't found.", saveTask.ResourceName));

                            request = PrepareSaveRequest(saveTask, saveResource);
                            request.SubRequests = PrepareSubRequests(saveTask.NestedTasks, saveResource.SubResources);
                            break;
                        case ProviderPluginTaskTypeEnum.Delete:
                            ProviderPluginDeleteTask deleteTask = (ProviderPluginDeleteTask)task;

                            // use the synery path as resource name
                            // because a sub ressource doesn't have a identifier path like primary ressources
                            deleteTask.ResourceName = deleteTask.FullSyneryPath;

                            DeleteResource deleteResource = (from r in listOfNestedResources
                                                             where r.RequestType == RequestTypeEnum.Delete
                                                             && r.Name == deleteTask.ResourceName
                                                             select (DeleteResource)r).FirstOrDefault();

                            if (deleteResource == null)
                                throw new ProviderPluginManagerException(this, String.Format(
                                    "A delete subresource with name '{0}' wasn't found.", deleteTask.ResourceName));

                            request = PrepareDeleteRequest(deleteTask, deleteResource);
                            request.SubRequests = PrepareSubRequests(deleteTask.NestedTasks, deleteResource.SubResources);
                            break;
                        case ProviderPluginTaskTypeEnum.Execute:
                            ProviderPluginExecuteTask executeTask = (ProviderPluginExecuteTask)task;

                            // use the synery path as resource name
                            // because a sub ressource doesn't have a identifier path like primary ressources
                            executeTask.ResourceName = executeTask.FullSyneryPath;

                            ExecuteResource executeResource = (from r in listOfNestedResources
                                                               where r.RequestType == RequestTypeEnum.Execute
                                                               && r.Name == executeTask.ResourceName
                                                               select (ExecuteResource)r).FirstOrDefault();

                            if (executeResource == null)
                                throw new ProviderPluginManagerException(this, String.Format(
                                    "An execute subresource with name '{0}' wasn't found.", executeTask.ResourceName));

                            request = PrepareExecuteRequest(executeTask, executeResource);
                            request.SubRequests = PrepareSubRequests(executeTask.NestedTasks, executeResource.SubResources);
                            break;
                        default:
                            throw new ProviderPluginManagerException(this, String.Format(
                                "The task type '{0}' is not supported.",
                                Enum.GetName(typeof(ProviderPluginTaskTypeEnum), task.Type)));
                    }

                    listOfRequests.Add(request);
                }
            }

            return listOfRequests;
        }

        private ReadRequest PrepareReadRequest(ProviderPluginReadTask task, ReadResource resource)
        {
            ReadRequest request = null;

            // get the answers on the questions asked by the provider plugin by converting the set-parameters
            AnswerList listOfAnswers = GetSettingAnswersFromParameters(task.Parameters, resource.Questions);

            // check whether all required questions were answered
            CheckRequiredAnswers(resource.Questions, listOfAnswers);

            IEnumerable<Field> listOfRequestedFields;

            if (resource.Schema == null || resource.Schema.Fields == null || resource.Schema.Fields.Count == 0)
            {
                // the resource doesn't provide a schema
                // create an empty list
                listOfRequestedFields = new List<Field>();

                if (task.FieldNames.Count > 0)
                    throw new ProviderPluginManagerException(this, String.Format(
                        "Cannot request fields in read request. The resource at '{0}' doesn't provide a schema.", task.FullSyneryPath));
            }
            else
            {
                listOfRequestedFields = GetListOfRequestedFields(resource.Schema, task.FieldNames);
            }

            request = new ReadRequest();
            request.Resource = resource;
            request.Answers = listOfAnswers;
            request.RequestedFields = listOfRequestedFields;

            // TODO: Add Filters

            return request;
        }

        private CreateRequest PrepareCreateRequest(ProviderPluginCreateTask task, CreateResource resource)
        {
            // get the answers on the questions asked by the provider plugin by converting the set-parameters
            AnswerList listOfAnswers = GetSettingAnswersFromParameters(task.Parameters, resource.Questions);

            // check whether all required questions were answered
            CheckRequiredAnswers(resource.Questions, listOfAnswers);

            CreateRequest request = new CreateRequest();
            request.Resource = resource;
            request.Answers = listOfAnswers;
            request.RecordSet = PrepareRecordSetForExport(task.Memory.Database, task.SourceTableName, resource.Schema);

            return request;
        }

        private UpdateRequest PrepareUpdateRequest(ProviderPluginUpdateTask task, UpdateResource resource)
        {
            // get the answers on the questions asked by the provider plugin by converting the set-parameters
            AnswerList listOfAnswers = GetSettingAnswersFromParameters(task.Parameters, resource.Questions);

            // check whether all required questions were answered
            CheckRequiredAnswers(resource.Questions, listOfAnswers);

            UpdateRequest request = new UpdateRequest();
            request.Resource = resource;
            request.Answers = listOfAnswers;
            request.RecordSet = PrepareRecordSetForExport(task.Memory.Database, task.SourceTableName, resource.Schema);

            // TODO: Add Filters

            return request;
        }

        private SaveRequest PrepareSaveRequest(ProviderPluginSaveTask task, SaveResource resource)
        {
            // get the answers on the questions asked by the provider plugin by converting the set-parameters
            AnswerList listOfAnswers = GetSettingAnswersFromParameters(task.Parameters, resource.Questions);

            // check whether all required questions were answered
            CheckRequiredAnswers(resource.Questions, listOfAnswers);

            SaveRequest request = new SaveRequest();
            request.Resource = resource;
            request.Answers = listOfAnswers;
            request.RecordSet = PrepareRecordSetForExport(task.Memory.Database, task.SourceTableName, resource.Schema);

            return request;
        }

        private DeleteRequest PrepareDeleteRequest(ProviderPluginDeleteTask task, DeleteResource resource)
        {
            // get the answers on the questions asked by the provider plugin by converting the set-parameters
            AnswerList listOfAnswers = GetSettingAnswersFromParameters(task.Parameters, resource.Questions);

            // check whether all required questions were answered
            CheckRequiredAnswers(resource.Questions, listOfAnswers);

            DeleteRequest request = new DeleteRequest();
            request.Resource = resource;
            request.Answers = listOfAnswers;
            request.RecordSet = PrepareRecordSetForExport(task.Memory.Database, task.SourceTableName, resource.Schema);

            // TODO: Add Filters

            return request;
        }

        private ExecuteRequest PrepareExecuteRequest(ProviderPluginExecuteTask task, ExecuteResource resource)
        {
            // get the answers on the questions asked by the provider plugin by converting the set-parameters
            AnswerList listOfAnswers = GetSettingAnswersFromParameters(task.Parameters, resource.Questions);

            // check whether all required questions were answered
            CheckRequiredAnswers(resource.Questions, listOfAnswers);

            ExecuteRequest request = new ExecuteRequest();
            request.Resource = resource;
            request.Answers = listOfAnswers;
            request.RequestedValues = task.GetValues.Select(v => v.ProviderPluginValueName);

            return request;
        }

        #endregion

        #region REQUEST HELPERS

        /// <summary>
        /// Takes the FullPath of the given task and splits it up into a ConnectionPath, EndpointPath and EndpointName.
        /// This information is stored in the corresponding properties of the given task.
        /// </summary>
        /// <param name="task"></param>
        private void ExtractPaths(ProviderPluginDataExchangeTask task)
        {
            /* 
             * The following exmaple shows the pattern how the full path for addressing an endpoint is split up:
             * 
             * Example (full Synery path):
             *      \\someCategory\myConnection\firstSubPath\secondSubPath\endpointName
             *      
             * Example (C# string array):
             *      FullPath = new string[] { "someCategory", "myConnection", "fistSubPath", "secondSubPath", "endpointName" };
             *      ConnectionPath = new string[] { "someCategory", "myConnection" };
             *      EndpointPath = new string[] { "fistSubPath", "secondSubPath" };
             *      EndpointName = "endpointName";
             */


            // extract the connection path from the hole source path
            // the source path can also contain parts of a group's address
            task.ConnectionPath = GetConnectionPath(task.FullPath);

            if (task.ConnectionPath != null)
            {
                int fullPathLenght = task.FullPath.Length;
                int connectionPathLenght = task.ConnectionPath.Length;
                int endpointPathLenght = task.FullPath.Length - task.ConnectionPath.Length - 1;

                // extract the path of the provider plugin endpoint by removing the connection path
                task.EndpointPath = task.FullPath.Skip(task.ConnectionPath.Length).Take(endpointPathLenght).ToArray();
                task.EndpointName = task.FullPath.LastOrDefault();
            }
        }

        /// <summary>
        /// Tries to find out which part of a source path is the path of the connection.
        /// This is done by testing whether a connection with some parts of the source path exists.
        /// If no existing connection was found this method returns null.
        /// </summary>
        /// <param name="sourcePath">the full path</param>
        /// <returns>the connection path or null</returns>
        private string[] GetConnectionPath(string[] sourcePath)
        {
            string[] currentPath = sourcePath;
            IProviderConnection connection;

            // use a string-array equality comparer
            ArrayEqualityComparer<string> equalityComparer = new ArrayEqualityComparer<string>();

            // try to find a connection by removing the last item of the path with each iteration
            while (currentPath.Length > 0)
            {
                // search for the connection with the current path
                connection = (from c in _Connections
                              where equalityComparer.Equals(c.Key, currentPath)
                              select c.Value).FirstOrDefault();

                if (connection != null)
                {
                    return currentPath;
                }

                // remove the last item of the path
                currentPath = currentPath.Take(currentPath.Length - 1).ToArray();
            }

            return null;
        }

        /// <summary>
        /// Tries to find the connection with the given path. Throws an exception if path is invalid or no connection was found.
        /// </summary>
        /// <param name="connectionPath"></param>
        /// <returns></returns>
        private IProviderConnection GetConnection(ProviderPluginDataExchangeTask task)
        {
            try
            {
                if (task.ConnectionPath == null || task.ConnectionPath.Length == 0)
                    throw new ProviderPluginManagerException(this, "Provider Plugin connection path is null or empty");

                // resolve connection
                IProviderConnection connection = _Connections[task.ConnectionPath];

                if (connection != null)
                {
                    return connection;
                }
                else
                {
                    throw new ProviderPluginManagerException(this, String.Format(
                        "No Provider Plugin connection with Path='{0}' found. {1} connections initialized.",
                        task.ConnectionPath,
                        _Connections.Count));
                }
            }
            catch (Exception ex)
            {
                throw new ProviderPluginManagerException(this, String.Format(
                    "Error while loading the Provider Plugin connection using path='{0}' (mode='{1}').",
                    task.FullSyneryPath,
                    Enum.GetName(typeof(ProviderPluginTaskTypeEnum), task.Type)), ex);
            }
        }

        /// <summary>
        /// Gets an endpoint of the given type <typeparamref name="T"/> with the given path and name (stored in <paramref name="task"/>) from the given <paramref name="connection"/>.
        /// Throws an exception if no endpoint was found.
        /// </summary>
        /// <typeparam name="T">The expected endpoint type (read, update, create etc.)</typeparam>
        /// <param name="connection">The connection that should contain the expected endpoint.</param>
        /// <param name="task">The task contains the search criteria (EndpointName and EndpointPath) for getting the endpoint.</param>
        /// <exception cref="ProviderPluginManagerException"></exception>
        /// <returns>The first endpoint that matches the given criteria.</returns>
        private T GetEndpoint<T>(IProviderConnection connection, ProviderPluginDataExchangeTask task) where T : IEndpoint
        {
            if (connection == null)
                throw new ProviderPluginManagerException(this, "No connection given");
            if (connection.Endpoints == null)
                throw new ProviderPluginManagerException(this, "The Provider Plugin connection has no Endpoints specified.");

            T endpoint = (T)(from e in connection.Endpoints
                             where e is T
                             && e.Name == task.EndpointName
                             && (
                                 // check whether the path is empty on both sides:
                                ((task.EndpointPath == null || task.EndpointPath.Count() == 0) && (e.Path == null || e.Path.Length == 0))
                                 // otherwise compare the paths:
                                 || ArrayEqualityComparer.Equals(e.Path, task.EndpointPath)
                                 )
                             select e).FirstOrDefault();

            if (endpoint == null)
                throw new ProviderPluginManagerException(this,
                    String.Format("No endpoint found in '{0}'.", task.FullSyneryPath));

            return endpoint;
        }

        /// <summary>
        /// prepares the shema by removing fields which are not requested
        /// </summary>
        /// <param name="schema">full schema</param>
        /// <param name="listOfRequestFieldNames">requested fields</param>
        /// <returns>filtered list of fields</returns>
        private IEnumerable<Field> GetListOfRequestedFields(Schema schema, IEnumerable<string> listOfRequestFieldNames)
        {
            IList<Field> listOfRequestedFields;

            if (listOfRequestFieldNames == null || listOfRequestFieldNames.Count() == 0)
            {
                // no fields in list: request all fields
                listOfRequestedFields = schema.Fields;
            }
            else
            {
                listOfRequestedFields = new List<Field>();

                foreach (var name in listOfRequestFieldNames)
                {
                    Field currentField = schema.Fields.Where(f => f.Name == name).FirstOrDefault();

                    if (currentField != null)
                    {
                        listOfRequestedFields.Add(currentField);
                    }
                    else
                    {
                        throw new ProviderPluginManagerException(this, String.Format(
                            "Field with name='{0}' not found", name));
                    }
                }
            }

            return listOfRequestedFields;
        }

        private RecordSet PrepareRecordSetForExport(IDatabase database, string sourceTableName, Schema destinationSchema)
        {
            // check whether the export table exists
            if (database.IsTable(sourceTableName) == false)
                throw new ProviderPluginManagerException(this, String.Format(
                    "A table with the name '{0}' doesn't exists.",
                    sourceTableName));

            ITable tableForRequest = database.LoadTable(sourceTableName);

            IEnumerable<Field> fields = GetListOfRequestedFields(destinationSchema, tableForRequest.Schema.Fields.Select(f => f.Name));
            Schema structureSchemaForRequest = new Schema(fields);
            structureSchemaForRequest.InternalName = destinationSchema.InternalName;
            structureSchemaForRequest.Description = destinationSchema.Description;

            RecordSet recordSet = new RecordSet(structureSchemaForRequest, tableForRequest);

            return recordSet;
        }

        #endregion

        #region RESPONSE HELPERS

        /// <summary>
        /// imports a record set from a provider plugin into a database table
        /// </summary>
        /// <param name="database"></param>
        /// <param name="tableName"></param>
        /// <param name="recordSet"></param>
        /// <param name="fieldNames"></param>
        private void ImportRecordSetToDatabase(IDatabase database, string tableName, RecordSet recordSet, IList<string> fieldNames)
        {
            if (String.IsNullOrEmpty(tableName))
                return; // no table to import

            if (recordSet == null)
                throw new ProviderPluginManagerException(this, String.Format(
                    "The Provider Plugin didn't return a record set that can be stored to the table '{0}'.",
                    tableName));

            if (recordSet.Schema == null)
                throw new ProviderPluginManagerException(this,
                    "The record set returned by the Provider Plugin doesn't contain a schema");

            // create new schema and add all fields
            ISchema schema = database.NewSchema();

            if (fieldNames.Count > 0)
            {
                // remove unselected fields

                Field[] availableFields = recordSet.Schema.Fields.ToArray();

                foreach (var field in availableFields)
                {
                    if (fieldNames.Contains(field.Name) == false)
                    {
                        recordSet.RemoveField(field);
                    }
                }
            }

            foreach (var pluginField in recordSet.Schema.Fields)
            {
                schema.AddField(pluginField.Name, pluginField.Type);
            }

            // create the table from the record set data
            ITable table = database.NewTable(schema, recordSet.Select(r => (object[])r));

            // update the databse
            database.CreateOrUpdateTable(tableName, table);
        }

        /// <summary>
        /// Extracts the responded values and assigns it to the corresponding GetValue-object from <paramref name="listOfRequestedValues"/>.
        /// </summary>
        /// <param name="listOfRequestedValues"></param>
        /// <param name="dictionaryOfRespondedValues"></param>
        private void ImportGetValues(IEnumerable<ProviderPluginGetValue> listOfRequestedValues, IDictionary<string, object> dictionaryOfRespondedValues)
        {
            foreach (var requestedValue in listOfRequestedValues)
            {
                // search for the responded value and assign it to the GetValue-object
                requestedValue.Value = (from r in dictionaryOfRespondedValues
                                        where r.Key == requestedValue.ProviderPluginValueName
                                        select r.Value).FirstOrDefault();
            }
        }

        #endregion

        #endregion
    }
}
