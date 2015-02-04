using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Core.ProviderPlugins;
using InterfaceBooster.SyneryLanguage.Interpretation;
using InterfaceBooster.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.ProviderPlugin.Interfaces;
using InterfaceBooster.Common.Interfaces.SyneryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;

namespace InterfaceBooster.Test.SyneryLanguage.Interpretation.BaseLanguage.Statements.VariableStatementInterpreter_Test
{
    [TestFixture]
    public class Using_Variables_And_Expressions_Combined_Works : BaseTest
    {
        [Test]
        public void Executing_Add_Expression_On_Variable_And_Strings_Works()
        {
            RunTest(@"
                STRING first = ""Some"";
                STRING test = first + "" "" + ""Text"";
            ", "Some Text");
        }

        [Test]
        public void Executing_Add_Expression_On_Variable_And_Integers_Works()
        {
            RunTest(@"
                INT first = 15;
                INT test = first + 20 + 2;
            ", 37);
        }

        [Test]
        public void Executing_Add_Expression_On_Variable_And_Decimals_Works()
        {
            RunTest(@"
                DECIMAL first = 15.1111M;
                DECIMAL test = first + 20.2222M + 2.33333m;
            ", 37.66663M);
        }

        [Test]
        public void Executing_Add_Expression_On_Variable_And_Doubles_Works()
        {
            RunTest(@"
                DOUBLE first = 15.1111;
                DOUBLE test = first + 20.2222 + 2.33333;
            ", 37.66663);
        }

        #region HELPERS

        private void RunTest(string code, object expectedResult)
        {
            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("test");

            Assert.AreEqual(expectedResult, variable.Value);
        }

        #endregion
    }
}
