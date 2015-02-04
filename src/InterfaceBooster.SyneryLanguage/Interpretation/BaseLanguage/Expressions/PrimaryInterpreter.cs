using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.ErrorHandling;
using InterfaceBooster.Common.Interfaces.SyneryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;

namespace InterfaceBooster.SyneryLanguage.Interpretation.BaseLanguage.Expressions
{
    /// <summary>
    /// Interprets primary expressions like literal values, variable references or function calls
    /// </summary>
    public class PrimaryInterpreter : IInterpreter<SyneryParser.PrimaryContext, IValue>
    {
        #region PROPERTIES
        
        public ISyneryMemory Memory { get; set; }
        public IInterpretationController Controller { get; set; }

        #endregion

        #region PUBLIC METHODS

        public IValue Run(SyneryParser.PrimaryContext context)
        {
            if (context.ChildCount > 1)
            {
                if (context.GetChild(0).GetText() == "("
                    && context.GetChild(2).GetText() == ")")
                {
                    return Controller.Interpret<SyneryParser.ExpressionContext, IValue>(context.expression());
                }
            }
            else if (context.singleValue() != null)
            {
                return Controller.Interpret<SyneryParser.SingleValueContext, IValue>(context.singleValue());
            }
            else if (context.recordInitializer() != null)
            {
                return Controller.Interpret<SyneryParser.RecordInitializerContext, IValue>(context.recordInitializer());
            }

            throw new SyneryInterpretationException(context, 
                string.Format("Unknown primary expression in {0}: '{1}'", this.GetType().Name, context.GetText()));
        }

        #endregion
    }
}
