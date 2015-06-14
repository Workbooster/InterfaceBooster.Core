using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.ProviderPlugin.Control;
using InterfaceBooster.Common.Interfaces.SyneryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;

namespace InterfaceBooster.SyneryLanguage.Interpretation.ProviderPlugins.Commands
{
    class GetCommandInterpreter : IInterpreter<SyneryParser.GetCommandContext, IList<ProviderPluginGetValue>>
    {
        #region PROPERTIES

        public ISyneryMemory Memory { get; set; }
        public IInterpretationController Controller { get; set; }

        #endregion

        #region PUBLIC METHODS

        public IList<ProviderPluginGetValue> RunWithResult(SyneryParser.GetCommandContext context)
        {
            List<ProviderPluginGetValue> listOfValues = new List<ProviderPluginGetValue>();

            if (context.asAssignmentList() != null)
            {
                foreach (var item in context.asAssignmentList().asAssignmentListItem())
                {
                    ProviderPluginGetValue getValue = new ProviderPluginGetValue();

                    // extract the name of the synery variable and the name of the external value provided by the provider plugin

                    getValue.SyneryVariableName = item.asAssignmentListItemInternalVariable().GetText();
                    getValue.ProviderPluginValueName = item.asAssignmentListItemExternalField().GetText();

                    listOfValues.Add(getValue);
                }
            }

            return listOfValues;
        }

        #endregion
    }
}
