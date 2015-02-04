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
    public class IfStatementInterpreter : IInterpreter<SyneryParser.IfStatementContext>
    {
        #region PROPERTIES

        public ISyneryMemory Memory { get; set; }
        public IInterpretationController Controller { get; set; }

        #endregion

        #region PUBLIC METHODS

        public void Run(SyneryParser.IfStatementContext context)
        {
            IValue conditionValue = Controller.Interpret<SyneryParser.ExpressionContext, IValue>(context.expression());

            if (conditionValue.Type != typeof(bool))
            {
                throw new SyneryInterpretationException(context, string.Format("The conditional part of the if statement is not a boolean value: {0}", conditionValue.Value));
            }

            if ((bool)conditionValue.Value == true)
            {
                // TRUE

                Controller.Interpret<SyneryParser.BlockContext>(context.block());
            }
            else
            {
                // FALSE

                if (context.elseStatement() != null)
                {
                    // ELSE or ELSE IF statement(s)

                    foreach (var elseStatement in context.elseStatement())
                    {
                        bool isConditionTrue = Controller.Interpret<SyneryParser.ElseStatementContext, bool>(elseStatement);

                        if (isConditionTrue)
                        {
                            // break execution if an else or else-if statement was executed
                            break;   
                        }
                    }
                }

            }
        }

        #endregion
    }
}
