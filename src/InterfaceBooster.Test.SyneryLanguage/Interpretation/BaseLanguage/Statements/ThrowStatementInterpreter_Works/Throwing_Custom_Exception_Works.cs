using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;

namespace InterfaceBooster.Test.SyneryLanguage.Interpretation.BaseLanguage.Statements.ThrowStatementInterpreter_Works
{
    [TestFixture]    
    public class Throwing_Custom_Exception_Works : BaseTest
    {
        [Test]
        public void Throwing_Custom_Exception_From_A_Funktion_Works()
        {
            string code = @"
#MyException(INT Code) : #.Exception;
BOOL isChanged = FALSE;

OBSERVE
    doSomething();
HANDLE(#MyException ex)
    isChanged = TRUE;
END

doSomething()
    THROW #MyException(Code = 15);
END

";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("isChanged");

            Assert.AreEqual(true, variable.Value);
        }

        [Test]
        public void Accessing_Field_Of_Thrown_Custom_Exception_Works()
        {
            string code = @"
#MyException(INT Code) : #.Exception;
INT test = 0;

OBSERVE
    doSomething();
HANDLE(#MyException ex)
    test = ex.Code;
END

doSomething()
    THROW #MyException(Code = 15);
END

";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("test");

            Assert.AreEqual(15, variable.Value);
        }
    }
}
