using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;

namespace InterfaceBooster.Test.SyneryLanguage.Interpretation.BaseLanguage.Statements.LibraryPluginVariableStatementInterpreter_Test
{
    [TestFixture]
    public class Using_LibraryPlugin_Variables_Works : BaseTest
    {
        [SetUp]
        public void SetupTestWithDummyPlugin()
        {
            SetupLibraryPlugin("Dummy");
        }

        [Test]
        public void Getting_Value_Of_Readonly_Variable_Returns_Expected_Result()
        {
            string code = @"
STRING result = ""unknown"";

result = $Dummy.FirstVariable;
";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("result");

            Assert.AreEqual("First Variable Value", variable.Value);
        }

        [Test]
        public void Setting_And_Getting_Value_Of_Writable_Variable_Returns_Expected_Result()
        {
            string code = @"
STRING result = ""unknown"";

$Dummy.SecondVariable = ""My Test Value"";

result = $Dummy.SecondVariable;
";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("result");

            Assert.AreEqual("My Test Value", variable.Value);
        }
    }
}
