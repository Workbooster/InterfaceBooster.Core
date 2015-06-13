using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.SyneryLanguage.Interpretation;
using InterfaceBooster.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.Broadcasting;

namespace InterfaceBooster.Test.SyneryLanguage.Interpretation.SyneryFunctionDeclarationInterpretationClient_Test
{
    [TestFixture]
    public class Extracting_Function_Data_Works
    {
        private SyneryFunctionDeclarationInterpretationClient _Client;
        private ISyneryMemory _SyneryMemory;
        private string _CodeOne;

        [SetUp]
        public void SetupTest()
        {
            _SyneryMemory = new SyneryMemory(null, new DefaultBroadcaster(), null, null);
            _Client = new SyneryFunctionDeclarationInterpretationClient(_SyneryMemory);

            _CodeOne = @"
INT addIntegers(INT first, INT second)
    INT result = 0;
    result = first + second;
    RETURN result;
END

STRING concatenateText(STRING left, STRING middle, STRING right)
    STRING result = """";
    result = left + middle + right;
    RETURN result;
END

STRING mixedParams(STRING testString, INT testInt = 15, BOOL testBool = TRUE)
    STRING result = """";
    result = testString + (STRING)testInt + (STRING)testBool;
    RETURN result;
END
";
        }

        [Test]
        public void Extracting_Three_Functions_Works()
        {
            // check whether 2 functions are detected in _CodeOne

            IList<IFunctionData> listOfFunctions = _Client.Run(_CodeOne);

            Assert.AreEqual(3, listOfFunctions.Count);
        }

        [Test]
        public void Extracting_Function_Name_Works()
        {
            // check whether the name of a function from _CodeOne is correctly detected

            IList<IFunctionData> listOfFunctions = _Client.Run(_CodeOne);

            Assert.AreEqual("concatenateText", listOfFunctions[1].Name);
        }

        [Test]
        public void Extracting_Function_Name_With_Alias_Works()
        {
            // check whether the name and the alias of a function from _CodeOne is correctly detected
            // the FullName is expected to use the following pattern: <CodeAlias>.<Name>

            Dictionary<string, string> includeCode = new Dictionary<string, string>();

            includeCode.Add("alias", _CodeOne);

            IList<IFunctionData> listOfFunctions = _Client.Run("", includeCode);

            Assert.AreEqual("alias.concatenateText", listOfFunctions[1].FullName);
        }

        [Test]
        public void Extracting_Correct_Number_Of_Parameters_Works()
        {
            // check whether 3 parameters are detected in function "concatenateText" in _CodeOne

            IList<IFunctionData> listOfFunctions = _Client.Run(_CodeOne);

            Assert.AreEqual(3, listOfFunctions[1].FunctionDefinition.Parameters.Count);
        }

        [Test]
        public void Extracting_Correct_Parameter_Types_Works()
        {
            // check whether all parameter types are correctly detected in _CodeOne

            IList<IFunctionData> listOfFunctions = _Client.Run(_CodeOne);

            // assert all types to be correct

            Assert.AreEqual(typeof(int), listOfFunctions[0].FunctionDefinition.Parameters[0].Type.UnterlyingDotNetType);
            Assert.AreEqual(typeof(int), listOfFunctions[0].FunctionDefinition.Parameters[1].Type.UnterlyingDotNetType);
            Assert.AreEqual(typeof(string), listOfFunctions[1].FunctionDefinition.Parameters[0].Type.UnterlyingDotNetType);
            Assert.AreEqual(typeof(string), listOfFunctions[1].FunctionDefinition.Parameters[1].Type.UnterlyingDotNetType);
            Assert.AreEqual(typeof(string), listOfFunctions[1].FunctionDefinition.Parameters[2].Type.UnterlyingDotNetType);
            Assert.AreEqual(typeof(string), listOfFunctions[2].FunctionDefinition.Parameters[0].Type.UnterlyingDotNetType);
            Assert.AreEqual(typeof(int), listOfFunctions[2].FunctionDefinition.Parameters[1].Type.UnterlyingDotNetType);
            Assert.AreEqual(typeof(bool), listOfFunctions[2].FunctionDefinition.Parameters[2].Type.UnterlyingDotNetType);
        }

        [Test]
        public void Extracting_Correct_DefaulValues_Works()
        {
            // check whether all default values are correctly detected in _CodeOne

            IList<IFunctionData> listOfFunctions = _Client.Run(_CodeOne);

            // assert all default values to be correct (including the parameter without defualt value)

            Assert.AreEqual(null, listOfFunctions[2].FunctionDefinition.Parameters[0].DefaultValue);
            Assert.AreEqual(15, listOfFunctions[2].FunctionDefinition.Parameters[1].DefaultValue.Value);
            Assert.AreEqual(true, listOfFunctions[2].FunctionDefinition.Parameters[2].DefaultValue.Value);
        }
    }
}
