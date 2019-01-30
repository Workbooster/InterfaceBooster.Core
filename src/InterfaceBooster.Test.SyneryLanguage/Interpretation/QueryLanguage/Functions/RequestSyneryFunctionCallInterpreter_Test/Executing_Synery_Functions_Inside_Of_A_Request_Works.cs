using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Database.Interfaces.Structure;
using InterfaceBooster.Common.Tools.Data.Array;

namespace InterfaceBooster.Test.SyneryLanguage.Interpretation.QueryLanguage.Functions.RequestSyneryFunctionCallInterpreter_Test
{
    [TestFixture]
    public class Executing_Synery_Functions_Inside_Of_A_Request_Works : QueryLanguageTestBase
    {
        [SetUp]
        public void SetupSpecificTest()
        {
            CreatePeopleTable(_Database);
        }

        [Test]
        public void Executing_Function_With_One_Parameter_Returns_Expected_Result()
        {
            string code = @"
STRING getResult(STRING text)
    STRING returnValue;

    returnValue = ""Result: "" + text;

    RETURN returnValue;
END

\QueryLanguageTests\Test = 
    FROM \QueryLanguageTests\People AS p
    SELECT result = getResult(p.Firstname);
";

            _SyneryClient.Run(code);

            ITable sourceTable = _Database.LoadTable(@"\QueryLanguageTests\People");
            ITable destinationTable = _Database.LoadTable(@"\QueryLanguageTests\Test");

            string expectedResult = String.Format("Result: {0}", sourceTable[0][1]);

            Assert.AreEqual(expectedResult, destinationTable[0][0]);
        }

        [Test]
        public void Executing_Function_With_Two_Parameter_Returns_Expected_Result()
        {
            string code = @"
STRING getResult(STRING text, INT second)
    STRING returnValue;

    returnValue = ""Result: "" + text + ((STRING)second);

    RETURN returnValue;
END

\QueryLanguageTests\Test = 
    FROM \QueryLanguageTests\People AS p
    SELECT result = getResult(p.Firstname, 100);
";

            _SyneryClient.Run(code);

            ITable sourceTable = _Database.LoadTable(@"\QueryLanguageTests\People");
            ITable destinationTable = _Database.LoadTable(@"\QueryLanguageTests\Test");

            string expectedResult = String.Format("Result: {0}100", sourceTable[0][1]);

            Assert.AreEqual(expectedResult, destinationTable[0][0]);
        }

        [Test]
        public void Executing_Function_With_One_Parameter_From_An_Expression_Returns_Expected_Result()
        {
            string code = @"
STRING getResult(STRING text)
    STRING returnValue;

    returnValue = ""Result: "" + text;

    RETURN returnValue;
END

\QueryLanguageTests\Test = 
    FROM \QueryLanguageTests\People AS p
    SELECT result = getResult(p.Firstname + "" "" + p.Lastname);
";

            _SyneryClient.Run(code);

            ITable sourceTable = _Database.LoadTable(@"\QueryLanguageTests\People");
            ITable destinationTable = _Database.LoadTable(@"\QueryLanguageTests\Test");

            string expectedResult = String.Format("Result: {0} {1}", sourceTable[0][1], sourceTable[0][2]);

            Assert.AreEqual(expectedResult, destinationTable[0][0]);
        }

        [Test]
        public void Executing_Function_With_Two_Parameters_One_With_Default_Value_Returns_Expected_Result()
        {
            string code = @"
STRING getResult(STRING text, STRING label = ""Default Label: "")
    STRING returnValue;

    returnValue = label + text;

    RETURN returnValue;
END

\QueryLanguageTests\Test = 
    FROM \QueryLanguageTests\People AS p
    SELECT result = getResult(p.Firstname);
";

            _SyneryClient.Run(code);

            ITable sourceTable = _Database.LoadTable(@"\QueryLanguageTests\People");
            ITable destinationTable = _Database.LoadTable(@"\QueryLanguageTests\Test");

            string expectedResult = String.Format("Default Label: {0}", sourceTable[0][1]);

            Assert.AreEqual(expectedResult, destinationTable[0][0]);
        }

        [Test]
        public void Executing_Function_With_One_Parameters_And_Some_Logic_Returns_Expected_Result()
        {
            string code = @"
BOOL getResult(INT flag)
    BOOL returnValue;

    IF flag == 0
        returnValue = FALSE;
    ELSE
        returnValue = TRUE;
    END

    RETURN returnValue;
END

\QueryLanguageTests\Test = 
    FROM \QueryLanguageTests\People AS p
    SELECT result = getResult(p.NumberOfChildren);
";

            _SyneryClient.Run(code);

            ITable sourceTable = _Database.LoadTable(@"\QueryLanguageTests\People");
            ITable destinationTable = _Database.LoadTable(@"\QueryLanguageTests\Test");

            Assert.AreEqual(true, destinationTable[0][0]);
            Assert.AreEqual(false, destinationTable[2][0]);
        }

        [Test]
        public void Executing_Function_With_One_Parameters_From_A_JOIN_Operation_Returns_Expected_Result()
        {
            /*
             * Description:
             * This test calls the synery function isDeath() from the key-comparison in a JOIN-command.
             * The isDeath()-function returns TRUE if the given date isn't NULL. The key-comparison of the JOIN-command
             * expects the result of the function to be FALSE (people that aren't death).
             * 
             * The table "\QueryLanguageTests\Test" should finally contain all registrations of people that arn't death.
             */


            // create registration test data

            CreateRegistrationsTable(_Database);

            // run the test

            string code = @"
BOOL isDeath(DATETIME dateOfDeath)
    BOOL returnValue;

    IF dateOfDeath == NULL
        returnValue = FALSE;
    ELSE
        returnValue = TRUE;
    END

    RETURN returnValue;
END

\QueryLanguageTests\Test = 
    FROM \QueryLanguageTests\Registrations AS r 
    JOIN \QueryLanguageTests\People AS p COMPARE p.Id, isDeath(p.DateOfDeath) TO r.IdPerson, FALSE
    SELECT IdPerson = p.Id, IdRegistration = r.Id, Fullname = p.Firstname + "" "" + p.Lastname, r.DateOfRegistration;
";

            _SyneryClient.Run(code);

            // get expected result for validating the test result

            ITable outerTable = _Database.LoadTable(@"\QueryLanguageTests\Registrations");
            ITable innerTable = _Database.LoadTable(@"\QueryLanguageTests\People");

            IEnumerable<object[]> expectedResult = outerTable.Join(innerTable,
                o => new object[] { o[1], false },
                i => new object[] { i[0], i[10] == null ? false : true },
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
