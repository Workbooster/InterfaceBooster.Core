using InterfaceBooster.Database.Interfaces.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.SyneryLanguage.Interpretation.General;
using InterfaceBooster.SyneryLanguage.Model.QueryLanguage;
using InterfaceBooster.Common.Interfaces.ErrorHandling;
using InterfaceBooster.Common.Interfaces.SyneryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.QueryLanguage;

namespace InterfaceBooster.SyneryLanguage.Interpretation.QueryLanguage.Expressions
{
    /// <summary>
    /// Interprets a reference on a table field. It creates an Expression that accesses to the array index the referenced field has.
    /// </summary>
    public class RequestFieldReferenceInterpreter : IInterpreter<SyneryParser.RequestFieldReferenceContext, IExpressionValue, QueryMemory>
    {
        #region PROPERTIES

        public ISyneryMemory Memory { get; set; }
        public IInterpretationController Controller { get; set; }

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Creates an Expression that accesses to the array index the referenced field has.
        /// If the referenced field wasn't found it throws a SyneryException.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="queryMemory"></param>
        /// <returns></returns>
        public IExpressionValue Run(SyneryParser.RequestFieldReferenceContext context, QueryMemory queryMemory)
        {
            // field identifier
            // create a linq expression to get the value from the IEnumerable
            string fieldName = context.GetText();
            IField schemaField = queryMemory.CurrentTable.Schema.GetField(fieldName);
            int fieldPosition = queryMemory.CurrentTable.Schema.GetFieldPosition(fieldName);

            if (fieldPosition != -1)
            {
                // the field was found - create an array index Expression to access the field value

                Expression arraySelectorExpression = Expression.ArrayIndex(queryMemory.RowExpression, Expression.Constant(fieldPosition));

                return new ExpressionValue(
                    expression: arraySelectorExpression,
                    resultType: TypeHelper.GetSyneryType(schemaField.Type)
                );
            }
            else
            {
                throw new SyneryInterpretationException(context, String.Format("A field with the name '{0}' wasn't found.", fieldName));
            }

        }

        #endregion
    }
}
