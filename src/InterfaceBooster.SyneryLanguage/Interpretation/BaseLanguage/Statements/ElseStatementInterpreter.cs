using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.ErrorHandling;
using InterfaceBooster.Common.Interfaces.SyneryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;

namespace InterfaceBooster.SyneryLanguage.Interpretation.BaseLanguage.Statements
{
    public class ElseStatementInterpreter : IInterpreter<SyneryParser.ElseStatementContext, bool>
    {
        #region PROPERTIES

        public ISyneryMemory Memory { get; set; }
        public IInterpretationController Controller { get; set; }

        #endregion

        #region PUBLIC METHODS

        public bool RunWithResult(SyneryParser.ElseStatementContext context)
        {
            bool execute = true;
            IValue conditionValue;

            if (context.expression() != null)
            {
                // it's an else-if-statement
                // check whether the conditional expression returns a TRUE value

                conditionValue = Controller.Interpret<SyneryParser.ExpressionContext, IValue>(context.expression());


                if (conditionValue.Type != typeof(bool))
                {
                    throw new SyneryInterpretationException(context, string.Format("The conditional part of the else-if statement is not a boolean value: {0}", conditionValue.Value));
                }

                if ((bool)conditionValue.Value != true)
                {
                    // FALSE
                    // don't execute the else-if-statement block

                    execute = false;
                }

                // TRUE
                // continue with execution
            }

            if (execute == true)
            {
                Controller.Interpret<SyneryParser.BlockContext>(context.block());   
            }

            return execute;
        }

        #endregion
    }
}
