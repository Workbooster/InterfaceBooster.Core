using InterfaceBooster.Database.Core;
using InterfaceBooster.Database.Interfaces;
using InterfaceBooster.Database.Interfaces.ErrorHandling;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Core.InterfaceDefinitions;
using InterfaceBooster.Core.LibraryPlugins;
using InterfaceBooster.Core.ProviderPlugins;
using InterfaceBooster.SyneryLanguage.Interpretation;
using InterfaceBooster.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.ErrorHandling;
using InterfaceBooster.Common.Interfaces.InterfaceDefinition.Data;
using InterfaceBooster.Common.Interfaces.LibraryPlugin;
using InterfaceBooster.Common.Interfaces.ProviderPlugin.Interfaces;
using InterfaceBooster.Common.Interfaces.SyneryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.Broadcasting;
using InterfaceBooster.Common.Interfaces.Execution.Model;
using InterfaceBooster.Common.Interfaces.Execution;

namespace InterfaceBooster.RuntimeController
{
    /// <summary>
    /// Handels the initionalization of an Import Definition and enables the application to execute one or multiple job(s).
    /// </summary>
    public class ExecutionManager : IExecutionManager, IDisposable
    {
        #region CONSTANTS

        public static readonly string INTERFACE_DEFINITION_XML_FILENAME = @"definition.xml";
        public static readonly string DEFAULT_SYNERY_CODE_DIRECTORY_RELATIVE_PATH = @"code";
        public static readonly string DEFAULT_SYNERY_DATABASE_DIRECTORY_RELATIVE_PATH = @"db";
        public static readonly string DEFAULT_PROVIDER_PLUGINS_DIRECTORY_RELATIVE_PATH = @"plugins\provider_plugins";
        public static readonly string DEFAULT_LIBRARY_PLUGINS_DIRECTORY_RELATIVE_PATH = @"plugins\library_plugins";

        #endregion

        #region MEMBERS

        private IBroadcaster _Broadcaster;
        private bool _IsInitialized;
        private InterfaceDefinitionData _InterfaceDefinitionData;
        private IDatabase _SyneryDB;
        private ISyneryMemory _SyneryMemory;
        private ISyneryClient<bool> _SyneryClient;

        #endregion

        #region PROPERTIES

        public ExecutionVariables EnvironmentVariables { get; set; }

        public bool IsInitialized { get { return _IsInitialized; } }

        public InterfaceDefinitionData InterfaceDefinitionData { get { return _InterfaceDefinitionData; } }

        public ISyneryMemory SyneryMemory { get { return _SyneryMemory; } }

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Handels the initionalization of an Import Definition and enables the application to execute one or multiple job(s).
        /// </summary>
        /// <param name="environmentVariables"></param>
        public ExecutionManager()
        {
            _IsInitialized = false;
        }

        /// <summary>
        /// Runs all Jobs from the current Interface Definition.
        /// </summary>
        /// <returns></returns>
        public ExecutionResult RunAllJobs()
        {
            if (IsInitialized == false)
                throw new Exception(String.Format("{0} must be initialized before running a job", this.GetType().Name));

            bool success;
            ExecutionResult result = PrepareRuntimeResult(false);

            foreach (var jobData in _InterfaceDefinitionData.Jobs)
            {
                success = RunJob(jobData);

                if (success == false) return result;
            }

            result.IsSuccess = true;

            return result;
        }

        /// <summary>
        /// Runs the Job with the given name.
        /// </summary>
        /// <param name="name">The full name (case insensitive) of an existing Job from the current Interface Definition.</param>
        /// <returns></returns>
        public ExecutionResult RunJob(string name)
        {
            if (IsInitialized == false)
                throw new Exception(String.Format("{0} must be initialized before running a job", this.GetType().Name));

            ExecutionResult result = PrepareRuntimeResult(false);

            InterfaceDefinitionJobData jobData = (from j in _InterfaceDefinitionData.Jobs
                                                  // compare case insensitive
                                                  where j.Name.ToLower() == name.ToLower()
                                                  select j).FirstOrDefault();

            if (jobData == null)
            {
                _Broadcaster.Error("The job with the name '{0}' wasn't found.", name);
                return result;
            }

            result.IsSuccess = RunJob(jobData);

            return result;
        }

        /// <summary>
        /// Runs the Job with the given GUID.
        /// </summary>
        /// <param name="guid">The GUID of an existing Job from the current Interface Definition.</param>
        /// <returns></returns>
        public ExecutionResult RunJob(Guid guid)
        {
            if (IsInitialized == false)
                throw new Exception(String.Format("{0} must be initialized before running a job", this.GetType().Name));

            ExecutionResult result = PrepareRuntimeResult(false);

            InterfaceDefinitionJobData jobData = (from j in _InterfaceDefinitionData.Jobs
                                                  where j.Id == guid
                                                  select j).FirstOrDefault();

            if (jobData == null)
            {
                _Broadcaster.Error("The job with the Id '{0}' wasn't found.", guid.ToString());
                return result;
            }

            result.IsSuccess = RunJob(jobData);

            return result;
        }

        /// <summary>
        /// Runs a single code file without include files.
        /// </summary>
        /// <param name="relativeFilePath">The relative file path starting from the code directory.</param>
        /// <returns></returns>
        public ExecutionResult RunSingleCodeFile(string relativeFilePath)
        {
            if (IsInitialized == false)
                throw new Exception(String.Format("{0} must be initialized before running a job", this.GetType().Name));

            ExecutionResult result = PrepareRuntimeResult(false);

            result.IsSuccess = RunCodeFileWithoutJob(relativeFilePath);

            return result;
        }

        /// <summary>
        /// Initialize the interface definition, the database, the plugin managers, the SyneryMemory and the SyneryInterpreter
        /// This method must be called before running a job.
        /// </summary>
        /// <param name="environmentVariables">At least Broadcaster and InterfaceDefinitionDirectoryPath are required!</param>
        /// <returns>true = success / false = error (see broadcasted messages for details)</returns>
        public bool Initialize(ExecutionVariables environmentVariables)
        {
            IProviderPluginManager providerPluginManager;
            ILibraryPluginManager libraryPluginManager;

            EnvironmentVariables = environmentVariables;

            // check required environment variables

            if (EnvironmentVariables == null)
                throw new ArgumentNullException("runtimeEnvironment", "The EnvironmentVariables are required.");

            if (EnvironmentVariables.Broadcaster == null)
                throw new ArgumentNullException("A Broadcaster is required");

            if (EnvironmentVariables.InterfaceDefinitionDirectoryPath == null)
                throw new ArgumentNullException("The InterfaceDefinitionDirectoryPath is required");

            // set defaults if no values given

            if (EnvironmentVariables.InterfaceDefinitionCodeDirectoryPath == null)
                EnvironmentVariables.InterfaceDefinitionCodeDirectoryPath = Path.Combine(EnvironmentVariables.InterfaceDefinitionDirectoryPath, DEFAULT_SYNERY_CODE_DIRECTORY_RELATIVE_PATH);

            if (EnvironmentVariables.DatabaseDirectoryPath == null)
                EnvironmentVariables.DatabaseDirectoryPath = Path.Combine(EnvironmentVariables.InterfaceDefinitionDirectoryPath, DEFAULT_SYNERY_DATABASE_DIRECTORY_RELATIVE_PATH);

            if (EnvironmentVariables.ProviderPluginDirectoryPath == null)
                EnvironmentVariables.ProviderPluginDirectoryPath = Path.Combine(EnvironmentVariables.InterfaceDefinitionDirectoryPath, DEFAULT_PROVIDER_PLUGINS_DIRECTORY_RELATIVE_PATH);

            if (EnvironmentVariables.LibraryPluginDirectoryPath == null)
                EnvironmentVariables.LibraryPluginDirectoryPath = Path.Combine(EnvironmentVariables.InterfaceDefinitionDirectoryPath, DEFAULT_LIBRARY_PLUGINS_DIRECTORY_RELATIVE_PATH);


            // create a local reference
            _Broadcaster = EnvironmentVariables.Broadcaster;

            // initialize the interface definition

            try
            {
                string interfaceDefinitionFilePath = Path.Combine(EnvironmentVariables.InterfaceDefinitionDirectoryPath, INTERFACE_DEFINITION_XML_FILENAME);

                _InterfaceDefinitionData = InterfaceDefinitionDataController.Load(interfaceDefinitionFilePath);
            }
            catch (Exception ex)
            {
                _Broadcaster.Error(ex);
                return false;
            }

            _Broadcaster.Info("Interface definition data successfully loaded.");

            // initialize the Synery Database

            try
            {
                if (Directory.Exists(EnvironmentVariables.DatabaseDirectoryPath) == false)
                {
                    Directory.CreateDirectory(EnvironmentVariables.DatabaseDirectoryPath);
                }

                _SyneryDB = new SyneryDB(EnvironmentVariables.DatabaseDirectoryPath);
            }
            catch (SyneryDBException ex)
            {
                _Broadcaster.Error(ex, "SyneryDB");
                return false;
            }
            catch (Exception ex)
            {
                _Broadcaster.Error(ex);
                return false;
            }

            // initialize the ProviderPluginManager

            try
            {
                providerPluginManager = new ProviderPluginManager(EnvironmentVariables.ProviderPluginDirectoryPath);

                // activate the provider plugin instance references from the interface definition
                providerPluginManager.Activate(_InterfaceDefinitionData.RequiredPlugins.ProviderPluginInstances);
            }
            catch (ProviderPluginManagerException ex)
            {
                _Broadcaster.Error(ex, "ProviderPluginManager");
                return false;
            }
            catch (Exception ex)
            {
                _Broadcaster.Error(ex);
                return false;
            }

            // initialize the LibraryPluginManager

            try
            {
                libraryPluginManager = new LibraryPluginManager(EnvironmentVariables.LibraryPluginDirectoryPath);

                // activate the provider plugin instance references from the interface definition
                libraryPluginManager.Activate(_InterfaceDefinitionData.RequiredPlugins.LibraryPlugins);
            }
            catch (LibraryPluginManagerException ex)
            {
                _Broadcaster.Error(ex, "LibraryPluginManager");
                return false;
            }
            catch (Exception ex)
            {
                _Broadcaster.Error(ex);
                return false;
            }

            // initialize the SyneryMemory

            _SyneryMemory = new SyneryMemory(_SyneryDB, _Broadcaster, providerPluginManager, libraryPluginManager);

            // initialize the SyneryInterpreter

            _SyneryClient = new InterpretationClient(_SyneryMemory);

            // success

            _IsInitialized = true;

            return true;
        }

        public void Dispose()
        {
            _IsInitialized = false;
            _InterfaceDefinitionData = null;
            _Broadcaster = null;
            _SyneryClient = null;
            EnvironmentVariables = null;

            if (_SyneryDB != null)
                _SyneryDB.Dispose();
        }

        #endregion

        #region INTERNAL METHODS

        private bool RunJob(InterfaceDefinitionJobData jobData)
        {
            // display some information about the job
            _Broadcaster.Info("Found the job with the name '{0}'.", jobData.Name);
            _Broadcaster.Info("Description: '{0}'", jobData.Description);
            _Broadcaster.Info("Estimated Duration: '{0}'", jobData.EstimatedDurationRemarks);

            string codeFileName = String.Format("{0}.sny", jobData.Id.ToString());
            string codeFilePath = Path.Combine(EnvironmentVariables.InterfaceDefinitionCodeDirectoryPath, codeFileName);

            if (File.Exists(codeFilePath) == false)
            {
                _Broadcaster.Error("Code file of job '{0}' not found at '{1}'.", jobData.Name, codeFilePath);
                return false;
            }

            try
            {
                // TODO: Handle include files

                RunFile(codeFilePath, null);

                _Broadcaster.Info("Successfully finished running the job '{0}'.", jobData.Name);
            }
            catch (SyneryException ex)
            {
                _Broadcaster.Error(ex, "Synery");
                return false;
            }
            catch (Exception ex)
            {
                _Broadcaster.Error("An unknown error occured while running the job '{0}'. Message: '{1}'.", jobData.Name, ex.Message);
                return false;
            }

            return true;
        }

        private bool RunCodeFileWithoutJob(string relativeFilePath)
        {
            string codeFilePath = Path.Combine(EnvironmentVariables.InterfaceDefinitionCodeDirectoryPath, relativeFilePath);

            if (File.Exists(codeFilePath) == false)
            {
                _Broadcaster.Error("Code file not found at '{0}'.", codeFilePath);
                return false;
            }

            try
            {
                RunFile(codeFilePath);

                _Broadcaster.Info("Successfully finished file at '{0}'.", codeFilePath);
            }
            catch (SyneryException ex)
            {
                _Broadcaster.Error(ex, "Synery");
                return false;
            }
            catch (Exception ex)
            {
                _Broadcaster.Error("An unknown error occured while running the file at '{0}'. Message: '{1}'.", codeFilePath, ex.Message);
                return false;
            }

            return true;
        }

        private void RunFile(string codeFilePath, IDictionary<string, string> includeFiles = null)
        {
            string code = File.ReadAllText(codeFilePath);

            _Broadcaster.Info("Start running the file at '{0}'.", codeFilePath);

            _SyneryClient.Run(code, includeFiles);
        }

        private ExecutionResult PrepareRuntimeResult(bool isSuccess)
        {
            return new ExecutionResult()
            {
                IsSuccess = isSuccess,
                EnvironmentVariables = EnvironmentVariables,
                InterfaceDefinitionData = _InterfaceDefinitionData,
                SyneryMemory = _SyneryMemory,
            };
        }

        #endregion
    }
}
