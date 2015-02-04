using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.SyneryLanguage.Interpretation;
using InterfaceBooster.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.ErrorHandling;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.SyneryTypes;
using InterfaceBooster.Test.SyneryLanguage.Interpretation.BaseLanguage;

namespace InterfaceBooster.Test.SyneryLanguage.Interpretation.RecordTypeDeclarationInterpretationClient_Test
{
    [TestFixture]
    public class Catching_RecordType_Declaration_Errors_Works : BaseTest
    {


        [Test]
        public void Using_The_Same_Name_Throws_Exception()
        {
            string code = @"
#Test(STRING Name);
#Test(INT Id, STRING Name);
";

            Exception ex = Assert.Throws<SyneryInterpretationException>(delegate { _SyneryClient.Run(code); });
            Assert.AreEqual("A record type with the name 'Test' has already been specified at line '2'.", ex.Message);
        }

        [Test]
        public void Using_A_Leading_Dot_Like_In_SystemRecordSets_Throws_Exception()
        {
            string code = @"
#.Test(STRING Name);
";

            Exception ex = Assert.Throws<SyneryException>(delegate { _SyneryClient.Run(code); });
            Assert.AreEqual("Syntax error on line 2 at char 6: no viable alternative at input '#.Test('.", ex.Message);
        }
    }
}
