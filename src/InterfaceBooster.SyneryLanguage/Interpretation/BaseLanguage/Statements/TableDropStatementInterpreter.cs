using InterfaceBooster.Common.Interfaces.SyneryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.SyneryLanguage.Interpretation.BaseLanguage.Statements
{
    class TableDropStatementInterpreter : IInterpreter<SyneryParser.TableDropStatementContext>
    {
        #region PROPERTIES

        public ISyneryMemory Memory { get; set; }
        public IInterpretationController Controller { get; set; }

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Interprets a statement that removes a table.
        /// </summary>
        /// <param name="context"></param>
        public void Run(SyneryParser.TableDropStatementContext context)
        {
            string destinationTableName = context.InternalPathIdentifier().GetText();

            if (Memory.Database.IsTable(destinationTableName))
            {
                Memory.Database.DeleteTable(destinationTableName);
            }
        }

        #endregion
    }
}
