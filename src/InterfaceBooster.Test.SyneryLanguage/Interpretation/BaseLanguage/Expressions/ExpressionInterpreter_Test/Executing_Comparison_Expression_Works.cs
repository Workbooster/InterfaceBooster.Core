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
    public class Executing_Comparison_Expression_Works : BaseTest
    {
        #region Integers

        [Test]
        public void Executing_GreaterThan_Expression_On_Equal_Integers_Works()
        {
            RunComparisonTest("BOOL test = 15 > 15;", false);
        }

        [Test]
        public void Executing_GreaterThan_Expression_On_Left_Lower_Integers_Works()
        {
            RunComparisonTest("BOOL test = 15 > 22;", false);
        }

        [Test]
        public void Executing_GreaterThan_Expression_On_Right_Lower_Integers_Works()
        {
            RunComparisonTest("BOOL test = 22 > 15;", true);
        }

        [Test]
        public void Executing_LessThan_Expression_On_Equal_Integers_Works()
        {
            RunComparisonTest("BOOL test = 15 < 15;", false);
        }

        [Test]
        public void Executing_LessThan_Expression_On_Left_Lower_Integers_Works()
        {
            RunComparisonTest("BOOL test = 15 < 22;", true);
        }

        [Test]
        public void Executing_LessThan_Expression_On_Right_Lower_Integers_Works()
        {
            RunComparisonTest("BOOL test = 15 < 22;", true);
        }

        #endregion

        #region Decimals

        [Test]
        public void Executing_GreaterThan_Expression_On_Equal_Decimals_Works()
        {
            RunComparisonTest("BOOL test = 15.694596644M > 15.694596644M;", false);
        }

        [Test]
        public void Executing_GreaterThan_Expression_On_Left_Lower_Decimals_Works()
        {
            RunComparisonTest("BOOL test = 15.694596644M > 22.9999999M;", false);
        }

        [Test]
        public void Executing_GreaterThan_Expression_On_Right_Lower_Decimals_Works()
        {
            RunComparisonTest("BOOL test = 22.9999999M > 15.694596644M;", true);
        }

        [Test]
        public void Executing_LessThan_Expression_On_Equal_Decimals_Works()
        {
            RunComparisonTest("BOOL test = 15.694596644M < 15.694596644M;", false);
        }

        [Test]
        public void Executing_LessThan_Expression_On_Left_Lower_Decimals_Works()
        {
            RunComparisonTest("BOOL test = 15.694596644M < 22.9999999M;", true);
        }

        [Test]
        public void Executing_LessThan_Expression_On_Right_Lower_Decimals_Works()
        {
            RunComparisonTest("BOOL test = 15.694596644M < 22.9999999M;", true);
        }

        #endregion

        #region Doubles

        [Test]
        public void Executing_GreaterThan_Expression_On_Equal_Doubles_Works()
        {
            RunComparisonTest("BOOL test = 15.6446462626663 > 15.6446462626663;", false);
        }

        [Test]
        public void Executing_GreaterThan_Expression_On_Left_Lower_Doubles_Works()
        {
            RunComparisonTest("BOOL test = 15.6446462626663 > 22.9999999;", false);
        }

        [Test]
        public void Executing_GreaterThan_Expression_On_Right_Lower_Doubles_Works()
        {
            RunComparisonTest("BOOL test = 22.9999999 > 15.6446462626663;", true);
        }

        [Test]
        public void Executing_LessThan_Expression_On_Equal_Doubles_Works()
        {
            RunComparisonTest("BOOL test = 15.6446462626663 < 15.6446462626663;", false);
        }

        [Test]
        public void Executing_LessThan_Expression_On_Left_Lower_Doubles_Works()
        {
            RunComparisonTest("BOOL test = 15.6446462626663 < 22.9999999;", true);
        }

        [Test]
        public void Executing_LessThan_Expression_On_Right_Lower_Doubles_Works()
        {
            RunComparisonTest("BOOL test = 15.6446462626663 < 22.9999999;", true);
        }

        #endregion

        #region HELPERS

        private void RunComparisonTest(string code, bool expectedResult)
        {
            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("test");

            Assert.AreEqual(expectedResult, variable.Value);
        }

        #endregion
    }
}
