using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.ErrorHandling;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;

namespace InterfaceBooster.Test.SyneryLanguage.Interpretation.BaseLanguage.Expressions.ExpressionInterpreter_Test
{
    [TestFixture]
    public class Catching_Null_Values_Works : BaseTest
    {
        [Test]
        public void Executing_Add_Expression_On_STRING_And_NULL_Works()
        {
            string code = "STRING test = \"My Text\" + NULL;";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("test");

            Assert.AreEqual("My Text", variable.Value);
        }

        [Test]
        public void Executing_Add_Expression_On_STRING_And_Variable_With_NULL_Value_Works()
        {
            string code = @"
STRING nullValue = NULL;
STRING test = ""My Text"" + nullValue;";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("test");

            Assert.AreEqual("My Text", variable.Value);
        }

        [Test]
        public void Executing_Add_Expression_On_INT_And_NULL_Works()
        {
            string code = "INT test = 15 + NULL;";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("test");

            Assert.AreEqual(15, variable.Value);
        }

        [Test]
        public void Executing_Slash_Expression_On_INT_And_NULL_Throws_SyneryLanguageException()
        {
            string code = "INT test = 15 / NULL;";

            Assert.Throws<SyneryInterpretationException>(() => { _SyneryClient.Run(code); });
        }

        [Test]
        public void Executing_OR_Expression_On_NULL_Value_Throws_SyneryLanguageException()
        {
            string code = @"
BOOL nullValue = NULL;
BOOL test = nullValue OR TRUE;";

            Assert.Throws<SyneryInterpretationException>(() => { _SyneryClient.Run(code); });
        }

        [Test]
        public void Executing_AND_Expression_On_NULL_Value_Throws_SyneryLanguageException()
        {
            string code = @"
BOOL nullValue = NULL;
BOOL test = nullValue AND TRUE;";

            Assert.Throws<SyneryInterpretationException>(() => { _SyneryClient.Run(code); });
        }

        [Test]
        public void Executing_Equality_Comparison_With_NULL_Works()
        {
            string code = @"
STRING nullValue = NULL;
BOOL test = nullValue == NULL;";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("test");

            Assert.AreEqual(true, variable.Value);
        }

        [Test]
        public void Executing_Equality_Comparison_With_NULL_Values_Of_Two_Different_Types_orks()
        {
            string code = @"
STRING stringNullValue = NULL;
INT intNullValue = NULL;
BOOL test = stringNullValue == intNullValue;";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("test");

            Assert.AreEqual(true, variable.Value);
        }

        [Test]
        public void Executing_GreaterThan_Comparison_With_NULL_Works()
        {
            string code = @"
INT nullValue = NULL;
BOOL test = nullValue > 15;";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("test");

            Assert.AreEqual(false, variable.Value);
        }

        [Test]
        public void Executing_GreaterThanOrEqual_Comparison_With_NULL_Works()
        {
            string code = @"
INT nullValue = NULL;
BOOL test = nullValue >= 15;";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("test");

            Assert.AreEqual(false, variable.Value);
        }

        [Test]
        public void Executing_LessThan_Comparison_With_NULL_Works()
        {
            string code = @"
INT nullValue = NULL;
BOOL test = nullValue < 15;";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("test");

            Assert.AreEqual(true, variable.Value);
        }

        [Test]
        public void Executing_LessThanOrEqual_Comparison_With_NULL_Works()
        {
            string code = @"
INT nullValue = NULL;
BOOL test = nullValue <= 15;";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("test");

            Assert.AreEqual(true, variable.Value);
        }

        [Test]
        public void Zero_Does_Not_Equal_To_NULL()
        {
            string code = "BOOL test = 0 == NULL;";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("test");

            Assert.AreEqual(false, variable.Value);
        }

        [Test]
        public void Empty_String_Does_Not_Equal_To_NULL()
        {
            string code = "BOOL test = \"\" == NULL;";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("test");

            Assert.AreEqual(false, variable.Value);
        }
    }
}
