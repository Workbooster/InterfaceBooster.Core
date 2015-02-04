using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Database.Interfaces.Structure;

namespace InterfaceBooster.Test.SyneryLanguage.Interpretation.QueryLanguage.Commands.RequestWhereCommandInterpreter_Test
{
    [TestFixture]
    public class Filtering_With_NULL_Works : QueryLanguageTestBase
    {
        [Test]
        public void WHERE_Command_Filters_Rows_With_NULL_Value()
        {
            // create test data

            CreatePeopleTable(_Database);

            // run the test

            string code = @"
\QueryLanguageTests\Test = 
    FROM \QueryLanguageTests\People AS p
    WHERE p.DateOfDeath == NULL;
";

            _SyneryClient.Run(code);

            // get expected result for validating the test result

            ITable peopleTable = _Database.LoadTable(@"\QueryLanguageTests\People");

            IEnumerable<object[]> expectedResult = peopleTable.Where(x => x[10] == null);

            ITable destinationTable = _Database.LoadTable(@"\QueryLanguageTests\Test");

            Assert.AreEqual(expectedResult.Count(), destinationTable.Count);
        }

        [Test]
        public void WHERE_Command_Filters_Rows_With_No_NULL_Value()
        {
            // create test data

            CreatePeopleTable(_Database);

            // run the test

            string code = @"
\QueryLanguageTests\Test = 
    FROM \QueryLanguageTests\People AS p
    WHERE p.DateOfDeath != NULL;
";

            _SyneryClient.Run(code);

            // get expected result for validating the test result

            ITable peopleTable = _Database.LoadTable(@"\QueryLanguageTests\People");

            IEnumerable<object[]> expectedResult = peopleTable.Where(x => x[10] != null);

            ITable destinationTable = _Database.LoadTable(@"\QueryLanguageTests\Test");

            Assert.AreEqual(expectedResult.Count(), destinationTable.Count);
        }
    }
}
