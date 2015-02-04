using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.ErrorHandling;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;

namespace InterfaceBooster.Test.SyneryLanguage.Interpretation.BaseLanguage.Functions.SyneryFunctionCallInterpreter_Test
{
    [TestFixture]
    public class Using_Records_With_Functions_Works : BaseTest
    {
        [Test]
        public void Using_Record_As_Function_Parameter_Works()
        {
            string code = @"
#Person(INT Id, STRING Firstname, STRING Lastname);

STRING test = ""unknown"";
#Person debby = #Person(1, ""Debby"", ""Meyer"");

GetMarried(#Person p, STRING newLastName)
    p.Lastname = newLastName;
END

GetMarried(debby, ""Smith"");

test = debby.Lastname;
";

            _SyneryClient.Run(code);

            IValue result = _SyneryMemory.CurrentScope.ResolveVariable("test");

            Assert.AreEqual("Smith", result.Value);
        }

        [Test]
        public void Using_Wrong_Record_Type_As_Parameter_Throws_Exception()
        {
            string code = @"
#Person(INT Id, STRING Name);
#Car(STRING Name, STRING Brand, INT Power);

#Person debby = #Person(1, ""Debby"");

SetCarName(#Car c, STRING name)
    c.Name = name;
END

SetCarName(debby, ""Corolla"");
";

            string exceptionMessage = "No matching function signature found: SetCarName(Person,String)";

            var exception = Assert.Throws<SyneryInterpretationException>(delegate { _SyneryClient.Run(code); });
            Assert.AreEqual(exceptionMessage, exception.Message);
        }

        [Test]
        public void Using_Base_Record_Type_As_Parameter_Works_For_Derived_Record_Type()
        {
            string code = @"
#Person(INT Id, STRING Name);
#Employee(DOUBLE Salary) : #Person;

STRING test = ""unknown"";
#Employee employee = #Employee(1, ""Debby"");

SetPersonName(#Person p, STRING name)
    p.Name = name;
END

SetPersonName(employee, ""Lisa"");

test = employee.Name;
";

            _SyneryClient.Run(code);

            IValue result = _SyneryMemory.CurrentScope.ResolveVariable("test");

            Assert.AreEqual("Lisa", result.Value);
        }

        [Test]
        public void Using_Record_As_Return_Value_Works()
        {
            string code = @"
#Person(INT Id, STRING Firstname, STRING Lastname);

STRING test = ""unknown"";

#Person InitializeDebby()
    #Person p = #Person();
    p.Id = 15;
    p.Firstname = ""Debby"";
    p.Lastname = ""Meyer"";

    RETURN p;
END

#Person debby = InitializeDebby();

test = debby.Firstname;
";

            _SyneryClient.Run(code);

            IValue result = _SyneryMemory.CurrentScope.ResolveVariable("test");

            Assert.AreEqual("Debby", result.Value);
        }
    }
}
