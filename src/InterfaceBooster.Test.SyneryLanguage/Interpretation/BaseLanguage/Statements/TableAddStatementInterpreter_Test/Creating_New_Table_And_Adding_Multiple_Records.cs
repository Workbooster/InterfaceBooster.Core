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
    public class Creating_New_Table_And_Adding_Multiple_Records : BaseTest
    {
        [Test]
        public void Table_Contains_All_Rows()
        {
            string code = @"
#Person(INT Id, STRING Firstname, STRING Lastname);

#Person mike = #Person(15, ""Mike"", ""Meyer"");
#Person debby = #Person(22, ""Debby"", ""Smith"");

ADD mike, debby TO \StatementTest\TableAddMultipleRecords;
ADD #Person(27, ""Susan"", ""Miller""), #Person(33, ""Peter"", ""White"") TO \StatementTest\TableAddMultipleRecords;
";

            _SyneryClient.Run(code);

            ITable destinationTable = _Database.LoadTable(@"\StatementTest\TableAddMultipleRecords");

            Assert.AreEqual(4, destinationTable.Count);
        }

        [Test]
        public void Records_Added__In_One_Statement_By_Variable_Reference_Are_Complete()
        {
            string code = @"
#Person(INT Id, STRING Firstname, STRING Lastname);

#Person mike = #Person(15, ""Mike"", ""Meyer"");
#Person debby = #Person(22, ""Debby"", ""Smith"");

ADD mike, debby TO \StatementTest\TableAddMultipleRecords;
";

            _SyneryClient.Run(code);

            ITable destinationTable = _Database.LoadTable(@"\StatementTest\TableAddMultipleRecords");

            Assert.AreEqual(15, destinationTable[0][0]);
            Assert.AreEqual("Mike", destinationTable[0][1]);
            Assert.AreEqual("Meyer", destinationTable[0][2]);
            Assert.AreEqual(22, destinationTable[1][0]);
            Assert.AreEqual("Debby", destinationTable[1][1]);
            Assert.AreEqual("Smith", destinationTable[1][2]);
        }

        [Test]
        public void Records_Added_In_Two_Statements_By_Variable_Reference_Are_Complete()
        {
            string code = @"
#Person(INT Id, STRING Firstname, STRING Lastname);

#Person mike = #Person(15, ""Mike"", ""Meyer"");
#Person debby = #Person(22, ""Debby"", ""Smith"");

ADD mike TO \StatementTest\TableAddMultipleRecords;
ADD debby TO \StatementTest\TableAddMultipleRecords;
";

            _SyneryClient.Run(code);

            ITable destinationTable = _Database.LoadTable(@"\StatementTest\TableAddMultipleRecords");

            Assert.AreEqual(15, destinationTable[0][0]);
            Assert.AreEqual("Mike", destinationTable[0][1]);
            Assert.AreEqual("Meyer", destinationTable[0][2]);
            Assert.AreEqual(22, destinationTable[1][0]);
            Assert.AreEqual("Debby", destinationTable[1][1]);
            Assert.AreEqual("Smith", destinationTable[1][2]);
        }

        [Test]
        public void Records_Added_In_One_Statement_By_Record_Initializer_Are_Complete()
        {
            string code = @"
#Person(INT Id, STRING Firstname, STRING Lastname);

ADD #Person(27, ""Susan"", ""Miller""), #Person(33, ""Peter"", ""White"") TO \StatementTest\TableAddMultipleRecords;
";

            _SyneryClient.Run(code);

            ITable destinationTable = _Database.LoadTable(@"\StatementTest\TableAddMultipleRecords");

            Assert.AreEqual(27, destinationTable[0][0]);
            Assert.AreEqual("Susan", destinationTable[0][1]);
            Assert.AreEqual("Miller", destinationTable[0][2]);
            Assert.AreEqual(33, destinationTable[1][0]);
            Assert.AreEqual("Peter", destinationTable[1][1]);
            Assert.AreEqual("White", destinationTable[1][2]);
        }

        [Test]
        public void Records_Added_In_Two_Statements_By_Record_Initializer_Are_Complete()
        {
            string code = @"
#Person(INT Id, STRING Firstname, STRING Lastname);

ADD #Person(27, ""Susan"", ""Miller"") TO \StatementTest\TableAddMultipleRecords;
ADD #Person(33, ""Peter"", ""White"") TO \StatementTest\TableAddMultipleRecords;
";

            _SyneryClient.Run(code);

            ITable destinationTable = _Database.LoadTable(@"\StatementTest\TableAddMultipleRecords");

            Assert.AreEqual(27, destinationTable[0][0]);
            Assert.AreEqual("Susan", destinationTable[0][1]);
            Assert.AreEqual("Miller", destinationTable[0][2]);
            Assert.AreEqual(33, destinationTable[1][0]);
            Assert.AreEqual("Peter", destinationTable[1][1]);
            Assert.AreEqual("White", destinationTable[1][2]);
        }

        [Test]
        public void Records_Added_By_Using_Mixed_Methods_Are_Complete()
        {
            string code = @"
#Person(INT Id, STRING Firstname, STRING Lastname);

#Person mike = #Person(15, ""Mike"", ""Meyer"");
#Person debby = #Person(22, ""Debby"", ""Smith"");

ADD mike TO \StatementTest\TableAddMultipleRecords;
ADD debby TO \StatementTest\TableAddMultipleRecords;

ADD #Person(27, ""Susan"", ""Miller""), #Person(33, ""Peter"", ""White"") TO \StatementTest\TableAddMultipleRecords;
";

            _SyneryClient.Run(code);

            ITable destinationTable = _Database.LoadTable(@"\StatementTest\TableAddMultipleRecords");


            Assert.AreEqual(15, destinationTable[0][0]);
            Assert.AreEqual("Mike", destinationTable[0][1]);
            Assert.AreEqual("Meyer", destinationTable[0][2]);
            Assert.AreEqual(22, destinationTable[1][0]);
            Assert.AreEqual("Debby", destinationTable[1][1]);
            Assert.AreEqual("Smith", destinationTable[1][2]);
            Assert.AreEqual(27, destinationTable[2][0]);
            Assert.AreEqual("Susan", destinationTable[2][1]);
            Assert.AreEqual("Miller", destinationTable[2][2]);
            Assert.AreEqual(33, destinationTable[3][0]);
            Assert.AreEqual("Peter", destinationTable[3][1]);
            Assert.AreEqual("White", destinationTable[3][2]);
        }
    }
}
