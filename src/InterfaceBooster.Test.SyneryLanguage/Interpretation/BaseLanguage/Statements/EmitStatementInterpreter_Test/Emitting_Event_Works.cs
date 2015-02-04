using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.ErrorHandling;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;

namespace InterfaceBooster.Test.SyneryLanguage.Interpretation.BaseLanguage.Statements.EmitStatementInterpreter_Test
{
    [TestFixture]
    public class Emitting_Event_Works : BaseTest
    {
        [Test]
        public void Simple_Emitted_Event_Directly_From_Inside_An_Observe_Block_Is_Handled()
        {
            string code = @"
BOOL isChanged = FALSE;

OBSERVE
    EMIT #.Event();
HANDLE(#.Event evt)
    isChanged = TRUE;
END

";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("isChanged");

            Assert.AreEqual(true, variable.Value);
        }

        [Test]
        public void Emitting_Event_From_A_Funktion_Works()
        {
            string code = @"
BOOL isChanged = FALSE;

OBSERVE
    doSomething();
HANDLE(#.Event evt)
    isChanged = TRUE;
END

doSomething()
    EMIT #.Event();
END

";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("isChanged");

            Assert.AreEqual(true, variable.Value);
        }

        [Test]
        public void Emitting_Event_From_A_Nested_Block_Inside_Of_A_Funktion_Works()
        {
            string code = @"
BOOL isChanged = FALSE;

OBSERVE
    doSomething();
HANDLE(#.Event evt)
    isChanged = TRUE;
END

doSomething()
    IF 5 * 2 == 10
        EMIT #.Event();
    END
END

";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("isChanged");

            Assert.AreEqual(true, variable.Value);
        }

        [Test]
        public void Using_Multiple_Hanlde_Blocks_Works()
        {
            string code = @"
INT counter = 0;

OBSERVE
    handleEvent();
HANDLE(#.Event evt)
    counter = counter + 1;
END

handleEvent()
    OBSERVE
        doSomething();
    HANDLE(#.Event evt)
        counter = counter + 1;
    END
END

doSomething()
    IF 5 * 2 == 10
        EMIT #.Event();
    END
END

";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("counter");

            Assert.AreEqual(2, variable.Value);
        }

        [Test]
        public void Using_Variable_From_The_Calling_Block_Is_Prohibited()
        {
            string code = @"
INT test = 0;

OBSERVE
    doSomething();
HANDLE(#.Event evt)
    test = someValue;
END

doSomething()
    INT someValue = 15;
    
    EMIT #.Event();
END

";
            Assert.Throws<SyneryException>(delegate { _SyneryClient.Run(code); });
        }

        [Test]
        public void Emitting_Event_Does_Not_Break_Execution()
        {
            string code = @"
INT counter = 0;

OBSERVE
    counter = counter + 1;
    
    doSomething();

    counter = counter + 1;
HANDLE(#.Event evt)
    counter = counter + 1;
END

doSomething()
    counter = counter + 1;

    EMIT #.Event();

    counter = counter + 1;
END

";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("counter");

            Assert.AreEqual(5, variable.Value);
        }
    }
}
