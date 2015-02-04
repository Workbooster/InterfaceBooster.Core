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

namespace InterfaceBooster.Test.SyneryLanguage.Interpretation.BaseLanguage.Expressions.ExpressionInterpreter_Test
{
    [TestFixture]
    public class Executing_Arithmetic_Expression_Works : BaseTest
    {
        #region Strings

        [Test]
        public void Executing_Add_Expression_On_Strings_Works()
        {
            RunArithmeticTest("STRING test = \"Some\" + \"Text\";", "SomeText");
        }

        [Test]
        public void Executing_Multiple_Add_Expression_On_Strings_Works()
        {
            RunArithmeticTest("STRING test = \"Some\" + \" \" + \"Text\";", "Some Text");
        }

        #endregion

        #region Integers

        [Test]
        public void Executing_Add_Expression_On_Integers_Works()
        {
            RunArithmeticTest("INT test = 22 + 15;", 37);
        }

        [Test]
        public void Executing_Minus_Expression_On_Integers_Works()
        {
            RunArithmeticTest("INT test = 22 - 15;", 7);
        }

        [Test]
        public void Executing_Star_Expression_On_Integers_Works()
        {
            RunArithmeticTest("INT test = 22 * 15;", 330);
        }

        [Test]
        public void Executing_Slash_Expression_On_Integers_Works()
        {
            RunArithmeticTest("INT test = 22 / 15;", 1);
        }

        #endregion

        #region Decimals

        [Test]
        public void Executing_Add_Expression_On_Decimals_Works()
        {
            RunArithmeticTest("DECIMAL test = 22.0123M + 15.1M;", 37.1123M);
        }

        [Test]
        public void Executing_Minus_Expression_On_Decimals_Works()
        {
            RunArithmeticTest("DECIMAL test = 22.0123M - 15.1M;", 6.9123M);
        }

        [Test]
        public void Executing_Star_Expression_On_Decimals_Works()
        {
            RunArithmeticTest("DECIMAL test = 22.0123M * 15.1M;", 332.38573M);
        }

        [Test]
        public void Executing_Slash_Expression_On_Decimals_Works()
        {
            RunArithmeticTest("DECIMAL test = 22.0123M / 15.1M;", 1.4577682119205298013245033112583M);
        }

        #endregion

        #region Doubles

        [Test]
        public void Executing_Add_Expression_On_Doubles_Works()
        {
            RunArithmeticTest("DOUBLE test = 22.0123 + 15.1;", 37.1123);
        }

        [Test]
        public void Executing_Minus_Expression_On_Doubles_Works()
        {
            RunArithmeticTest("DOUBLE test = 22.0123 - 15.1;", 6.9123);
        }

        [Test]
        public void Executing_Star_Expression_On_Doubles_Works()
        {
            RunArithmeticTest("DOUBLE test = 22.0123 * 15.1;", 22.0123 * 15.1);
        }

        [Test]
        public void Executing_Slash_Expression_On_Doubles_Works()
        {
            RunArithmeticTest("DOUBLE test = 22.0123 / 15.1;", 1.4577682119205298013245033112583);
        }

        #endregion

        #region HELPERS

        private void RunArithmeticTest(string code, object expectedResult)
        {
            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("test");

            Assert.AreEqual(expectedResult, variable.Value);
        }

        #endregion
    }
}
