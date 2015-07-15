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
using InterfaceBooster.Common.Interfaces.Utilities.SyneryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.SyneryTypes;

namespace InterfaceBooster.SyneryLanguage.Interpretation.QueryLanguage.Expressions
{
    /// <summary>
    /// Interprets a reference on a table field. It creates an Expression that accesses to the array index the referenced field has.
    /// </summary>
    public class RequestFieldReferenceInterpreter : IInterpreter<SyneryParser.RequestFieldReferenceContext, KeyValuePair<string, IExpressionValue>, QueryMemory>
    {
        #region PROPERTIES

        public ISyneryMemory Memory { get; set; }
        public IInterpretationController Controller { get; set; }

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Creates an Expression that either accesses the array index of the referenced field or a Constand for a variable value.
        /// If the referenced field or variable wasn't found it throws a SyneryException.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="queryMemory"></param>
        /// <returns></returns>
        public KeyValuePair<string, IExpressionValue> RunWithResult(SyneryParser.RequestFieldReferenceContext context, QueryMemory queryMemory)
        {
            if (context.complexReference() != null)
            {
                // a ComplexReference could either be a field reference from a FROM or a JOIN statement or a real reference of an external variable
                // the field reference has priority over variable reference
                return InterpretComplexReference(context.complexReference(), queryMemory);
            }
            else if (context.variableReference() != null)
            {
                // a VariableReference could either be a field reference from a prior SELECT statement or a real reference of a variable
                // the field reference has priority over variable reference
                return InterpretVariableReference(context.variableReference(), queryMemory);
            }

            throw new SyneryInterpretationException(context,
                string.Format("Unknown field reference expression in {0}: '{1}'", this.GetType().Name, context.GetText()));
        }

        #endregion

        #region INTERNAL METHODS

        private KeyValuePair<string, IExpressionValue> InterpretComplexReference(SyneryParser.ComplexReferenceContext context, QueryMemory queryMemory)
        {
            // get the future fieldname from it's old name by removing the alias
            string complexIdentifier = context.GetText();
            string[] path = IdentifierHelper.ParseComplexIdentifier(complexIdentifier);
            string fieldName = path[path.Length - 1];

            // get the LINQ expression for the field reference
            IField schemaField = queryMemory.CurrentTable.Schema.GetField(complexIdentifier);
            int fieldPosition = queryMemory.CurrentTable.Schema.GetFieldPosition(complexIdentifier);

            if (fieldPosition != -1)
            {
                // the field was found - create an array index Expression to access the field value

                Expression arraySelectorExpression = Expression.ArrayIndex(queryMemory.RowExpression, Expression.Constant(fieldPosition));

                ExpressionValue expressionValue = new ExpressionValue(
                    expression: arraySelectorExpression,
                    resultType: TypeHelper.GetSyneryType(schemaField.Type)
                );

                return new KeyValuePair<string, IExpressionValue>(fieldName, expressionValue);
            }

            // search for an other kind of complex references

            IValue value = Controller.Interpret<SyneryParser.ComplexReferenceContext, IValue>(context);

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

                    ExpressionValue expressionValue = new ExpressionValue(
                        expression: expression,
                        resultType: value.Type
                    );

                    return new KeyValuePair<string, IExpressionValue>(fieldName, expressionValue);
                }
                else
                {
                    throw new SyneryInterpretationException(context, String.Format(
                        "Cannot use the reference to the record (name='{0}') inside of a query. Only primitve types are allowed."
                        , fieldName));
                }
            }

            throw new SyneryInterpretationException(context, String.Format("A field or variable with the name '{0}' wan't found.", complexIdentifier));
        }

        private KeyValuePair<string, IExpressionValue> InterpretVariableReference(SyneryParser.VariableReferenceContext context, QueryMemory queryMemory)
        {
            // get the field name from the reference text
            string fieldName = context.GetText();

            IField schemaField = queryMemory.CurrentTable.Schema.GetField(fieldName);
            int fieldPosition = queryMemory.CurrentTable.Schema.GetFieldPosition(fieldName);

            if (fieldPosition != -1)
            {
                // the field was found in the current table - create an array index Expression to access the field value

                Expression arraySelectorExpression = Expression.ArrayIndex(queryMemory.RowExpression, Expression.Constant(fieldPosition));

                ExpressionValue expressionValue = new ExpressionValue(
                    expression: arraySelectorExpression,
                    resultType: TypeHelper.GetSyneryType(schemaField.Type)
                );

                return new KeyValuePair<string, IExpressionValue>(fieldName, expressionValue);
            }
            else
            {
                // no table field found - search for a variable with the given name

                Expression expression;
                IValue value = null;

                try
                {
                    value = Memory.CurrentScope.ResolveVariable(fieldName);
                }
                catch (Exception) { /* throw exception later */ }

                // check whether the variable has been resolved

                if (value != null)
                {
                    // the variable is available - create a Constant expression from the value

                    if (value.Type != null)
                    {
                        expression = Expression.Constant(value.Value, value.Type.UnterlyingDotNetType);
                    }
                    else
                    {
                        expression = Expression.Constant(null);
                    }

                    ExpressionValue expressionValue = new ExpressionValue(
                        expression: expression,
                        resultType: value.Type
                    );

                    return new KeyValuePair<string, IExpressionValue>(fieldName, expressionValue);
                }
            }

            throw new SyneryInterpretationException(context, String.Format("A field or variable with the name '{0}' wasn't found.", fieldName));
        }

        #endregion
    }
}
