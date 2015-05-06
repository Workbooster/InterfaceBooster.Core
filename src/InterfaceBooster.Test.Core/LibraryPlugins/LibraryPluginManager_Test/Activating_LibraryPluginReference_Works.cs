using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Core.LibraryPlugins;
using InterfaceBooster.Common.Interfaces.InterfaceDefinition.Data;
using InterfaceBooster.Common.Interfaces.LibraryPlugin;
using InterfaceBooster.Common.Interfaces.LibraryPlugin.Information.ReflectionData;
using InterfaceBooster.Common.Interfaces.LibraryPlugin.Information.XmlData;
using InterfaceBooster.Core.TestHelpers;
using InterfaceBooster.Test.Dummy.LibraryPluginDummy;
using InterfaceBooster.LibraryPluginApi.Static;

namespace InterfaceBooster.Core.LibraryPlugins.LibraryPluginManager_Test
{
    [TestFixture]
    public class Activating_LibraryPluginReference_Works
    {
        private string _PluginMainDirectoryPath;
        private ILibraryPluginManager _LibraryPluginManager;
        private LibraryPluginReference _SimpleDummyReference;

        [SetUp]
        public void SetupTest()
        {
            string solutionDirectoryPath = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())));
            _PluginMainDirectoryPath = Path.Combine(solutionDirectoryPath, @"_TestData\InterfaceBooster\LibraryPluginDirectory");

            _LibraryPluginManager = new LibraryPluginManager(_PluginMainDirectoryPath);

            _SimpleDummyReference = LibraryPluginReferenceHelper.GetSimpleDummyReference("First");
        }

        [Test]
        public void Test_All_StaticExtensions_Are_Loaded()
        {
            _LibraryPluginManager.Activate(_SimpleDummyReference);

            // get the first available plugin
            ILibraryPluginData testLibraryPluginData = (from p in _LibraryPluginManager.AvailablePlugins
                                                        select p.Value).FirstOrDefault();

            // get the expected result
            // load a list of types from the plugin assebly that implement the IStaticExtension interface

            Type staticExtensionInterfaceType = typeof(IStaticExtension);

            var expectedStaticExtensionTypes = from t in Assembly.GetAssembly(typeof(FirstDummyStaticExtension)).GetTypes()
                                               where t.IsClass
                                               && staticExtensionInterfaceType.IsAssignableFrom(t)
                                               select t;

            Assert.AreEqual(expectedStaticExtensionTypes.Count(), testLibraryPluginData.StaticExtensionContainer.Extensions.Count);
        }

        [Test]
        public void Test_All_Functions_Are_Loaded()
        {
            _LibraryPluginManager.Activate(_SimpleDummyReference);

            // get the first available plugin
            ILibraryPluginData testLibraryPluginData = (from p in _LibraryPluginManager.AvailablePlugins
                                                        select p.Value).FirstOrDefault();

            // get the expected result
            // load a list of types from the plugin assebly that implement the IStaticExtension interface
            // count all methods that have a StaticFunctionAttribute

            Type staticExtensionInterfaceType = typeof(IStaticExtension);

            var expectedStaticExtensionTypes = from t in Assembly.GetAssembly(typeof(FirstDummyStaticExtension)).GetTypes()
                                               where t.IsClass
                                               && staticExtensionInterfaceType.IsAssignableFrom(t)
                                               select t;

            int expectedNumberOfFunctions = 0;

            foreach (var extensionType in expectedStaticExtensionTypes)
            {
                var functionMethods = from m in extensionType.GetMembers()
                                      where m.MemberType == MemberTypes.Method
                                      && m.GetCustomAttribute<StaticFunctionAttribute>() != null
                                      select m;

                expectedNumberOfFunctions += functionMethods.Count();
            }

            Assert.AreEqual(expectedNumberOfFunctions, testLibraryPluginData.StaticExtensionContainer.Functions.Count);
        }

        [Test]
        public void Test_All_Variables_Are_Loaded()
        {
            _LibraryPluginManager.Activate(_SimpleDummyReference);

            // get the first available plugin
            ILibraryPluginData testLibraryPluginData = (from p in _LibraryPluginManager.AvailablePlugins
                                                        select p.Value).FirstOrDefault();

            // get the expected result
            // load a list of types from the plugin assebly that implement the IStaticExtension interface
            // count all properties that have a StaticVariableAttribute

            Type staticExtensionInterfaceType = typeof(IStaticExtension);

            var expectedStaticExtensionTypes = from t in Assembly.GetAssembly(typeof(FirstDummyStaticExtension)).GetTypes()
                                               where t.IsClass
                                               && staticExtensionInterfaceType.IsAssignableFrom(t)
                                               select t;

            int expectedNumberOfVariables = 0;

            foreach (var extensionType in expectedStaticExtensionTypes)
            {
                var variableProperties = from m in extensionType.GetMembers()
                                         where m.MemberType == MemberTypes.Property
                                         && m.GetCustomAttribute<StaticVariableAttribute>() != null
                                         select m;

                expectedNumberOfVariables += variableProperties.Count();
            }

            Assert.AreEqual(expectedNumberOfVariables, testLibraryPluginData.StaticExtensionContainer.Variables.Count);
        }

        [Test]
        public void Test_Variable_HasSetter_Is_Recognized()
        {
            _LibraryPluginManager.Activate(_SimpleDummyReference);

            // get the first available plugin
            ILibraryPluginData testLibraryPluginData = (from p in _LibraryPluginManager.AvailablePlugins
                                                        select p.Value).FirstOrDefault();

            // get the expected result
            // load a list of types from the plugin assebly that implement the IStaticExtension interface
            // loop threw all properties that have a StaticVariableAttribute and check for a public setter

            Type staticExtensionInterfaceType = typeof(IStaticExtension);

            var expectedStaticExtensionTypes = from t in Assembly.GetAssembly(typeof(FirstDummyStaticExtension)).GetTypes()
                                               where t.IsClass
                                               && staticExtensionInterfaceType.IsAssignableFrom(t)
                                               select t;

            List<PropertyInfo> expectedListOfPropertiesWithSetter = new List<PropertyInfo>();
            List<PropertyInfo> expectedListOfReadOnlyProperties = new List<PropertyInfo>();

            foreach (var extensionType in expectedStaticExtensionTypes)
            {
                var propertiesWithSetter = from m in extensionType.GetProperties()
                                           where m.MemberType == MemberTypes.Property
                                           && m.SetMethod != null
                                           && m.SetMethod.IsPublic == true
                                           && m.GetCustomAttribute<StaticVariableAttribute>() != null
                                           select m;
                var readOnlyProperties = from m in extensionType.GetProperties()
                                         where m.MemberType == MemberTypes.Property
                                         && (m.SetMethod == null || m.SetMethod.IsPublic == false)
                                         && m.GetCustomAttribute<StaticVariableAttribute>() != null
                                         select m;

                expectedListOfPropertiesWithSetter.AddRange(propertiesWithSetter);
                expectedListOfReadOnlyProperties.AddRange(readOnlyProperties);
            }

            IEnumerable<IStaticExtensionVariableData> listOfVariableDataWithSetter = from v in testLibraryPluginData.StaticExtensionContainer.Variables
                                                                                     where v.HasSetter == true
                                                                                     select v;


            IEnumerable<IStaticExtensionVariableData> listOfVariableDataWithoutSetter = from v in testLibraryPluginData.StaticExtensionContainer.Variables
                                                                                        where v.HasSetter == false
                                                                                        select v;

            Assert.AreEqual(expectedListOfPropertiesWithSetter.Count, listOfVariableDataWithSetter.Count());
            Assert.AreEqual(expectedListOfReadOnlyProperties.Count, listOfVariableDataWithoutSetter.Count());
        }
    }
}
