using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Test.SyneryLanguage.Interpretation.QueryLanguage.Expressions.RequestExpressionInterpreter_Test
{
    public class Executing_Equality_Expression_Works : QueryLanguageTestBase
    {
        [SetUp]
        public void SetupSpecificTest()
        {
            CreatePeopleTable(_Database);
        }

        #region Strings

        [Test]
        public void Executing_Equal_Expression_On_Equal_Strings_Works()
        {
            RunEqualityTest("test = \"same value\" == \"same value\"", true);
        }

        [Test]
        public void Executing_Equal_Expression_On_Non_Equal_Strings_Works()
        {
            RunEqualityTest("test = \"first\" == \"second\"", false);
        }

        [Test]
        public void Executing_NotEqual_Expression_On_Equal_Strings_Works()
        {
            RunEqualityTest("test = \"same value\" != \"same value\"", false);
        }

        [Test]
        public void Executing_NotEqual_Expression_On_Non_Equal_Strings_Works()
        {
            RunEqualityTest("test = \"first\" != \"second\"", true);
        }

        #endregion

        #region Booleans

        [Test]
        public void Executing_Equal_Expression_On_Equal_Booleans_Works()
        {
            RunEqualityTest("test = TRUE == TRUE", true);
        }

        [Test]
        public void Executing_Equal_Expression_On_Non_Equal_Booleans_Works()
        {
            RunEqualityTest("test = TRUE == FALSE", false);
        }

        [Test]
        public void Executing_NotEqual_Expression_On_Equal_Booleans_Works()
        {
            RunEqualityTest("test = TRUE != TRUE", false);
        }

        [Test]
        public void Executing_NotEqual_Expression_On_Non_Equal_Booleans_Works()
        {
            RunEqualityTest("test = TRUE != FALSE", true);
        }

        #endregion

        #region Integers

        [Test]
        public void Executing_Equal_Expression_On_Equal_Integers_Works()
        {
            RunEqualityTest("test = 15 == 15", true);
        }

        [Test]
        public void Executing_Equal_Expression_On_Non_Equal_Integers_Works()
        {
            RunEqualityTest("test = 15 == 22", false);
        }

        [Test]
        public void Executing_NotEqual_Expression_On_Equal_Integers_Works()
        {
            RunEqualityTest("test = 15 != 15", false);
        }

        [Test]
        public void Executing_NotEqual_Expression_On_Non_Equal_Integers_Works()
        {
            RunEqualityTest("test = 15 != 22", true);
        }

        #endregion

        #region Decimals

        [Test]
        public void Executing_Equal_Expression_On_Equal_Decimals_Works()
        {
            RunEqualityTest("test = 15.6596499M == 15.6596499m", true);
        }

        [Test]
        public void Executing_Equal_Expression_On_Non_Equal_Decimals_Works()
        {
            RunEqualityTest("test = 15.6596499M == 22.999999M", false);
        }

        [Test]
        public void Executing_NotEqual_Expression_On_Equal_Decimals_Works()
        {
            RunEqualityTest("test = 15.6596499M != 15.6596499m", false);
        }

        [Test]
        public void Executing_NotEqual_Expression_On_Non_Equal_Decimals_Works()
        {
            RunEqualityTest("test = 15.6596499M != 22.999999M", true);
        }

        #endregion

        #region Doubles

        [Test]
        public void Executing_Equal_Expression_On_Equal_Doubles_Works()
        {
            RunEqualityTest("test = 15.8994948596526263 == 15.8994948596526263", true);
        }

        [Test]
        public void Executing_Equal_Expression_On_Non_Equal_Doubles_Works()
        {
            RunEqualityTest("test = 15.8994948596526263 == 22.999999999999999", false);
        }

        [Test]
        public void Executing_NotEqual_Expression_On_Equal_Doubles_Works()
        {
            RunEqualityTest("test = 15.8994948596526263 != 15.8994948596526263", false);
        }

        [Test]
        public void Executing_NotEqual_Expression_On_Non_Equal_Doubles_Works()
        {
            RunEqualityTest("test = 15.8994948596526263 != 22.999999999999999", true);
        }

        #endregion

        #region Chars

        [Test]
        public void Executing_Equal_Expression_On_Equal_Chars_Works()
        {
            RunEqualityTest("test = 'R' == 'R'", true);
        }

        [Test]
        public void Executing_Equal_Expression_On_Non_Equal_Chars_Works()
        {
            RunEqualityTest("test = 'R' == 'G'", false);
        }

        [Test]
        public void Executing_NotEqual_Expression_On_Equal_Chars_Works()
        {
            RunEqualityTest("test = 'R' != 'R'", false);
        }

        [Test]
        public void Executing_NotEqual_Expression_On_Non_Equal_Chars_Works()
        {
            RunEqualityTest("test = 'R' != 'G'", true);
        }

        #endregion

        #region HELPERS

        private void RunEqualityTest(string selectStatement, bool expectedResult)
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
