using InterfaceBooster.Database.Interfaces.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.SyneryLanguage.Model.QueryLanguage;
using InterfaceBooster.Common.Interfaces.ErrorHandling;
using InterfaceBooster.Common.Interfaces.SyneryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.SyneryTypes;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.QueryLanguage;

namespace InterfaceBooster.SyneryLanguage.Interpretation.QueryLanguage.Expressions
{
    /// <summary>
    /// Interprets a reference on a table field. It creates an Expression that accesses to the array index the referenced field has.
    /// </summary>
    public class RequestComplexReferenceInterpreter : IInterpreter<SyneryParser.RequestComplexReferenceContext, IExpressionValue, QueryMemory>
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
        public IExpressionValue Run(SyneryParser.RequestComplexReferenceContext context, QueryMemory queryMemory)
        {
            string fieldName = context.GetText();
            IField schemaField = queryMemory.CurrentTable.Schema.GetField(fieldName);

            if (schemaField != null)
            {
                // the field was found - threat this like a field reference
                // create a LINQ expression to get the value from the IEnumerable

                return Controller.Interpret<SyneryParser.RequestFieldReferenceContext, IExpressionValue, QueryMemory>(context.requestFieldReference(), queryMemory);
            }

            // search for an other kind of complex references

            IValue value = Controller.Interpret<SyneryParser.ComplexReferenceContext, IValue>(context.requestFieldReference().complexReference());

            if (value != null)
            {
                // the value could be resolved
                // create a constant LINQ expression for the value

                // check whether it's a primitive type

                if (value.Type != typeof(IRecord))
                {
                    Expression expression;

                    if (value.Type != null)
                    {
                        expression = Expression.Constant(value.Value, value.Type.UnterlyingDotNetType);
                    }
                    else
                    {
                        expression = Expression.Constant(null);
                    }

                    return new ExpressionValue(
                        expression: expression,
                        resultType: value.Type
                    );
                }
                else
                {
                    throw new SyneryInterpretationException(context, String.Format(
                        "Cannot use the reference to the record (name='{0}') iside of a query. Only primitve types are allowed."
                        , fieldName));
                }
            }

            throw new SyneryInterpretationException(context, String.Format("A field or variable with the name '{0}' wan't found.", fieldName));
        }

        #endregion
    }
}
