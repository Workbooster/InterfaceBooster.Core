using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Core.Common.Model;
using InterfaceBooster.Core.LibraryPlugins;
using InterfaceBooster.SyneryLanguage.Interpretation.General;
using InterfaceBooster.Common.Interfaces.InterfaceDefinition.Data;
using InterfaceBooster.Common.Interfaces.LibraryPlugin;
using InterfaceBooster.Common.Interfaces.LibraryPlugin.Information.ReflectionData;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;
using InterfaceBooster.Core.TestHelpers;
using InterfaceBooster.Test.Dummy.LibraryPluginDummy.NestedNamespace;
using InterfaceBooster.Common.Interfaces.Utilities.SyneryLanguage;

namespace InterfaceBooster.Core.LibraryPlugins.LibraryPluginManager_Test
{
    [TestFixture]
    public class Setting_Variable_Value_Works
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
        public void Setting_String_Value_Works()
        {
            TypedValue testValue = new TypedValue(TypeHelper.GetSyneryType(typeof(string)), "Some Text!");

            IStaticExtensionVariableData variableData = _LibraryPluginManager.GetStaticVariableDataByIdentifier("First", "StringValue");

            _LibraryPluginManager.SetStaticVariableWithPrimitiveReturn(variableData, testValue);

            object result = _LibraryPluginManager.GetStaticVariableWithPrimitiveReturn(variableData);

            Assert.AreEqual(testValue.Value, result);
        }

        [Test]
        public void Setting_Bool_Value_Works()
        {
            TypedValue testValue = new TypedValue(TypeHelper.GetSyneryType(typeof(bool)), true);

            IStaticExtensionVariableData variableData = _LibraryPluginManager.GetStaticVariableDataByIdentifier("First", "BoolValue");

            _LibraryPluginManager.SetStaticVariableWithPrimitiveReturn(variableData, testValue);

            object result = _LibraryPluginManager.GetStaticVariableWithPrimitiveReturn(variableData);

            Assert.AreEqual(testValue.Value, result);
        }

        [Test]
        public void Setting_Int_Value_Works()
        {
            TypedValue testValue = new TypedValue(TypeHelper.GetSyneryType(typeof(int)), 30);

            IStaticExtensionVariableData variableData = _LibraryPluginManager.GetStaticVariableDataByIdentifier("First", "IntValue");

            _LibraryPluginManager.SetStaticVariableWithPrimitiveReturn(variableData, testValue);

            object result = _LibraryPluginManager.GetStaticVariableWithPrimitiveReturn(variableData);

            Assert.AreEqual(testValue.Value, result);
        }

        [Test]
        public void Setting_Decimal_Value_Works()
        {
            TypedValue testValue = new TypedValue(TypeHelper.GetSyneryType(typeof(decimal)), 20.2164M);

            IStaticExtensionVariableData variableData = _LibraryPluginManager.GetStaticVariableDataByIdentifier("First", "DecimalValue");

            _LibraryPluginManager.SetStaticVariableWithPrimitiveReturn(variableData, testValue);

            object result = _LibraryPluginManager.GetStaticVariableWithPrimitiveReturn(variableData);

            Assert.AreEqual(testValue.Value, result);
        }

        [Test]
        public void Setting_Double_Value_Works()
        {
            TypedValue testValue = new TypedValue(TypeHelper.GetSyneryType(typeof(double)), 21.5651545);

            IStaticExtensionVariableData variableData = _LibraryPluginManager.GetStaticVariableDataByIdentifier("First", "DoubleValue");

            _LibraryPluginManager.SetStaticVariableWithPrimitiveReturn(variableData, testValue);

            object result = _LibraryPluginManager.GetStaticVariableWithPrimitiveReturn(variableData);

            Assert.AreEqual(testValue.Value, result);
        }

        [Test]
        public void Setting_Char_Value_Works()
        {
            TypedValue testValue = new TypedValue(TypeHelper.GetSyneryType(typeof(char)), 'X');

            IStaticExtensionVariableData variableData = _LibraryPluginManager.GetStaticVariableDataByIdentifier("First", "CharValue");

            _LibraryPluginManager.SetStaticVariableWithPrimitiveReturn(variableData, testValue);

            object result = _LibraryPluginManager.GetStaticVariableWithPrimitiveReturn(variableData);

            Assert.AreEqual(testValue.Value, result);
        }

        [Test]
        public void Setting_DateTime_Value_Works()
        {
            TypedValue testValue = new TypedValue(TypeHelper.GetSyneryType(typeof(DateTime)), new DateTime(1989, 10, 1, 15, 30, 0));

            IStaticExtensionVariableData variableData = _LibraryPluginManager.GetStaticVariableDataByIdentifier("First", "DateTimeValue");

            _LibraryPluginManager.SetStaticVariableWithPrimitiveReturn(variableData, testValue);

            object result = _LibraryPluginManager.GetStaticVariableWithPrimitiveReturn(variableData);

            Assert.AreEqual(testValue.Value, result);
        }
    }
}
