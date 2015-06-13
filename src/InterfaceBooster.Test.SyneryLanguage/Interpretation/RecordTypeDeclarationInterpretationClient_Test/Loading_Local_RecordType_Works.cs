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
using InterfaceBooster.Common.Interfaces.Broadcasting;

namespace InterfaceBooster.Test.SyneryLanguage.Interpretation.RecordTypeDeclarationInterpretationClient_Test
{
    [TestFixture]
    public class Loading_Local_RecordType_Works
    {
        private RecordTypeDeclarationInterpretationClient _Client;
        private ISyneryMemory _SyneryMemory;
        private string _Code;

        [SetUp]
        public void SetupTest()
        {
            _SyneryMemory = new SyneryMemory(null, new DefaultBroadcaster(), null, null);
            _Client = new RecordTypeDeclarationInterpretationClient(_SyneryMemory);
            _Code = @"
#Person(INT Id, STRING Firstname, STRING Lastname, STRING Note = ""No Notes"");
#Employee(DOUBLE Salary) : #Person;
";
        }

        [Test]
        public void Loading_RecordTypes_Works()
        {
            _Client.Run(_Code);

            Assert.AreEqual(2, _SyneryMemory.RecordTypes.Count);
        }

        [Test]
        public void Loading_RecordType_Name_Works()
        {
            _Client.Run(_Code);

            Assert.AreEqual("Person", _SyneryMemory.RecordTypes.Values.ElementAt(0).Name);
        }

        [Test]
        public void Loading_RecordType_FullName_Works()
        {
            _Client.Run(_Code);

            Assert.AreEqual("Person", _SyneryMemory.RecordTypes.Values.ElementAt(0).FullName);
        }

        [Test]
        public void Loading_RecordType_Without_Base_Has_No_BaseRecordType_Works()
        {
            _Client.Run(_Code);

            Assert.AreEqual(null, _SyneryMemory.RecordTypes.Values.ElementAt(0).BaseRecordType);
        }

        [Test]
        public void Loading_RecordType_BaseRecordType_Works()
        {
            _Client.Run(_Code);

            Assert.AreEqual(_SyneryMemory.RecordTypes.Values.ElementAt(0), _SyneryMemory.RecordTypes.Values.ElementAt(1).BaseRecordType);
        }

        [Test]
        public void Loading_RecordType_Fields_Works()
        {
            _Client.Run(_Code);

            Assert.AreEqual(4, _SyneryMemory.RecordTypes.Values.ElementAt(0).Fields.Count);
        }

        [Test]
        public void Loading_RecordType_Field_Name_Works()
        {
            _Client.Run(_Code);

            Assert.AreEqual("Id", _SyneryMemory.RecordTypes.Values.ElementAt(0).Fields[0].Name);
        }

        [Test]
        public void Loading_RecordType_Field_Type_Works()
        {
            _Client.Run(_Code);

            Assert.AreEqual(typeof(int), _SyneryMemory.RecordTypes.Values.ElementAt(0).Fields[0].Type.UnterlyingDotNetType);
        }

        [Test]
        public void Loading_RecordType_Field_DefaultValue_Works()
        {
            _Client.Run(_Code);

            Assert.AreEqual(typeof(string), _SyneryMemory.RecordTypes.Values.ElementAt(0).Fields[3].DefaultValue.Type.UnterlyingDotNetType);
            Assert.AreEqual("No Notes", _SyneryMemory.RecordTypes.Values.ElementAt(0).Fields[3].DefaultValue.Value);
        }

        [Test]
        public void Loading_RecordType_Field_RecordType_Works()
        {
            _Client.Run(_Code);

            Assert.AreEqual(_SyneryMemory.RecordTypes.Values.ElementAt(0), _SyneryMemory.RecordTypes.Values.ElementAt(0).Fields[0].RecordType);
        }
    }
}
