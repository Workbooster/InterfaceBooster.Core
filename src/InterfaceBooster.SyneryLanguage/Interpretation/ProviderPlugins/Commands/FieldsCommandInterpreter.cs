using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.SyneryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;

namespace InterfaceBooster.SyneryLanguage.Interpretation.ProviderPlugins.Commands
{
    public class FieldsCommandInterpreter : IInterpreter<SyneryParser.FieldsCommandContext, IList<string>>
    {
        #region PROPERTIES

        public ISyneryMemory Memory { get; set; }
        public IInterpretationController Controller { get; set; }

        #endregion

        #region PUBLIC METHODS

        public IList<string> RunWithResult(SyneryParser.FieldsCommandContext context)
        {
            // get the list of selected fields

            return Controller.Interpret<SyneryParser.InternalIdentifierListContext, IList<string>>(context.internalIdentifierList());
        }

        #endregion
    }
}
