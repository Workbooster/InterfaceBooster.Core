using InterfaceBooster.Common.Interfaces.ErrorHandling;
using InterfaceBooster.Common.Interfaces.SyneryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.SyneryTypes;
using InterfaceBooster.Database.Interfaces.Structure;
using InterfaceBooster.SyneryLanguage.Interpretation.General;
using InterfaceBooster.SyneryLanguage.Model.Context;
using InterfaceBooster.SyneryLanguage.Model.SyneryTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.SyneryLanguage.Interpretation.BaseLanguage.Statements
{
    public class EachStatementInterpreter : IInterpreter<SyneryParser.EachStatementContext>
    {
        #region PROPERTIES

        public ISyneryMemory Memory { get; set; }
        public IInterpretationController Controller { get; set; }

        #endregion

        #region PUBLIC METHODS

        public void Run(SyneryParser.EachStatementContext context)
        {
            string parameterName = context.Identifier().GetText();
            string tableName = context.tableIdentifier().GetText();
            var recordTypeDefinition = Controller.Interpret<SyneryParser.RecordTypeContext, KeyValuePair<SyneryType, IRecordType>>(context.recordType());

            if (Memory.Database.IsTable(tableName))
            {
                ITable table = Memory.Database.LoadTable(tableName);

                foreach (var row in table)
                {
                    // initialize a new scope for this block and push it on the scope stack

                    INestedScope blockScope = new BlockScope(Memory.CurrentScope);

                    // create the record from the row and declare/assign it as local variable

                    IRecord record = GetRecordFromTableRow(recordTypeDefinition.Value, row, table.Schema);

                    blockScope.DeclareVariable(parameterName, recordTypeDefinition.Key);
                    blockScope.AssignVariable(parameterName, record);

                    Controller.Interpret<SyneryParser.BlockContext, INestedScope, INestedScope>(context.block(), blockScope);

                    // stop execution if return was called
                    if (blockScope.IsTerminated == true)
                        break;
                }
            }
        }

        #endregion

        #region INTERNAL METHODS

        private IRecord GetRecordFromTableRow(IRecordType recordType, object[] row, ISchema schema)
        {
            IRecord record = new Record(recordType);

            foreach (var field in recordType.Fields)
            {
                var column = schema.GetField(field.Name);

                if (column != null && column.Type == field.Type.UnterlyingDotNetType)
                {
                    int fieldPosition = schema.GetFieldPosition(field.Name);

                    IValue value = new TypedValue(field.Type, row[fieldPosition]);
                    record.SetFieldValue(field.Name, value);
                }
            }

            return record;
        }

        #endregion
    }
}
