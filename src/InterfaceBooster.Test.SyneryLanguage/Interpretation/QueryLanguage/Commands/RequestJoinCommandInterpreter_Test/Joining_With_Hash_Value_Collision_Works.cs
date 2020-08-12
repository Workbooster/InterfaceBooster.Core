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
    public class Joining_With_Hash_Value_Collision_Works : QueryLanguageTestBase
    {
        /// <summary>
        /// We've detected a bug in ObjectArrayEqualityComparer.GetHashCode() where these two arrays would give a match with the JOIN statement in Synery:
        ///     var one = new object[] { 1212359, 240 };
        ///     var two = new object[] { 1212369, 10 };
        /// The reason is a weaknes in the formula for calculating hash values that leads to the same hash value (=27893294) for both arrays.
        /// This test is to proof that the bug doesn't exists anymore.
        /// </summary>
        [Test]
        public void JOIN_Integer_Field_Pair_With_Same_Hash_Values()
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

        /// <summary>
        /// We've detected a bug in ObjectArrayEqualityComparer.Equals() where some different strings would give a match with the JOIN statement in Synery:
        /// For example in .NET 4.5 both strings "32150 180" and "33100 100" return the same hash value of 1934329864.
        /// </summary>
        [Test]
        public void JOIN_String_With_Same_Hash_Values()
        {
            string code = @"
DROP \test\result;
DROP \test\pos;
DROP \test\Articles;

#Position(STRING ArticleId, STRING Name, DECIMAL Amount);
#Article(STRING ArticleId, STRING Name);

ADD #Position(ArticleId = ""32150 180"", Name = ""SEVEN"", Amount = 7.0M) TO \test\pos;
ADD #Position(ArticleId = ""32150 880"", Name = ""TWO"", Amount = 2.0M) TO \test\pos;


ADD #Article(ArticleId = ""32150 180"", Name = ""SEVEN"") TO \test\Articles;
ADD #Article(ArticleId = ""32150 880"", Name = ""TWO"") TO \test\Articles;
ADD #Article(ArticleId = ""33100 100"", Name = ""WRONG 1"") TO \test\Articles;
ADD #Article(ArticleId = ""33100 800"", Name = ""WRONG 2"") TO \test\Articles;

\test\result =
    FROM \test\pos AS pos
    JOIN \test\Articles AS art   COMPARE art.ArticleId TO pos.ArticleId
        SELECT 
        pos.*,
        art.*
    ;
";

            _SyneryClient.Run(code);

            ITable destinationTable = _Database.LoadTable(@"\test\result");

            Assert.AreEqual(2, destinationTable.Count);
        }
    }
}
