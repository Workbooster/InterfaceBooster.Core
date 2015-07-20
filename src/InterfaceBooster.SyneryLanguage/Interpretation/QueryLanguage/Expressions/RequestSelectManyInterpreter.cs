using InterfaceBooster.Common.Interfaces.SyneryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.QueryLanguage;
using InterfaceBooster.Common.Interfaces.Utilities.SyneryLanguage;
using InterfaceBooster.Database.Interfaces.Structure;
using InterfaceBooster.SyneryLanguage.Model.QueryLanguage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.SyneryLanguage.Interpretation.QueryLanguage.Expressions
{
    public class RequestSelectManyInterpreter : IInterpreter<SyneryParser.RequestSelectManyContext, IDictionary<string, IExpressionValue>, QueryMemory>
    {
        #region PROPERTIES

        public ISyneryMemory Memory { get; set; }
        public IInterpretationController Controller { get; set; }

        #endregion

        #region PUBLIC METHODS

        public IDictionary<string, IExpressionValue> RunWithResult(SyneryParser.RequestSelectManyContext context, QueryMemory queryMemory)
        {
            string fieldNamePrefix = null;
            Dictionary<string, IExpressionValue> listOfFields = new Dictionary<string, IExpressionValue>();

            // A Select-Many expression can look like this: a.* or *
            // get the field prefix if one is given
            if (context.Identifier() != null)
            {
                fieldNamePrefix = String.Format("{0}.", context.Identifier().GetText());
            }

            // loop threw all fields
            for (int i = 0; i < queryMemory.CurrentTable.Schema.Fields.Count; i++)
            {
                IField field = queryMemory.CurrentTable.Schema.Fields[i];

                // add the field if there is no table prefix or the name of the current field starts with the given prefix
                if (fieldNamePrefix == null || field.Name.StartsWith(fieldNamePrefix))
                {
                    string newFieldName = field.Name.Substring(field.Name.IndexOf('.') + 1);

                    // create the array selector based on the position of the current field
                    Expression arraySelectorExpression = Expression.ArrayIndex(queryMemory.RowExpression, Expression.Constant(i));

                    ExpressionValue expressionValue = new ExpressionValue(
                        expression: arraySelectorExpression,
                        resultType: TypeHelper.GetSyneryType(field.Type)
                    );

                    listOfFields.Add(newFieldName, expressionValue);
                }
            }

            return listOfFields;
        }

        #endregion
    }
}
