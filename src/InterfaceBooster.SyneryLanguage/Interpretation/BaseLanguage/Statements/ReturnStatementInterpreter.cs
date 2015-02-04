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
    public class ReturnStatementInterpreter : IInterpreter<SyneryParser.ReturnStatementContext>
    {
        #region PROPERTIES

        public ISyneryMemory Memory { get; set; }

        public IInterpretationController Controller { get; set; }

        #endregion

        #region PUBLIC METHODS

        public void Run(SyneryParser.ReturnStatementContext context)
        {
            IValue returnValue = null;

            // check for a correct scope

            if (!(Memory.CurrentScope is INestedScope))
            {
                string message = String.Format(
                    "Return statement is expected to be inside of a block scope. CurrentScope is of type '{0}'",
                    Memory.CurrentScope.GetType().Name);
                throw new SyneryInterpretationException(context, message);
            }

            if (context.expression() != null)
            {
                // evaluate expression for getting the value to return

                returnValue = Controller.Interpret<SyneryParser.ExpressionContext, IValue>(context.expression());
            }

            // broadcast the termination up to the function block

            IScope scope = Memory.CurrentScope;

            while (scope is INestedScope)
            {
                INestedScope nestedScope = (INestedScope)scope;

                nestedScope.IsTerminated = true;

                if (scope is IFunctionScope)
                {
                    // if its the scope of the function block break the loop and set the return value
                    ((IFunctionScope)scope).ReturnValue = returnValue;

                    break;
                }

                // get next scope from the scope tree
                scope = nestedScope.Parent;
            }
        }

        #endregion
    }
}
