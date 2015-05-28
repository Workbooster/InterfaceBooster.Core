using InterfaceBooster.Common.Interfaces.InterfaceDefinition.Data;
using InterfaceBooster.Core.InterfaceDefinitions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Test.Core.InterfaceDefinitions.InterfaceDefinitionDataController_Test
{
    [TestFixture]
    public class Saving_Interface_Definition_As_Xml_Works
    {
        private string _XmlFilePath;
        private string _ExpectedXmlFilePath;
        private InterfaceDefinitionData _InterfaceDefinitionData;

        [SetUp]
        public void Setup()
        {
            // Prepare the paths of the temporary XML file and the expected XML file
            string solutionDirectoryPath = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())));
            _XmlFilePath = Path.Combine(solutionDirectoryPath, @"_TestData\InterfaceBooster\InterfaceDefinition\saveInterfaceDefinition_TestFile.xml");
            _ExpectedXmlFilePath = Path.Combine(solutionDirectoryPath, @"_TestData\InterfaceBooster\InterfaceDefinition\saveInterfaceDefinition_ExpectedContent.xml");

            // Delete the XML file if it already exists
            if (File.Exists(_XmlFilePath)) File.Delete(_XmlFilePath);

            _InterfaceDefinitionData = new InterfaceDefinitionData();

            // Details

            _InterfaceDefinitionData.Id = new Guid("f3b58f84-cbc0-4560-856d-f6c2b2feea5b");
            _InterfaceDefinitionData.Details.Name = "PROFFIX CSV Article Import";
            _InterfaceDefinitionData.Details.Description = "Imports some articles from a comma separated value file to PROFFIX.";
            _InterfaceDefinitionData.Details.Author = "Roger Guillet";
            _InterfaceDefinitionData.Details.DateOfCreation = new DateTime(2014, 2, 7);
            _InterfaceDefinitionData.Details.DateOfLastChange = new DateTime(2014, 2, 26);
            _InterfaceDefinitionData.Details.Version = new Version(1, 0, 0, 1);
            _InterfaceDefinitionData.Details.RequiredRuntimeVersion = new Version(1, 0, 0, 0);

            // Provider Plugins

            _InterfaceDefinitionData.RequiredPlugins.ProviderPluginInstances.Add(new ProviderPluginInstanceReference()
            {
                SyneryIdentifier = "PROFFIX",
                IdPlugin = new Guid("485eccb4-3920-4dc3-9ed4-27f65e8b3c91"),
                PluginName = "PROFFIX Database",
                IdPluginInstance = new Guid("b139306d-a688-43ae-a9dd-4e692fc2caea"),
                PluginInstanceName = "4.0.0000.0001",
            });

            _InterfaceDefinitionData.RequiredPlugins.ProviderPluginInstances.Add(new ProviderPluginInstanceReference()
            {
                SyneryIdentifier = "CSV",
                IdPlugin = new Guid("66CE1D53-14B3-420E-949F-EB94A3D69072"),
                PluginName = "CSV Provider Plugin",
                IdPluginInstance = new Guid("F897A501-60D9-4AE5-B214-920F450E9323"),
                PluginInstanceName = "CSV Version 1.0",
            });

            // Library Plugins

            _InterfaceDefinitionData.RequiredPlugins.LibraryPlugins.Add(new LibraryPluginReference()
            {
                SyneryIdentifier = "String",
                IdPlugin = new Guid("74A8005D-C9F3-455F-94FC-04846493AB7B"),
                PluginName = "String Helpers",
            });

            // Jobs

            var job1 = new InterfaceDefinitionJobData()
            {
                Id = new Guid("79f8ca0f-c785-4f35-810e-835bc216f6b6"),
                Name = "Run CSV Import",
                Description = "Executes the import of some articles to PROFFIX.",
                EstimatedDurationRemarks = "This import will only take a few seconds",
            };

            job1.IncludeFiles.Add(new IncludeFile() { Alias = "h", RelativePath = "helperFunctions.syn" });

            var job2 = new InterfaceDefinitionJobData()
            {
                Id = new Guid("2D74CCFB-B1B3-4625-8763-99CFEB077207"),
                Name = "Check for doublets",
                Description = "Executes a check for some doublets in PROFFIX.",
                EstimatedDurationRemarks = "This check will only take a few seconds",
            };

            job2.IncludeFiles.Add(new IncludeFile() { Alias = "h", RelativePath = "helperFunctions.syn" });
            job2.IncludeFiles.Add(new IncludeFile() { Alias = "px_con", RelativePath = "proffixConnection.syn" });

            _InterfaceDefinitionData.Jobs.Add(job1);
            _InterfaceDefinitionData.Jobs.Add(job2);
        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(_XmlFilePath)) File.Delete(_XmlFilePath);
        }

        [Test]
        public void Saving_Definition_As_XML_Works()
        {
            InterfaceDefinitionDataController.Save(_XmlFilePath, _InterfaceDefinitionData);

            string generatedXml = File.ReadAllText(_XmlFilePath, Encoding.UTF8);
            string expectedXml = File.ReadAllText(_ExpectedXmlFilePath, Encoding.UTF8);

            Assert.AreEqual(expectedXml, generatedXml);
        }

        [Test]
        public void Loading_And_Saving_Definition_As_XML_Works()
        {
            // load an existing definition
            var loadedInterfaceDefinitionData = InterfaceDefinitionDataController.Load(_ExpectedXmlFilePath);

            // save the existing definition again in a seperate file
            InterfaceDefinitionDataController.Save(_XmlFilePath, loadedInterfaceDefinitionData);

            // compare the two files

            string generatedXml = File.ReadAllText(_XmlFilePath, Encoding.UTF8);
            string expectedXml = File.ReadAllText(_ExpectedXmlFilePath, Encoding.UTF8);

            Assert.AreEqual(expectedXml, generatedXml);
        }
    }
}
