using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.ErrorHandling;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;

namespace InterfaceBooster.Test.SyneryLanguage.Interpretation.BaseLanguage.Expressions.ComplexReferenceInterpreter_Test
{
    [TestFixture]
    public class Using_Record_Fields_Works : BaseTest
    {
        [Test]
        public void Assigning_Field_Value_To_A_Variable_Works()
        {
            string code = @"
#Person(INT Id, STRING Firstname, STRING Lastname);

#Person mike = #Person(1, ""Mike"", ""Meyer"");

INT idTest = mike.Id;
STRING lastnameTest = mike.Lastname;
";

            _SyneryClient.Run(code);

            IValue resultOne = _SyneryMemory.CurrentScope.ResolveVariable("idTest");
            IValue resultTwo = _SyneryMemory.CurrentScope.ResolveVariable("lastnameTest");

            Assert.AreEqual(1, resultOne.Value);
            Assert.AreEqual("Meyer", resultTwo.Value);
        }

        [Test]
        public void Using_Field_In_A_Conditional_Expression_Works()
        {
            string code = @"
#Person(INT Id, STRING Firstname, STRING Lastname);

STRING test = ""unknown"";
#Person mike = #Person(1, ""Mike"", ""Meyer"");

IF mike.Firstname == ""Peter""
    test = ""It's Peter"";
ELSE
    test = ""It's not Peter"";
END
";

            _SyneryClient.Run(code);

            IValue result = _SyneryMemory.CurrentScope.ResolveVariable("test");

            Assert.AreEqual("It's not Peter", result.Value);
        }

        [Test]
        public void Calculation_With_Two_Fields_Works()
        {
            string code = @"
#Mult(INT First, INT Second);

#Mult one = #Mult(First = 5, Second = 7);

INT test = one.First * one.Second;
";

            _SyneryClient.Run(code);

            IValue result = _SyneryMemory.CurrentScope.ResolveVariable("test");

            Assert.AreEqual(35, result.Value);
        }

        [Test]
        public void Calculation_With_Two_Fields_And_One_Variable_Works()
        {
            string code = @"
#Mult(INT First, INT Second);

INT divisor = 5;
#Mult one = #Mult(First = 3, Second = 10);

INT test = one.First * one.Second / divisor;
";

            _SyneryClient.Run(code);

            IValue result = _SyneryMemory.CurrentScope.ResolveVariable("test");

            Assert.AreEqual(6, result.Value);
        }

        [Test]
        public void Uninitialized_Field_Is_Available_With_NULL_Value()
        {
            string code = @"
#Person(INT Id, STRING Firstname, STRING Lastname);

#Person mike = #Person(Id = 1, Firstname = ""Mike"");

STRING test = mike.Lastname;
";

            _SyneryClient.Run(code);

            IValue result = _SyneryMemory.CurrentScope.ResolveVariable("test");

            Assert.AreEqual(null, result.Value);
        }

        [Test]
        public void Accessing_Unknown_Field_Throws_Exception()
        {
            string code = @"
#Person(INT Id, STRING Firstname, STRING Lastname);

#Person mike = #Person(Id = 1, Firstname = ""Mike"");

STRING test = mike.SomeUnknownField;
";

            Assert.Throws<SyneryInterpretationException>(delegate { _SyneryClient.Run(code); });
        }
    }
}
