using InterfaceBooster.Database.Interfaces.Structure;
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
using InterfaceBooster.Common.Tools.Data.Array;

namespace InterfaceBooster.SyneryLanguage.Interpretation.QueryLanguage.Commands
{
    public class RequestOrderByCommandInterpreter : IInterpreter<SyneryParser.RequestOrderByCommandContext, ITable, QueryMemory>
    {
        #region PROPERTIES

        public ISyneryMemory Memory { get; set; }
        public IInterpretationController Controller { get; set; }

        #endregion

        #region PUBLIC METHODS

        public ITable Run(SyneryParser.RequestOrderByCommandContext context, QueryMemory queryMemory)
        {
            List<Expression> listOfFieldExpressions = new List<Expression>();

            foreach (var fieldReferenceContext in context.requestFieldReference())
            {
                IExpressionValue expressionValue = Controller
                    .Interpret<SyneryParser.RequestFieldReferenceContext, IExpressionValue, QueryMemory>(fieldReferenceContext, queryMemory);

                listOfFieldExpressions.Add(expressionValue.Expression);
            }

            // create a lambda expression that selects all orderby-fields as an object-array
            var body = Expression.NewArrayInit(typeof(object), listOfFieldExpressions);
            var lambda = Expression.Lambda<Func<object[], object[]>>(body, queryMemory.RowExpression);

            // run the LINQ orderby using the created object-array lambda expression as the list of sort-fields

            IEnumerable<object[]> data;

            if (context.DESC() == null)
            {
                // order by ascending (default)
                data = queryMemory.CurrentTable.OrderBy(lambda.Compile(), new ObjectArrayComparer());
            }
            else
            {
                // order by descending
                data = queryMemory.CurrentTable.OrderByDescending(lambda.Compile(), new ObjectArrayComparer());
            }

            // overwrite the data of the current table and return it

            queryMemory.CurrentTable.SetData(data);

            return queryMemory.CurrentTable;
        }

        #endregion
    }
}
