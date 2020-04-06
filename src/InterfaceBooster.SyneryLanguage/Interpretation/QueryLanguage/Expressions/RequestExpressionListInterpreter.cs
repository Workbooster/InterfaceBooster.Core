using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.SyneryLanguage.Model.QueryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.QueryLanguage;

namespace InterfaceBooster.SyneryLanguage.Interpretation.QueryLanguage.Expressions
{
    public class RequestExpressionListInterpreter : IInterpreter<SyneryParser.RequestExpressionListContext, IList<IExpressionValue>, QueryMemory>
    {
        #region PROPERTIES

        public ISyneryMemory Memory { get; set; }
        public IInterpretationController Controller { get; set; }

        #endregion

        #region PUBLIC METHODS

        public IList<IExpressionValue> RunWithResult(SyneryParser.RequestExpressionListContext context, QueryMemory queryMemory)
        {
            List<IExpressionValue> listOfExpressions = new List<IExpressionValue>();

            foreach (var requestExpressionContext in context.requestExpression())
            {
                IExpressionValue expressionValue = Controller
                    .Interpret<SyneryParser.RequestExpressionContext, IExpressionValue, QueryMemory>(requestExpressionContext, queryMemory);

                // Convert all values to objects to avoid problems when initializing an array of objects.
                // Otherwise an exception may be thrown. For example:
                // "An expression of type 'System.Int32' cannot be used to initialize an array of type 'System.Object'"
                expressionValue.Expression = Expression.Convert(expressionValue.Expression, typeof(object));

                listOfExpressions.Add(expressionValue);
            }

            return listOfExpressions;
        }

        #endregion
    }
}
