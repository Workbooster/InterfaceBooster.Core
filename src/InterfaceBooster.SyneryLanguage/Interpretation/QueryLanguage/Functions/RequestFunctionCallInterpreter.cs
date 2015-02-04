using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.SyneryLanguage.Model.QueryLanguage;
using InterfaceBooster.Common.Interfaces.ErrorHandling;
using InterfaceBooster.Common.Interfaces.SyneryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.QueryLanguage;

namespace InterfaceBooster.SyneryLanguage.Interpretation.QueryLanguage.Functions
{
    public class RequestFunctionCallInterpreter : IInterpreter<SyneryParser.RequestFunctionCallContext, IExpressionValue, QueryMemory>
    {
        #region PROPERTIES

        public ISyneryMemory Memory { get; set; }
        public IInterpretationController Controller { get; set; }

        #endregion

        #region PUBLIC METHODS

        public IExpressionValue Run(SyneryParser.RequestFunctionCallContext context, QueryMemory queryMemory)
        {
            if (context.requestSyneryFunctionCall() != null)
            {
                return Controller.Interpret<SyneryParser.RequestSyneryFunctionCallContext, IExpressionValue, QueryMemory>(context.requestSyneryFunctionCall(), queryMemory);
            }
            else if (context.requestLibraryPluginFunctionCall() != null)
            {
                return Controller.Interpret<SyneryParser.RequestLibraryPluginFunctionCallContext, IExpressionValue, QueryMemory>(context.requestLibraryPluginFunctionCall(), queryMemory);
            }

            throw new SyneryInterpretationException(context, "Unknown function call.");
        }

        #endregion
    }
}
