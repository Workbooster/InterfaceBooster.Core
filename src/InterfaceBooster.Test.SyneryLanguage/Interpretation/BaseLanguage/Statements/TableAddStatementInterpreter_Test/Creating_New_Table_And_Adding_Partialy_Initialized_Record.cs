using InterfaceBooster.Database.Interfaces.Structure;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Test.SyneryLanguage.Interpretation.BaseLanguage.Statements.TableAddStatementInterpreter_Test
{
    public class Creating_New_Table_And_Adding_Partialy_Initialized_Record : BaseTest
    {
        private string _Code;

        [SetUp]
        public void SetupSpecificTest()
        {
            _Code = @"
#Person(INT Id, STRING Firstname, STRING Lastname);

// only initialize the Id
#Person mike = #Person(15);

ADD mike TO \StatementTest\TableAdd;
";

        }

        [Test]
        public void New_Table_Is_Created()
        {
            _SyneryClient.Run(_Code);

            Assert.IsTrue(_Database.IsTable(@"\StatementTest\TableAdd"));
        }

        [Test]
        public void Table_Contains_One_Row()
        {
            _SyneryClient.Run(_Code);

            ITable destinationTable = _Database.LoadTable(@"\StatementTest\TableAdd");

            Assert.AreEqual(1, destinationTable.Count);
        }

        [Test]
        public void Row_Data_Are_Complete()
        {
            _SyneryClient.Run(_Code);

            ITable destinationTable = _Database.LoadTable(@"\StatementTest\TableAdd");

            Assert.AreEqual(15, destinationTable[0][0]);
            Assert.AreEqual(null, destinationTable[0][1]);
            Assert.AreEqual(null, destinationTable[0][2]);
        }
    }
}
