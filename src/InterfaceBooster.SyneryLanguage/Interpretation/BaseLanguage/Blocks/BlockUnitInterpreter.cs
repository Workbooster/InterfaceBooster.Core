using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.ErrorHandling;
using InterfaceBooster.Common.Interfaces.SyneryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;

namespace InterfaceBooster.SyneryLanguage.Interpretation.BaseLanguage.Blocks
{
    public class BlockUnitInterpreter : IInterpreter<SyneryParser.BlockUnitContext>
    {
        #region PROPERTIES

        public ISyneryMemory Memory { get; set; }

        public IInterpretationController Controller { get; set; }

        #endregion

        #region PUBLIC METHODS

        public void Run(SyneryParser.BlockUnitContext context)
        {
            if (context.programUnit() != null)
            {
                Controller.Interpret<SyneryParser.ProgramUnitContext>(context.programUnit());
            }
            else if (context.returnStatement() != null)
            {
                Controller.Interpret<SyneryParser.ReturnStatementContext>(context.returnStatement());
            }
            else if (context.emitStatement() != null)
            {
                Controller.Interpret<SyneryParser.EmitStatementContext>(context.emitStatement());
            }
            else if (context.throwStatement() != null)
            {
                Controller.Interpret<SyneryParser.ThrowStatementContext>(context.throwStatement());
            }
            else
            {
                throw new SyneryInterpretationException(context, 
                    String.Format("Unknown statement in {0}. No interpreter found for the given context: '{1}'", this.GetType().Name, context.GetText()));
            }
        }

        #endregion
    }
}
