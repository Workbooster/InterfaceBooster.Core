using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.SyneryLanguage.Common;
using InterfaceBooster.SyneryLanguage.Interpretation.General;
using InterfaceBooster.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.ErrorHandling;
using InterfaceBooster.Common.Interfaces.SyneryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.SyneryTypes;
using InterfaceBooster.Common.Interfaces.Utilities.SyneryLanguage;
using InterfaceBooster.SyneryLanguage.Model.SyneryTypes.SyneryRecords;

namespace InterfaceBooster.SyneryLanguage.Interpretation.BaseLanguage.Statements
{
    public class EmitStatementInterpreter : IInterpreter<SyneryParser.EmitStatementContext>
    {
        #region PROPERTIES

        public ISyneryMemory Memory { get; set; }

        public IInterpretationController Controller { get; set; }
         
        #endregion

        #region PUBLIC METHODS

        public void Run(SyneryParser.EmitStatementContext context)
        {
            // get the emitted value
            IValue emitValue = Controller.Interpret<SyneryParser.ExpressionContext, IValue>(context.expression());

            Controller.HandleSyneryEvent(context, emitValue);
        }

        #endregion
    }
}
