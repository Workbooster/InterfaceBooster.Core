using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Database.Interfaces.Structure;

namespace InterfaceBooster.Test.SyneryLanguage.Interpretation.QueryLanguage.Commands.RequestSelectCommandInterpreter_Test
{
    [TestFixture]
    public class Selecting_With_Mixed_Select_Items_Works : QueryLanguageTestBase
    {
        private string _Code;

        [SetUp]
        public void SetupSpecificTest()
        {
            _Code = @"
INT testVariable = 15;

\QueryLanguageTests\Test = 
    FROM \QueryLanguageTests\People AS p
    SELECT p.Firstname, VariableTest = testVariable, TestLastname = p.Lastname, p.Size, TestWeight = p.Weight;
";
            CreatePeopleTable(_Database);
        }

        [Test]
        public void SELECT_Command_Loads_All_Rows()
        {
            _SyneryClient.Run(_Code);

            ITable sourceTable = _Database.LoadTable(@"\QueryLanguageTests\People");
            ITable destinationTable = _Database.LoadTable(@"\QueryLanguageTests\Test");

            Assert.AreEqual(sourceTable.Count, destinationTable.Count);
        }

        [Test]
        public void SELECT_Command_Creates_The_Correct_Number_Of_Fields()
        {
            _SyneryClient.Run(_Code);

            ITable sourceTable = _Database.LoadTable(@"\QueryLanguageTests\People");
            ITable destinationTable = _Database.LoadTable(@"\QueryLanguageTests\Test");

            Assert.AreEqual(5, destinationTable.Schema.Fields.Count);
        }

        [Test]
        public void SELECT_Command_Creates_New_Fields_With_Correct_Name()
        {
            _SyneryClient.Run(_Code);

            ITable sourceTable = _Database.LoadTable(@"\QueryLanguageTests\People");
            ITable destinationTable = _Database.LoadTable(@"\QueryLanguageTests\Test");

            Assert.AreEqual("Firstname", destinationTable.Schema.Fields[0].Name);
            Assert.AreEqual("VariableTest", destinationTable.Schema.Fields[1].Name);
            Assert.AreEqual("TestWeight", destinationTable.Schema.Fields[4].Name);
        }

        [Test]
        public void SELECT_Command_Row_Data_Has_The_Correct_Lenght()
        {
            _SyneryClient.Run(_Code);

            ITable sourceTable = _Database.LoadTable(@"\QueryLanguageTests\People");
            ITable destinationTable = _Database.LoadTable(@"\QueryLanguageTests\Test");

            Assert.AreEqual(5, destinationTable[0].Length);
        }

        [Test]
        public void SELECT_Command_Result_Data_Content_Is_Correct()
        {
            _SyneryClient.Run(_Code);

            ITable sourceTable = _Database.LoadTable(@"\QueryLanguageTests\People");
            ITable destinationTable = _Database.LoadTable(@"\QueryLanguageTests\Test");

            Assert.AreEqual("Roger", destinationTable[0][0]);
            Assert.AreEqual(15, destinationTable[0][1]);
            Assert.AreEqual(81.3M, destinationTable[0][4]);
        }
    }
}
