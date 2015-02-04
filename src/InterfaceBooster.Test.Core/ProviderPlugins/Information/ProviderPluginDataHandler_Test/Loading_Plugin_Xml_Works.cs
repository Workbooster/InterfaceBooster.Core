using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Core.ProviderPlugins.Information;
using InterfaceBooster.Core.ProviderPlugins.Information.Data;
using InterfaceBooster.Common.Interfaces.ProviderPlugin.Interfaces.Information.Data;

namespace InterfaceBooster.Core.ProviderPlugins.Information.ProviderPluginDataHandler_Test
{
    [TestFixture]
    public class Loading_Plugin_Xml_Works
    {
        private string _PluginXmlFilePath;
        private string _PluginDirectoryPath;
        private IProviderPluginData _ProviderPluginData;

        [SetUp]
        public void SetupTest()
        {
            string solutionDirectoryPath = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())));
            _PluginDirectoryPath = Path.Combine(solutionDirectoryPath, @"_TestData\InterfaceBooster\ProviderPluginData");
            _PluginXmlFilePath = Path.Combine(_PluginDirectoryPath, "plugin.xml");
            _ProviderPluginData = ProviderPluginDataController.Load(_PluginXmlFilePath);
        }

        [TearDown]
        public void TearDown()
        {

        }

        [Test]
        public void Test_Loading_Plugin_Id_Works()
        {
            Assert.AreEqual("485eccb4-3920-4dc3-9ed4-27f65e8b3c91", _ProviderPluginData.Id.ToString());
        }

        [Test]
        public void Test_Loading_Plugin_Name_Works()
        {
            Assert.AreEqual("PROFFIX Database", _ProviderPluginData.Name);
        }

        [Test]
        public void Test_Loading_Plugin_Description_Works()
        {
            Assert.AreEqual("This plugin enables you to access the PROFFIX Database using InterfaceBooster.", _ProviderPluginData.Description);
        }

        [Test]
        public void Test_Loading_Two_Assemblies_Works()
        {
            Assert.AreEqual(2, _ProviderPluginData.Assemblies.Count);
        }

        [Test]
        public void Test_Loading_An_Assemblies_Path_Works()
        {
            string expected = @"proffixCurrent\MyCompany.InterfaceBooster.ProviderPlugins.Proffix.dll";
            Assert.AreEqual(expected, _ProviderPluginData.Assemblies[1].Path);
        }

        [Test]
        public void Test_Loading_An_Assemblies_RequiredInterfaceVersion_Works()
        {
            string expected = "1.2.7.1";
            Assert.AreEqual(expected, _ProviderPluginData.Assemblies[1].RequiredInterfaceVersion.ToString());
        }

        public void Test_Loading_All_Instances_Works()
        {
            Assert.AreEqual(1, _ProviderPluginData.Assemblies[0].Instances.Count);
            Assert.AreEqual(2, _ProviderPluginData.Assemblies[1].Instances.Count);
        }

        [Test]
        public void Test_Loading_An_Instances_Id_Works()
        {
            string expected = "47dee3fa-2c86-4a1a-aea1-7be09948180d";
            Assert.AreEqual(expected, _ProviderPluginData.Assemblies[1].Instances[0].Id.ToString());
        }

        [Test]
        public void Test_Loading_An_Instances_Name_Works()
        {
            string expected = "3.0.1039.0001";
            Assert.AreEqual(expected, _ProviderPluginData.Assemblies[1].Instances[0].Name);
        }

        [Test]
        public void Test_Loading_An_Instances_Description_Works()
        {
            string expected = "The version before 2014";
            Assert.AreEqual(expected, _ProviderPluginData.Assemblies[1].Instances[0].Description);
        }

        [Test]
        public void Test_Loading_The_PluginXmlFilePath_Works()
        {
            string expected = _PluginXmlFilePath;
            Assert.AreEqual(expected, _ProviderPluginData.PluginXmlFilePath);
        }

        [Test]
        public void Test_Loading_The_PluginDirectoryPath_Works()
        {
            string expected = _PluginDirectoryPath;
            Assert.AreEqual(expected, _ProviderPluginData.PluginDirectoryPath);
        }
    }
}
