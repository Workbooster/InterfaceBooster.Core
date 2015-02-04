using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Test.SyneryLanguage.Interpretation.QueryLanguage.Expressions.RequestExpressionInterpreter_Test
{
    public class Executing_Arithmetic_Expression_Works : QueryLanguageTestBase
    {
        [SetUp]
        public void SetupSpecificTest()
        {
            CreatePeopleTable(_Database);
        }

        #region Strings

        [Test]
        public void Executing_Add_Expression_On_Strings_Works()
        {
            RunArithmeticTest("test = \"Some\" + \"Text\"", "SomeText");
        }

        [Test]
        public void Executing_Multiple_Add_Expression_On_Strings_Works()
        {
            RunArithmeticTest("test = \"Some\" + \" \" + \"Text\"", "Some Text");
        }

        #endregion

        #region Integers

        [Test]
        public void Executing_Add_Expression_On_Integers_Works()
        {
            RunArithmeticTest("test = 22 + 15", 37);
        }

        [Test]
        public void Executing_Minus_Expression_On_Integers_Works()
        {
            RunArithmeticTest("test = 22 - 15", 7);
        }

        [Test]
        public void Executing_Star_Expression_On_Integers_Works()
        {
            RunArithmeticTest("test = 22 * 15", 330);
        }

        [Test]
        public void Executing_Slash_Expression_On_Integers_Works()
        {
            RunArithmeticTest("test = 22 / 15", 1);
        }

        #endregion

        #region Decimals

        [Test]
        public void Executing_Add_Expression_On_Decimals_Works()
        {
            RunArithmeticTest("test = 22.0123M + 15.1M", 37.1123M);
        }

        [Test]
        public void Executing_Minus_Expression_On_Decimals_Works()
        {
            RunArithmeticTest("test = 22.0123M - 15.1M", 6.9123M);
        }

        [Test]
        public void Executing_Star_Expression_On_Decimals_Works()
        {
            RunArithmeticTest("test = 22.0123M * 15.1M", 332.38573M);
        }

        [Test]
        public void Executing_Slash_Expression_On_Decimals_Works()
        {
            RunArithmeticTest("test = 22.0123M / 15.1M", 1.4577682119205298013245033112583M);
        }

        #endregion

        #region Doubles

        [Test]
        public void Executing_Add_Expression_On_Doubles_Works()
        {
            RunArithmeticTest("test = 22.0123 + 15.1", 37.1123);
        }

        [Test]
        public void Executing_Minus_Expression_On_Doubles_Works()
        {
            RunArithmeticTest("test = 22.0123 - 15.1", 6.9123);
        }

        [Test]
        public void Executing_Star_Expression_On_Doubles_Works()
        {
            RunArithmeticTest("test = 22.0123 * 15.1", 22.0123 * 15.1);
        }

        [Test]
        public void Executing_Slash_Expression_On_Doubles_Works()
        {
            RunArithmeticTest("test = 22.0123 / 15.1", 1.4577682119205298013245033112583);
        }

        #endregion

        #region HELPERS

        private void RunArithmeticTest(string selectStatement, object expectedResult)
        {
            string code = String.Format(@"
\QueryLanguageTests\Test = 
    FROM \QueryLanguageTests\People AS p
    SELECT {0};
",
                  selectStatement);

            _SyneryClient.Run(code);

            object resultValue = _Database.LoadTable(@"\QueryLanguageTests\Test")[0][0];

            Assert.AreEqual(expectedResult, resultValue);
        }

        #endregion
    }
}
