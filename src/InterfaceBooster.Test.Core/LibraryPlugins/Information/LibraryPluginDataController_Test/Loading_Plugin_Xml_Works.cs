using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Core.LibraryPlugins.Information;
using InterfaceBooster.Common.Interfaces.LibraryPlugin.Information.XmlData;

namespace InterfaceBooster.Core.LibraryPlugins.Information.LibraryPluginDataController_Test
{
    [TestFixture]
    public class Loading_Plugin_Xml_Works
    {
        private string _PluginXmlFilePath;
        private string _PluginDirectoryPath;
        private ILibraryPluginData _LibraryPluginData;

        [SetUp]
        public void SetupTest()
        {
            string solutionDirectoryPath = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())));
            _PluginDirectoryPath = Path.Combine(solutionDirectoryPath, @"_TestData\InterfaceBooster\LibraryPluginData");
            _PluginXmlFilePath = Path.Combine(_PluginDirectoryPath, "plugin.xml");
            _LibraryPluginData = LibraryPluginDataController.Load(_PluginXmlFilePath);
        }

        [Test]
        public void Test_Loading_Plugin_Id_Works()
        {
            Assert.AreEqual("74a8005d-c9f3-455f-94fc-04846493ab7b", _LibraryPluginData.Id.ToString());
        }

        [Test]
        public void Test_Loading_Plugin_Name_Works()
        {
            Assert.AreEqual("String Helpers", _LibraryPluginData.Name);
        }

        [Test]
        public void Test_Loading_Plugin_Description_Works()
        {
            Assert.AreEqual("Some description...", _LibraryPluginData.Description);
        }

        [Test]
        public void Test_Loading_Two_Assemblies_Works()
        {
            Assert.AreEqual(2, _LibraryPluginData.Assemblies.Count);
        }

        [Test]
        public void Test_Loading_An_Assemblies_Path_Works()
        {
            string expected = @"current\MyCompany.InterfaceBooster.LibraryPlugins.StringHelpers.dll";
            Assert.AreEqual(expected, _LibraryPluginData.Assemblies[1].Path);
        }

        [Test]
        public void Test_Loading_An_Assemblies_RequiredInterfaceVersion_Works()
        {
            string expected = "1.2.7.1";
            Assert.AreEqual(expected, _LibraryPluginData.Assemblies[1].RequiredInterfaceVersion.ToString());
        }
    }
}
