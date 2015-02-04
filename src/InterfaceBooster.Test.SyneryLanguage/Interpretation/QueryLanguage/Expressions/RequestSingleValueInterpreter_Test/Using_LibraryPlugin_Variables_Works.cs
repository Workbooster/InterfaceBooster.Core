using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Database.Interfaces.Structure;

namespace InterfaceBooster.Test.SyneryLanguage.Interpretation.QueryLanguage.Expressions.RequestSingleValueInterpreter_Test
{
    [TestFixture]
    public class Using_LibraryPlugin_Variables_Works : QueryLanguageTestBase
    {
        [SetUp]
        public void SetupSpecificTest()
        {
            CreatePeopleTable(_Database);
            SetupLibraryPlugin("Dummy");
        }

        [Test]
        public void Assigning_Field_From_Static_Variable_Works()
        {
            string code = @"
\QueryLanguageTests\Test = 
    FROM \QueryLanguageTests\People AS p
    SELECT result = $Dummy.FirstVariable;
";

            _SyneryClient.Run(code);

            ITable destinationTable = _Database.LoadTable(@"\QueryLanguageTests\Test");

            Assert.AreEqual("First Variable Value", destinationTable[0][0]);
        }
    }
}
