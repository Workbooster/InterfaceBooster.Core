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
    public class Catching_Invalid_Expressions_Works : QueryLanguageTestBase
    {
        [SetUp]
        public void SetupSpecificTest()
        {
            CreatePeopleTable(_Database);
        }

        [Test]
        public void Casting_String_As_Int_Throws_SyneryQueryInterpretationException()
        {
            string code = GetQuery("test = (INT)\"bla\"");

            Assert.Throws<SyneryQueryInterpretationException>(() => { _SyneryClient.Run(code); });
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
