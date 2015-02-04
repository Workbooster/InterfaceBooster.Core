using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.SyneryLanguage.Interpretation;
using InterfaceBooster.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.SyneryTypes;

namespace InterfaceBooster.Test.SyneryLanguage.Interpretation.RecordTypeDeclarationInterpretationClient_Test
{
    [TestFixture]
    public class Defining_Nested_RecordTypes_Works
    {
        private RecordTypeDeclarationInterpretationClient _Client;
        private ISyneryMemory _SyneryMemory;

        [SetUp]
        public void SetupTest()
        {
            _SyneryMemory = new SyneryMemory(null, null, null);
            _Client = new RecordTypeDeclarationInterpretationClient(_SyneryMemory);
        }

        [Test]
        public void Loading_RecordType_Inside_Of_Another_RecordType_Works()
        {
            string code = @"
#Course(STRING Name);
#Meal(STRING Name, #Course FirstCourse, #Course SecondCourse);
";

            _Client.Run(code);

            Assert.AreEqual(3, _SyneryMemory.RecordTypes.Values.ElementAt(1).Fields.Count);
        }
    }
}
