using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Test.SyneryLanguage.Interpretation.QueryLanguage.Expressions.RequestExpressionInterpreter_Test
{
    public class Executing_Logical_Expression_Works : QueryLanguageTestBase
    {
        [SetUp]
        public void SetupSpecificTest()
        {
            CreatePeopleTable(_Database);
        }

        #region AND

        [Test]
        public void Executing_Simple_And_Expression_With_Static_True_Values_Works()
        {
            string code = "test = TRUE AND TRUE";

            _SyneryClient.Run(GenerateCode(code));

            object resultValue = _Database.LoadTable(@"\QueryLanguageTests\Test")[0][0];

            Assert.AreEqual(true, resultValue);
        }

        [Test]
        public void Executing_Simple_And_Expression_With_Static_True_And_False_Values_Works()
        {
            string code = "test = FALSE AND TRUE";

            _SyneryClient.Run(GenerateCode(code));

            object resultValue = _Database.LoadTable(@"\QueryLanguageTests\Test")[0][0];

            Assert.AreEqual(false, resultValue);
        }

        [Test]
        public void Executing_Complex_And_Expression_With_Calculated_True_Values_Works()
        {
            string code = "test = 15 == 15 AND \"Bla\" == \"Bla\"";

            _SyneryClient.Run(GenerateCode(code));

            object resultValue = _Database.LoadTable(@"\QueryLanguageTests\Test")[0][0];

            Assert.AreEqual(true, resultValue);
        }

        [Test]
        public void Executing_Complex_And_Expression_With_Calculated_True_And_False_Values_Works()
        {
            string code = "test = 15 == 15 AND \"First\" == \"Second\"";

            _SyneryClient.Run(GenerateCode(code));

            object resultValue = _Database.LoadTable(@"\QueryLanguageTests\Test")[0][0];

            Assert.AreEqual(false, resultValue);
        }

        [Test]
        public void Executing_Complex_And_Expression_With_Multiple_Calculated_True_Values_Works()
        {
            string code = "test = 22.99999999M == 22.99999999M AND 15.987654 != 22.1 AND 15 == 15 AND \"Bla\" == \"Bla\" AND \"First\" != \"Second\"";

            _SyneryClient.Run(GenerateCode(code));

            object resultValue = _Database.LoadTable(@"\QueryLanguageTests\Test")[0][0];

            Assert.AreEqual(true, resultValue);
        }

        [Test]
        public void Executing_Complex_And_Expression_With_Multiple_Calculated_True_And_False_Values_Works()
        {
            string code = "test = 22.99999999M == 22.99999999M AND 15.987654 == 22.1 AND 15 == 15 AND \"Bla\" == \"Bla\" AND \"First\" != \"Second\"";

            _SyneryClient.Run(GenerateCode(code));

            object resultValue = _Database.LoadTable(@"\QueryLanguageTests\Test")[0][0];

            Assert.AreEqual(false, resultValue);
        }

        #endregion

        #region OR

        [Test]
        public void Executing_Simple_Or_Expression_With_Static_True_Values_Works()
        {
            string code = "test = TRUE OR TRUE";

            _SyneryClient.Run(GenerateCode(code));

            object resultValue = _Database.LoadTable(@"\QueryLanguageTests\Test")[0][0];

            Assert.AreEqual(true, resultValue);
        }

        [Test]
        public void Executing_Simple_Or_Expression_With_Static_True_Or_False_Values_Works()
        {
            string code = "test = FALSE OR TRUE";

            _SyneryClient.Run(GenerateCode(code));

            object resultValue = _Database.LoadTable(@"\QueryLanguageTests\Test")[0][0];

            Assert.AreEqual(true, resultValue);
        }

        [Test]
        public void Executing_Complex_Or_Expression_With_Calculated_True_Values_Works()
        {
            string code = "test = 15 == 15 OR \"Bla\" == \"Bla\"";

            _SyneryClient.Run(GenerateCode(code));

            object resultValue = _Database.LoadTable(@"\QueryLanguageTests\Test")[0][0];

            Assert.AreEqual(true, resultValue);
        }

        [Test]
        public void Executing_Complex_Or_Expression_With_Calculated_True_Or_False_Values_Works()
        {
            string code = "test = 15 == 15 OR \"First\" == \"Second\"";

            _SyneryClient.Run(GenerateCode(code));

            object resultValue = _Database.LoadTable(@"\QueryLanguageTests\Test")[0][0];

            Assert.AreEqual(true, resultValue);
        }

        [Test]
        public void Executing_Complex_Or_Expression_With_Multiple_Calculated_True_Values_Works()
        {
            string code = "test = 22.99999999M == 22.99999999M OR 15.987654 != 22.1 OR 15 == 15 OR \"Bla\" == \"Bla\" OR \"First\" != \"Second\"";

            _SyneryClient.Run(GenerateCode(code));

            object resultValue = _Database.LoadTable(@"\QueryLanguageTests\Test")[0][0];

            Assert.AreEqual(true, resultValue);
        }

        [Test]
        public void Executing_Complex_Or_Expression_With_Multiple_Calculated_True_Or_False_Values_Works()
        {
            string code = "test = 22.99999999M == 22.99999999M OR 15.987654 == 22.1 OR 15 == 15 OR \"Bla\" == \"Bla\" OR \"First\" != \"Second\"";

            _SyneryClient.Run(GenerateCode(code));

            object resultValue = _Database.LoadTable(@"\QueryLanguageTests\Test")[0][0];

            Assert.AreEqual(true, resultValue);
        }

        [Test]
        public void Executing_Complex_Or_Expression_With_Multiple_Calculated_False_Values_Works()
        {
            string code = "test = 22.7M == 22.99999999M OR 15.987654 == 22.1 OR 22 == 15 OR \"Bla\" != \"Bla\" OR \"First\" == \"Second\"";

            _SyneryClient.Run(GenerateCode(code));

            object resultValue = _Database.LoadTable(@"\QueryLanguageTests\Test")[0][0];

            Assert.AreEqual(false, resultValue);
        }

        #endregion

        #region HELPERS

        private string GenerateCode(string selectStatement, string codeBefore = "", string codeAfter = "")
        {
            return String.Format(@"
{1}

\QueryLanguageTests\Test = 
    FROM \QueryLanguageTests\People AS p
    SELECT {0};

{2}",
                  selectStatement, codeBefore, codeAfter);
        }

        #endregion
    }
}
