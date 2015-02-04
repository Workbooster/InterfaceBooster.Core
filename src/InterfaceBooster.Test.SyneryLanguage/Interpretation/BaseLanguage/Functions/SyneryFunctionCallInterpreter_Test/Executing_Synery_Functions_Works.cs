using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;

namespace InterfaceBooster.Test.SyneryLanguage.Interpretation.BaseLanguage.Functions.SyneryFunctionCallInterpreter_Test
{
    [TestFixture]
    public class Executing_Synery_Functions_Works : BaseTest
    {
        [Test]
        public void Execunting_Function_With_One_Parameter_And_DefualtValue_Returns_Expected_Result()
        {
            string code = @"
STRING result = ""unknown"";

STRING getResult(STRING text = ""some text"")
    STRING returnValue;

    returnValue = ""Result: "" + text;

    RETURN returnValue;
END

result = getResult();
";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("result");

            Assert.AreEqual("Result: some text", variable.Value);
        }

        [Test]
        public void Execunting_Complex_Function_With_DefualtValue_Returns_Expected_Result()
        {
            string code = @"
STRING result = ""unknown"";

STRING getResult(INT left, INT right, STRING label = ""Result: "")
    INT result;
    STRING returnValue;

    result = left + right;
    returnValue = label + (STRING)result;

    RETURN returnValue;
END

INT leftValue, rightValue;
leftValue = 5;
rightValue = 10;

result = getResult(leftValue, rightValue);
";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("result");

            Assert.AreEqual("Result: 15", variable.Value);
        }


        [Test]
        public void Execunting_Recoursive_Function_Works()
        {
            string code = @"
STRING result = ""unknown"";

STRING getResult(INT numberOfCalls, STRING currentText)
    currentText = currentText + ""test"";

    numberOfCalls = numberOfCalls - 1;

    IF numberOfCalls > 0
        RETURN getResult(numberOfCalls, currentText);
    ELSE
        RETURN currentText;
    END
END

result = getResult(3, """");
";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("result");

            Assert.AreEqual("testtesttest", variable.Value);
        }
    }
}
