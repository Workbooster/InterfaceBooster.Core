using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Core.InterfaceDefinitions;
using InterfaceBooster.Common.Interfaces.ErrorHandling;
using InterfaceBooster.Common.Interfaces.InterfaceDefinition.Data;

namespace InterfaceBooster.Core.InterfaceDefinitions.InterfaceDefinitionDataController_Test
{
    [TestFixture]
    public class Handling_Interface_Definition_Xml_Schema_Errors_Works
    {
        private string _ImportDefinitionDirectoryPath;

        [SetUp]
        public void SetupTest()
        {
            string solutionDirectoryPath = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())));
            _ImportDefinitionDirectoryPath = Path.Combine(solutionDirectoryPath, @"_TestData\InterfaceBooster\ImportDefinition");
        }

        [Test]
        public void Loading_Definition_Without_Details_Name_Throws_Exception()
        {
            string importDefinitionXmlFilePath = Path.Combine(_ImportDefinitionDirectoryPath, "importDefinition_Without_Details_Name.xml");

            Assert.Throws<XmlLoadingException>(delegate { InterfaceDefinitionDataController.Load(importDefinitionXmlFilePath); });
        }

        [Test]
        public void Loading_Definition_Without_Details_Throws_Exception()
        {
            string importDefinitionXmlFilePath = Path.Combine(_ImportDefinitionDirectoryPath, "importDefinition_Without_Details.xml");

            Assert.Throws<XmlLoadingException>(delegate { InterfaceDefinitionDataController.Load(importDefinitionXmlFilePath); });
        }

        [Test]
        public void Loading_Definition_Without_LibraryPlugins_Works()
        {
            string importDefinitionXmlFilePath = Path.Combine(_ImportDefinitionDirectoryPath, "importDefinition_Without_LibraryPlugins.xml");

            Assert.DoesNotThrow(delegate { InterfaceDefinitionDataController.Load(importDefinitionXmlFilePath); });
        }

        [Test]
        public void Loading_Definition_Job_Without_IncludeFiles_Works()
        {
            string importDefinitionXmlFilePath = Path.Combine(_ImportDefinitionDirectoryPath, "importDefinition_Job_Without_IncludeFiles.xml");

            Assert.DoesNotThrow(delegate { InterfaceDefinitionDataController.Load(importDefinitionXmlFilePath); });
        }

        [Test]
        public void Loading_Definition_Without_Jobs_Works()
        {
            string importDefinitionXmlFilePath = Path.Combine(_ImportDefinitionDirectoryPath, "importDefinition_Without_Jobs.xml");

            Assert.DoesNotThrow(delegate { InterfaceDefinitionDataController.Load(importDefinitionXmlFilePath); });
        }
    }
}
