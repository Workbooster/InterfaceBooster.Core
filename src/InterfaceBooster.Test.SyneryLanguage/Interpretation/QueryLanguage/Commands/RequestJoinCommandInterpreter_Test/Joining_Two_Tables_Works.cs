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
    public class Joining_Two_Tables_Works : QueryLanguageTestBase
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
    FROM \QueryLanguageTests\People AS p
    JOIN \QueryLanguageTests\Registrations AS r COMPARE r.IdPerson TO p.Id
    SELECT IdPerson = p.Id, IdRegistration = r.Id, Fullname = p.Firstname + "" "" + p.Lastname, r.DateOfRegistration;
";

            _SyneryClient.Run(code);

            // get expected result for validating the test result

            ITable outerTable = _Database.LoadTable(@"\QueryLanguageTests\People");
            ITable innerTable = _Database.LoadTable(@"\QueryLanguageTests\Registrations");

            IEnumerable<object[]> result = outerTable.Join(innerTable,
                o => o[0],
                i => i[1],
                (o, i) =>
                {
                    return new object[] {
                    o[0],
                    i[0],
                    (string)o[1] + " " + (string)o[2],
                    i[3]
                };
                });

            ITable destinationTable = _Database.LoadTable(@"\QueryLanguageTests\Test");

            Assert.AreEqual(result.Count(), destinationTable.Count);
        }

        [Test]
        public void JOIN_With_Null_Values_Works()
        {
            // create outer table (First)

            ISchema schema = _Database.NewSchema();
            schema.AddField<int>("Id");
            schema.AddField<int>("IdSecond");
            schema.AddField<string>("NullValue");

            List<object[]> data = new List<object[]>
            {
                new object[] { 5, 2, null },
                new object[] { 10, 4, null },
                new object[] { 15, 4, null },
            };

            ITable table = _Database.NewTable(schema, data);

            _Database.CreateTable(@"\NullJoinTest\First", table);

            // create inner table (Second)

            ISchema schema2 = _Database.NewSchema();
            schema2.AddField<int>("Id");
            schema2.AddField<string>("NullValue");

            List<object[]> data2 = new List<object[]>
            {
                new object[] { 2, null },
                new object[] { 4, null },
            };

            ITable table2 = _Database.NewTable(schema2, data2);

            _Database.CreateTable(@"\NullJoinTest\Second", table2);

            // run the test

            string code = @"
\QueryLanguageTests\Test = 
    FROM \NullJoinTest\First AS f
    JOIN \NullJoinTest\Second AS s COMPARE s.Id, s.NullValue TO f.IdSecond, f.NullValue
    SELECT IdFirst = f.Id, IdSecond = f.Id;
";

            _SyneryClient.Run(code);

            // get expected result for validating the test result


            ITable outerTable = _Database.LoadTable(@"\NullJoinTest\First");
            ITable innerTable = _Database.LoadTable(@"\NullJoinTest\Second");

            IEnumerable<object[]> result = outerTable.Join(innerTable,
                o => { return new object[] { o[1], o[2] }; },
                i => { return new object[] { i[0], i[1] }; },
                (o, i) =>
                {
                    return new object[] {
                    o[0],
                    i[0],
                };
                }, new ObjectArrayEqualityComparer());

            ITable destinationTable = _Database.LoadTable(@"\QueryLanguageTests\Test");

            Assert.AreEqual(result.Count(), destinationTable.Count);
        }

        /// <summary>
        /// 
        /// </summary>
        [Test]
        public void JOIN_With_Integer_to_String_Conversion()
        {
            string code = @"
DROP \test\result;
DROP \test\pos;
DROP \test\Articles;

#Position(STRING ArticleId, STRING Name, DECIMAL Amount);
#Article(INT ArticleId, STRING Name);

ADD #Position(ArticleId = ""32150180"", Name = ""ONE"", Amount = 7.0M) TO \test\pos;
ADD #Position(ArticleId = ""32150880"", Name = ""TWO"", Amount = 2.0M) TO \test\pos;


ADD #Article(ArticleId = 32150180, Name = ""ONE"") TO \test\Articles;
ADD #Article(ArticleId = 32150880, Name = ""TWO"") TO \test\Articles;
ADD #Article(ArticleId = 33100100, Name = ""WRONG 1"") TO \test\Articles;
ADD #Article(ArticleId = 33100800, Name = ""WRONG 2"") TO \test\Articles;

\test\result =
    FROM \test\pos AS pos
    JOIN \test\Articles AS art   COMPARE ((STRING)art.ArticleId) TO pos.ArticleId
        SELECT 
        pos.*,
        art.*
    ;
";

            _SyneryClient.Run(code);

            ITable destinationTable = _Database.LoadTable(@"\test\result");

            Assert.AreEqual(2, destinationTable.Count);
        }

        [Test]
        public void JOIN_With_Value_Pair()
        {
            string code = @"
DROP \test\result;
DROP \test\pos;
DROP \test\Movements;

#Position(INT DocumentId, STRING MovementId, STRING Name, DECIMAL Amount);
#Movement(INT DocumentId, STRING MovementId, STRING Name);

ADD #Position(DocumentId = 11, MovementId = ""32150 180"", Name = ""FIRST"", Amount = 7.0M) TO \test\pos;
ADD #Position(DocumentId = 22, MovementId = ""32150 880"", Name = ""SECOND"", Amount = 2.0M) TO \test\pos;


ADD #Movement(DocumentId = 11, MovementId = ""32150 180"", Name = ""FIRST-ONE"") TO \test\Movements;
ADD #Movement(DocumentId = 11, MovementId = ""32150 180"", Name = ""FIRST-TWO"") TO \test\Movements;
ADD #Movement(DocumentId = 11, MovementId = ""32150 180"", Name = ""FIRST-THREE"") TO \test\Movements;
ADD #Movement(DocumentId = 22, MovementId = ""32150 880"", Name = ""SECOND-ONE"") TO \test\Movements;
ADD #Movement(DocumentId = 22, MovementId = ""32150 880"", Name = ""SECOND-TWO"") TO \test\Movements;
ADD #Movement(DocumentId = 11, MovementId = ""33100 100"", Name = ""WRONG 1"") TO \test\Movements;
ADD #Movement(DocumentId = 11, MovementId = ""33100 800"", Name = ""WRONG 2"") TO \test\Movements;
ADD #Movement(DocumentId = 22, MovementId = ""32150 180"", Name = ""WRONG 3"") TO \test\Movements;

\test\result =
    FROM \test\pos AS pos
    JOIN \test\Movements AS art   COMPARE art.DocumentId, art.MovementId TO pos.DocumentId, pos.MovementId
        SELECT 
        pos.*,
        art.*
    ;
";

            _SyneryClient.Run(code);

            ITable destinationTable = _Database.LoadTable(@"\test\result");

            Assert.AreEqual(5, destinationTable.Count);
        }
    }
}
