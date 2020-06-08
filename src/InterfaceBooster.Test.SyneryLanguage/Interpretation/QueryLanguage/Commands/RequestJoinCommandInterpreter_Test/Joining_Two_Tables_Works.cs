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
        /// We've detected a bug in ObjectArrayEqualityComparer.GetHashCode() where these two arrays would give a match with the JOIN statement in Synery:
        ///     var one = new object[] { 1212359, 240 };
        ///     var two = new object[] { 1212369, 10 };
        /// The reason is a weaknes in the formula for calculating hash values that leads to the same hash value (=27893294) for both arrays.
        /// This test is to proof that the bug doesn't exists anymore.
        /// </summary>
        [Test]
        public void JOIN_With_Same_Hash_Values()
        {
            // create outer table (First)

            ISchema schema = _Database.NewSchema();
            schema.AddField<int>("KeyOne");
            schema.AddField<int>("KeyTwo");
            schema.AddField<string>("Value");

            List<object[]> data = new List<object[]>
            {
                new object[] { 1212369, 10, "First Bla" },
                new object[] { 1212700, 10, "First Aha" },
                new object[] { 1212700, 20, "First Soso" },
            };

            ITable table = _Database.NewTable(schema, data);

            _Database.CreateTable(@"\SameHashCheck\First", table);

            // create inner table (Second)

            ISchema schema2 = _Database.NewSchema();
            schema2.AddField<int>("KeyOne");
            schema2.AddField<int>("KeyTwo");
            schema2.AddField<string>("Value");

            List<object[]> data2 = new List<object[]>
            {
                new object[] { 55999, 10, "Second Huhu" },
                new object[] { 66999, 20, "Second Haha" },
                new object[] { 1212369, 10, "Second Bla" },
                new object[] { 1212359, 240, "Second NOT OK!" },
            };

            ITable table2 = _Database.NewTable(schema2, data2);

            _Database.CreateTable(@"\SameHashCheck\Second", table2);

            // run the test

            string code = @"
\SameHashCheck\Test = 
    FROM \SameHashCheck\First AS f
    JOIN \SameHashCheck\Second AS s COMPARE s.KeyOne, s.KeyTwo TO f.KeyOne, f.KeyTwo
    SELECT ValueFirst = f.Value, ValueSecond = f.Value;
";

            _SyneryClient.Run(code);

            ITable destinationTable = _Database.LoadTable(@"\SameHashCheck\Test");

            Assert.AreEqual(1, destinationTable.Count);
        }
    }
}
