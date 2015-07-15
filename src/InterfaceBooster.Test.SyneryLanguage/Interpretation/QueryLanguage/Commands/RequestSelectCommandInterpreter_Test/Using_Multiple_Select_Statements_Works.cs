using InterfaceBooster.Database.Interfaces.Structure;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Test.SyneryLanguage.Interpretation.QueryLanguage.Commands.RequestSelectCommandInterpreter_Test
{
    [TestFixture]
    public class Using_Multiple_Select_Statements_Works : QueryLanguageTestBase
    {
        private string _Code;

        [SetUp]
        public void SetupSpecificTest()
        {
            _Code = @"
INT VariableTest = 15;

\QueryLanguageTests\Test = 
    FROM \QueryLanguageTests\People AS p
    SELECT p.Firstname, VariableTest = VariableTest + 5, TestLastname = p.Lastname
    DISTINCT
    SELECT Firstname, VariableTest, TestLastname;
";
            CreatePeopleTable(_Database);
        }

        [Test]
        public void Schema_Contains_Expected_Fields()
        {
            _SyneryClient.Run(_Code);

            ITable destinationTable = _Database.LoadTable(@"\QueryLanguageTests\Test");

            Assert.AreEqual(3, destinationTable.Schema.Fields.Count);
            Assert.AreEqual(1, destinationTable.Schema.Fields.Count(f => f.Name == "Firstname"));
            Assert.AreEqual(1, destinationTable.Schema.Fields.Count(f => f.Name == "VariableTest"));
            Assert.AreEqual(1, destinationTable.Schema.Fields.Count(f => f.Name == "TestLastname"));

        }
    }
}
