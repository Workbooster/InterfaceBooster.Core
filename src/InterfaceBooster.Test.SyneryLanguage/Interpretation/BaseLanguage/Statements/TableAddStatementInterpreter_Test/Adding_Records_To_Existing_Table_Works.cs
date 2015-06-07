using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.SyneryTypes;
using InterfaceBooster.Database.Interfaces.Structure;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Test.SyneryLanguage.Interpretation.BaseLanguage.Statements.TableAddStatementInterpreter_Test
{
    [TestFixture]
    public class Adding_Records_To_Existing_Table_Works : BaseTest
    {
        [SetUp]
        public void SetupSpecificTest()
        {
            // prepare a table to have an existing table

            ISchema schema = _Database.NewSchema();
            schema.AddField<int>("Id");
            schema.AddField<string>("Firstname");
            schema.AddField<string>("Secondname");
            schema.AddField<string>("Lastname");
            schema.AddField<bool>("IsMarried");

            ITable table = _Database.NewTable(schema);
            table.Add(new object[] { 3, "Steve", "Patrik", "Lawrence", false });
            table.Add(new object[] { 9, "Sivlia", null, "Dagustina", true });

            _Database.CreateTable(@"\StatementTest\TableAddToExisting", table);
        }

        [Test]
        public void Adding_Record_With_The_Same_Structure_Works()
        {
            string code = @"
#Person(INT Id, STRING Firstname, STRING Secondname, STRING Lastname, BOOL IsMarried);

#Person mike = #Person(15, ""Mike"", ""Steve"", ""Meyer"", TRUE);

ADD mike TO \StatementTest\TableAddToExisting;
";

            _SyneryClient.Run(code);

            ITable destinationTable = _Database.LoadTable(@"\StatementTest\TableAddToExisting");

            Assert.AreEqual(15, destinationTable[2][0]);
            Assert.AreEqual("Mike", destinationTable[2][1]);
            Assert.AreEqual("Steve", destinationTable[2][2]);
            Assert.AreEqual("Meyer", destinationTable[2][3]);
            Assert.AreEqual(true, destinationTable[2][4]);
        }

        [Test]
        public void Adding_Record_With_Less_Fields_Works()
        {
            string code = @"
#Person(INT Id, STRING Firstname, STRING Lastname);

#Person mike = #Person(15, ""Mike"", ""Meyer"");

ADD mike TO \StatementTest\TableAddToExisting;
";

            _SyneryClient.Run(code);

            ITable destinationTable = _Database.LoadTable(@"\StatementTest\TableAddToExisting");

            // count the number of fields in the schema
            Assert.AreEqual(5, destinationTable.Schema.Fields.Count);

            // count the number of fields in the record
            Assert.AreEqual(5, destinationTable[2].Count());

            Assert.AreEqual(15, destinationTable[2][0]);
            Assert.AreEqual("Mike", destinationTable[2][1]);
            Assert.AreEqual(null, destinationTable[2][2]);
            Assert.AreEqual("Meyer", destinationTable[2][3]);
            Assert.AreEqual(null, destinationTable[2][4]);
        }

        [Test]
        public void Adding_Record_With_More_Fields_Works()
        {
            string code = @"
#Person(INT Id, STRING ContactType, STRING Firstname, STRING Secondname, STRING Lastname, INT NumberOfChildren, BOOL IsMarried);

#Person mike = #Person(15, ""Business"", ""Mike"", ""Steve"", ""Meyer"", 2, TRUE);

ADD mike TO \StatementTest\TableAddToExisting;
";

            _SyneryClient.Run(code);

            ITable destinationTable = _Database.LoadTable(@"\StatementTest\TableAddToExisting");

            // count the number of fields in the schema
            Assert.AreEqual(5, destinationTable.Schema.Fields.Count);

            // count the number of fields in the record
            Assert.AreEqual(5, destinationTable[2].Count());

            Assert.AreEqual(15, destinationTable[2][0]);
            Assert.AreEqual("Mike", destinationTable[2][1]);
            Assert.AreEqual("Steve", destinationTable[2][2]);
            Assert.AreEqual("Meyer", destinationTable[2][3]);
            Assert.AreEqual(true, destinationTable[2][4]);
        }
    }
}
