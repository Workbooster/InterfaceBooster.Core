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
    public class Executing_Synery_Functions_With_Nested_Blocks_Works : BaseTest
    {
        [Test]
        public void Executing_Synery_Function_With_Return_From_Function_Block_With_One_Nested_If_Statement_Works()
        {
            string code = @"
INT result = 0;

INT getResult()
    INT returnValue;

    IF (15 > 10)
        returnValue = 1;  // expected
    ELSE
        returnValue = 2;
    END

    RETURN returnValue;
END

result = getResult();
";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("result");

            Assert.AreEqual(1, variable.Value);
        }

        [Test]
        public void Executing_Synery_Function_With_Return_From_One_Nested_If_Statement_Works()
        {
            string code = @"
INT result = 0;

INT getResult()
    IF (1 > 10)
        RETURN 1;
    ELSE
        RETURN 2; // expected
    END
END

result = getResult();
";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("result");

            Assert.AreEqual(2, variable.Value);
        }

        [Test]
        public void Executing_Synery_Function_With_Return_From_Three_Nested_If_Statements_Works()
        {
            string code = @"
INT result = 0;

INT getResult()
    IF (1 > 10)
        IF (2 > 10)
            IF (3 > 10)
                RETURN 1;
            ELSE
                RETURN 2;
            END
        ELSE
            RETURN 3;
        END
    ELSE
        IF (15 > 10)
            IF (20 > 10)
                RETURN 4; // expected
            ELSE
                RETURN 5;
            END
        ELSE
            RETURN 6;
        END
    END
END

result = getResult();
";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("result");

            Assert.AreEqual(4, variable.Value);
        }

        [Test]
        public void Executing_Synery_Function_That_Calls_Another_Function_With_Return_Statement_Works()
        {
            string code = @"
INT result = 0;

INT addOne(INT value)
    value = value + 1;

    RETURN value;
END

INT addTwo(INT value)
    value = addOne(value);    
    value = addOne(value);

    RETURN value;
END

result = addTwo(5);
";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("result");

            Assert.AreEqual(7, variable.Value);
        }

        [Test]
        public void Accessing_Global_Variables_Is_Possible()
        {
            // If a function calls another function the second function should still have access to the global variables.

            string code = @"
INT test = 15;

first(INT value)
    second(value);
END

second(INT value)
    test = 7;
END

first(5);
";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("test");

            Assert.AreEqual(7, variable.Value);
        }

        [Test]
        public void Accessing_Variables_From_The_Calling_Function_Block_Is_Avoided()
        {
            // If a function calls another function the second function should not have access to the variables of the first function.

            string code = @"
first(INT value)
    INT valueFromFirstFunction = 15;

    second(value);
END

second(INT value)
    valueFromFirstFunction = 7;
END

first(5);
";

            Exception ex = Assert.Throws<SyneryException>(delegate { _SyneryClient.Run(code); });
            Assert.AreEqual(ex.Message, "Variable with name='valueFromFirstFunction' doesn't exists.");
        }
    }
}
