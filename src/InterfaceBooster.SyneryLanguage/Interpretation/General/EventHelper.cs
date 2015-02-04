using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.ErrorHandling;
using InterfaceBooster.Common.Interfaces.SyneryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;

namespace InterfaceBooster.SyneryLanguage.Interpretation.General
{
    public static class EventHelper
    {
        #region PUBLIC METHODS

        public static void InterpretHandleBlock(IInterpretationController controller, IHandleBlockData data, IValue eventRecord)
        {
            SyneryParser.BlockContext blockContext = data.Context as SyneryParser.BlockContext;

            if (blockContext != null)
            {
                // create a new block scope and set the parent scope of the OBSERVE-block as parent
                INestedScope blockScope = new BlockScope(data.ParentScope);

                // add the event record as local variable
                blockScope.DeclareVariable(data.ParameterName, eventRecord.Type);
                blockScope.AssignVariable(data.ParameterName, eventRecord.Value);

                // start interpreting the given HANDLE-block
                controller.Interpret<SyneryParser.BlockContext, INestedScope, INestedScope>(blockContext, blockScope);
            }
            else
            {
                throw new SyneryException("Can not interpret handle block because the given context is not a BlockContext");
            }
        }

        #endregion
    }
}
