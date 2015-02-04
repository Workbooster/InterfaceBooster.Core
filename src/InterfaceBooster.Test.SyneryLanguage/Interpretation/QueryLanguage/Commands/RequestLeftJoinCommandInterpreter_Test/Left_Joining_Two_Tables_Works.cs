using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Database.Interfaces.Structure;

namespace InterfaceBooster.Test.SyneryLanguage.Interpretation.QueryLanguage.Commands.RequestLeftJoinCommandInterpreter_Test
{
    [TestFixture]
    public class Left_Joining_Two_Tables_Works : QueryLanguageTestBase
    {
        [Test]
        public void LEFT_JOIN_Command_Loads_All_Rows()
        {
            // create test data

            CreatePeopleTable(_Database);
            CreateRegistrationsTable(_Database);

            // run the test

            string code = @"
\QueryLanguageTests\Test = 
    FROM \QueryLanguageTests\People AS p
    LEFT JOIN \QueryLanguageTests\Registrations AS r COMPARE r.IdPerson TO p.Id
    SELECT IdPerson = p.Id, IdRegistration = r.Id, Fullname = p.Firstname + "" "" + p.Lastname, r.DateOfRegistration;
";

            _SyneryClient.Run(code);

            // get expected result for validating the test result

            ITable outerTable = _Database.LoadTable(@"\QueryLanguageTests\People");
            ITable innerTable = _Database.LoadTable(@"\QueryLanguageTests\Registrations");

            // calculate the expected number of rows by grouping the rows from the inner table to the rows of the outer table

            var grouped = outerTable.GroupJoin(innerTable, o => o[0], i => i[1], (o, i) => new KeyValuePair<object[], IEnumerable<object[]>>(o, i));
            
            int expectedNumberOfRows = 0;

            foreach (var item in grouped)
            {
                if (item.Value.Count() == 0)
                {
                    // there should be at least one row from the outer table
                    // even if no related entries from the inner table are found
                    expectedNumberOfRows++;
                }
                else
                {
                    // if there are related entries from the inner table count these
                    expectedNumberOfRows += item.Value.Count();
                }
            }


            ITable destinationTable = _Database.LoadTable(@"\QueryLanguageTests\Test");

            Assert.AreEqual(expectedNumberOfRows, destinationTable.Count);
        }
    }
}
