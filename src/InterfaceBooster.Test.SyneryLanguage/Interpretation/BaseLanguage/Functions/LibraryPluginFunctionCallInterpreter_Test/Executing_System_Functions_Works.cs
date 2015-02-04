using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;

namespace InterfaceBooster.Test.SyneryLanguage.Interpretation.BaseLanguage.Functions.LibraryPluginFunctionCallInterpreter_Test
{
    [TestFixture]
    public class Executing_System_Functions_Works : BaseTest
    {
        [SetUp]
        public void SetupTestWithDummyPlugin()
        {
            SetupLibraryPlugin("Dummy");
        }

        [Test]
        public void Execunting_Function_With_One_Parameter_And_DefualtValue_Returns_Expected_Result()
        {
            string code = @"
STRING result = ""unknown"";

result = $Dummy.ThirdMethod(""some text"");
";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("result");

            Assert.AreEqual("SOME TEXT", variable.Value);
        }
    }
}
