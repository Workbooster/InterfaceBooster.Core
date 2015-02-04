using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Test.SyneryLanguage.Interpretation.QueryLanguage.Expressions.RequestExpressionInterpreter_Test
{
    public class Executing_Cast_Expression_Works : QueryLanguageTestBase
    {
        [SetUp]
        public void SetupSpecificTest()
        {
            CreatePeopleTable(_Database);
        }


        #region from String  to ***

        [Test]
        public void Executing_Cast_From_String_To_Integer_Works()
        {
            RunTest(@"test = (INT)""15""", 15);
        }

        [Test]
        public void Executing_Cast_From_String_To_Boolean_Works()
        {
            RunTest(@"test = (BOOL)""true""", true);
        }

        [Test]
        public void Executing_Cast_From_TRUE_String_To_Boolean_Works()
        {
            RunTest(@"test = (BOOL)""TRUE""", true);
        }

        [Test]
        public void Executing_Cast_From_FALSE_String_To_Boolean_Works()
        {
            RunTest(@"test = (BOOL)""FALSE""", false);
        }

        [Test]
        public void Executing_Cast_From_String_To_Decimal_Works()
        {
            RunTest(@"test = (DECIMAL)""15.6259644""", 15.6259644M);
        }

        [Test]
        public void Executing_Cast_From_String_To_Double_Works()
        {
            RunTest(@"test = (DOUBLE)""15.6526133""", 15.6526133);
        }

        [Test]
        public void Executing_Cast_From_String_To_Char_Works()
        {
            RunTest(@"test = (CHAR)""R""", 'R');
        }

        #endregion

        #region from *** to String

        [Test]
        public void Executing_Cast_From_Integer_To_String_Works()
        {
            RunTest(@"test = (STRING)15", "15");
        }

        [Test]
        public void Executing_Cast_From_Bool_To_String_Works()
        {
            RunTest(@"test = (STRING)TRUE", "True");
        }

        [Test]
        public void Executing_Cast_From_Decimal_To_String_Works()
        {
            RunTest(@"test = (STRING)15.562616M", "15.562616");
        }

        [Test]
        public void Executing_Cast_From_Double_To_String_Works()
        {
            RunTest(@"test = (STRING)15.6656814", "15.6656814");
        }

        [Test]
        public void Executing_Cast_From_Char_To_String_Works()
        {
            RunTest(@"test = (STRING)'R'", "R");
        }

        #endregion

        #region from *** to integer

        [Test]
        public void Executing_Cast_From_Bool_To_Integer_Works()
        {
            RunTest(@"test = (INT)TRUE", 1);
        }

        [Test]
        public void Executing_Cast_From_Decimal_To_Integer_Works()
        {
            RunTest(@"test = (INT)15.562616M", 16);
        }

        [Test]
        public void Executing_Cast_From_Double_To_Integer_Works()
        {
            RunTest(@"test = (INT)15.8799752", 16);
        }

        [Test]
        public void Executing_Cast_From_Char_To_Integer_Works()
        {
            RunTest(@"test = (INT)'R'", 82);
        }

        #endregion

        #region from *** to decimal

        [Test]
        public void Executing_Cast_From_Bool_To_Decimal_Works()
        {
            RunTest(@"test = (DECIMAL)TRUE", 1M);
        }

        [Test]
        public void Executing_Cast_From_Integer_To_Decimal_Works()
        {
            RunTest(@"test = (DECIMAL)15", 15M);
        }

        [Test]
        public void Executing_Cast_From_Double_To_Decimal_Works()
        {
            RunTest(@"test = (DECIMAL)15.8799752", 15.8799752M);
        }

        #endregion

        #region from *** to double

        [Test]
        public void Executing_Cast_From_Bool_To_Double_Works()
        {
            RunTest(@"test = (DOUBLE)TRUE", 1);
        }

        [Test]
        public void Executing_Cast_From_Integer_To_Double_Works()
        {
            RunTest(@"test = (DOUBLE)15", 15);
        }

        [Test]
        public void Executing_Cast_From_Decimal_To_Double_Works()
        {
            RunTest(@"test = (DOUBLE)15.8799752M", 15.8799752);
        }

        #endregion

        #region HELPERS

        private void RunTest(string selectStatement, object expectedResult)
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
