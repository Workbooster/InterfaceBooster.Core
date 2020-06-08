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
        public void DISTINCT_With_Same_Hash_Values()
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
    }
}
