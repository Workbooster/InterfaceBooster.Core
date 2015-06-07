using InterfaceBooster.Common.Interfaces.ErrorHandling;
using InterfaceBooster.Common.Interfaces.SyneryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.SyneryTypes;
using InterfaceBooster.Common.Interfaces.Utilities.SyneryLanguage;
using InterfaceBooster.Database.Interfaces.Structure;
using InterfaceBooster.SyneryLanguage.Interpretation.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.SyneryLanguage.Interpretation.BaseLanguage.Statements
{
    public class TableAddStatementInterpreter : IInterpreter<SyneryParser.TableAddStatementContext>
    {
        #region PROPERTIES

        public ISyneryMemory Memory { get; set; }
        public IInterpretationController Controller { get; set; }

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Interprets a table assignment. It uses the RequestStatementInterpreter to get the new table.
        /// </summary>
        /// <param name="context"></param>
        public void Run(SyneryParser.TableAddStatementContext context)
        {
            string destinationTableName = context.InternalPathIdentifier().GetText();
            bool isFirstExpression = true;
            ITable table = null;

            foreach (var expressionContext in context.expressionList().expression())
            {
                // get the value from the expression
                IValue value = Controller.Interpret<SyneryParser.ExpressionContext, IValue>(expressionContext);

                // verify that the value is a record

                if ((value.Value is IRecord) == false)
                    throw new SyneryInterpretationException(expressionContext,
                        String.Format("The given value must be a Record."));


                IRecord record = (IRecord)value.Value;

                if (isFirstExpression == true)
                {
                    // prepare the table based on the first record

                    if (Memory.Database.IsTable(destinationTableName))
                    {
                        table = Memory.Database.LoadTable(destinationTableName);
                    }
                    else
                    {
                        table = CreateNewTableFromRecord(record);
                    }

                    isFirstExpression = false;
                }

                // append the record as a table row

                AddRecordAsRow(table, record);
            }

            // save the table

            Memory.Database.CreateOrUpdateTable(destinationTableName, table);
        }

        #endregion

        #region INTERNAL METHODS

        private ITable CreateNewTableFromRecord(IRecord record)
        {
            ISchema schema = Memory.Database.NewSchema();
            ITable table = Memory.Database.NewTable(schema);

            foreach (var field in record.RecordType.Fields)
            {
                if (TypeHelper.IsPrimitiveType(field.Type))
                {
                    schema.AddField(field.Name, field.Type.UnterlyingDotNetType);
                }
            }

            return table;
        }

        private void AddRecordAsRow(ITable table, IRecord record)
        {
            object[] row = new object[table.Schema.Fields.Count];

            // loop threw all table fields and try to get the value from the record
            for (int i = 0; i < table.Schema.Fields.Count; i++)
            {
                string fieldName = table.Schema.Fields[i].Name;

                if (record.DoesFieldExists(fieldName))
                {
                    row[i] = record.Data[fieldName].Value;
                }
            }

            table.Add(row);
        }

        #endregion
    }
}
