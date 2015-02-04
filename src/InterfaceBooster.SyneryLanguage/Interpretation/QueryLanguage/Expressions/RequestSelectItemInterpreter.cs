using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.SyneryLanguage.Interpretation.General;
using InterfaceBooster.SyneryLanguage.Model.QueryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.QueryLanguage;

namespace InterfaceBooster.SyneryLanguage.Interpretation.QueryLanguage.Expressions
{
    public class RequestSelectItemInterpreter : IInterpreter<SyneryParser.RequestSelectItemContext, Expression, QueryMemory>
    {
        #region PROPERTIES

        public ISyneryMemory Memory { get; set; }
        public IInterpretationController Controller { get; set; }

        #endregion

        #region PUBLIC METHODS

        public Expression Run(SyneryParser.RequestSelectItemContext context, QueryMemory memory)
        {
            if (context.requestFieldReference() != null || context.requestSelectFieldAssignment() != null)
            {
                // interpret single field (direct field reference or field expression

                IExpressionValue fieldExpressionValue = null;
                string fieldName = null;

                if (context.requestFieldReference() != null)
                {
                    // single field
                    // Example: SELECT p.PersonName

                    KeyValuePair<string, IExpressionValue> field = InterpretRequestFieldReference(context.requestFieldReference(), memory);
                    fieldName = field.Key;
                    fieldExpressionValue = field.Value;
                }
                else if (context.requestSelectFieldAssignment() != null)
                {
                    // field assignment
                    // Example: SELECT Name = p.PersonFirstname

                    KeyValuePair<string, IExpressionValue> field = InterpretRequestSelectFieldAssignment(context.requestSelectFieldAssignment(), memory);
                    fieldName = field.Key;
                    fieldExpressionValue = field.Value;
                }

                // append the field expression and field definition

                if (fieldExpressionValue != null && String.IsNullOrEmpty(fieldName) == false)
                {
                    Expression selectItemExpresson = fieldExpressionValue.Expression;
                    Expression objectExpression = Expression.Convert(selectItemExpresson, typeof(object));

                    memory.NewSchema.AddField(fieldName, fieldExpressionValue.ResultType.UnterlyingDotNetType);
                    return objectExpression;
                }
            }
            else if (context.requestSelectMany() != null)
            {
                throw new NotImplementedException("requestSelectMany rule isn't implemented");
            }

            throw new NotImplementedException("Unknown RequestSelectItem");
        }

        #endregion

        #region INTERNAL METHODS

        private KeyValuePair<string, IExpressionValue> InterpretRequestFieldReference(SyneryParser.RequestFieldReferenceContext context, QueryMemory queryMemory)
        {
            // get the future fieldname from it's old name by removing the alias
            string complexIdentifier = context.complexReference().GetText();
            string[] path = IdentifierHelper.ParseComplexIdentifier(complexIdentifier);
            string fieldName = path[path.Length - 1];

            // get the LINQ expression for the field reference
            IExpressionValue expressionValue = Controller
                .Interpret<SyneryParser.RequestFieldReferenceContext, IExpressionValue, QueryMemory>(context, queryMemory);

            return new KeyValuePair<string, IExpressionValue>(fieldName, expressionValue);
        }

        private KeyValuePair<string, IExpressionValue> InterpretRequestSelectFieldAssignment(SyneryParser.RequestSelectFieldAssignmentContext context, QueryMemory queryMemory)
        {
            // get the future fieldname from the user defined identifier
            SyneryParser.RequestExpressionContext expressionContext = context.requestExpression();
            string fieldName = context.Identifier().GetText();

            // get the LINQ expression for the Synery field expression
            IExpressionValue expressionValue = Controller
                .Interpret<SyneryParser.RequestExpressionContext, IExpressionValue, QueryMemory>(expressionContext, queryMemory);

            return new KeyValuePair<string, IExpressionValue>(fieldName, expressionValue);
        }

        #endregion
    }
}
