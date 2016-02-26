using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;
using InterfaceBooster.Test.SyneryLanguage.Interpretation.QueryLanguage;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Test.SyneryLanguage.Interpretation.BaseLanguage.Blocks.ObserveBlockInterpreter_Test
{
    [TestFixture]
    public class Handling_System_Exception_Works : BaseTest
    {
        [Test]
        public void System_Exception_Caused_By_An_Unknown_Table_Is_Handled_By_Observe_Block()
        {
            string code = @"
#MyException(INT Code) : #.Exception;
BOOL firstHandler = FALSE;
BOOL secondHandler = FALSE;
BOOL afterException = FALSE;
BOOL afterObserveBlock = FALSE;

OBSERVE
    doSomething();

    afterException = TRUE;

HANDLE(#MyException ex)
    firstHandler = TRUE;
HANDLE(#.Exception ex)
    secondHandler = TRUE;
END

doSomething()
    // unknown table
    \targetTable = 
        FROM \unknownTable AS t
        SELECT t.*;
END

afterObserveBlock = TRUE;

";
            _SyneryClient.Run(code);

            IValue firstHandlerVariable = _SyneryClient.Memory.CurrentScope.ResolveVariable("firstHandler");
            IValue secondHandlerVariable = _SyneryClient.Memory.CurrentScope.ResolveVariable("secondHandler");
            IValue afterExceptionVariable = _SyneryClient.Memory.CurrentScope.ResolveVariable("afterException");
            IValue afterObserveBlockVariable = _SyneryClient.Memory.CurrentScope.ResolveVariable("afterObserveBlock");

            // only the first handler should be handled, because the IsHandled-flag must be set automatically.
            Assert.AreEqual(false, firstHandlerVariable.Value);
            Assert.AreEqual(true, secondHandlerVariable.Value);

            // the code after the occurrence of the exception inside of the OBSERVE-block should be executed. 
            Assert.AreEqual(false, afterExceptionVariable.Value);

            // but the code after the OBSERVE-block should be executed (because the exception has already been handled).
            Assert.AreEqual(true, afterObserveBlockVariable.Value);
        }

        [Test]
        public void System_Exception_Caused_By_A_Parsing_Error_Is_Handled_By_Observe_Block()
        {
            string code = @"
#MyException(INT Code) : #.Exception;
BOOL firstHandler = FALSE;
BOOL secondHandler = FALSE;
BOOL afterException = FALSE;
BOOL afterObserveBlock = FALSE;

OBSERVE
    doSomething();

    afterException = TRUE;

HANDLE(#MyException ex)
    firstHandler = TRUE;
HANDLE(#.Exception ex)
    secondHandler = TRUE;
    LOG(ex.Message);
END

doSomething()
    STRING bla = ""bla"";
    INT someNumber = bla;
END

afterObserveBlock = TRUE;

";

            _SyneryClient.Run(code);

            IValue firstHandlerVariable = _SyneryClient.Memory.CurrentScope.ResolveVariable("firstHandler");
            IValue secondHandlerVariable = _SyneryClient.Memory.CurrentScope.ResolveVariable("secondHandler");
            IValue afterExceptionVariable = _SyneryClient.Memory.CurrentScope.ResolveVariable("afterException");
            IValue afterObserveBlockVariable = _SyneryClient.Memory.CurrentScope.ResolveVariable("afterObserveBlock");

            // only the first handler should be handled, because the IsHandled-flag must be set automatically.
            Assert.AreEqual(false, firstHandlerVariable.Value);
            Assert.AreEqual(true, secondHandlerVariable.Value);

            // the code after the occurrence of the exception inside of the OBSERVE-block should be executed. 
            Assert.AreEqual(false, afterExceptionVariable.Value);

            // but the code after the OBSERVE-block should be executed (because the exception has already been handled).
            Assert.AreEqual(true, afterObserveBlockVariable.Value);
        }

        [Test]
        public void System_Exception_Caused_By_An_Unknown_LibraryPlugin_Is_Handled_By_Observe_Block()
        {
            string code = @"
#MyException(INT Code) : #.Exception;
BOOL firstHandler = FALSE;
BOOL secondHandler = FALSE;
BOOL afterException = FALSE;
BOOL afterObserveBlock = FALSE;

OBSERVE
    doSomething();

    afterException = TRUE;

HANDLE(#MyException ex)
    firstHandler = TRUE;
HANDLE(#.Exception ex)
    secondHandler = TRUE;
    LOG(ex.Message);
END

doSomething()
    STRING param = ""bla"";
    $unknownPlugin.someMethod(param);
END

afterObserveBlock = TRUE;

";

            _SyneryClient.Run(code);

            IValue firstHandlerVariable = _SyneryClient.Memory.CurrentScope.ResolveVariable("firstHandler");
            IValue secondHandlerVariable = _SyneryClient.Memory.CurrentScope.ResolveVariable("secondHandler");
            IValue afterExceptionVariable = _SyneryClient.Memory.CurrentScope.ResolveVariable("afterException");
            IValue afterObserveBlockVariable = _SyneryClient.Memory.CurrentScope.ResolveVariable("afterObserveBlock");

            // only the first handler should be handled, because the IsHandled-flag must be set automatically.
            Assert.AreEqual(false, firstHandlerVariable.Value);
            Assert.AreEqual(true, secondHandlerVariable.Value);

            // the code after the occurrence of the exception inside of the OBSERVE-block should be executed. 
            Assert.AreEqual(false, afterExceptionVariable.Value);

            // but the code after the OBSERVE-block should be executed (because the exception has already been handled).
            Assert.AreEqual(true, afterObserveBlockVariable.Value);
        }

        [Test]
        public void System_Exception_Caused_By_An_Unknown_Field_Is_Handled_By_Observe_Block()
        {
            string code = @"
#MyException(INT Code) : #.Exception;
BOOL firstHandler = FALSE;
BOOL secondHandler = FALSE;
BOOL afterException = FALSE;
BOOL afterObserveBlock = FALSE;

OBSERVE
    doSomething();

    afterException = TRUE;

HANDLE(#MyException ex)
    firstHandler = TRUE;
HANDLE(#.Exception ex)
    secondHandler = TRUE;
    LOG(ex.Message);
END

doSomething()
    \QueryLanguageTests\Test = 
        FROM \QueryLanguageTests\People AS p
        SELECT TestFirstname = p.Firstname, p.UnknownField;
END

afterObserveBlock = TRUE;

";

            QueryLanguageTestBase.CreatePeopleTable(_Database);

            _SyneryClient.Run(code);

            IValue firstHandlerVariable = _SyneryClient.Memory.CurrentScope.ResolveVariable("firstHandler");
            IValue secondHandlerVariable = _SyneryClient.Memory.CurrentScope.ResolveVariable("secondHandler");
            IValue afterExceptionVariable = _SyneryClient.Memory.CurrentScope.ResolveVariable("afterException");
            IValue afterObserveBlockVariable = _SyneryClient.Memory.CurrentScope.ResolveVariable("afterObserveBlock");

            // only the first handler should be handled, because the IsHandled-flag must be set automatically.
            Assert.AreEqual(false, firstHandlerVariable.Value);
            Assert.AreEqual(true, secondHandlerVariable.Value);

            // the code after the occurrence of the exception inside of the OBSERVE-block should be executed. 
            Assert.AreEqual(false, afterExceptionVariable.Value);

            // but the code after the OBSERVE-block should be executed (because the exception has already been handled).
            Assert.AreEqual(true, afterObserveBlockVariable.Value);
        }

        [Test]
        public void System_Exception_Thrown_By_A_Function_Called_In_A_Query_Statement_Is_Handled_By_First_Parent_Observe_Block()
        {
            string code = @"
#MyException(INT Code) : #.Exception;
BOOL firstInnerHandler = FALSE;
BOOL secondInnerHandler = FALSE;
BOOL firstOuterHandler = FALSE;
BOOL secondOuterHandler = FALSE;
BOOL afterException = FALSE;
BOOL afterInnerObserveBlock = FALSE;
BOOL endOfProgram = FALSE;


OBSERVE
    OBSERVE
        doSomething();

        afterException = TRUE;

    HANDLE(#MyException ex)
        firstInnerHandler = TRUE;
    HANDLE(#.Exception ex)
        secondInnerHandler = TRUE;
    END

    afterInnerObserveBlock = TRUE;

HANDLE(#MyException ex)
    firstOuterHandler = TRUE;
HANDLE(#.Exception ex)
    secondOuterHandler = TRUE;
    LOG(ex.Message);
END


doSomething()
    \QueryLanguageTests\Test = 
        FROM \QueryLanguageTests\People AS p
        SELECT 
            TestFirstname = p.Firstname, 
            TestCombinationWithNull = p.Lastname + throwExceptionOnNull(p.Notes);
END

STRING throwExceptionOnNull(STRING value)
    IF value == NULL
        THROW #MyException(Code = 15, Message = ""Null Value"");
    ELSE
        RETURN value;
    END
END
    

endOfProgram = TRUE;

";

            QueryLanguageTestBase.CreatePeopleTable(_Database);

            _SyneryClient.Run(code);

            IValue firstInnerHandlerVariable = _SyneryClient.Memory.CurrentScope.ResolveVariable("firstInnerHandler");
            IValue secondInnerHandlerVariable = _SyneryClient.Memory.CurrentScope.ResolveVariable("secondInnerHandler");
            IValue firstOuterHandlerVariable = _SyneryClient.Memory.CurrentScope.ResolveVariable("firstOuterHandler");
            IValue secondOuterHandlerVariable = _SyneryClient.Memory.CurrentScope.ResolveVariable("secondOuterHandler");
            IValue afterExceptionVariable = _SyneryClient.Memory.CurrentScope.ResolveVariable("afterException");
            IValue afterInnerObserveBlockVariable = _SyneryClient.Memory.CurrentScope.ResolveVariable("afterInnerObserveBlock");
            IValue endOfProgramVariable = _SyneryClient.Memory.CurrentScope.ResolveVariable("endOfProgram");

            // both handlers should be handled, because the thrown exception is either a #.Exception and a #MyException record type.
            Assert.AreEqual(true, firstInnerHandlerVariable.Value, "First inner handler wasn't called.");
            Assert.AreEqual(true, secondInnerHandlerVariable.Value, "Second inner handler wasn't called.");
            Assert.AreEqual(false, firstOuterHandlerVariable.Value, "First outer handler shouldn't be called.");
            Assert.AreEqual(false, secondOuterHandlerVariable.Value, "Second outer handler shouldn't be called.");

            // check whether the code after the occurrence of the exception inside of the OBSERVE-block was executed:
            Assert.AreEqual(false, afterExceptionVariable.Value, "the code after the occurrence of the exception inside of the OBSERVE-block shouldn't be executed.");

            // check whether the code after the OBSERVE-block was executed (it should because the exception has already been handled).
            Assert.AreEqual(true, afterInnerObserveBlockVariable.Value, "Inner OBSERVE-block didn't complete successfully.");
            Assert.AreEqual(true, endOfProgramVariable.Value, "The execution didn't reach the end of the program.");
        }
    }
}
