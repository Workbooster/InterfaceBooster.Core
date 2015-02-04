using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Database.Interfaces.Structure;
using InterfaceBooster.Common.Tools.Data.Array;

namespace InterfaceBooster.Test.SyneryLanguage.Interpretation.QueryLanguage.Commands.RequestJoinCommandInterpreter_Test
{
    [TestFixture]
    public class Joining_With_NULL_Values_Works : QueryLanguageTestBase
    {
        [Test]
        public void JOIN_Command_Loads_All_Rows()
        {
            // create test data

            CreatePeopleTable(_Database);
            CreateRegistrationsTable(_Database);

            // run the test

            string code = @"
\QueryLanguageTests\Test = 
    FROM \QueryLanguageTests\Registrations AS r 
    JOIN \QueryLanguageTests\People AS p COMPARE p.Id, p.DateOfDeath TO r.IdPerson, NULL
    SELECT IdPerson = p.Id, IdRegistration = r.Id, Fullname = p.Firstname + "" "" + p.Lastname, r.DateOfRegistration;
";

            _SyneryClient.Run(code);

            // get expected result for validating the test result

            ITable outerTable = _Database.LoadTable(@"\QueryLanguageTests\Registrations");
            ITable innerTable = _Database.LoadTable(@"\QueryLanguageTests\People");

            IEnumerable<object[]> expectedResult = outerTable.Join(innerTable,
                o => new object[] { o[1], null },
                i => new object[] { i[0], i[10]},
                (o, i) =>
                {
                    return new object[] {
                    i[0], 
                    o[0],
                    (string)i[1] + " " + (string)i[2],
                    o[3]
                };
                },
                new ObjectArrayEqualityComparer());

            ITable destinationTable = _Database.LoadTable(@"\QueryLanguageTests\Test");

            Assert.AreEqual(expectedResult.Count(), destinationTable.Count);
        }
    }
}
