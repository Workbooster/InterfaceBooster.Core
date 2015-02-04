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
    public class ObserveBlockInterpreter : IInterpreter<SyneryParser.ObserveBlockContext>
    {
        #region PROPERTIES

        public ISyneryMemory Memory { get; set; }

        public IInterpretationController Controller { get; set; }

        #endregion

        #region PUBLIC METHODS

        public void Run(SyneryParser.ObserveBlockContext context)
        {
            List<IHandleBlockData> listOfHandleBlockData = new List<IHandleBlockData>();

            foreach (var handleBlockContext in context.handleBlock())
            {
                IHandleBlockData data = Controller.Interpret<SyneryParser.HandleBlockContext, IHandleBlockData>(handleBlockContext);
                listOfHandleBlockData.Add(data);
            }

            IObserveScope scope = new ObserveScope(Memory.CurrentScope, listOfHandleBlockData);

            Controller.Interpret<SyneryParser.BlockContext, INestedScope, INestedScope>(context.block(), scope);
        }

        #endregion
    }
}
