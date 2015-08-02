using InterfaceBooster.Common.Interfaces.ErrorHandling;
using InterfaceBooster.Common.Interfaces.SyneryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;
using InterfaceBooster.SyneryLanguage.Interpretation.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.SyneryLanguage.Interpretation.BaseLanguage.Statements
{
    public class LogStatementInterpreter : IInterpreter<SyneryParser.LogStatementContext>
    {
        #region PROPERTIES

        public ISyneryMemory Memory { get; set; }
        public IInterpretationController Controller { get; set; }

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Interprets a statement that broadcasts a log message.
        /// </summary>
        /// <param name="context"></param>
        public void Run(SyneryParser.LogStatementContext context)
        {
            // set default channel
            string logChannel = "debug";

            // evaluate expression for getting the log message
            IValue logValue = Controller.Interpret<SyneryParser.ExpressionContext, IValue>(context.expression());

            if (logValue.Type != typeof(string))
            {
                throw new SyneryInterpretationException(context, string.Format("The first value of a LOG statement must be a string value. Given: {0}", logValue.Value));
            }

            if (context.StringLiteral() != null)
            {
                logChannel = LiteralHelper.ParseStringLiteral(context.StringLiteral());
            }

            string logText = logValue.Value == null ? "" : logValue.Value.ToString();

            if (Memory.Broadcaster != null)
                Memory.Broadcaster.Broadcast(logChannel, logText);
        }

        #endregion
    }
}
