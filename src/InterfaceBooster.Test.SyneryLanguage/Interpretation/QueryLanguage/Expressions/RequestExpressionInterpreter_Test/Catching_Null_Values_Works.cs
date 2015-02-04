using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.ErrorHandling;

namespace InterfaceBooster.Test.SyneryLanguage.Interpretation.QueryLanguage.Expressions.RequestExpressionInterpreter_Test
{
    [TestFixture]
    public class Catching_Null_Values_Works : QueryLanguageTestBase
    {
        [SetUp]
        public void SetupSpecificTest()
        {
            CreatePeopleTable(_Database);
        }

        [Test]
        public void Executing_Equality_Comparison_With_NULL_Works()
        {
            string code = GetQuery("test = p.DateOfDeath == NULL, p.DateOfDeath");

            _SyneryClient.Run(code);

            object resultValue = _Database.LoadTable(@"\QueryLanguageTests\Test")[0][0];

            Assert.AreEqual(true, resultValue);
        }

        [Test]
        public void Zero_Does_Not_Equal_To_NULL()
        {
            string code = GetQuery("test = 0 == NULL");

            _SyneryClient.Run(code);

            object resultValue = _Database.LoadTable(@"\QueryLanguageTests\Test")[0][0];

            Assert.AreEqual(false, resultValue);
        }

        [Test]
        public void Empty_String_Does_Not_Equal_To_NULL()
        {
            string code = GetQuery("test = \"\" == NULL");

            _SyneryClient.Run(code);

            object resultValue = _Database.LoadTable(@"\QueryLanguageTests\Test")[0][0];

            Assert.AreEqual(false, resultValue);
        }

        #region HELPERS

        private string GetQuery(string selectStatement)
        {
            string code = String.Format(@"
\QueryLanguageTests\Test = 
    FROM \QueryLanguageTests\People AS p
    SELECT {0};
",
                  selectStatement);

            return code;
        }

        #endregion
    }
}
