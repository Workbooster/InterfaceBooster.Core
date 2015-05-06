using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.RuntimeController.InterfaceDefinition;
using InterfaceBooster.RuntimeController.Log;
using InterfaceBooster.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.Broadcasting;

namespace InterfaceBooster.RuntimeController.Console
{
    /// <summary>
    /// handels all runtime actions from the view and manages the internal ressources from the model
    /// </summary>
    public class ConsoleRuntimeManager
    {

        #region MEMBERS

        private static ConsoleRuntimeManager _Instance;
        private IBroadcaster _Broadcaster;
        private string _ProviderPluginMainDirectoryPath;
        private InterfaceDefinitionRunner _CurrentRunner;

        #endregion

        #region PROPRTIES

        /// <summary>
        /// singleton instance of the runtime manager
        /// </summary>
        public static ConsoleRuntimeManager Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new ConsoleRuntimeManager();
                }

                return _Instance;
            }
        }

        public IBroadcaster Broadcaster
        {
            get { return _Broadcaster; }
        }

        #endregion

        #region CONSTRUCTORS

        /// <summary>
        /// constructor is protected because only one singleton instance is allowed
        /// </summary>
        protected ConsoleRuntimeManager()
        {
            _Broadcaster = new StandardBroadcaster();
        }

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// supported parameters:
        /// - InterfaceDefinitionPath       required        absolute path to the interface definition's main directory
        /// - RunJob                        required        the name of the job that should be executed
        /// </summary>
        /// <param name="parameters"></param>
        public void Run(string[] args)
        {
            try
            {
                BroadcastFileLogger logger = new BroadcastFileLogger(_Broadcaster, Path.Combine(Environment.CurrentDirectory, "log"));

                Broadcaster.Info("InterfaceBooster started. Welcome!"); 

                bool result = Execute(args);

                if (result == true)
                {
                    Broadcaster.Info("The execution was successfull.");
                }
                else
                {
                    Broadcaster.Info("There was a problem during the execution.");
                }

                Broadcaster.Info("InterfaceBooster finished.");

                logger.Finish();
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(String.Format("An unhandled exception occured: {0}", ex.Message));
            }
        }

        #endregion

        #region INTERNAL METHODS

        #region EXECUTION

        public bool Execute(string[] args)
        {
            IDictionary<string, string> parameters = InitializeParameters(args);

            if (parameters == null)
            {
                return false;
            }

            if (parameters.ContainsKey("InterfaceDefinitionPath"))
            {
                if (InitializeProviderPluginDirectory(parameters["InterfaceDefinitionPath"]) == false)
                {
                    return false;
                }

                try
                {
                    Broadcaster.Info("Using interface definition from '{0}'", parameters["InterfaceDefinitionPath"]);

                    _CurrentRunner = new InterfaceDefinitionRunner(Broadcaster, parameters["InterfaceDefinitionPath"], _ProviderPluginMainDirectoryPath);
                }
                catch (Exception ex)
                {
                    Broadcaster.Error("An unexpected error occured while initializing the InterfaceDefinitionRunner: '{0}'.", ex.Message);
                    return false;
                }

                if (_CurrentRunner.Initialize() == false)
                    return false;

                if (parameters.ContainsKey("RunJob"))
                {
                    return _CurrentRunner.RunJob(parameters["RunJob"]);
                }
                else
                {
                    Broadcaster.Error("The startup parameter 'RunJob' must be set.");
                    return false;
                }
            }
            else
            {
                Broadcaster.Error("The startup parameter 'InterfaceDefinitionPath' must be set.");
                return false;
            }
        }

        #endregion

        #region INITIALIZATION

        /// <summary>
        /// Tries to identify the parameters specified by the view.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private IDictionary<string, string> InitializeParameters(string[] args)
        {
            if (args.Length == 0 || (args.Length % 2) != 0)
            {
                Broadcaster.Error("Invalid startup parameters. Please set some parameters using the format <Key> <Value> <Key> <Value> and so on.");

                return null;
            }
            else
            {
                string name, value;
                IDictionary<string, string> parameters = new Dictionary<string, string>();

                for (int i = 0; i < args.Length; i = i + 2)
                {
                    name = args[i];
                    value = args[i + 1];

                    // remove unneeded chars
                    name = name.Trim(new char[] { '"', ' ', '-' });
                    value = value.Trim(new char[] { '"', ' ', '-' });

                    parameters.Add(name, value);
                }

                return parameters;
            }
        }

        /// <summary>
        /// Checks whether a Provider Plugin directory exists
        /// </summary>
        /// <returns></returns>
        private bool InitializeProviderPluginDirectory(string interfaceDefinitionPath)
        {
            _ProviderPluginMainDirectoryPath = Path.Combine(interfaceDefinitionPath, "plugins", "providerplugins");

            if (Directory.Exists(_ProviderPluginMainDirectoryPath) == false)
            {
                Broadcaster.Error("no provider plugin directory in {0} found.", _ProviderPluginMainDirectoryPath);
                return false;
            }

            return true;
        }

        #endregion

        #endregion
    }
}
