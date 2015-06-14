using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.SyneryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;

namespace InterfaceBooster.SyneryLanguage.Interpretation.BaseLanguage.Expressions
{
    public class ExpressionListInterpreter : IInterpreter<SyneryParser.ExpressionListContext, IList<IValue>>
    {
        public ISyneryMemory Memory { get; set; }

        public IInterpretationController Controller { get; set; }

        public IList<IValue> RunWithResult(SyneryParser.ExpressionListContext context)
        {
            IList<IValue> listOfValues = new List<IValue>();

            if (context.expression() != null)
            {
                foreach (var expresssion in context.expression())
                {
                    IValue value = Controller.Interpret<SyneryParser.ExpressionContext, IValue>(expresssion);

                    listOfValues.Add(value);
                }
            }

            return listOfValues;
        }
    }
}
