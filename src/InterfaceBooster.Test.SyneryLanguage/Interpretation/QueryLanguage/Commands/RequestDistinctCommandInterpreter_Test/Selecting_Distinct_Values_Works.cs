using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Database.Interfaces.Structure;
using InterfaceBooster.Common.Tools.Data.Array;

namespace InterfaceBooster.Test.SyneryLanguage.Interpretation.QueryLanguage.Commands.RequestDistinctCommandInterpreter_Test
{
    public class Selecting_Distinct_Values_Works : QueryLanguageTestBase
    {
        [Test]
        public void DISTINCT_Integer_Field_Pair_With_Same_Hash_Values()
        {
            // create table with values

            ISchema schema = _Database.NewSchema();
            schema.AddField<int>("KeyOne");
            schema.AddField<int>("KeyTwo");

            List<object[]> data = new List<object[]>
            {
                // these value-pairs get the same GetHashCode()-Value
                new object[] { 1212369, 10, },
                new object[] { 1212359, 240, },

                // add some more doublettes
                new object[] { 1212369, 10, },
                new object[] { 1212369, 10, },
                new object[] { 1212359, 240, },
                new object[] { 1212359, 240, },
                new object[] { 1212369, 10, },
                new object[] { 1212359, 240, },
            };

            ITable table = _Database.NewTable(schema, data);

            _Database.CreateTable(@"\SameHashCheck\First", table);

            // run the test

            string code = @"
\SameHashCheck\Test = 
    FROM \SameHashCheck\First AS f
    DISTINCT;
";

            _SyneryClient.Run(code);

            ITable destinationTable = _Database.LoadTable(@"\SameHashCheck\Test");

            Assert.AreEqual(2, destinationTable.Count);
        }

        /// <summary>
        /// We've detected another bug in ObjectArrayEqualityComparer.Equals() where these two arrays would give a match with the JOIN statement in Synery:
        /// In .NET 4.5 both strings "32150 180" and "33100 100" return the same hash value of 1934329864
        /// </summary>
        [Test]
        public void DISTINCT_String_With_Same_Hash_Values()
        {

            // run the test

            string code = @"
DROP \test\result;
DROP \test\Articles;

#Article(STRING ArticleId);

ADD #Article(ArticleId = ""32150 180"") TO \test\Articles;
ADD #Article(ArticleId = ""32150 180"") TO \test\Articles; // duplicate
ADD #Article(ArticleId = ""32150 180"") TO \test\Articles; // duplicate
ADD #Article(ArticleId = ""32150 880"") TO \test\Articles;
ADD #Article(ArticleId = ""33100 100"") TO \test\Articles;
ADD #Article(ArticleId = ""33100 800"") TO \test\Articles;

\test\result =
    FROM \test\Articles AS art
    DISTINCT;
";

            _SyneryClient.Run(code);

            ITable destinationTable = _Database.LoadTable(@"\test\result");

            Assert.AreEqual(4, destinationTable.Count);
        }
    }
}
