using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;

namespace InterfaceBooster.Test.SyneryLanguage.Interpretation.BaseLanguage.Statements.EmitStatementInterpreter_Test
{
    [TestFixture]
    public class Emitting_Custom_Event_Works : BaseTest
    {
        [Test]
        public void Emitting_Custom_Event_From_A_Funktion_Works()
        {
            string code = @"
#MyEvent(INT Code) : #.Event;
BOOL isChanged = FALSE;

OBSERVE
    doSomething();
HANDLE(#MyEvent evt)
    isChanged = TRUE;
END

doSomething()
    EMIT #MyEvent(Code = 15);
END

";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("isChanged");

            Assert.AreEqual(true, variable.Value);
        }

        [Test]
        public void Accessing_Field_Of_Emmited_Custom_Event_Works()
        {
            string code = @"
#MyEvent(INT Code) : #.Event;
INT test = 0;

OBSERVE
    doSomething();
HANDLE(#MyEvent evt)
    test = evt.Code;
END

doSomething()
    EMIT #MyEvent(Code = 15);
END

";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("test");

            Assert.AreEqual(15, variable.Value);
        }
    }
}
