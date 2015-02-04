using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;

namespace InterfaceBooster.Test.SyneryLanguage.Interpretation.BaseLanguage.Statements.IfStatemenInterpreter_Test
{
    [TestFixture]
    public class Using_If_Else_Conditions_Works : BaseTest
    {
        [Test]
        public void Statement_Block_Is_Executed_When_Condition_Is_True()
        {
            string code = @"
BOOL isChanged = FALSE;

IF(15 > 10)
    isChanged = TRUE;
END 
";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("isChanged");

            Assert.AreEqual(true, variable.Value);
        }

        [Test]
        public void Statement_Block_Is_Not_Executed_When_Condition_Is_False()
        {
            string code = @"
BOOL isChanged = FALSE;

IF(1 > 10)
    isChanged = TRUE;
END 
";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("isChanged");

            Assert.AreEqual(false, variable.Value);
        }

        [Test]
        public void Else_Statement_Block_Is_Executed_In_A_Simple_If_Else_Statement()
        {
            string code = @"
INT stepNumber = 0;

IF(1 > 10)
    stepNumber = 1;
ELSE
    stepNumber = 2;
END 
";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("stepNumber");

            Assert.AreEqual(2, variable.Value);
        }

        [Test]
        public void Else_Statement_Block_Is_Executed_When_Multiple_If_Statements_Fail()
        {
            string code = @"
INT stepNumber = 0;

IF(1 > 10)
    stepNumber = 1;
ELSE IF(5 > 10)
    stepNumber = 2;
ELSE IF(7 > 10)
    stepNumber = 3;
ELSE
    stepNumber = 4;
END 
";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("stepNumber");

            Assert.AreEqual(4, variable.Value);
        }

        [Test]
        public void Else_If_Statement_Block_Is_Executed_When_Multiple_If_Statements_Fail()
        {
            string code = @"
INT stepNumber = 0;

IF(1 > 10)
    stepNumber = 1;
ELSE IF(5 > 10)
    stepNumber = 2;
ELSE IF(7 > 10)
    stepNumber = 3;
ELSE IF(12 > 10)
    stepNumber = 4;
ELSE
    stepNumber = 5;
END 
";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("stepNumber");

            Assert.AreEqual(4, variable.Value);
        }
    }
}
