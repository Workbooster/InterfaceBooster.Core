using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Test.SyneryLanguage.Interpretation.BaseLanguage.Statements.ThrowStatementInterpreter_Works
{
    [TestFixture]
    public class Handling_Exception_Works : BaseTest
    {
        [Test]
        public void IsHandled_Flag_Is_Set()
        {
            string code = @"
#MyException(INT Code) : #.Exception;
BOOL firstHandler = FALSE;
BOOL secondHandler = FALSE;

OBSERVE
    doSomething();
HANDLE(#MyException ex)
    IF ex.IsHandled == FALSE
        firstHandler = TRUE;
    END
HANDLE(#.Exception ex)
    IF ex.IsHandled == FALSE
        secondHandler = TRUE;
    END
END

doSomething()
    THROW #MyException(Code = 15);
END

";

            _SyneryClient.Run(code);

            IValue firstHandlerVariable = _SyneryClient.Memory.CurrentScope.ResolveVariable("firstHandler");
            IValue secondHandlerVariable = _SyneryClient.Memory.CurrentScope.ResolveVariable("secondHandler");

            // only the first handler should be handled, becuase the IsHandled-flag must be set automatically.

            Assert.AreEqual(true, firstHandlerVariable.Value);
            Assert.AreEqual(false, secondHandlerVariable.Value);
        }
    }
}
