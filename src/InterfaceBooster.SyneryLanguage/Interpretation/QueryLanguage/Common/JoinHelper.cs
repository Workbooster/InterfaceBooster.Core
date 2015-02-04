using InterfaceBooster.Database.Interfaces.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.QueryLanguage;

namespace InterfaceBooster.SyneryLanguage.Interpretation.QueryLanguage.Common
{
    /// <summary>
    /// Contains some helper methods for joining tables
    /// </summary>
    public static class JoinHelper
    {
        /// <summary>
        /// Gets a lambda function for a key selector (e.g. in a JOIN command)
        /// </summary>
        /// <param name="listOfKeyExpressions"></param>
        /// <param name="rowExpression"></param>
        /// <returns></returns>
        public static Expression<Func<object[], object[]>> GetKeySelectorLambda(IEnumerable<IExpressionValue> listOfExpressionsValues, ParameterExpression rowExpression)
        {
            IEnumerable<Expression> listOfKeyExpressions = listOfExpressionsValues.Select(ev => Expression.Convert(ev.Expression, typeof(object)));

            // create the lambda function for the table's keys
            var innerKeyBody = Expression.NewArrayInit(typeof(object), listOfKeyExpressions);
            var innerKeyLambda = Expression.Lambda<Func<object[], object[]>>(innerKeyBody, rowExpression);

            return innerKeyLambda;
        }

        /// <summary>
        /// Prepends the given table alias to all fields in the given table.
        /// (Example of a table alias: "p.Firstname" - the "p" is the table alias)
        /// </summary>
        /// <param name="table"></param>
        /// <param name="tableAlias"></param>
        /// <returns></returns>
        public static ITable PrependTableAliasToFieldNames(ITable table, string tableAlias)
        {
            // prepend table preffix to the field names
            foreach (IField field in table.Schema.Fields)
            {
                field.Name = String.Format("{0}.{1}", tableAlias, field.Name);
            }

            return table;
        }
    }
}
