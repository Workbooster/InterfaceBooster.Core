using InterfaceBooster.Database.Interfaces.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.SyneryLanguage.Interpretation.General;
using InterfaceBooster.SyneryLanguage.Interpretation.QueryLanguage.Statements;
using InterfaceBooster.Common.Interfaces.SyneryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;

namespace InterfaceBooster.SyneryLanguage.Interpretation.BaseLanguage.Statements
{
    /// <summary>
    /// Interprets a table assignment. It uses the RequestStatementInterpreter to get the new table.
    /// </summary>
    public class TableStatementInterpreter : IInterpreter<SyneryParser.TableStatementContext>
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
        public void Run(SyneryParser.TableStatementContext context)
        {
            if (context.tableAssignment()!= null)
            {
                string destinationTableName = context.tableAssignment().InternalPathIdentifier().GetText();

                if (context.tableAssignment().tableInitializer() != null && String.IsNullOrEmpty(destinationTableName) == false)
                {
                    if (context.tableAssignment().tableInitializer().requestStatement() != null)
                    {
                        ITable newTable = Controller
                            .Interpret<SyneryParser.RequestStatementContext, ITable>(context.tableAssignment().tableInitializer().requestStatement());

                        Memory.Database.CreateOrUpdateTable(destinationTableName, newTable);

                        // free ressources
                        newTable = null;
                    }
                }
            }
        }

        #endregion
    }
}
