using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.SyneryLanguage.Interpretation.General;
using InterfaceBooster.Common.Interfaces.ProviderPlugin.Control;
using InterfaceBooster.Common.Interfaces.SyneryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;

namespace InterfaceBooster.SyneryLanguage.Interpretation.ProviderPlugins.Statements
{
    public class ProviderPluginUpdateStatementInterpreter : IInterpreter<SyneryParser.ProviderPluginUpdateStatementContext, ProviderPluginUpdateTask>
    {
        #region PROPERTIES

        public ISyneryMemory Memory { get; set; }
        public IInterpretationController Controller { get; set; }

        #endregion

        #region PUBLIC METHODS

        public ProviderPluginUpdateTask RunWithResult(SyneryParser.ProviderPluginUpdateStatementContext context)
        {
            ProviderPluginUpdateTask updateTask = new ProviderPluginUpdateTask();

            // get the full path consisting of the connection identifier, the provider plugin endpoint path and the endpoint name. 

            updateTask.FullSyneryPath = context.providerPluginResourceIdentifier().GetText();
            updateTask.FullPath = IdentifierHelper.ParsePathIdentifier(updateTask.FullSyneryPath);

            if (context.fromCommand() != null)
            {
                updateTask.SourceTableName = Controller.Interpret<SyneryParser.FromCommandContext, string>(context.fromCommand());
            }

            if (context.setCommand() != null)
            {
                IDictionary<string[], IValue> setParameters = Controller.Interpret<SyneryParser.SetCommandContext, IDictionary<string[], IValue>>(context.setCommand());

                foreach (var param in setParameters)
                {
                    // add the pramater with the key and the value (not IValue !)
                    updateTask.Parameters.Add(param.Key, param.Value.Value);
                }
            }

            // TODO: Interpret filters

            return updateTask;
        }

        #endregion
    }
}
