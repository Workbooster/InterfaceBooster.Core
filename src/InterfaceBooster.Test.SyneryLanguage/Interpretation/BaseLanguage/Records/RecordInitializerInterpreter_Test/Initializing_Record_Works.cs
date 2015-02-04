using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.ErrorHandling;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.SyneryTypes;

namespace InterfaceBooster.Test.SyneryLanguage.Interpretation.BaseLanguage.Records.RecordInitializerInterpreter_Test
{
    [TestFixture]
    public class Initializing_Record_Works : BaseTest
    {
        [Test]
        public void Initialize_Record_Using_ExpressionList_Works()
        {
            string code = @"
#Person(INT Id, STRING Firstname, STRING Lastname);

#Person mike = #Person(1, ""Mike"", ""Meyer"");
";

            _SyneryClient.Run(code);

            IValue result = _SyneryMemory.CurrentScope.ResolveVariable("mike");

            Assert.IsInstanceOf<IRecord>(result.Value);
            Assert.AreEqual(1, ((IRecord)result.Value).GetFieldValue("Id").Value);
            Assert.AreEqual("Mike", ((IRecord)result.Value).GetFieldValue("Firstname").Value);
            Assert.AreEqual("Meyer", ((IRecord)result.Value).GetFieldValue("Lastname").Value);
        }

        [Test]
        public void Initialize_Record_Using_KeyValueList_Works()
        {
            string code = @"
#Person(INT Id, STRING Firstname, STRING Lastname);

#Person mike = #Person(Id = 1, Firstname = ""Mike"", Lastname = ""Meyer"");
";

            _SyneryClient.Run(code);

            IValue result = _SyneryMemory.CurrentScope.ResolveVariable("mike");

            Assert.IsInstanceOf<IRecord>(result.Value);
            Assert.AreEqual(1, ((IRecord)result.Value).GetFieldValue("Id").Value);
            Assert.AreEqual("Mike", ((IRecord)result.Value).GetFieldValue("Firstname").Value);
            Assert.AreEqual("Meyer", ((IRecord)result.Value).GetFieldValue("Lastname").Value);
        }

        [Test]
        public void Initialize_Record_By_Assigning_Fields_Works()
        {
            string code = @"
#Person(INT Id, STRING Firstname, STRING Lastname);

#Person mike = #Person();
mike.Id = 1;
mike.Firstname = ""Mike"";
mike.Lastname = ""Meyer"";
";

            _SyneryClient.Run(code);

            IValue result = _SyneryMemory.CurrentScope.ResolveVariable("mike");

            Assert.IsInstanceOf<IRecord>(result.Value);
            Assert.AreEqual(1, ((IRecord)result.Value).GetFieldValue("Id").Value);
            Assert.AreEqual("Mike", ((IRecord)result.Value).GetFieldValue("Firstname").Value);
            Assert.AreEqual("Meyer", ((IRecord)result.Value).GetFieldValue("Lastname").Value);
        }

        [Test]
        public void Initialize_Nested_Record_Using_KeyValueList_Works()
        {
            string code = @"
#Person(STRING Name, #Person Mother);

#Person mike = #Person(Name = ""Mike"", Mother = #Person(""Marry""));
";

            _SyneryClient.Run(code);

            IValue result = _SyneryMemory.CurrentScope.ResolveVariable("mike");

            Assert.IsInstanceOf<IRecord>(result.Value);
            Assert.AreEqual("Mike", ((IRecord)result.Value).GetFieldValue("Name").Value);
            Assert.AreEqual("Marry", ((IRecord)((IRecord)result.Value).GetFieldValue("Mother").Value).GetFieldValue("Name").Value);
        }

        [Test]
        public void Initialize_Nested_Record_Using_ExpressionList_Works()
        {
            string code = @"
#Person(STRING Name, #Person Mother);

#Person mike = #Person(""Mike"", #Person(""Marry""));
";

            _SyneryClient.Run(code);

            IValue result = _SyneryMemory.CurrentScope.ResolveVariable("mike");

            Assert.IsInstanceOf<IRecord>(result.Value);
            Assert.AreEqual("Mike", ((IRecord)result.Value).GetFieldValue("Name").Value);
            Assert.AreEqual("Marry", ((IRecord)((IRecord)result.Value).GetFieldValue("Mother").Value).GetFieldValue("Name").Value);
        }

        [Test]
        public void Initialize_Nested_Record_By_Assigning_Fields_Works()
        {
            string code = @"
#Person(STRING Name, #Person Mother);

#Person mike = #Person();
mike.Name = ""Mike"";
mike.Mother = #Person(""Marry"");
";

            _SyneryClient.Run(code);

            IValue result = _SyneryMemory.CurrentScope.ResolveVariable("mike");

            Assert.IsInstanceOf<IRecord>(result.Value);
            Assert.AreEqual("Mike", ((IRecord)result.Value).GetFieldValue("Name").Value);
            Assert.AreEqual("Marry", ((IRecord)((IRecord)result.Value).GetFieldValue("Mother").Value).GetFieldValue("Name").Value);
        }
    }
}
