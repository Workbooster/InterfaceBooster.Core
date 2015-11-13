using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;

namespace InterfaceBooster.Common.Interfaces.SyneryLanguage
{
    public delegate void StateChangedEventHandler(InterpreationStateEnum oldState, InterpreationStateEnum newState);
    public delegate void EnterContextEventHandler(Antlr4.Runtime.ParserRuleContext context);
    public delegate void ExitContextEventHandler(Antlr4.Runtime.ParserRuleContext context);

    public interface IInterpretationController
    {
        #region EVENTS

        event StateChangedEventHandler OnStateChanged;
        event EnterContextEventHandler OnEnterContext;
        event ExitContextEventHandler OnExitContext;

        #endregion

        #region PROPERTIES

        IInterpreterFactory Factory { get; set; }
        ISyneryMemory Memory { get; set; }
        InterpreationStateEnum State { get; }
        Antlr4.Runtime.ParserRuleContext CurrentContext { get; }

        #endregion

        #region PUBLIC METHODS

        void Interpret<contextT>(contextT context) where contextT : Antlr4.Runtime.ParserRuleContext;

        resultT Interpret<contextT, resultT>(contextT context) where contextT : Antlr4.Runtime.ParserRuleContext;

        resultT Interpret<contextT, resultT, paramT>(contextT context, paramT parameter) where contextT : Antlr4.Runtime.ParserRuleContext;

        void HandleSyneryEvent(Antlr4.Runtime.ParserRuleContext context, IValue eventValue);

        #endregion
    }
}
