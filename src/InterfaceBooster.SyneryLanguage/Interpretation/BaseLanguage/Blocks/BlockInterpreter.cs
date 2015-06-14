using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.SyneryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;

namespace InterfaceBooster.SyneryLanguage.Interpretation.BaseLanguage.Blocks
{
    public class BlockInterpreter : IInterpreter<SyneryParser.BlockContext, INestedScope, INestedScope>, IInterpreter<SyneryParser.BlockContext>
    {
        #region PROPERTIES
        
        public ISyneryMemory Memory { get; set; }

        public IInterpretationController Controller { get; set; }

        #endregion

        #region PUBLIC METHODS

        public void Run(SyneryParser.BlockContext context)
        {
            // initialize a new scope for this block and push it on the scope stack

            INestedScope blockScope = new BlockScope(Memory.CurrentScope);

            RunWithResult(context, blockScope);
        }

        public INestedScope RunWithResult(SyneryParser.BlockContext context, INestedScope blockScope)
        {
            Memory.PushScope(blockScope);

            // loop threw all statements inside of the scope and check whether the IsReturnCalled flag is changed

            foreach (var blockUnit in context.blockUnit())
            {
                Controller.Interpret<SyneryParser.BlockUnitContext>(blockUnit);

                // stop execution if return was called
                if (blockScope.IsTerminated == true)
                    break;
            }

            // remove the scope of the current block from the scope stack

            Memory.PopScope();

            // just return the original scope

            return blockScope;
        }

        #endregion
    }
}
