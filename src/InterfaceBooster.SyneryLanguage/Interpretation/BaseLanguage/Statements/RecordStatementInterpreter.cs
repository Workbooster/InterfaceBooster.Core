using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.SyneryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;

namespace InterfaceBooster.SyneryLanguage.Interpretation.BaseLanguage.Statements
{
    /*
    public class RecordStatementInterpreter : IInterpreter<SyneryParser.RecordStatementContext>, IInterpreter<SyneryParser.RecordDeclartionContext>, IInterpreter<SyneryParser.RecordAssignmentContext>
    {
        #region PROPERTIES

        public ISyneryMemory Memory { get; set; }
        public IInterpretationController Controller { get; set; }

        #endregion

        #region PUBLIC METHODS

        public void Run(SyneryParser.RecordStatementContext context)
        {
            if (context.recordDeclartion() != null)
            {
                Run(context.recordDeclartion());
            }

            if (context.recordAssignment() != null)
            {
                Run(context.recordAssignment());
            }
        }

        public void Run(SyneryParser.RecordDeclartionContext context)
        {
            throw new NotImplementedException();
        }

        public void Run(SyneryParser.RecordAssignmentContext context)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
    */
}
