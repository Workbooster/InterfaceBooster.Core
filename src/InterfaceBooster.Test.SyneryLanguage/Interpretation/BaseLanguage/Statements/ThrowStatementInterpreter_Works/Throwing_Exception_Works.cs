using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.ErrorHandling;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;

namespace InterfaceBooster.Test.SyneryLanguage.Interpretation.BaseLanguage.Statements.ThrowStatementInterpreter_Works
{
    [TestFixture]
    public class Throwing_Exception_Works : BaseTest
    {
        [Test]
        public void Simple_Throwing_Exception_Directly_From_Inside_An_Observe_Block_Is_Handled()
        {
            string code = @"
BOOL isChanged = FALSE;

OBSERVE
    THROW #.Exception();
HANDLE(#.Exception ex)
    isChanged = TRUE;
END

";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("isChanged");

            Assert.AreEqual(true, variable.Value);
        }

        [Test]
        public void Throwing_Exception_From_A_Funktion_Works()
        {
            string code = @"
BOOL isChanged = FALSE;

OBSERVE
    doSomething();
HANDLE(#.Exception ex)
    isChanged = TRUE;
END

doSomething()
    THROW #.Exception();
END

";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("isChanged");

            Assert.AreEqual(true, variable.Value);
        }

        [Test]
        public void Throwing_Exception_From_A_Nested_Block_Inside_Of_A_Funktion_Works()
        {
            string code = @"
BOOL isChanged = FALSE;

OBSERVE
    doSomething();
HANDLE(#.Exception ex)
    isChanged = TRUE;
END

doSomething()
    IF 5 * 2 == 10
        THROW #.Exception();
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
INT blockNr = 0;

OBSERVE
    handleException();
HANDLE(#.Exception ex)
    blockNr = 1;
END

handleException()
    OBSERVE
        doSomething();
    HANDLE(#.Exception ex)
        blockNr = 2;
    END
END

doSomething()
    IF 5 * 2 == 10
        THROW #.Exception();
    END
END

";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("blockNr");

            Assert.AreEqual(2, variable.Value);
        }

        [Test]
        public void Using_Variable_From_The_Calling_Block_Is_Prohibited()
        {
            string code = @"
INT test = 0;

OBSERVE
    doSomething();
HANDLE(#.Exception ex)
    test = someValue;
END

doSomething()
    INT someValue = 15;
    
    THROW #.Exception();
END

";
            Assert.Throws<SyneryException>(delegate { _SyneryClient.Run(code); });
        }

        [Test]
        public void Throwing_Exception_Breaks_Execution()
        {
            string code = @"
INT counter = 0;

OBSERVE
    counter = counter + 1;
    
    doSomething();

    counter = counter + 10;
HANDLE(#.Exception ex)
    counter = counter + 1;
END

doSomething()
    counter = counter + 1;

    THROW #.Exception();

    counter = counter + 100;
END

";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("counter");

            Assert.AreEqual(3, variable.Value);
        }

        [Test]
        public void Throwing_Exception_Only_Breaks_Current_OBSERVE_Block_Execution()
        {
            string code = @"
STRING test = """";

first();

first()
    OBSERVE
        test = test + "">first-start"";
    
        second();

    HANDLE(#.Exception ex)
        test = test + "">first-HANDLE"";
    END

    test = test + "">first-end"";
END

second()
    OBSERVE
        test = test + "">second-start"";
    
        doSomething();

        test = test + 100;
    HANDLE(#.Exception ex)
        test = test + "">second-HANDLE"";
    END

    test = test + "">second-end"";
END

doSomething()
    test = test + "">doSomething-start"";

    THROW #.Exception();

    test = test + "">doSomething-end"";
END

";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("test");

            string expectedTestText =
                ">first-start>second-start>doSomething-start>second-HANDLE>second-end>first-end";

            Assert.AreEqual(expectedTestText, variable.Value);
        }
    }
}
