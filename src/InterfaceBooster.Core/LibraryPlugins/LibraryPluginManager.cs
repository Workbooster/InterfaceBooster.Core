using InterfaceBooster.LibraryPluginApi.Static;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Core.Common.Model;
using InterfaceBooster.Core.LibraryPlugins.Information;
using InterfaceBooster.Core.LibraryPlugins.Information.ReflectionData;
using InterfaceBooster.Common.Interfaces.ErrorHandling;
using InterfaceBooster.Common.Interfaces.InterfaceDefinition.Data;
using InterfaceBooster.Common.Interfaces.LibraryPlugin;
using InterfaceBooster.Common.Interfaces.LibraryPlugin.Information.ReflectionData;
using InterfaceBooster.Common.Interfaces.LibraryPlugin.Information.XmlData;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;

namespace InterfaceBooster.Core.LibraryPlugins
{
    public class LibraryPluginManager : ILibraryPluginManager
    {
        #region CONSTANTS

        public readonly Type[] SUPPORTED_TYPES = new Type[] { typeof(string), typeof(bool), typeof(int), typeof(decimal), typeof(double), typeof(char), typeof(DateTime) };

        #endregion

        #region MEMBERS

        private Dictionary<LibraryPluginReference, ILibraryPluginData> _AvailablePlugins = new Dictionary<LibraryPluginReference, ILibraryPluginData>();

        #endregion

        #region PROPERTIES

        public string PluginMainDirectoryPath { get; set; }
        public IReadOnlyDictionary<LibraryPluginReference, ILibraryPluginData> AvailablePlugins { get { return _AvailablePlugins; } }

        #endregion

        #region PUBLIC METHODS

        public LibraryPluginManager(string pluginMainDirectoryPath)
        {
            PluginMainDirectoryPath = pluginMainDirectoryPath;
        }

        #region PLUGIN ACTIVATION

        public void Activate(LibraryPluginReference reference)
        {
            if (string.IsNullOrEmpty(PluginMainDirectoryPath))
            {
                throw new LibraryPluginManagerException(this, String.Format(
                    "A valid PluginMainDirectoryPath must be set before using the LibraryPluginManager. Path given: {0}",
                    PluginMainDirectoryPath));
            }

            ILibraryPluginData pluginData = (from d in _AvailablePlugins
                                             where d.Key.IdPlugin == reference.IdPlugin
                                             select d.Value).FirstOrDefault();

            // If the plugin data was found the plugin has already been initialized. So nothing must be done.
            // Otherwise the plugin must be searched on the filesystem and the assembly must be loaded.
            if (pluginData == null)
            {
                // check whether the SyneryIdentifier is unique

                var numberOfExistingPluginsWithTheSameIdentfier = (from i in _AvailablePlugins
                                                                   where i.Key.SyneryIdentifier == reference.SyneryIdentifier
                                                                   select i).Count();

                if (numberOfExistingPluginsWithTheSameIdentfier != 0)
                {
                    throw new LibraryPluginManagerException(this, String.Format(
                        "Plugin activation for SyneryIdentifier='{0}' failed. A plugin with the given identifier already exists. The identifier must be unique.",
                        reference.SyneryIdentifier));
                }

                // start loading plugin data

                // load plugin.xml
                pluginData = LoadLibraryPluginXmlData(reference);

                if (pluginData != null)
                {
                    // try to load the assembly by using the data from the plugin.xml
                    Assembly assembly = LoadAssembly(reference, pluginData);

                    // get the data of the StaticExtensions by reflection
                    pluginData.StaticExtensionContainer = LoadStaticExtensionContainer(reference, assembly);

                    // append the new plugin data to the dictionary
                    _AvailablePlugins.Add(reference, pluginData);
                }
                else
                {
                    throw new LibraryPluginManagerException(this, String.Format(
                        "Plugin activation for SyneryIdentifier='{0}' failed. The requested Plugin with Name='{1}' and Id='{2}' was not found.",
                        reference.SyneryIdentifier, reference.PluginName, reference.IdPlugin));
                }
            }
        }

        public void Activate(IList<LibraryPluginReference> listOfReferences)
        {
            if (listOfReferences != null)
            {
                foreach (var reference in listOfReferences)
                {
                    Activate(reference);
                }
            }
        }

        #endregion

        #region PERPARATION

        /// <summary>
        /// Gets the declaration data of a static function by searching for the give signature (<paramref name="functionIdentifier"/> and <paramref name="parameterTypes"/>)
        /// in the LibraryPlugin with the given <paramref name="libraryPluginIdentifier"/>.
        /// </summary>
        /// <param name="libraryPluginIdentifier">the synery identifier of the plugin</param>
        /// <param name="functionIdentifier">the long name of the function including the namespace</param>
        /// <param name="parameterTypes">the data typs of the parameters (the correct order is important!)</param>
        /// <returns></returns>
        public IStaticExtensionFunctionData GetStaticFunctionDataBySignature(string libraryPluginIdentifier, string functionIdentifier, Type[] parameterTypes)
        {
            var libraryPluginData = (from i in _AvailablePlugins
                                     where i.Key.SyneryIdentifier == libraryPluginIdentifier
                                     select i.Value).FirstOrDefault();

            if (libraryPluginData == null)
            {
                throw new LibraryPluginManagerException(this, String.Format(
                    "Error while calling a static extension function. The LibraryPlugin with the given SyneryIdentifier='{0}' wasn't found.",
                    libraryPluginIdentifier));
            }
            else
            {
                IStaticExtensionFunctionData functionData = FindFunctionDeclaration(libraryPluginData, functionIdentifier, parameterTypes);

                if (functionData == null)
                {
                    string paramTypeNames = string.Join(",", parameterTypes.Select(p => p.Name));

                    throw new LibraryPluginManagerException(this, String.Format(
                        "No static extension function with the signature {0}.{1}({2}) wasn't found.",
                        libraryPluginIdentifier, functionIdentifier, paramTypeNames));
                }

                return functionData;
            }
        }

        public IStaticExtensionVariableData GetStaticVariableDataByIdentifier(string libraryPluginIdentifier, string variableIdentifier)
        {
            var libraryPluginData = (from i in _AvailablePlugins
                                     where i.Key.SyneryIdentifier == libraryPluginIdentifier
                                     select i.Value).FirstOrDefault();

            if (libraryPluginData == null)
            {
                throw new LibraryPluginManagerException(this, String.Format(
                    "Error while accessing (get) a static extension variable. The LibraryPlugin with the given SyneryIdentifier='{0}' wasn't found.",
                    libraryPluginIdentifier));
            }
            else
            {
                IStaticExtensionVariableData variableData = (from v in libraryPluginData.StaticExtensionContainer.Variables
                                                             where v.FullSyneryIdentifier == variableIdentifier
                                                             select v).FirstOrDefault();

                if (variableData != null)
                {
                    return variableData;
                }

                throw new LibraryPluginManagerException(this, String.Format(
                    "Error while accessing a static extension variable. The static extension variable with the name='{0}.{1}' wasn't found.",
                    libraryPluginIdentifier, variableIdentifier));
            }
        }

        #endregion

        #region INTERACTION

        /// <summary>
        /// Executes a static function that returns a primitive value like string, int, bool etc.
        /// </summary>
        /// <param name="functionData"></param>
        /// <param name="listOfParameters">parameter values (the correct order is important!)</param>
        /// <returns>the return value or null if the function has no return value</returns>
        public object CallStaticFunctionWithPrimitiveReturn(IStaticExtensionFunctionData functionData, object[] listOfParameters)
        {
            object result;

            Type[] parameterTypes = (from p in functionData.Parameters
                                     select p.Type).ToArray();

            IList<object> parameterValues = listOfParameters.ToList();

            while (parameterValues.Count() < parameterTypes.Count())
            {
                // Optional parameters must be set as "Missing".
                // Otherwise the Invoke-method will throw an exception because the lenght of the parameter array doesn't match the method signature.
                parameterValues.Add(Type.Missing);
            }

            Type staticExtensionType = functionData.StaticExtension.GetType();
            MethodInfo method = staticExtensionType.GetMethod(functionData.MethodName, parameterTypes);

            if (method == null)
            {
                throw new LibraryPluginManagerException(this, String.Format(
                    "Couldn't resolve a method with the name='{0}' and {1} parameter(s) (type order: {2}).",
                    functionData.MethodName, functionData.Parameters.Count, string.Join(",", functionData.Parameters.Select(p => p.Type.Name))));
            }

            try
            {
                // call the function
                result = method.Invoke(functionData.StaticExtension, parameterValues.ToArray());
            }
            catch (Exception ex)
            {
                throw new LibraryPluginManagerException(this, String.Format(
                    "An unexpected error occured while calling the static extension function with name='{0}'. The called method is named '{1}' and has {2} parameter(s) (type order: {3}).",
                    functionData.FullSyneryIdentifier, functionData.MethodName, functionData.Parameters.Count, string.Join(",", functionData.Parameters.Select(p => p.Type.Name))),
                    ex);
            }

            if (functionData.ReturnType != null)
            {
                return result;
            }
            else
            {
                return null;
            }
        }

        public object GetStaticVariableWithPrimitiveReturn(IStaticExtensionVariableData variableData)
        {
            PropertyInfo propertyInfo = variableData.StaticExtension.GetType().GetProperty(variableData.PropertyName);

            object result = propertyInfo.GetValue(variableData.StaticExtension);

            return result;
        }

        public void SetStaticVariableWithPrimitiveReturn(IStaticExtensionVariableData variableData, IValue value)
        {
            if (variableData.HasSetter != true)
            {
                throw new LibraryPluginManagerException(this, String.Format(
                    "Error while assigning the static extension variable with the name='{0}'. The variable is read-only.",
                    variableData.FullSyneryIdentifier));
            }
            else if (variableData.Type != value.Type)
            {
                throw new LibraryPluginManagerException(this, String.Format(
                    "Error while assigning the static extension variable with the name='{0}'. The type of the given value ({1}) and the variable ({2}) don't match.",
                    variableData.FullSyneryIdentifier, value.Type.PublicName, variableData.Type.Name));
            }
            else
            {
                PropertyInfo propertyInfo = variableData.StaticExtension.GetType().GetProperty(variableData.PropertyName);

                propertyInfo.SetValue(variableData.StaticExtension, value.Value);
            }
        }

        #endregion

        #endregion

        #region INTERNAL METHODS

        #region PLUGIN ACTIVATION

        private ILibraryPluginData LoadLibraryPluginXmlData(LibraryPluginReference reference)
        {
            ILibraryPluginData pluginData;
            string pluginDirectoryPath = Path.Combine(PluginMainDirectoryPath, reference.IdPlugin.ToString());

            if (Directory.Exists(pluginDirectoryPath))
            {
                string pluginXmlFilePath = Path.Combine(pluginDirectoryPath, "plugin.xml");

                try
                {
                    // load plugin.xml
                    pluginData = LibraryPluginDataController.Load(pluginXmlFilePath);
                }
                catch (Exception ex)
                {
                    throw new LibraryPluginManagerException(this, String.Format(
                        "Error while loading the plugin.xml file from path='{0}'.", pluginXmlFilePath), ex);
                }

                return pluginData;
            }
            else
            {
                throw new LibraryPluginManagerException(this, String.Format(
                    "The requested plugin with Name='{0}' and Id='{1}' wasn't found. The plugin directory is missing: {2}",
                    reference.PluginName, reference.IdPlugin, pluginDirectoryPath));

            }
        }

        private Assembly LoadAssembly(LibraryPluginReference reference, ILibraryPluginData pluginData)
        {
            Version availableInterfaceVersion = GetInterfaceAssemblyVersion();

            // check for the correct interface version
            // if the versions don't match the ProviderPlugin bases on another interface .dll-file than provided by the current runtime
            ILibraryPluginAssemblyData assemblyData = (from a in pluginData.Assemblies
                                                       where a.RequiredInterfaceVersion == availableInterfaceVersion
                                                       select a).FirstOrDefault();

            if (assemblyData != null)
            {
                Assembly assembly;
                string assemblyFilePath = Path.Combine(pluginData.PluginDirectoryPath, assemblyData.Path);

                if (!File.Exists(assemblyFilePath))
                {
                    throw new LibraryPluginManagerException(this, String.Format(
                        "The given assembly of LibraryPlugin with name='{0}' and id='{1}' cannot be found. Assembly file path='{2}'.",
                        pluginData.Name, pluginData.Id, assemblyFilePath));
                }

                try
                {
                    // now we are ready to load the foreign assembly

                    assembly = Assembly.LoadFrom(assemblyFilePath);

                    return assembly;
                }
                catch (Exception ex)
                {
                    // catch unexpected exceptions while loading the assembly to enrich the thrown exception with some additional information

                    throw new LibraryPluginManagerException(this, String.Format(
                        "An unexpected error occured while loading the LibraryPlugin with name='{0}' and id='{1}'. Assembly file path='{2}'.",
                        pluginData.Name, pluginData.Id, assemblyFilePath),
                        ex);
                }
            }
            else
            {
                throw new LibraryPluginManagerException(this, String.Format(
                    "No assembly found for the LibraryPlugin with name='{0}' and id='{1}' that supports the required interface version '{2}'",
                    reference.PluginName, reference.IdPlugin, availableInterfaceVersion.ToString()));
            }
        }

        /// <summary>
        /// Loads the functions (methods) and the variables (properties) that are marked by an attribute
        /// from all classes in the assebly that implement "IStaticExtension".
        /// </summary>
        /// <param name="reference">the plugin reference from the interface definition</param>
        /// <param name="assembly">the plugin's assembly</param>
        /// <returns></returns>
        private IStaticExtensionContainer LoadStaticExtensionContainer(LibraryPluginReference reference, Assembly assembly)
        {
            IStaticExtensionContainer container = new StaticExtensionContainer();

            // try to find classes that implement the interface "IStaticExtension"

            // TODO: Load dependent assemblies

            Type staticExtensionInterfaceType = typeof(IStaticExtension);
            IEnumerable<Type> listOfStaticExtensionTypes = from t in assembly.GetTypes()
                                                           where staticExtensionInterfaceType.IsAssignableFrom(t)
                                                           select t;

            // loop throw all IStaticExtension-classes and extract the functions and the variables
            // the functions are methods that are marked by a "StaticFunctionAttribute"
            // the variables are properties that are marked by a "StaticVariableAttribute"

            foreach (Type staticExtensionType in listOfStaticExtensionTypes)
            {
                // check whether the current class has a constructor without any parameters
                if (staticExtensionType.GetConstructor(new Type[0]) != null)
                {
                    IStaticExtension extension;

                    try
                    {
                        // create an instance of the StaticExtension
                        extension = (IStaticExtension)Activator.CreateInstance(staticExtensionType);
                    }
                    catch (Exception ex)
                    {
                        // catch unexpected exceptions while instantiating the StaticExtension to enrich the thrown exception with some additional information

                        throw new LibraryPluginManagerException(this, String.Format(
                            "An unexpected error occured while instantiating the StaticExtension '{0}' from LibraryPlugin with name='{1}' and id='{2}'.",
                            staticExtensionType.Name, reference.PluginName, reference.IdPlugin),
                            ex);
                    }

                    // extract the functions and the variables by reflection
                    ExtractFunctionData(container, extension);
                    ExtractVariableData(container, extension);

                    // append the extension to the container
                    container.Extensions.Add(extension);
                }
            }

            return container;
        }

        /// <summary>
        /// Extracts all needed data of any function from the given StaticExtension by using reflection and appends it to the given container.
        /// </summary>
        /// <param name="container">the container where the data is appended to.</param>
        /// <param name="staticExtension">the StaticExtension that should be searched by using reflection.</param>
        /// <returns></returns>
        private void ExtractFunctionData(IStaticExtensionContainer container, IStaticExtension staticExtension)
        {
            Type staticExtensionType = staticExtension.GetType();

            // get all relevant methods that are marked by a StaticFunctionAttribute
            IEnumerable<MethodInfo> listOfFunctionMethods = staticExtensionType.GetMethods().Where(m => m.GetCustomAttribute<StaticFunctionAttribute>() != null);

            foreach (var functionMethod in listOfFunctionMethods)
            {
                StaticFunctionAttribute attribute = functionMethod.GetCustomAttribute<StaticFunctionAttribute>();

                IStaticExtensionFunctionData functionData = new StaticExtensionFunctionData();
                functionData.StaticExtension = staticExtension;
                functionData.MethodName = functionMethod.Name;

                // resolve the identifier of the function that is used in synery

                if (String.IsNullOrEmpty(attribute.AlternativeIdentifier))
                {
                    // there is no alternative identifier -> take the original name of the method as function identifier
                    functionData.SyneryIdentifier = functionMethod.Name;
                }
                else
                {
                    // take the alternative identifier
                    functionData.SyneryIdentifier = attribute.AlternativeIdentifier;
                }

                // resolve the return type of the method

                if (functionMethod.ReturnType == typeof(void))
                {
                    // the function doesn't return anything
                    functionData.ReturnType = null;
                }
                else if (SUPPORTED_TYPES.Contains(functionMethod.ReturnType) == true)
                {
                    functionData.ReturnType = functionMethod.ReturnType;
                }
                else
                {
                    throw new LibraryPluginManagerException(this, String.Format(
                        "The return type '{0}' of method '{1}' in class '{2}' is not supported.",
                        functionMethod.ReturnType.Name, functionMethod.Name, staticExtensionType.Name));
                }

                // resolve the parameters

                foreach (var parameter in functionMethod.GetParameters())
                {
                    // validate the parameter

                    if (parameter.IsOut)
                        throw new LibraryPluginManagerException(this, String.Format(
                            "The parameter '{0}' of method '{1}' in class '{2}' is marked as out parameter. Out parameters are not supported.",
                            parameter.Name, functionMethod.Name, staticExtensionType.Name));

                    if (parameter.IsRetval)
                        throw new LibraryPluginManagerException(this, String.Format(
                            "The parameter '{0}' of method '{1}' in class '{2}' is marked as Retval parameter. Retval parameters are not supported.",
                            parameter.Name, functionMethod.Name, staticExtensionType.Name));

                    if (SUPPORTED_TYPES.Contains(parameter.ParameterType) == false)
                        throw new LibraryPluginManagerException(this, String.Format(
                            "The type '{3}' of parameter '{0}' of method '{1}' in class '{2}' is not supported.",
                            parameter.Name, functionMethod.Name, staticExtensionType.Name, parameter.ParameterType.Name));

                    // resolve neeeded parameter information

                    IStaticExtensionFunctionParameterData parameterData = new StaticExtensionFunctionParameterData();

                    parameterData.Name = parameter.Name;
                    parameterData.Type = parameter.ParameterType;
                    parameterData.IsOptional = parameter.IsOptional;

                    functionData.Parameters.Add(parameterData);
                }

                container.Functions.Add(functionData);
            }
        }

        /// <summary>
        /// Extracts all needed data of any variable from the given StaticExtension by using reflection and appends it to the given container.
        /// </summary>
        /// <param name="container">the container where the data is appended to.</param>
        /// <param name="staticExtension">the StaticExtension that should be searched by using reflection.</param>
        private void ExtractVariableData(IStaticExtensionContainer container, IStaticExtension staticExtension)
        {
            Type staticExtensionType = staticExtension.GetType();

            // get all relevant properties that are marked by a StaticVariableAttribute
            IEnumerable<PropertyInfo> listOfVariableProperties = staticExtensionType.GetProperties().Where(p => p.GetCustomAttribute<StaticVariableAttribute>() != null);

            foreach (var variableProperty in listOfVariableProperties)
            {
                if (variableProperty.MemberType != MemberTypes.Property)
                    throw new LibraryPluginManagerException(this, String.Format(
                        "The member '{0}' in class '{1}' is not a property. Only properties are supported as static variables.",
                        variableProperty.Name, staticExtensionType.Name));

                if (variableProperty.CanRead == false)
                    throw new LibraryPluginManagerException(this, String.Format(
                        "The property '{0}' in class '{1}' doesn't have a getter. Only readable properties are supported as static variables.",
                        variableProperty.Name, staticExtensionType.Name));

                if (SUPPORTED_TYPES.Contains(variableProperty.PropertyType) == false)
                    throw new LibraryPluginManagerException(this, String.Format(
                        "The type '{2}' of property '{0}' in class '{1}' is not supported.",
                        variableProperty.Name, staticExtensionType.Name, variableProperty.PropertyType));

                IStaticExtensionVariableData variableData = new StaticExtensionVariableData();
                variableData.StaticExtension = staticExtension;
                variableData.PropertyName = variableProperty.Name;
                variableData.Type = variableProperty.PropertyType;

                StaticVariableAttribute attribute = variableProperty.GetCustomAttribute<StaticVariableAttribute>();

                // resolve the identifier that is used in synery

                if (String.IsNullOrEmpty(attribute.AlternativeIdentifier))
                {
                    // there is no alternative identifier -> take the original name of the property as variable identifier
                    variableData.SyneryIdentifier = variableProperty.Name;
                }
                else
                {
                    // take the alternative name
                    variableData.SyneryIdentifier = attribute.AlternativeIdentifier;
                }

                // check whether the property has a public setter

                if (variableProperty.SetMethod != null && variableProperty.SetMethod.IsPublic == true)
                {
                    variableData.HasSetter = true;
                }
                else
                {
                    variableData.HasSetter = false;
                }

                container.Variables.Add(variableData);
            }
        }

        private Version GetInterfaceAssemblyVersion()
        {
            Assembly assembly = Assembly.GetAssembly(typeof(IStaticExtension));
            return assembly.GetName().Version;
        }

        #endregion

        #region INTERACTION

        private IStaticExtensionFunctionData FindFunctionDeclaration(ILibraryPluginData libraryPluginData, string functionIdentifier, Type[] listOfParameterTypes)
        {
            int numberOfParameterTypes = listOfParameterTypes.Count();

            // try to find function(s) with the same name with at least the same number of parameters as given
            var listOfFunctionData = (from f in libraryPluginData.StaticExtensionContainer.Functions
                                      where f.FullSyneryIdentifier == functionIdentifier
                                      && f.Parameters.Count >= numberOfParameterTypes
                                      select f);

            // try to find the matching function signature
            // also consider that a parameter isn't set because it is optional
            foreach (var functionData in listOfFunctionData)
            {
                bool isMatching = true;

                for (int i = 0; i < functionData.Parameters.Count; i++)
                {
                    if (numberOfParameterTypes > i && listOfParameterTypes[i] != null)
                    {
                        // compare the types of the expected parameter and the given value
                        if (listOfParameterTypes[i] != functionData.Parameters[i].Type)
                        {
                            isMatching = false;
                        }
                    }
                    else
                    {
                        // no value given for the parameter: check whether parameter is optional
                        if (functionData.Parameters[i].IsOptional == false)
                        {
                            isMatching = false;
                        }
                    }
                }

                // the current function declaration seems to match -> return it
                if (isMatching == true)
                    return functionData;
            }

            return null;
        }

        #endregion

        #endregion
    }
}
