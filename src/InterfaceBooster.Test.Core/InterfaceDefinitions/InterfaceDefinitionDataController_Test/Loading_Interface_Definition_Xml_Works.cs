using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Core.InterfaceDefinitions;
using InterfaceBooster.Common.Interfaces.InterfaceDefinition.Data;

namespace InterfaceBooster.Core.InterfaceDefinitions.InterfaceDefinitionDataController_Test
{
    [TestFixture]
    public class Loading_Interface_Definition_Xml_Works
    {
        private string _InterfaceDefinitionDirectoryPath;
        private string _InterfaceDefinitionXmlFilePath;
        private InterfaceDefinitionData _InterfaceDefinitionData;

        [SetUp]
        public void SetupTest()
        {
            string solutionDirectoryPath = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())));
            _InterfaceDefinitionDirectoryPath = Path.Combine(solutionDirectoryPath, @"_TestData\InterfaceBooster\InterfaceDefinition");
            _InterfaceDefinitionXmlFilePath = Path.Combine(_InterfaceDefinitionDirectoryPath, "interfaceDefinition.xml");
            _InterfaceDefinitionData = InterfaceDefinitionDataController.Load(_InterfaceDefinitionXmlFilePath);
        }

        [Test]
        public void Loading_Id_Works()
        {
            Assert.AreEqual("f3b58f84-cbc0-4560-856d-f6c2b2feea5b", _InterfaceDefinitionData.Id.ToString());
        }

        [Test]
        public void Loading_Details_Name_Works()
        {
            Assert.AreEqual("PROFFIX CSV Article Import", _InterfaceDefinitionData.Details.Name);
        }

        [Test]
        public void Loading_Details_Description_Works()
        {
            Assert.AreEqual("Imports some articles from a comma separated value file to PROFFIX.", _InterfaceDefinitionData.Details.Description);
        }

        [Test]
        public void Loading_Details_Author_Works()
        {
            Assert.AreEqual("Roger Guillet", _InterfaceDefinitionData.Details.Author);
        }

        [Test]
        public void Loading_Details_DateOfCreation_Works()
        {
            Assert.AreEqual(new DateTime(2014, 02, 07), _InterfaceDefinitionData.Details.DateOfCreation);
        }

        [Test]
        public void Loading_Details_DateOfLastChange_Works()
        {
            Assert.AreEqual(new DateTime(2014, 02, 26), _InterfaceDefinitionData.Details.DateOfLastChange);
        }

        [Test]
        public void Loading_Details_Version_Works()
        {
            string expected = "1.0.0.1";
            Assert.AreEqual(expected, _InterfaceDefinitionData.Details.Version.ToString());
        }

        [Test]
        public void Loading_Details_RequiredRuntimeVersion_Works()
        {
            string expected = "1.0.0.0";
            Assert.AreEqual(expected, _InterfaceDefinitionData.Details.RequiredRuntimeVersion.ToString());
        }

        [Test]
        public void Loading_Correct_Number_Of_RequiredProviderPluginInstances_Works()
        {
            Assert.AreEqual(2, _InterfaceDefinitionData.RequiredProviderPluginInstances.Count);
        }

        [Test]
        public void Loading_RequiredProviderPluginInstance_SyneryIdentifier_Works()
        {
            Assert.AreEqual("CSV", _InterfaceDefinitionData.RequiredProviderPluginInstances[1].SyneryIdentifier);
        }

        [Test]
        public void Loading_RequiredProviderPluginInstance_IdPlugin_Works()
        {
            Assert.AreEqual("66ce1d53-14b3-420e-949f-eb94a3d69072", _InterfaceDefinitionData.RequiredProviderPluginInstances[1].IdPlugin.ToString());
        }

        [Test]
        public void Loading_RequiredProviderPluginInstance_PluginName_Works()
        {
            Assert.AreEqual("CSV Provider Plugin", _InterfaceDefinitionData.RequiredProviderPluginInstances[1].PluginName);
        }

        [Test]
        public void Loading_RequiredProviderPluginInstance_IdPluginInstance_Works()
        {
            Assert.AreEqual("f897a501-60d9-4ae5-b214-920f450e9323", _InterfaceDefinitionData.RequiredProviderPluginInstances[1].IdPluginInstance.ToString());
        }

        [Test]
        public void Loading_RequiredProviderPluginInstance_PluginInstanceName_Works()
        {
            Assert.AreEqual("CSV Version 1.0", _InterfaceDefinitionData.RequiredProviderPluginInstances[1].PluginInstanceName);
        }

        [Test]
        public void Loading_RequiredLibraryPlugins_SyneryIdentifier_Works()
        {
            Assert.AreEqual("String", _InterfaceDefinitionData.RequiredLibraryPlugins[0].SyneryIdentifier);
        }

        [Test]
        public void Loading_RequiredLibraryPlugins_IdPlugin_Works()
        {
            Assert.AreEqual("74a8005d-c9f3-455f-94fc-04846493ab7b", _InterfaceDefinitionData.RequiredLibraryPlugins[0].IdPlugin.ToString());
        }

        [Test]
        public void Loading_RequiredLibraryPlugins_PluginName_Works()
        {
            Assert.AreEqual("String Helpers", _InterfaceDefinitionData.RequiredLibraryPlugins[0].PluginName);
        }

        [Test]
        public void Loading_Jobs_Id_Works()
        {
            Assert.AreEqual("2d74ccfb-b1b3-4625-8763-99cfeb077207", _InterfaceDefinitionData.Jobs[1].Id.ToString());
        }

        [Test]
        public void Loading_Jobs_Name_Works()
        {
            Assert.AreEqual("Check for doublets", _InterfaceDefinitionData.Jobs[1].Name);
        }

        [Test]
        public void Loading_Jobs_Description_Works()
        {
            Assert.AreEqual("Executes a check for some doublets in PROFFIX.", _InterfaceDefinitionData.Jobs[1].Description);
        }

        [Test]
        public void Loading_Jobs_EstimatedDurationRemarks_Works()
        {
            Assert.AreEqual("This check will only take a few seconds", _InterfaceDefinitionData.Jobs[1].EstimatedDurationRemarks);
        }

        [Test]
        public void Loading_Jobs_IncludeFile_Works()
        {
            Assert.AreEqual("proffixConnection.syn", _InterfaceDefinitionData.Jobs[1].IncludeFiles["px_con"]);
        }
    }
}
