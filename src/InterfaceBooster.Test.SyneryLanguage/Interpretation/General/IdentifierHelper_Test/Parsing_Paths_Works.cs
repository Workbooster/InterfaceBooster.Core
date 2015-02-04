using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.SyneryLanguage.Interpretation.General;

namespace InterfaceBooster.Test.SyneryLanguage.Interpretation.General.IdentifierHelper_Test
{
    [TestFixture]
    public class Parsing_Paths_Works
    {
        [Test]
        public void Splitting_Short_InternalPathIdentifier_Works()
        {
            string internalPathIdentifier = @"\tableGroup\myTable\";

            string[] path = IdentifierHelper.ParsePathIdentifier(internalPathIdentifier);

            Assert.AreEqual(2, path.Count());
            Assert.AreEqual("tableGroup", path[0]);
            Assert.AreEqual("myTable", path[1]);
        }

        [Test]
        public void Splitting_Long_InternalPathIdentifier_Works()
        {
            string internalPathIdentifier = @"\tableGroup\subGroup\myTable";

            string[] path = IdentifierHelper.ParsePathIdentifier(internalPathIdentifier);

            Assert.AreEqual(3, path.Count());
            Assert.AreEqual("tableGroup", path[0]);
            Assert.AreEqual("subGroup", path[1]);
            Assert.AreEqual("myTable", path[2]);
        }

        [Test]
        public void Splitting_Short_ExternalPathIdentifier_Works()
        {
            string externalPathIdentifier = @"\\myConnection\someRecordSet\";

            string[] path = IdentifierHelper.ParsePathIdentifier(externalPathIdentifier);

            Assert.AreEqual(2, path.Count());
            Assert.AreEqual("myConnection", path[0]);
            Assert.AreEqual("someRecordSet", path[1]);
        }

        [Test]
        public void Splitting_Long_ExternalPathIdentifier_Works()
        {
            string externalPathIdentifier = @"\\myConnection\recodSetGroup\subGroup\someRecordSet";

            string[] path = IdentifierHelper.ParsePathIdentifier(externalPathIdentifier);

            Assert.AreEqual(4, path.Count());
            Assert.AreEqual("myConnection", path[0]);
            Assert.AreEqual("recodSetGroup", path[1]);
            Assert.AreEqual("subGroup", path[2]);
            Assert.AreEqual("someRecordSet", path[3]);
        }

        [Test]
        public void Splitting_Short_ComplexIdentifier_Works()
        {
            string complexIdentifier = @"p.PersonId";

            string[] path = IdentifierHelper.ParseComplexIdentifier(complexIdentifier);

            Assert.AreEqual(2, path.Count());
            Assert.AreEqual("p", path[0]);
            Assert.AreEqual("PersonId", path[1]);
        }

        [Test]
        public void Splitting_Long_ComplexIdentifier_Works()
        {
            string complexIdentifier = @"r.Person.Id";

            string[] path = IdentifierHelper.ParseComplexIdentifier(complexIdentifier);

            Assert.AreEqual(3, path.Count());
            Assert.AreEqual("r", path[0]);
            Assert.AreEqual("Person", path[1]);
            Assert.AreEqual("Id", path[2]);
        }
    }
}
