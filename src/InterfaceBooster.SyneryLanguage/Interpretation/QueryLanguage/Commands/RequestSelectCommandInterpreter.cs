﻿using InterfaceBooster.Database.Interfaces.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.SyneryLanguage.Interpretation.General;
using InterfaceBooster.SyneryLanguage.Interpretation.QueryLanguage.Expressions;
using InterfaceBooster.SyneryLanguage.Model.QueryLanguage;
using InterfaceBooster.Common.Interfaces.ErrorHandling;
using InterfaceBooster.Common.Interfaces.SyneryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.QueryLanguage;

namespace InterfaceBooster.SyneryLanguage.Interpretation.QueryLanguage.Commands
{
    public class RequestSelectCommandInterpreter : IInterpreter<SyneryParser.RequestSelectCommandContext, ITable, QueryMemory>
    {
        #region PROPERTIES

        public ISyneryMemory Memory { get; set; }
        public IInterpretationController Controller { get; set; }

        #endregion

        #region PUBLIC METHODS

        public ITable RunWithResult(SyneryParser.RequestSelectCommandContext context, QueryMemory queryMemory)
        {
            List<Expression> listOfSelectItemExpressions = new List<Expression>();
            queryMemory.NewSchema = Memory.Database.NewSchema();

            // interpret all fields in the select list
            foreach (var item in context.requestSelectItem())
            {
                if (item.requestSelectSingle() != null)
                {
                    // get expression for select item
                    Expression selectItemExpression = Controller
                        .Interpret<SyneryParser.RequestSelectSingleContext, Expression, QueryMemory>(item.requestSelectSingle(), queryMemory);

                    Expression tryCatch = SorroundFieldExpressionWithTryCatch(item.requestSelectSingle(), selectItemExpression, queryMemory);

                    listOfSelectItemExpressions.Add(tryCatch);
                }
                else if (item.requestSelectMany() != null)
                {
                    try
                    {
                        // many fields
                        // Example: SELECT a.*
                        // Example: SELECT *

                        IDictionary<string, IExpressionValue> listOfFieldItems = Controller
                            .Interpret<SyneryParser.RequestSelectManyContext, IDictionary<string, IExpressionValue>, QueryMemory>(item.requestSelectMany(), queryMemory);

                        // loop threw all the field items (name and expression) and append them to the new Schema

                        foreach (var fieldItem in listOfFieldItems)
                        {
                            string fieldName = fieldItem.Key;
                            IExpressionValue fieldExpressionValue = fieldItem.Value;

                            Expression selectItemExpression = fieldExpressionValue.Expression;
                            Expression objectExpression = Expression.Convert(selectItemExpression, typeof(object));

                            queryMemory.NewSchema.AddField(fieldName, fieldExpressionValue.ResultType.UnterlyingDotNetType);

                            Expression tryCatch = SorroundFieldExpressionWithTryCatch(item.requestSelectMany(), objectExpression, queryMemory);

                            listOfSelectItemExpressions.Add(tryCatch);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex is SyneryException) throw;

                        throw new SyneryInterpretationException(item.requestSelectSingle(), ex.Message, ex);
                    }
                }
            }

            // create a lambda expression that selects all fields as an object-array
            var body = Expression.NewArrayInit(typeof(object), listOfSelectItemExpressions);
            var lambda = Expression.Lambda<Func<object[], int, object[]>>(body, queryMemory.RowExpression, queryMemory.IndexExpression);

            // run the LINQ select using the created object-array select lambda expression
            IEnumerable<object[]> data = queryMemory.CurrentTable.Select(lambda.Compile());

            // create a new table instance
            ITable newTable = Memory.Database.NewTable(queryMemory.NewSchema, data);

            return newTable;
        }

        #endregion

        #region INTERNAL METHODS

        /// <summary>
        /// Surrounds the select item with a try/catch block
        /// </summary>
        /// <param name="selectItemExpression"></param>
        /// <returns></returns>
        private static Expression SorroundFieldExpressionWithTryCatch(Antlr4.Runtime.ParserRuleContext context, Expression selectItemExpression, QueryMemory queryMemory)
        {
            ParameterExpression exceptionParameter = Expression.Parameter(typeof(Exception), "ex");
            MethodInfo createException = typeof(RequestSelectCommandInterpreter).GetMethod("CreateRequestSelectItemContextInterpretationException", new Type[] { typeof(Antlr4.Runtime.ParserRuleContext), typeof(int), typeof(object[]), typeof(Exception) });
            Expression throwExpression = Expression.Throw(Expression.Call(createException, Expression.Constant(context, typeof(Antlr4.Runtime.ParserRuleContext)), queryMemory.IndexExpression, queryMemory.RowExpression, exceptionParameter), typeof(object));
            CatchBlock catchExpression = Expression.Catch(exceptionParameter, throwExpression);
            Expression tryCatch = Expression.TryCatch(selectItemExpression, catchExpression);

            return tryCatch;
        }

        /// <summary>
        /// creates an exception that represents a runtime error with the interpretation of a RequestSelectItem
        /// </summary>
        /// <param name="context"></param>
        /// <param name="index">the row index of the current record</param>
        /// <param name="record">the current record</param>
        /// <param name="innerException">the original exception</param>
        /// <returns>an exception that contains all available details for the current context</returns>
        public static SyneryQueryInterpretationException CreateRequestSelectItemContextInterpretationException(Antlr4.Runtime.ParserRuleContext context, int index, object[] record, Exception innerException)
        {
            string values = "";

            foreach (var item in record)
            {
                values += item + ",";
            }

            values = values.TrimEnd(new char[] { ',' });

            return new SyneryQueryInterpretationException(context, index, record, string.Format("Error starting on line {0} with record index {1} ({2}). The error message was: {3}", context.Start.Line, index, values, innerException.Message), innerException);
        }

        #endregion
    }
}
