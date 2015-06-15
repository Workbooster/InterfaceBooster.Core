using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Test.SyneryLanguage.Interpretation.QueryLanguage.Expressions.RequestExpressionInterpreter_Test
{
    [TestFixture]
    public class Executing_Ternary_Conditional_Expression_Works : QueryLanguageTestBase
    {
        [SetUp]
        public void SetupSpecificTest()
        {
            CreatePeopleTable(_Database);
        }

        [Test]
        public void Executing_Condition_With_Boolean_Comparison_Works()
        {
            string code = "test = p.IsMale == TRUE ? \"Mister\" : \"Miss\"";

            _SyneryClient.Run(GenerateCode(code));

            object resultValue = _Database.LoadTable(@"\QueryLanguageTests\Test")[0][0];

            Assert.AreEqual("Mister", resultValue);
        }

        [Test]
        public void Executing_Condition_With_Multiple_Comparisons_Works()
        {
            string code = "test = p.IsMale == TRUE AND p.Lastname == \"Guillet\" ? \"Hello\" : \"Dear\"";

            _SyneryClient.Run(GenerateCode(code));

            object resultValue = _Database.LoadTable(@"\QueryLanguageTests\Test")[0][0];

            Assert.AreEqual("Hello", resultValue);
        }

        [Test]
        public void Executing_Condition_With_Field_Or_Constant_Value_Options_Works()
        {
            // Test after Bugfix:
            // The bug occured if the options in a ternary operation came from a different source.
            // In that case an Exception "Argument types do not match" is thrown.

            // For that reason this test uses the constant "Super!" and the field "p.Lastname" as trueValue/falseValue.

            string code = "test = p.Lastname == \"Guillet\" ? \"Super!\" : p.Lastname";

            _SyneryClient.Run(GenerateCode(code));

            object resultValue = _Database.LoadTable(@"\QueryLanguageTests\Test")[0][0];
            object resultValue2 = _Database.LoadTable(@"\QueryLanguageTests\Test")[1][0];

            Assert.AreEqual("Super!", resultValue);
            Assert.AreEqual("Bloch", resultValue2);
        }

        [Test]
        public void Executing_Condition_With_Field_Or_NULL_Value_Options_Works()
        {
            string code = "test = p.Lastname != \"Guillet\" ? NULL : p.Lastname";

            _SyneryClient.Run(GenerateCode(code));

            object resultValue = _Database.LoadTable(@"\QueryLanguageTests\Test")[0][0];
            object resultValue2 = _Database.LoadTable(@"\QueryLanguageTests\Test")[1][0];

            Assert.AreEqual("Guillet", resultValue);
            Assert.AreEqual(null, resultValue2);
        }

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
