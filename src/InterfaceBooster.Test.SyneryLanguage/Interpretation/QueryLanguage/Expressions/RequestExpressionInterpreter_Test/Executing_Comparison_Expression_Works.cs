using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Test.SyneryLanguage.Interpretation.QueryLanguage.Expressions.RequestExpressionInterpreter_Test
{
    public class Executing_Comparison_Expression_Works : QueryLanguageTestBase
    {
        [SetUp]
        public void SetupSpecificTest()
        {
            CreatePeopleTable(_Database);
        }

        #region Integers

        [Test]
        public void Executing_GreaterThan_Expression_On_Equal_Integers_Works()
        {
            RunComparisonTest("test = 15 > 15", false);
        }

        [Test]
        public void Executing_GreaterThan_Expression_On_Left_Lower_Integers_Works()
        {
            RunComparisonTest("test = 15 > 22", false);
        }

        [Test]
        public void Executing_GreaterThan_Expression_On_Right_Lower_Integers_Works()
        {
            RunComparisonTest("test = 22 > 15", true);
        }

        [Test]
        public void Executing_LessThan_Expression_On_Equal_Integers_Works()
        {
            RunComparisonTest("test = 15 < 15", false);
        }

        [Test]
        public void Executing_LessThan_Expression_On_Left_Lower_Integers_Works()
        {
            RunComparisonTest("test = 15 < 22", true);
        }

        [Test]
        public void Executing_LessThan_Expression_On_Right_Lower_Integers_Works()
        {
            RunComparisonTest("test = 15 < 22", true);
        }

        #endregion

        #region Decimals

        [Test]
        public void Executing_GreaterThan_Expression_On_Equal_Decimals_Works()
        {
            RunComparisonTest("test = 15.694596644M > 15.694596644M", false);
        }

        [Test]
        public void Executing_GreaterThan_Expression_On_Left_Lower_Decimals_Works()
        {
            RunComparisonTest("test = 15.694596644M > 22.9999999M", false);
        }

        [Test]
        public void Executing_GreaterThan_Expression_On_Right_Lower_Decimals_Works()
        {
            RunComparisonTest("test = 22.9999999M > 15.694596644M", true);
        }

        [Test]
        public void Executing_LessThan_Expression_On_Equal_Decimals_Works()
        {
            RunComparisonTest("test = 15.694596644M < 15.694596644M", false);
        }

        [Test]
        public void Executing_LessThan_Expression_On_Left_Lower_Decimals_Works()
        {
            RunComparisonTest("test = 15.694596644M < 22.9999999M", true);
        }

        [Test]
        public void Executing_LessThan_Expression_On_Right_Lower_Decimals_Works()
        {
            RunComparisonTest("test = 15.694596644M < 22.9999999M", true);
        }

        #endregion

        #region Doubles

        [Test]
        public void Executing_GreaterThan_Expression_On_Equal_Doubles_Works()
        {
            RunComparisonTest("test = 15.6446462626663 > 15.6446462626663", false);
        }

        [Test]
        public void Executing_GreaterThan_Expression_On_Left_Lower_Doubles_Works()
        {
            RunComparisonTest("test = 15.6446462626663 > 22.9999999", false);
        }

        [Test]
        public void Executing_GreaterThan_Expression_On_Right_Lower_Doubles_Works()
        {
            RunComparisonTest("test = 22.9999999 > 15.6446462626663", true);
        }

        [Test]
        public void Executing_LessThan_Expression_On_Equal_Doubles_Works()
        {
            RunComparisonTest("test = 15.6446462626663 < 15.6446462626663", false);
        }

        [Test]
        public void Executing_LessThan_Expression_On_Left_Lower_Doubles_Works()
        {
            RunComparisonTest("test = 15.6446462626663 < 22.9999999", true);
        }

        [Test]
        public void Executing_LessThan_Expression_On_Right_Lower_Doubles_Works()
        {
            RunComparisonTest("test = 15.6446462626663 < 22.9999999", true);
        }

        #endregion

        #region DateTime

        [Test]
        public void Executing_GreaterThan_Expression_On_Equal_DateTime_Works()
        {
            RunComparisonTest("test = DATETIME(1987, 12, 15) > DATETIME(1987, 12, 15)", false);
        }

        [Test]
        public void Executing_GreaterThan_Expression_On_Left_Lower_DateTime_Works()
        {
            RunComparisonTest("test = DATETIME(1987, 12, 15) > DATETIME(2014, 8, 13)", false);
        }

        [Test]
        public void Executing_GreaterThan_Expression_On_Right_Lower_DateTime_Works()
        {
            RunComparisonTest("test = DATETIME(2014, 8, 13) > DATETIME(1987, 12, 15)", true);
        }

        [Test]
        public void Executing_LessThan_Expression_On_Equal_DateTime_Works()
        {
            RunComparisonTest("test = DATETIME(1987, 12, 15) < DATETIME(1987, 12, 15)", false);
        }

        [Test]
        public void Executing_LessThan_Expression_On_Left_Lower_DateTime_Works()
        {
            RunComparisonTest("test = DATETIME(1987, 12, 15) < DATETIME(2014, 8, 13)", true);
        }

        [Test]
        public void Executing_LessThan_Expression_On_Right_Lower_DateTime_Works()
        {
            RunComparisonTest("test = DATETIME(1987, 12, 15) < DATETIME(2014, 8, 13)", true);
        }

        #endregion

        #region HELPERS

        private void RunComparisonTest(string selectStatement, bool expectedResult)
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
