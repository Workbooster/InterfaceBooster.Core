using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.SyneryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;

namespace InterfaceBooster.SyneryLanguage.Interpretation.BaseLanguage.Functions
{
    public class FunctionCallInterpreter : IInterpreter<SyneryParser.FunctionCallContext, IValue>
    {
        public ISyneryMemory Memory { get; set; }

        public IInterpretationController Controller { get; set; }

        public IValue Run(SyneryParser.FunctionCallContext context)
        {
            if (context.syneryFunctionCall() != null)
            {
                return Controller.Interpret<SyneryParser.SyneryFunctionCallContext, IValue>(context.syneryFunctionCall());
            }
            else if (context.libraryPluginFunctionCall() != null)
            {
                return Controller.Interpret<SyneryParser.LibraryPluginFunctionCallContext, IValue>(context.libraryPluginFunctionCall());
            }

            throw new NotImplementedException("Function call type not implemented");
        }
    }
}
