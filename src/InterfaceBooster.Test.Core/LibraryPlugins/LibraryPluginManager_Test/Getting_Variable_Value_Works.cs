using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Core.LibraryPlugins;
using InterfaceBooster.Common.Interfaces.InterfaceDefinition.Data;
using InterfaceBooster.Common.Interfaces.LibraryPlugin;
using InterfaceBooster.Common.Interfaces.LibraryPlugin.Information.ReflectionData;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;
using InterfaceBooster.Core.TestHelpers;
using InterfaceBooster.Test.Dummy.LibraryPluginDummy.NestedNamespace;

namespace InterfaceBooster.Core.LibraryPlugins.LibraryPluginManager_Test
{
    [TestFixture]
    public class Getting_Variable_Value_Works
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

            // immediately activate the dummy plugin because that isn't part of the test
            _LibraryPluginManager.Activate(_SimpleDummyReference);
        }

        [Test]
        public void Getting_String_Value_Works()
        {
            SecondDummyStaticExtension extension = new SecondDummyStaticExtension();
            string expectedResult = extension.StringValue;

            IStaticExtensionVariableData variableData = _LibraryPluginManager.GetStaticVariableDataByIdentifier("First", "StringValue");

            object result = _LibraryPluginManager.GetStaticVariableWithPrimitiveReturn(variableData);

            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void Getting_Bool_Value_Works()
        {
            SecondDummyStaticExtension extension = new SecondDummyStaticExtension();
            bool expectedResult = extension.BoolValue;

            IStaticExtensionVariableData variableData = _LibraryPluginManager.GetStaticVariableDataByIdentifier("First", "BoolValue");

            object result = _LibraryPluginManager.GetStaticVariableWithPrimitiveReturn(variableData);

            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void Getting_Int_Value_Works()
        {
            SecondDummyStaticExtension extension = new SecondDummyStaticExtension();
            int expectedResult = extension.IntValue;

            IStaticExtensionVariableData variableData = _LibraryPluginManager.GetStaticVariableDataByIdentifier("First", "IntValue");

            object result = _LibraryPluginManager.GetStaticVariableWithPrimitiveReturn(variableData);

            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void Getting_Decimal_Value_Works()
        {
            SecondDummyStaticExtension extension = new SecondDummyStaticExtension();
            decimal expectedResult = extension.DecimalValue;

            IStaticExtensionVariableData variableData = _LibraryPluginManager.GetStaticVariableDataByIdentifier("First", "DecimalValue");

            object result = _LibraryPluginManager.GetStaticVariableWithPrimitiveReturn(variableData);

            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void Getting_Double_Value_Works()
        {
            SecondDummyStaticExtension extension = new SecondDummyStaticExtension();
            double expectedResult = extension.DoubleValue;

            IStaticExtensionVariableData variableData = _LibraryPluginManager.GetStaticVariableDataByIdentifier("First", "DoubleValue");

            object result = _LibraryPluginManager.GetStaticVariableWithPrimitiveReturn(variableData);

            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void Getting_Char_Value_Works()
        {
            SecondDummyStaticExtension extension = new SecondDummyStaticExtension();
            char expectedResult = extension.CharValue;

            IStaticExtensionVariableData variableData = _LibraryPluginManager.GetStaticVariableDataByIdentifier("First", "CharValue");

            object result = _LibraryPluginManager.GetStaticVariableWithPrimitiveReturn(variableData);

            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void Getting_DateTime_Value_Works()
        {
            SecondDummyStaticExtension extension = new SecondDummyStaticExtension();
            DateTime expectedResult = extension.DateTimeValue;

            IStaticExtensionVariableData variableData = _LibraryPluginManager.GetStaticVariableDataByIdentifier("First", "DateTimeValue");

            object result = _LibraryPluginManager.GetStaticVariableWithPrimitiveReturn(variableData);

            Assert.AreEqual(expectedResult, result);
        }
    }
}
