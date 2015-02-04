using InterfaceBooster.Database.Interfaces.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.SyneryLanguage.Interpretation.QueryLanguage.Expressions;
using InterfaceBooster.SyneryLanguage.Model.QueryLanguage;
using InterfaceBooster.Common.Interfaces.ErrorHandling;
using InterfaceBooster.Common.Interfaces.SyneryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.QueryLanguage;

namespace InterfaceBooster.SyneryLanguage.Interpretation.QueryLanguage.Commands
{
    public class RequestWhereCommandInterpreter : IInterpreter<SyneryParser.RequestWhereCommandContext, ITable, QueryMemory>
    {
        #region PROPERTIES

        public ISyneryMemory Memory { get; set; }
        public IInterpretationController Controller { get; set; }

        #endregion

        #region PUBLIC METHODS

        public ITable Run(SyneryParser.RequestWhereCommandContext context, QueryMemory queryMemory)
        {
            IExpressionValue expressionValue = Controller.Interpret<SyneryParser.RequestExpressionContext, IExpressionValue, QueryMemory>(context.requestExpression(), queryMemory);

            if (expressionValue.ResultType == typeof(bool))
            {
                // create a lambda expression that selects all fields as an object-array
                var body = expressionValue.Expression;
                var lambda = Expression.Lambda<Func<object[], int, bool>>(body, queryMemory.RowExpression, queryMemory.IndexExpression);

                // run the LINQ select using the created object-arry select lambda expression
                IEnumerable<object[]> data = queryMemory.CurrentTable.Where(lambda.Compile());

                queryMemory.CurrentTable.SetData(data);

                return queryMemory.CurrentTable;
            }
            else
            {
                throw new SyneryInterpretationException(context,
                    String.Format("The expression of a WHERE clause must result in a boolean value. The give value was of type '{0}'. Expression='{1}'",
                        expressionValue.ResultType.PublicName, context.GetText()));
            }
        }

        #endregion
    }
}
