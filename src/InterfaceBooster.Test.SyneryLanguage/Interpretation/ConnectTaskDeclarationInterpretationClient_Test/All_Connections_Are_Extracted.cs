using InterfaceBooster.Common.Interfaces.Broadcasting;
using InterfaceBooster.Common.Interfaces.InterfaceDefinition.Data;
using InterfaceBooster.Common.Interfaces.ProviderPlugin.Interfaces;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;
using InterfaceBooster.Core.ProviderPlugins;
using InterfaceBooster.SyneryLanguage.Interpretation;
using InterfaceBooster.SyneryLanguage.Model.Context;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Test.SyneryLanguage.Interpretation.ConnectTaskDeclarationInterpretationClient_Test
{
    [TestFixture]
    public class All_Connections_Are_Extracted
    {
        private ConnectTaskDeclarationInterpretationClient _Client;
        private ISyneryMemory _SyneryMemory;
        private string _Code;

        [SetUp]
        public void SetupTest()
        {
            string solutionDirectoryPath = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())));
            string pluginMainDirectoryPath = Path.Combine(solutionDirectoryPath, @"_TestData\InterfaceBooster\ProviderPluginDirectory");

            ProviderPluginInstanceReference simpleDummyReference = new ProviderPluginInstanceReference();
            simpleDummyReference.SyneryIdentifier = "DummyOne";
            simpleDummyReference.IdPlugin = new Guid("485eccb4-3920-4dc3-9ed4-27f65e8b3c91");
            simpleDummyReference.PluginName = "ReferencePluginName";
            simpleDummyReference.IdPluginInstance = new Guid("58e6a1b5-9eb5-45ab-8c9b-8b267e2d09e8");
            simpleDummyReference.PluginInstanceName = "ReferencePluginInstanceName";

            // activate provider plugin instance

            IProviderPluginManager providerPluginManager = new ProviderPluginManager(pluginMainDirectoryPath);
            providerPluginManager.Activate(simpleDummyReference);

            _SyneryMemory = new SyneryMemory(null, new DefaultBroadcaster(), providerPluginManager, null);
            _Client = new ConnectTaskDeclarationInterpretationClient(_SyneryMemory);
            _Code = @"
CONNECT ""DummyOne"" 
    AS \\Connections\DummyConnection
    SET (
        Database.Connection.Server = ""Testserver"",
        Database.Connection.Database = ""TestDb"",
        Database.Connection.User = ""TestUser"",
        Database.Connection.Password = ""TestPassword"",
        Proffix.Tables.ShowAdditionalTables = TRUE,
        Proffix.Tables.ShowSystemTables = TRUE
    ) 
END
";
        }

        [Test]
        public void Loading_RecordTypes_Works()
        {
            var listOfConnectTasks = _Client.Run(_Code);

            Assert.AreEqual(1, listOfConnectTasks.Count);
        }
    }
}
