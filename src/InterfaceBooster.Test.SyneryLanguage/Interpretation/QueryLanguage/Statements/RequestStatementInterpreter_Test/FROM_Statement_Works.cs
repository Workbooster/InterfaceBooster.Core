using InterfaceBooster.Database.Interfaces.Structure;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Test.SyneryLanguage.Interpretation.QueryLanguage.Statements.RequestStatementInterpreter_Test
{
    [TestFixture]
    public class FROM_Statement_Works : QueryLanguageTestBase
    {
        [SetUp]
        public void SetupSpecificTest()
        {
            CreatePeopleTable(_Database);
        }

        [Test]
        public void FROM_Command_Without_AS_Copies_Table_To_New_Destination()
        {
            string code = @"\QueryLanguageTests\CopyOfPeople = FROM \QueryLanguageTests\People;";

            _SyneryClient.Run(code);

            ITable sourceTable = _Database.LoadTable(@"\QueryLanguageTests\People");
            ITable destinationTable = _Database.LoadTable(@"\QueryLanguageTests\CopyOfPeople");

            Assert.AreEqual(sourceTable.Count, destinationTable.Count);
            Assert.AreEqual(sourceTable.Schema.Fields.Count, destinationTable.Schema.Fields.Count);
            Assert.AreEqual(sourceTable.Schema.Fields[0].Name, destinationTable.Schema.Fields[0].Name);
        }

        [Test]
        public void FROM_Command_With_AS_Copies_Table_To_New_Destination()
        {
            string code = @"\QueryLanguageTests\CopyOfPeople = FROM \QueryLanguageTests\People AS p;";

            _SyneryClient.Run(code);

            ITable sourceTable = _Database.LoadTable(@"\QueryLanguageTests\People");
            ITable destinationTable = _Database.LoadTable(@"\QueryLanguageTests\CopyOfPeople");

            Assert.AreEqual(sourceTable.Count, destinationTable.Count);
            Assert.AreEqual(sourceTable.Schema.Fields.Count, destinationTable.Schema.Fields.Count);
            Assert.AreEqual(sourceTable.Schema.Fields[0].Name, destinationTable.Schema.Fields[0].Name);
        }
    }
}
