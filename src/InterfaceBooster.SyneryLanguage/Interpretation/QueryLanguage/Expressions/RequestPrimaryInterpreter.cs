using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.SyneryLanguage.Interpretation.BaseLanguage.Expressions;
using InterfaceBooster.SyneryLanguage.Model.QueryLanguage;
using InterfaceBooster.Common.Interfaces.ErrorHandling;
using InterfaceBooster.Common.Interfaces.SyneryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.QueryLanguage;

namespace InterfaceBooster.SyneryLanguage.Interpretation.QueryLanguage.Expressions
{
    /// <summary>
    /// Interprets primary expressions like literal values, variable references or field references
    /// </summary>
    public class RequestPrimaryInterpreter : IInterpreter<SyneryParser.RequestPrimaryContext, IExpressionValue, QueryMemory>
    {
        #region PROPERTIES

        public ISyneryMemory Memory { get; set; }
        public IInterpretationController Controller { get; set; }

        #endregion

        #region PUBLIC METHODS

        public IExpressionValue RunWithResult(SyneryParser.RequestPrimaryContext context, QueryMemory queryMemory)
        {

            if (context.ChildCount > 1)
            {
                if (context.GetChild(0).GetText() == "("
                    && context.GetChild(2).GetText() == ")")
                {
                    return Controller.Interpret<SyneryParser.RequestExpressionContext, IExpressionValue, QueryMemory>(context.requestExpression(), queryMemory);
                }
            }
            else if (context.requestSingleValue() != null)
            {
                return Controller.Interpret<SyneryParser.RequestSingleValueContext, IExpressionValue, QueryMemory>(context.requestSingleValue(), queryMemory);
            }

            throw new SyneryInterpretationException(context, string.Format("Unknown request primary expression: {0}", context.GetText()));
        }

        #endregion
    }
}
