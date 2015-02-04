using InterfaceBooster.Database.Interfaces.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.SyneryLanguage.Interpretation.QueryLanguage.Commands;
using InterfaceBooster.SyneryLanguage.Model.QueryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;

namespace InterfaceBooster.SyneryLanguage.Interpretation.QueryLanguage.Statements
{
    public class RequestStatementInterpreter : IInterpreter<SyneryParser.RequestStatementContext, ITable>
    {
        #region PROPERTIES

        public ISyneryMemory Memory { get; set; }
        public IInterpretationController Controller { get; set; }

        #endregion

        #region PUBLIC METHODS

        public ITable Run(SyneryParser.RequestStatementContext context)
        {
            QueryMemory queryMemory = new QueryMemory
            {
                CurrentTable = InterpretRequestFromCommand(context.requestFromCommand()),
                NewSchema = Memory.Database.NewSchema(),
                RowExpression = Expression.Parameter(typeof(object[]), "x"),
                IndexExpression = Expression.Parameter(typeof(int), "index"),
            };

            // loop threw the request commands and interpret them

            foreach (SyneryParser.RequestCommandContext command in context.requestCommand())
            {
                if (command.requestSelectCommand() != null)
                {
                    queryMemory.CurrentTable = Controller.Interpret<SyneryParser.RequestSelectCommandContext, ITable, QueryMemory>(command.requestSelectCommand(), queryMemory);
                }
                else if (command.requestWhereCommand() != null)
                {
                    queryMemory.CurrentTable = Controller.Interpret<SyneryParser.RequestWhereCommandContext, ITable, QueryMemory>(command.requestWhereCommand(), queryMemory);
                }
                else if (command.requestJoinCommand() != null)
                {
                    queryMemory.CurrentTable = Controller.Interpret<SyneryParser.RequestJoinCommandContext, ITable, QueryMemory>(command.requestJoinCommand(), queryMemory);
                }
                else if (command.requestLeftJoinCommand() != null)
                {
                    queryMemory.CurrentTable = Controller.Interpret<SyneryParser.RequestLeftJoinCommandContext, ITable, QueryMemory>(command.requestLeftJoinCommand(), queryMemory);
                }
                else if (command.requestOrderByCommand() != null)
                {
                    queryMemory.CurrentTable = Controller.Interpret<SyneryParser.RequestOrderByCommandContext, ITable, QueryMemory>(command.requestOrderByCommand(), queryMemory);
                }
                else if (command.requestDistinctCommand() != null)
                {
                    queryMemory.CurrentTable = Controller.Interpret<SyneryParser.RequestDistinctCommandContext, ITable, QueryMemory>(command.requestDistinctCommand(), queryMemory);
                }
            }

            //RemovePrimaryTableAliasesFromFieldNames(queryMemory.CurrentTable);
            CleanUpFieldNames(queryMemory.CurrentTable);

            return queryMemory.CurrentTable;
        }

        #endregion

        #region INTERNAL METHODS

        private ITable InterpretRequestFromCommand(SyneryParser.RequestFromCommandContext context)
        {
            string tableName = context.InternalPathIdentifier().GetText();
            ITable table = Memory.Database.LoadTable(tableName);

            // prepent the table alias if an alias is given
            if (context.Identifier() != null)
            {
                string tableAlias = context.Identifier().GetText();

                if (table != null)
                {
                    // prepend table preffix to the field names
                    foreach (IField field in table.Schema.Fields)
                    {
                        field.Name = String.Format("{0}.{1}", tableAlias, field.Name);
                    }
                }
            }

            return table;
        }

        private void RemovePrimaryTableAliasesFromFieldNames(ITable table)
        {
            // prepend table preffix to the field names
            foreach (IField field in table.Schema.Fields)
            {
                int dotIndex = field.Name.IndexOf('.');
                if (dotIndex != -1)
                {
                    field.Name = field.Name.Remove(0, dotIndex + 1);
                }
            }
        }

        /// <summary>
        /// Removes the table prefix from the field names and assures that all field names are unique
        /// by appending an incremented number at the end of the field name (e.g. "Name_2").
        /// </summary>
        /// <param name="table"></param>
        private void CleanUpFieldNames(ITable table)
        {
            foreach (var field in table.Schema.Fields)
            {
                // remove preffix
                field.Name = field.Name.Remove(0, field.Name.IndexOf('.') + 1);
                field.Name = GetUniqueFieldName(field, table.Schema.Fields);
            }
        }

        /// <summary>
        /// Gets a field name that is unique in the list of the given fields.
        /// It appends an incremented number at the end of the field name (e.g. "Name_2").
        /// </summary>
        /// <param name="field"></param>
        /// <param name="listOfFields"></param>
        /// <returns></returns>
        private string GetUniqueFieldName(IField field, IEnumerable<IField> listOfFields)
        {
            string newName = field.Name;
            int incrementor = 2;
            int numberOfEqualNames;

            do
            {
                numberOfEqualNames = (from f in listOfFields
                                      where f != field && f.Name == newName
                                      select f).Count();

                if (numberOfEqualNames == 0)
                {
                    // the current name is unique
                    return newName;
                }

                // the name is not unique -> append an incremented number and check again

                newName = String.Format("{0}_{1}", field.Name, incrementor);

                incrementor++;

            } while (numberOfEqualNames > 0);

            return newName;
        }

        #endregion
    }
}
