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
using InterfaceBooster.Common.Interfaces.ErrorHandling;
using InterfaceBooster.Common.Tools.Data.ExceptionHandling;

namespace InterfaceBooster.SyneryLanguage.Interpretation.QueryLanguage.Expressions
{
    public class RequestSelectSingleContextInterpreter : IInterpreter<SyneryParser.RequestSelectSingleContext, Expression, QueryMemory>
    {
        #region PROPERTIES

        public ISyneryMemory Memory { get; set; }
        public IInterpretationController Controller { get; set; }

        #endregion

        #region PUBLIC METHODS

        public Expression RunWithResult(SyneryParser.RequestSelectSingleContext context, QueryMemory memory)
        {
            // interpret single field (direct field reference or field expression)

            IExpressionValue fieldExpressionValue = null;
            string fieldName = null;

            try
            {
                if (context.requestFieldReference() != null)
                {
                    // single field
                    // Example: SELECT IdPerson
                    // Example: SELECT p.PersonName

                    KeyValuePair<string, IExpressionValue> field = Controller
                        .Interpret<SyneryParser.RequestFieldReferenceContext, KeyValuePair<string, IExpressionValue>, QueryMemory>(context.requestFieldReference(), memory);

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
            }
            catch (Exception ex)
            {
                if (ex is SyneryException) throw;

                throw new SyneryInterpretationException(context, String.Format(
                            "Error while initializing the field '{1}' in '{2}' starting on line {3}. {0}Message: {4}",
                            Environment.NewLine,
                            fieldName,
                            context.GetText(),
                            context.Start.Line,
                            ExceptionHelper.GetNestedExceptionMessages(ex)), ex);
            }

            // append the field expression and field definition

            if (fieldExpressionValue != null && String.IsNullOrEmpty(fieldName) == false)
            {
                Expression selectItemExpresson = fieldExpressionValue.Expression;
                Expression objectExpression = Expression.Convert(selectItemExpresson, typeof(object));

                memory.NewSchema.AddField(fieldName, fieldExpressionValue.ResultType.UnterlyingDotNetType);
                return objectExpression;
            }

            throw new NotImplementedException("Unknown RequestSelectItem");
        }

        #endregion

        #region INTERNAL METHODS

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
