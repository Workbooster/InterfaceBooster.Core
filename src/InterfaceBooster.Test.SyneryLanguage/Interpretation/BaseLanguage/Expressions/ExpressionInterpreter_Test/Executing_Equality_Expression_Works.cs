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
    public class Executing_Equality_Expression_Works : BaseTest
    {
        #region Strings

        [Test]
        public void Executing_Equal_Expression_On_Equal_Strings_Works()
        {
            RunEqualityTest("BOOL test = \"same value\" == \"same value\";", true);
        }

        [Test]
        public void Executing_Equal_Expression_On_Non_Equal_Strings_Works()
        {
            RunEqualityTest("BOOL test = \"first\" == \"second\";", false);
        }

        [Test]
        public void Executing_NotEqual_Expression_On_Equal_Strings_Works()
        {
            RunEqualityTest("BOOL test = \"same value\" != \"same value\";", false);
        }

        [Test]
        public void Executing_NotEqual_Expression_On_Non_Equal_Strings_Works()
        {
            RunEqualityTest("BOOL test = \"first\" != \"second\";", true);
        }

        #endregion

        #region Booleans

        [Test]
        public void Executing_Equal_Expression_On_Equal_Booleans_Works()
        {
            RunEqualityTest("BOOL test = TRUE == TRUE;", true);
        }

        [Test]
        public void Executing_Equal_Expression_On_Non_Equal_Booleans_Works()
        {
            RunEqualityTest("BOOL test = TRUE == FALSE;", false);
        }

        [Test]
        public void Executing_NotEqual_Expression_On_Equal_Booleans_Works()
        {
            RunEqualityTest("BOOL test = TRUE != TRUE;", false);
        }

        [Test]
        public void Executing_NotEqual_Expression_On_Non_Equal_Booleans_Works()
        {
            RunEqualityTest("BOOL test = TRUE != FALSE;", true);
        }

        #endregion

        #region Integers

        [Test]
        public void Executing_Equal_Expression_On_Equal_Integers_Works()
        {
            RunEqualityTest("BOOL test = 15 == 15;", true);
        }

        [Test]
        public void Executing_Equal_Expression_On_Non_Equal_Integers_Works()
        {
            RunEqualityTest("BOOL test = 15 == 22;", false);
        }

        [Test]
        public void Executing_NotEqual_Expression_On_Equal_Integers_Works()
        {
            RunEqualityTest("BOOL test = 15 != 15;", false);
        }

        [Test]
        public void Executing_NotEqual_Expression_On_Non_Equal_Integers_Works()
        {
            RunEqualityTest("BOOL test = 15 != 22;", true);
        }

        #endregion

        #region Decimals

        [Test]
        public void Executing_Equal_Expression_On_Equal_Decimals_Works()
        {
            RunEqualityTest("BOOL test = 15.6596499M == 15.6596499m;", true);
        }

        [Test]
        public void Executing_Equal_Expression_On_Non_Equal_Decimals_Works()
        {
            RunEqualityTest("BOOL test = 15.6596499M == 22.999999M;", false);
        }

        [Test]
        public void Executing_NotEqual_Expression_On_Equal_Decimals_Works()
        {
            RunEqualityTest("BOOL test = 15.6596499M != 15.6596499m;", false);
        }

        [Test]
        public void Executing_NotEqual_Expression_On_Non_Equal_Decimals_Works()
        {
            RunEqualityTest("BOOL test = 15.6596499M != 22.999999M;", true);
        }

        #endregion

        #region Doubles

        [Test]
        public void Executing_Equal_Expression_On_Equal_Doubles_Works()
        {
            RunEqualityTest("BOOL test = 15.8994948596526263 == 15.8994948596526263;", true);
        }

        [Test]
        public void Executing_Equal_Expression_On_Non_Equal_Doubles_Works()
        {
            RunEqualityTest("BOOL test = 15.8994948596526263 == 22.999999999999999;", false);
        }

        [Test]
        public void Executing_NotEqual_Expression_On_Equal_Doubles_Works()
        {
            RunEqualityTest("BOOL test = 15.8994948596526263 != 15.8994948596526263;", false);
        }

        [Test]
        public void Executing_NotEqual_Expression_On_Non_Equal_Doubles_Works()
        {
            RunEqualityTest("BOOL test = 15.8994948596526263 != 22.999999999999999;", true);
        }

        #endregion

        #region Chars

        [Test]
        public void Executing_Equal_Expression_On_Equal_Chars_Works()
        {
            RunEqualityTest("BOOL test = 'R' == 'R';", true);
        }

        [Test]
        public void Executing_Equal_Expression_On_Non_Equal_Chars_Works()
        {
            RunEqualityTest("BOOL test = 'R' == 'G';", false);
        }

        [Test]
        public void Executing_NotEqual_Expression_On_Equal_Chars_Works()
        {
            RunEqualityTest("BOOL test = 'R' != 'R';", false);
        }

        [Test]
        public void Executing_NotEqual_Expression_On_Non_Equal_Chars_Works()
        {
            RunEqualityTest("BOOL test = 'R' != 'G';", true);
        }

        #endregion

        #region HELPERS

        private void RunEqualityTest(string code, bool expectedResult)
        {
            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("test");

            Assert.AreEqual(expectedResult, variable.Value);
        }

        #endregion
    }
}
