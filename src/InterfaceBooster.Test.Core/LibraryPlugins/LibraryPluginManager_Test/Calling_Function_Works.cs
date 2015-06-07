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
using InterfaceBooster.Test.Dummy.LibraryPluginDummy;
using InterfaceBooster.Test.Dummy.LibraryPluginDummy.NestedNamespace;
using InterfaceBooster.Common.Interfaces.Utilities.SyneryLanguage;

namespace InterfaceBooster.Core.LibraryPlugins.LibraryPluginManager_Test
{
    [TestFixture]
    public class Calling_Function_Works
    {
        private string _PluginMainDirectoryPath;
        private string _LibraryPluginRuntimeTestPath;
        private ILibraryPluginManager _LibraryPluginManager;
        private LibraryPluginReference _SimpleDummyReference;

        [SetUp]
        public void SetupTest()
        {
            string solutionDirectoryPath = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())));
            _PluginMainDirectoryPath = Path.Combine(solutionDirectoryPath, @"_TestData\InterfaceBooster\LibraryPluginDirectory");
            _LibraryPluginRuntimeTestPath = Path.Combine(solutionDirectoryPath, @"_TestData\InterfaceBooster\LibraryPluginRuntimeTest");

            _LibraryPluginManager = new LibraryPluginManager(_PluginMainDirectoryPath);

            _SimpleDummyReference = LibraryPluginReferenceHelper.GetSimpleDummyReference("First");

            // immediately activate the dummy plugin because that isn't part of the test
            _LibraryPluginManager.Activate(_SimpleDummyReference);

            // the LibraryPluginRuntimeTest-directory is used by the dummy plugin to create some output files that indicates that the call was successfull.
            // cleanup the LibraryPluginRuntimeTest directory

            if (Directory.Exists(_LibraryPluginRuntimeTestPath))
            {
                Directory.Delete(_LibraryPluginRuntimeTestPath, true);
            }

            Directory.CreateDirectory(_LibraryPluginRuntimeTestPath);
        }

        [TearDown]
        public void TearDown()
        {
            // cleanup the LibraryPluginRuntimeTest directory
            if (Directory.Exists(_LibraryPluginRuntimeTestPath))
            {
                Directory.Delete(_LibraryPluginRuntimeTestPath, true);
            }
        }

        #region TESTING FUNCTIONS WITH REQUIRED PARAMETERS

        [Test]
        public void Calling_Function_Without_Parameters_And_Without_ReturnValue_Works()
        {
            // the dummy plugin creates a textfile that indicates whether the function was executed
            // the test was successfull if the textfile exists
            string expectedExistingFilePath = Path.Combine(_LibraryPluginRuntimeTestPath, "FirstMethod.txt");

            if (File.Exists(expectedExistingFilePath))
                throw new Exception("The test-file already exists before starting the test");

            IStaticExtensionFunctionData functionDeclaration = _LibraryPluginManager.GetStaticFunctionDataBySignature("First", "FirstMethod", new Type[] {});

            _LibraryPluginManager.CallStaticFunctionWithPrimitiveReturn(functionDeclaration, new object[]{});

            Assert.IsTrue(File.Exists(expectedExistingFilePath));
        }

        [Test]
        public void Calling_Function_Without_Parameters_And_With_A_String_ReturnValue_Works()
        {
            FirstDummyStaticExtension extension = new FirstDummyStaticExtension();
            string expectedResult = extension.SecondMethod();

            IStaticExtensionFunctionData functionDeclaration = _LibraryPluginManager.GetStaticFunctionDataBySignature("First", "SecondMethod", new Type[] { });

            object result = _LibraryPluginManager.CallStaticFunctionWithPrimitiveReturn(functionDeclaration, new object[]{});

            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void Calling_Function_With_One_Parameter_And_With_A_String_ReturnValue_Works()
        {
            List<IValue> parameters = new List<IValue>() { 
                new TypedValue(TypeHelper.GetSyneryType(typeof(string)), "This is a test."),
            };

            FirstDummyStaticExtension extension = new FirstDummyStaticExtension();
            string expectedResult = extension.ThirdMethod((string)parameters[0].Value);

            IStaticExtensionFunctionData functionDeclaration = _LibraryPluginManager.GetStaticFunctionDataBySignature("First", "ThirdMethod", parameters.Select(p => p.Type.UnterlyingDotNetType).ToArray());

            object result = _LibraryPluginManager.CallStaticFunctionWithPrimitiveReturn(functionDeclaration, parameters.Select(p => p.Value).ToArray());

            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void Calling_Function_With_Two_Parameter_And_With_A_String_ReturnValue_Works()
        {
            List<IValue> parameters = new List<IValue>() { 
                new TypedValue(TypeHelper.GetSyneryType(typeof(string)), "This is"),
                new TypedValue(TypeHelper.GetSyneryType(typeof(string)), "a big test."),
            };

            FirstDummyStaticExtension extension = new FirstDummyStaticExtension();
            string expectedResult = extension.FourthMethod((string)parameters[0].Value, (string)parameters[1].Value);

            IStaticExtensionFunctionData functionDeclaration = _LibraryPluginManager.GetStaticFunctionDataBySignature("First", "FourthMethod", parameters.Select(p => p.Type.UnterlyingDotNetType).ToArray());

            object result = _LibraryPluginManager.CallStaticFunctionWithPrimitiveReturn(functionDeclaration, parameters.Select(p => p.Value).ToArray());

            Assert.AreEqual(expectedResult, result);
        }

        #endregion

        #region TESTING FUNCTIONS WITH OPTIONAL PARAMETERS

        [Test]
        public void Calling_Function_With_Two_Parameter_And_One_Unset_Optional_Parameter_With_A_String_ReturnValue_Works()
        {
            List<IValue> parameters = new List<IValue>() { 
                new TypedValue(TypeHelper.GetSyneryType(typeof(string)), "This is"),
                new TypedValue(TypeHelper.GetSyneryType(typeof(string)), "a big test."),
            };

            FirstDummyStaticExtension extension = new FirstDummyStaticExtension();
            string expectedResult = extension.FifthMethod((string)parameters[0].Value, (string)parameters[1].Value);

            IStaticExtensionFunctionData functionDeclaration = _LibraryPluginManager.GetStaticFunctionDataBySignature("First", "FifthMethod", parameters.Select(p => p.Type.UnterlyingDotNetType).ToArray());

            object result = _LibraryPluginManager.CallStaticFunctionWithPrimitiveReturn(functionDeclaration, parameters.Select(p => p.Value).ToArray());

            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void Calling_Function_With_Two_Parameter_And_One_Set_Optional_Parameter_With_A_String_ReturnValue_Works()
        {
            List<IValue> parameters = new List<IValue>() { 
                new TypedValue(TypeHelper.GetSyneryType(typeof(string)), "This is"),
                new TypedValue(TypeHelper.GetSyneryType(typeof(string)), "a very"),
                new TypedValue(TypeHelper.GetSyneryType(typeof(string)), "big test."),
            };

            FirstDummyStaticExtension extension = new FirstDummyStaticExtension();
            string expectedResult = extension.FifthMethod((string)parameters[0].Value, (string)parameters[1].Value, (string)parameters[2].Value);

            IStaticExtensionFunctionData functionDeclaration = _LibraryPluginManager.GetStaticFunctionDataBySignature("First", "FifthMethod", parameters.Select(p => p.Type.UnterlyingDotNetType).ToArray());

            object result = _LibraryPluginManager.CallStaticFunctionWithPrimitiveReturn(functionDeclaration, parameters.Select(p => p.Value).ToArray());

            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void Calling_Function_With_Two_Parameter_And_Two_Unset_Optional_Parameter_With_A_String_ReturnValue_Works()
        {
            List<IValue> parameters = new List<IValue>() { 
                new TypedValue(TypeHelper.GetSyneryType(typeof(string)), "This is"),
                new TypedValue(TypeHelper.GetSyneryType(typeof(string)), "a big test."),
            };

            FirstDummyStaticExtension extension = new FirstDummyStaticExtension();
            string expectedResult = extension.SixthMethod((string)parameters[0].Value, (string)parameters[1].Value);

            IStaticExtensionFunctionData functionDeclaration = _LibraryPluginManager.GetStaticFunctionDataBySignature("First", "SixthMethod", parameters.Select(p => p.Type.UnterlyingDotNetType).ToArray());

            object result = _LibraryPluginManager.CallStaticFunctionWithPrimitiveReturn(functionDeclaration, parameters.Select(p => p.Value).ToArray());

            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void Calling_Function_With_Two_Parameter_And_Two_Set_Optional_Parameter_With_A_String_ReturnValue_Works()
        {
            List<IValue> parameters = new List<IValue>() { 
                new TypedValue(TypeHelper.GetSyneryType(typeof(string)), "This is"),
                new TypedValue(TypeHelper.GetSyneryType(typeof(string)), "a very,"),
                new TypedValue(TypeHelper.GetSyneryType(typeof(string)), "a very"),
                new TypedValue(TypeHelper.GetSyneryType(typeof(string)), "big test."),
            };

            FirstDummyStaticExtension extension = new FirstDummyStaticExtension();
            string expectedResult = extension.SixthMethod((string)parameters[0].Value, (string)parameters[1].Value, (string)parameters[2].Value, (string)parameters[3].Value);

            IStaticExtensionFunctionData functionDeclaration = _LibraryPluginManager.GetStaticFunctionDataBySignature("First", "SixthMethod", parameters.Select(p => p.Type.UnterlyingDotNetType).ToArray());

            object result = _LibraryPluginManager.CallStaticFunctionWithPrimitiveReturn(functionDeclaration, parameters.Select(p => p.Value).ToArray());

            Assert.AreEqual(expectedResult, result);
        }

        #endregion

        #region TESTING OVERLOADED FUNCTIONS

        [Test]
        public void Calling_Overloaded_Function_With_Bool_Parameter_Works()
        {
            List<IValue> parameters = new List<IValue>() { 
                new TypedValue(TypeHelper.GetSyneryType(typeof(bool)), true),
            };

            FirstDummyStaticExtension extension = new FirstDummyStaticExtension();
            string expectedResult = extension.OverloadedMethod((bool)parameters[0].Value);

            IStaticExtensionFunctionData functionDeclaration = _LibraryPluginManager.GetStaticFunctionDataBySignature("First", "OverloadedMethod", parameters.Select(p => p.Type.UnterlyingDotNetType).ToArray());

            object result = _LibraryPluginManager.CallStaticFunctionWithPrimitiveReturn(functionDeclaration, parameters.Select(p => p.Value).ToArray());

            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void Calling_Overloaded_Function_With_Int_Parameter_Works()
        {
            List<IValue> parameters = new List<IValue>() { 
                new TypedValue(TypeHelper.GetSyneryType(typeof(int)), 15),
            };

            FirstDummyStaticExtension extension = new FirstDummyStaticExtension();
            string expectedResult = extension.OverloadedMethod((int)parameters[0].Value);

            IStaticExtensionFunctionData functionDeclaration = _LibraryPluginManager.GetStaticFunctionDataBySignature("First", "OverloadedMethod", parameters.Select(p => p.Type.UnterlyingDotNetType).ToArray());

            object result = _LibraryPluginManager.CallStaticFunctionWithPrimitiveReturn(functionDeclaration, parameters.Select(p => p.Value).ToArray());

            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void Calling_Overloaded_Function_With_Decimal_Parameter_Works()
        {
            List<IValue> parameters = new List<IValue>() { 
                new TypedValue(TypeHelper.GetSyneryType(typeof(decimal)), 15.7M),
            };

            FirstDummyStaticExtension extension = new FirstDummyStaticExtension();
            string expectedResult = extension.OverloadedMethod((decimal)parameters[0].Value);

            IStaticExtensionFunctionData functionDeclaration = _LibraryPluginManager.GetStaticFunctionDataBySignature("First", "OverloadedMethod", parameters.Select(p => p.Type.UnterlyingDotNetType).ToArray());

            object result = _LibraryPluginManager.CallStaticFunctionWithPrimitiveReturn(functionDeclaration, parameters.Select(p => p.Value).ToArray());

            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void Calling_Overloaded_Function_With_Double_Parameter_Works()
        {
            List<IValue> parameters = new List<IValue>() { 
                new TypedValue(TypeHelper.GetSyneryType(typeof(double)), 15.655665),
            };

            FirstDummyStaticExtension extension = new FirstDummyStaticExtension();
            string expectedResult = extension.OverloadedMethod((double)parameters[0].Value);

            IStaticExtensionFunctionData functionDeclaration = _LibraryPluginManager.GetStaticFunctionDataBySignature("First", "OverloadedMethod", parameters.Select(p => p.Type.UnterlyingDotNetType).ToArray());

            object result = _LibraryPluginManager.CallStaticFunctionWithPrimitiveReturn(functionDeclaration, parameters.Select(p => p.Value).ToArray());

            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void Calling_Overloaded_Function_With_Char_Parameter_Works()
        {
            List<IValue> parameters = new List<IValue>() { 
                new TypedValue(TypeHelper.GetSyneryType(typeof(char)), 'R'),
            };

            FirstDummyStaticExtension extension = new FirstDummyStaticExtension();
            string expectedResult = extension.OverloadedMethod((char)parameters[0].Value);

            IStaticExtensionFunctionData functionDeclaration = _LibraryPluginManager.GetStaticFunctionDataBySignature("First", "OverloadedMethod", parameters.Select(p => p.Type.UnterlyingDotNetType).ToArray());

            object result = _LibraryPluginManager.CallStaticFunctionWithPrimitiveReturn(functionDeclaration, parameters.Select(p => p.Value).ToArray());

            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void Calling_Overloaded_Function_With_DateTime_Parameter_Works()
        {
            List<IValue> parameters = new List<IValue>() { 
                new TypedValue(TypeHelper.GetSyneryType(typeof(DateTime)), new DateTime(1987,12,15,22,30,0)),
            };

            FirstDummyStaticExtension extension = new FirstDummyStaticExtension();
            string expectedResult = extension.OverloadedMethod((DateTime)parameters[0].Value);

            IStaticExtensionFunctionData functionDeclaration = _LibraryPluginManager.GetStaticFunctionDataBySignature("First", "OverloadedMethod", parameters.Select(p => p.Type.UnterlyingDotNetType).ToArray());

            object result = _LibraryPluginManager.CallStaticFunctionWithPrimitiveReturn(functionDeclaration, parameters.Select(p => p.Value).ToArray());

            Assert.AreEqual(expectedResult, result);
        }

        #endregion

        #region TESTING FUNCTIONS WITH NAMESPACES

        [Test]
        public void Calling_Nested_Function_On_First_Namespace_Level()
        {
            List<IValue> parameters = new List<IValue>() { 
                new TypedValue(TypeHelper.GetSyneryType(typeof(string)), "This is a test."),
            };

            ThirdDummyStaticExtension extension = new ThirdDummyStaticExtension();
            string expectedResult = extension.FirstMethod((string)parameters[0].Value);

            IStaticExtensionFunctionData functionDeclaration = _LibraryPluginManager.GetStaticFunctionDataBySignature("First", "FirstLevelNamespace.FirstMethod", parameters.Select(p => p.Type.UnterlyingDotNetType).ToArray());

            object result = _LibraryPluginManager.CallStaticFunctionWithPrimitiveReturn(functionDeclaration, parameters.Select(p => p.Value).ToArray());

            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void Calling_Nested_Function_On_Second_Namespace_Level()
        {
            List<IValue> parameters = new List<IValue>() { 
                new TypedValue(TypeHelper.GetSyneryType(typeof(string)), "This is a test."),
            };

            FourthDummyStaticExtension extension = new FourthDummyStaticExtension();
            string expectedResult = extension.FirstMethod((string)parameters[0].Value);

            IStaticExtensionFunctionData functionDeclaration = _LibraryPluginManager.GetStaticFunctionDataBySignature("First", "FirstLevelNamespace.SecondLevelNamespace.FirstMethod", parameters.Select(p => p.Type.UnterlyingDotNetType).ToArray());

            object result = _LibraryPluginManager.CallStaticFunctionWithPrimitiveReturn(functionDeclaration, parameters.Select(p => p.Value).ToArray());

            Assert.AreEqual(expectedResult, result);
        }

        #endregion

        #region TESTING FUNCTIONS WITH ALTERNATIVE NAMES

        [Test]
        public void Calling_Function_With_Alternative_Name()
        {
            List<IValue> parameters = new List<IValue>() { 
                new TypedValue(TypeHelper.GetSyneryType(typeof(string)), "This is a test."),
            };

            FourthDummyStaticExtension extension = new FourthDummyStaticExtension();
            string expectedResult = extension.SecondMethod((string)parameters[0].Value);

            IStaticExtensionFunctionData functionDeclaration = _LibraryPluginManager.GetStaticFunctionDataBySignature("First", "FirstLevelNamespace.SecondLevelNamespace.FirstMethodWithAlternativeName", parameters.Select(p => p.Type.UnterlyingDotNetType).ToArray());

            object result = _LibraryPluginManager.CallStaticFunctionWithPrimitiveReturn(functionDeclaration, parameters.Select(p => p.Value).ToArray());

            Assert.AreEqual(expectedResult, result);
        }

        #endregion
    }
}
