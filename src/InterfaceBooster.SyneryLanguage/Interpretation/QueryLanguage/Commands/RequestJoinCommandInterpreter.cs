using InterfaceBooster.Database.Interfaces.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.SyneryLanguage.Interpretation.QueryLanguage.Common;
using InterfaceBooster.SyneryLanguage.Interpretation.QueryLanguage.Expressions;
using InterfaceBooster.SyneryLanguage.Model.QueryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.QueryLanguage;
using InterfaceBooster.Common.Tools.Data.Array;

namespace InterfaceBooster.SyneryLanguage.Interpretation.QueryLanguage.Commands
{
    public class RequestJoinCommandInterpreter : IInterpreter<SyneryParser.RequestJoinCommandContext, ITable, QueryMemory>
    {
        #region PROPERTIES

        public ISyneryMemory Memory { get; set; }
        public IInterpretationController Controller { get; set; }

        #endregion

        #region PUBLIC METHODS

        public ITable RunWithResult(SyneryParser.RequestJoinCommandContext context, QueryMemory queryMemory)
        {
            // load join tables
            string tableName = context.InternalPathIdentifier().GetText();
            ITable outerTable = queryMemory.CurrentTable;
            ITable innerTable = Memory.Database.LoadTable(tableName);

            // prepent the table alias if an alias is given
            if (context.requestJoinPrefix() != null)
            {
                string tableAlias = context.requestJoinPrefix().Identifier().GetText();
                JoinHelper.PrependTableAliasToFieldNames(innerTable, tableAlias);
            }

            // prepare the key fields for the comparison
            // all key fields are added to a new object-array which then are compared to connect the records of the inner and outer table

            // get the expressions for the outer table's keys
            IEnumerable<IExpressionValue> listOfOuterKeyExpressions = Controller
                .Interpret<SyneryParser.RequestExpressionListContext, IList<IExpressionValue>, QueryMemory>(context.requestJoinOuterKeySelector().requestExpressionList(), queryMemory);

            queryMemory.CurrentTable = innerTable;

            // get the expressions for the inner table's keys
            IEnumerable<IExpressionValue> listOfInnerKeyExpressions = Controller
                .Interpret<SyneryParser.RequestExpressionListContext, IList<IExpressionValue>, QueryMemory>(context.requestJoinInnerKeySelector().requestExpressionList(), queryMemory);

            // create the lambda function for the inner and the outer table keys
            var outerKeyLambda = JoinHelper.GetKeySelectorLambda(listOfOuterKeyExpressions, queryMemory.RowExpression);
            var innerKeyLambda = JoinHelper.GetKeySelectorLambda(listOfInnerKeyExpressions, queryMemory.RowExpression);

            // create the row parameters for the Join resultSelector
            ParameterExpression outerRowExpression = Expression.Parameter(typeof(object[]), "o");
            ParameterExpression innerRowExpression = Expression.Parameter(typeof(object[]), "i");

            // create the schema for the future table
            // create the expressions for the selected fields

            KeyValuePair<ISchema, List<Expression>> schemaAndFieldExpressions = GetSchemaAndFieldExpressionsFromJoinTables(innerTable, outerTable, innerRowExpression, outerRowExpression);

            ISchema newSchema = schemaAndFieldExpressions.Key;
            List<Expression> listOfFieldExpressions = schemaAndFieldExpressions.Value;

            // create the lamba function with the list of selected fields (all field from the outer and the inner table)
            Expression resultSelectorBody = Expression.NewArrayInit(typeof(object), listOfFieldExpressions);
            var resultSelectorLambda = Expression.Lambda<System.Func<object[], object[], object[]>>(resultSelectorBody, outerRowExpression, innerRowExpression);

            /*
            // example of an alternative for resultSelectorLambda as lambda-Func for manually testing:
            
            Func<object[], object[], object[]> resultSelector = (o, i) =>
            {
                object[] result = new object[i.Length + o.Length];
                o.CopyTo(result, 0);
                i.CopyTo(result, o.Length);
                return result;
            };
            */

            // put the LINQ join command together

            IEnumerable<object[]> data = outerTable.Join(innerTable
                                                        , outerKeyLambda.Compile()
                                                        , innerKeyLambda.Compile()
                                                        , resultSelectorLambda.Compile()
                                                        , new ObjectArrayEqualityComparer());

            ITable newTable = Memory.Database.NewTable(newSchema, data);

            return newTable;
        }

        #endregion

        #region INTERNAL METHODS

        private KeyValuePair<ISchema, List<Expression>> GetSchemaAndFieldExpressionsFromJoinTables(ITable innerTable, ITable outerTable, Expression innerRowExpression, Expression outerRowExpression)
        {
            ISchema schema = Memory.Database.NewSchema();
            List<Expression> listOfFieldExpressions = new List<Expression>();
            int numberOfOuterFields = outerTable.Schema.Fields.Count;
            int numberOfInnerFields = innerTable.Schema.Fields.Count;

            // add all outer fields to the schema
            for (int i = 0; i < numberOfOuterFields; i++)
            {
                var field = outerTable.Schema.Fields[i];

                schema.AddField(field.Name, field.Type);

                Expression arraySelectorExpression = Expression.ArrayIndex(outerRowExpression, Expression.Constant(i));
                listOfFieldExpressions.Add(arraySelectorExpression);
            }

            // add inner fields to the schema
            for (int i = 0; i < numberOfInnerFields; i++)
            {
                var field = innerTable.Schema.Fields[i];

                schema.AddField(field.Name, field.Type);

                Expression arraySelectorExpression = Expression.ArrayIndex(innerRowExpression, Expression.Constant(i));
                listOfFieldExpressions.Add(arraySelectorExpression);
            }

            return new KeyValuePair<ISchema, List<Expression>>(schema, listOfFieldExpressions);
        }

        #endregion
    }
}
