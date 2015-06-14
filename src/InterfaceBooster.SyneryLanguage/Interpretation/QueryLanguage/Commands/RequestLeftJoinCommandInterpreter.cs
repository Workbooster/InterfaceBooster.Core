using InterfaceBooster.Database.Interfaces.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.SyneryLanguage.Interpretation.QueryLanguage.Common;
using InterfaceBooster.SyneryLanguage.Model.QueryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.QueryLanguage;
using InterfaceBooster.Common.Tools.Data.Array;

namespace InterfaceBooster.SyneryLanguage.Interpretation.QueryLanguage.Commands
{
    public class RequestLeftJoinCommandInterpreter : IInterpreter<SyneryParser.RequestLeftJoinCommandContext, ITable, QueryMemory>
    {
        #region INTERNAL STRUCTS

        private struct LeftJoinGroup
        {
            public object[] Outer { get; set; }
            public IEnumerable<object[]> InnerItems { get; set; }
        }

        #endregion

        #region PROPERTIES

        public ISyneryMemory Memory { get; set; }
        public IInterpretationController Controller { get; set; }

        #endregion

        #region PUBLIC METHODS

        public ITable RunWithResult(SyneryParser.RequestLeftJoinCommandContext context, QueryMemory queryMemory)
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

            IEnumerable<LeftJoinGroup> grouped = CreateGroupJoin(outerTable, innerTable, queryMemory, context.requestJoinOuterKeySelector(), context.requestJoinInnerKeySelector());


            ParameterExpression outerRowExpression = Expression.Parameter(typeof(LeftJoinGroup), "o");
            ParameterExpression innerRowExpression = Expression.Parameter(typeof(object[]), "i");

            // create the schema for the future table
            // create the expressions for the selected fields

            KeyValuePair<ISchema, List<Expression>> schemaAndFieldExpressions = GetSchemaAndFieldExpressionsFromJoinTables(innerTable, outerTable, innerRowExpression, outerRowExpression);

            ISchema newSchema = schemaAndFieldExpressions.Key;
            List<Expression> listOfFieldExpressions = schemaAndFieldExpressions.Value;

            // create the lamba function with the list of selected fields (all field from the outer and the inner table)
            Expression resultSelectorBody = Expression.NewArrayInit(typeof(object), listOfFieldExpressions);
            var resultSelectorLambda = Expression.Lambda<System.Func<LeftJoinGroup, object[], object[]>>(resultSelectorBody, outerRowExpression, innerRowExpression);

            // put the LINQ SelectMany command together

            IEnumerable<object[]> data = grouped.SelectMany(
                group => group.InnerItems.DefaultIfEmpty(new object[innerTable.Schema.Fields.Count]),
                resultSelectorLambda.Compile()
                );

            ITable newTable = Memory.Database.NewTable(newSchema, data);

            return newTable;
        }

        #endregion

        #region INTERNAL METHODS

        private IEnumerable<LeftJoinGroup> CreateGroupJoin(ITable outerTable, ITable innerTable, QueryMemory queryMemory, SyneryParser.RequestJoinOuterKeySelectorContext outerKeyContext, SyneryParser.RequestJoinInnerKeySelectorContext innerKeyContext)
        {
            // prepare the key fields for the comparison
            // all key fields are added to a new object-array which then are compared to connect the records of the inner and outer table

            // get the expressions for the outer table's keys
            IEnumerable<IExpressionValue> listOfOuterKeyExpressionValues = Controller
                .Interpret<SyneryParser.RequestExpressionListContext, IList<IExpressionValue>, QueryMemory>(outerKeyContext.requestExpressionList(), queryMemory);

            queryMemory.CurrentTable = innerTable;

            // get the expressions for the inner table's keys
            IEnumerable<IExpressionValue> listOfInnerKeyExpressionValues = Controller
                .Interpret<SyneryParser.RequestExpressionListContext, IList<IExpressionValue>, QueryMemory>(innerKeyContext.requestExpressionList(), queryMemory);

            // create the lambda function for the inner and the outer table keys
            var outerKeyLambda = JoinHelper.GetKeySelectorLambda(listOfOuterKeyExpressionValues, queryMemory.RowExpression);
            var innerKeyLambda = JoinHelper.GetKeySelectorLambda(listOfInnerKeyExpressionValues, queryMemory.RowExpression);

            // create the row parameters for the GroupJoin resultSelector
            ParameterExpression outerRowExpression = Expression.Parameter(typeof(object[]), "o");
            ParameterExpression innerRowExpression = Expression.Parameter(typeof(IEnumerable<object[]>), "i");

            // create the body of the GroupJoin resultSelector that more ore less looks like this:
            // (o, i) => new LeftJoinGroup() { Outer = o, InnerItems = i }

            var leftJoinGroupType = typeof(LeftJoinGroup);
            var leftJoinGroupConstructor = Expression.New(leftJoinGroupType);
            var leftJoinGroupOuterProperty = leftJoinGroupType.GetProperty("Outer");
            var leftJoinGroupOuterAssignment = Expression.Bind(leftJoinGroupOuterProperty, outerRowExpression);
            var leftJoinGroupInnerItemsProperty = leftJoinGroupType.GetProperty("InnerItems");
            var leftJoinGroupInnerItemsAssignment = Expression.Bind(leftJoinGroupInnerItemsProperty, innerRowExpression);
            var leftJoinGroupInit = Expression.MemberInit(leftJoinGroupConstructor, leftJoinGroupOuterAssignment, leftJoinGroupInnerItemsAssignment);

            var resultSelectorLambda = Expression.Lambda<System.Func<object[], IEnumerable<object[]>, LeftJoinGroup>>(leftJoinGroupInit, outerRowExpression, innerRowExpression);

            return outerTable.GroupJoin(innerTable
                , outerKeyLambda.Compile()
                , innerKeyLambda.Compile()
                , resultSelectorLambda.Compile()
                , new ObjectArrayEqualityComparer());
        }

        private KeyValuePair<ISchema, List<Expression>> GetSchemaAndFieldExpressionsFromJoinTables(ITable innerTable, ITable outerTable, ParameterExpression innerRowExpression, ParameterExpression outerRowExpression)
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

                Expression accessOuterPropertyExpression = Expression.PropertyOrField(outerRowExpression, "Outer");
                Expression arraySelectorExpression = Expression.ArrayIndex(accessOuterPropertyExpression, Expression.Constant(i));
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
