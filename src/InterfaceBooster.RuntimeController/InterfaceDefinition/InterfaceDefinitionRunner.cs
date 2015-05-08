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

namespace InterfaceBooster.RuntimeController.InterfaceDefinition
{
    /// <summary>
    /// Handels the initionalization of an Import Definition and enables the application to execute one or multiple job(s).
    /// </summary>
    public class InterfaceDefinitionRunner
    {
        #region MEMBERS

        private IBroadcaster _Broadcaster;
        private bool _IsInitialized;
        private string _InterfaceDefinitionDirectoryPath;
        private string _ProviderPluginMainDirectoryPath;
        private string _LibraryPluginMainDirectoryPath;
        private string _DatabaseDirectoryPath;
        private InterfaceDefinitionData _InterfaceDefinitionData;
        private ISyneryMemory _SyneryMemory;
        private ISyneryClient<bool> _SyneryClient;

        #endregion

        #region PROPERTIES

        public IBroadcaster Broadcaster
        {
            get { return _Broadcaster; }
        }

        public InterfaceDefinitionData InterfaceDefinitionData
        {
            get { return _InterfaceDefinitionData; }
        }

        public bool IsInitialized
        {
            get { return _IsInitialized; }
        }

        public ISyneryMemory SyneryMemory
        {
            get { return _SyneryMemory; }
        }

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Handels the initionalization of an Import Definition and enables the application to execute one or multiple job(s).
        /// </summary>
        /// <param name="broadcaster"></param>
        /// <param name="interfaceDefinitionDirectoryPath"></param>
        /// <param name="providerPluginMainDirectoryPath"></param>
        public InterfaceDefinitionRunner(IBroadcaster broadcaster, string interfaceDefinitionDirectoryPath, string providerPluginMainDirectoryPath)
        {
            _Broadcaster = broadcaster;
            _IsInitialized = false;
            _InterfaceDefinitionDirectoryPath = interfaceDefinitionDirectoryPath;
            _ProviderPluginMainDirectoryPath = providerPluginMainDirectoryPath;
            _DatabaseDirectoryPath = Path.Combine(_InterfaceDefinitionDirectoryPath, @"runtime\db");
        }

        /// <summary>
        /// Initialize the interface definition, the database, the ProviderPluginManager, the SyneryMemory and the SyneryInterpreter
        /// This method must be called before running a job.
        /// </summary>
        /// <returns></returns>
        public bool Initialize()
        {
            IDatabase database;
            IProviderPluginManager providerPluginManager;
            ILibraryPluginManager libraryPluginManager;

            // initialize the interface definition

            try
            {
                string interfaceDefinitionFilePath = Path.Combine(_InterfaceDefinitionDirectoryPath, "definition.xml");

                _InterfaceDefinitionData = InterfaceDefinitionDataController.Load(interfaceDefinitionFilePath);
            }
            catch (Exception ex)
            {
                Broadcaster.Error(ex.Message);
                return false;
            }

            Broadcaster.Info("Interface definition data successfully loaded.");

            // initialize the Synery Database

            try
            {
                if (Directory.Exists(_DatabaseDirectoryPath) == false)
                {
                    Directory.CreateDirectory(_DatabaseDirectoryPath);
                }

                database = new SyneryDB(_DatabaseDirectoryPath);
            }
            catch (SyneryDBException ex)
            {
                Broadcaster.Error(ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                Broadcaster.Error("An unknown error occured. Message: '{0}'.", ex.Message);
                return false;
            }

            // initialize the ProviderPluginManager

            try
            {
                providerPluginManager = new ProviderPluginManager(_ProviderPluginMainDirectoryPath);

                // activate the provider plugin instance references from the interface definition
                providerPluginManager.Activate(_InterfaceDefinitionData.RequiredPlugins.ProviderPluginInstances);
            }
            catch (ProviderPluginManagerException ex)
            {
                Broadcaster.Error(ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                Broadcaster.Error("An unknown error occured. Message: '{0}'.", ex.Message);
                return false;
            }

            // initialize the LibraryPluginManager

            try
            {
                libraryPluginManager = new LibraryPluginManager(_LibraryPluginMainDirectoryPath);

                // activate the provider plugin instance references from the interface definition
                libraryPluginManager.Activate(_InterfaceDefinitionData.RequiredPlugins.LibraryPlugins);
            }
            catch (ProviderPluginManagerException ex)
            {
                Broadcaster.Error(ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                Broadcaster.Error("An unknown error occured. Message: '{0}'.", ex.Message);
                return false;
            }

            // initialize the SyneryMemory

            _SyneryMemory = new SyneryMemory(database, providerPluginManager, libraryPluginManager);

            // initialize the SyneryInterpreter

            _SyneryClient = new InterpretationClient(_SyneryMemory);

            // success

            _IsInitialized = true;
            return true;
        }

        /// <summary>
        /// runs the job with the given name
        /// </summary>
        /// <param name="name">the name of an existing job</param>
        /// <returns></returns>
        public bool RunJob(string name)
        {
            if (IsInitialized == false)
                throw new Exception("The InterfaceDefinitionRunner must be initialized before running a job");

            InterfaceDefinitionJobData jobData = (from j in _InterfaceDefinitionData.Jobs
                                                   where j.Name == name
                                                   select j).FirstOrDefault();

            if (jobData == null)
            {
                Broadcaster.Error("The job with the name '{0}' wasn't found.", name);
                return false;
            }
            else
            {
                // display some information about the job
                Broadcaster.Info("Found the job with the name '{0}'.", name);
                Broadcaster.Info("Description: '{0}'", jobData.Description);
                Broadcaster.Info("Estimated Duration: '{0}'", jobData.EstimatedDurationRemarks);

                string codeFileName = String.Format("{0}.sny", jobData.Id.ToString());
                string codeFilePath = Path.Combine(_InterfaceDefinitionDirectoryPath, "code", codeFileName);

                if (File.Exists(codeFilePath) == false)
                {
                    Broadcaster.Error("Code file of job '{0}' not found at '{1}'.", name, codeFilePath);
                    return false;
                }
                else
                {
                    string code = File.ReadAllText(codeFilePath);

                    Broadcaster.Info("Start running the job '{0}'.", name);

                    try
                    {
                        _SyneryClient.Run(code);
                    }
                    catch (SyneryException ex)
                    {
                        Broadcaster.Error(ex.Message);
                        return false;
                    }
                    catch (Exception ex)
                    {
                        Broadcaster.Error("An unknown error occured while running the job '{0}'. Message: '{1}'.", name, ex.Message);
                        return false;
                    }

                    Broadcaster.Info("Successfully finished running the job '{0}'.", name);
                }
            }

            return true;
        }

        #endregion
    }
}
