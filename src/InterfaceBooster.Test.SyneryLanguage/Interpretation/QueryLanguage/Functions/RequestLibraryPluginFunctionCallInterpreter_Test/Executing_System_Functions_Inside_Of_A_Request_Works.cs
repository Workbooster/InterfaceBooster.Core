using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Database.Interfaces.Structure;

namespace InterfaceBooster.Test.SyneryLanguage.Interpretation.QueryLanguage.Functions.RequestLibraryPluginFunctionCallInterpreter_Test
{
    [TestFixture]
    public class Executing_System_Functions_Inside_Of_A_Request_Works : QueryLanguageTestBase
    {
        [SetUp]
        public void SetupSpecificTest()
        {
            CreatePeopleTable(_Database);
            SetupLibraryPlugin("Dummy");
        }

        [Test]
        public void Executing_Function_With_One_Parameter_Returns_Expected_Result()
        {
            string code = @"
\QueryLanguageTests\Test = 
    FROM \QueryLanguageTests\People AS p
    SELECT result = $Dummy.ThirdMethod(p.Firstname);
";

            _SyneryClient.Run(code);

            ITable sourceTable = _Database.LoadTable(@"\QueryLanguageTests\People");
            ITable destinationTable = _Database.LoadTable(@"\QueryLanguageTests\Test");

            string expectedResult = ((string)sourceTable[0][1]).ToUpper();

            Assert.AreEqual(expectedResult, destinationTable[0][0]);
        }
    }
}
