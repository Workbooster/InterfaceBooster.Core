using InterfaceBooster.Database.Core;
using InterfaceBooster.Database.Interfaces;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Core.InterfaceDefinitions.Data;
using InterfaceBooster.Core.LibraryPlugins;
using InterfaceBooster.Core.ProviderPlugins;
using InterfaceBooster.SyneryLanguage.Interpretation;
using InterfaceBooster.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.InterfaceDefinition.Data;
using InterfaceBooster.Common.Interfaces.LibraryPlugin;
using InterfaceBooster.Common.Interfaces.ProviderPlugin.Interfaces;
using InterfaceBooster.Common.Interfaces.SyneryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;

namespace InterfaceBooster.Test.SyneryLanguage.Interpretation.BaseLanguage
{
    public class BaseTest
    {
        protected string _ProviderPluginMainDirectoryPath;
        protected string _LibraryPluginMainDirectoryPath;
        protected string _DatabaseWorkingDirectoryPath;
        protected ISyneryClient<bool> _SyneryClient;
        protected IDatabase _Database;
        protected IProviderPluginManager _ProviderPluginManager;
        protected ILibraryPluginManager _LibraryPluginManager;
        protected ISyneryMemory _SyneryMemory;

        [SetUp]
        public void SetupTest()
        {
            string solutionDirectoryPath = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())));
            _ProviderPluginMainDirectoryPath = Path.Combine(solutionDirectoryPath, @"_TestData\InterfaceBooster\ProviderPluginDirectory");
            _LibraryPluginMainDirectoryPath = Path.Combine(solutionDirectoryPath, @"_TestData\InterfaceBooster\LibraryPluginDirectory");
            _DatabaseWorkingDirectoryPath = Path.Combine(solutionDirectoryPath, @"_TestData\InterfaceBooster\SyneryDB");

            if (Directory.Exists(_DatabaseWorkingDirectoryPath))
            {
                Directory.Delete(_DatabaseWorkingDirectoryPath, true);
            }

            Directory.CreateDirectory(_DatabaseWorkingDirectoryPath);

            _Database = new SyneryDB(_DatabaseWorkingDirectoryPath);
            _ProviderPluginManager = new ProviderPluginManager(_ProviderPluginMainDirectoryPath);
            _LibraryPluginManager = new LibraryPluginManager(_LibraryPluginMainDirectoryPath);
            _SyneryMemory = new SyneryMemory(_Database, _ProviderPluginManager, _LibraryPluginManager);

            _SyneryClient = new InterpretationClient(_SyneryMemory);
        }

        [TearDown]
        public void TearDown()
        {
            _Database.Dispose();

            if (Directory.Exists(_DatabaseWorkingDirectoryPath))
            {
                Directory.Delete(_DatabaseWorkingDirectoryPath, true);
            }
        }

        public void SetupLibraryPlugin(string syneryIdentifier = "Dummy")
        {
            ILibraryPluginReference simpleDummyReference = new LibraryPluginReference();
            simpleDummyReference.SyneryIdentifier = syneryIdentifier;
            simpleDummyReference.IdPlugin = new Guid("74A8005D-C9F3-455F-94FC-04846493AB7B");
            simpleDummyReference.PluginName = "ReferencePluginName";

            // activate library plugin instance
            _LibraryPluginManager.Activate(simpleDummyReference);
        }
    }
}
