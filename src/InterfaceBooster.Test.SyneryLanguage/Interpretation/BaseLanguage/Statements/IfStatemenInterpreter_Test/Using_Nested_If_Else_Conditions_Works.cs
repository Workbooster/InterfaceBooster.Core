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
    public class Using_Nested_If_Else_Conditions_Works : BaseTest
    {
        [Test]
        public void Nested_If_Statement_Block_Is_Executed()
        {
            string code = @"
INT stepNumber = 0;

IF(15 > 10)
    stepNumber = 1;
    IF(20 > 10)
        stepNumber = 2; // expected
    END
END 
";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("stepNumber");

            Assert.AreEqual(2, variable.Value);
        }

        [Test]
        public void Nested_Else_Statement_Block_Is_Executed()
        {
            string code = @"
INT stepNumber = 0;

IF(15 > 10)
    stepNumber = 1;
    IF(1 > 10)
        stepNumber = 2;
    ELSE
        stepNumber = 3; // expected
    END
ELSE
    stepNumber = 4;
END 
";

            _SyneryClient.Run(code);

            IValue variable = _SyneryClient.Memory.CurrentScope.ResolveVariable("stepNumber");

            Assert.AreEqual(3, variable.Value);
        }
    }
}
