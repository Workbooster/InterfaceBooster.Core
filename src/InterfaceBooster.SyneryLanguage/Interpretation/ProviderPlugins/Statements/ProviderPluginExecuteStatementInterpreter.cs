using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.SyneryLanguage.Interpretation.General;
using InterfaceBooster.Common.Interfaces.ErrorHandling;
using InterfaceBooster.Common.Interfaces.ProviderPlugin.Control;
using InterfaceBooster.Common.Interfaces.SyneryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;

namespace InterfaceBooster.SyneryLanguage.Interpretation.ProviderPlugins.Statements
{
    public class ProviderPluginExecuteStatementInterpreter : IInterpreter<SyneryParser.ProviderPluginExecuteStatementContext, ProviderPluginExecuteTask>
    {
        #region PROPERTIES

        public ISyneryMemory Memory { get; set; }
        public IInterpretationController Controller { get; set; }

        #endregion

        #region PUBLIC METHODS

        public ProviderPluginExecuteTask RunWithResult(SyneryParser.ProviderPluginExecuteStatementContext context)
        {
            ProviderPluginExecuteTask executeTask = new ProviderPluginExecuteTask();

            // get the full path consisting of the connection identifier, the provider plugin endpoint path and the endpoint name. 

            executeTask.FullSyneryPath = context.providerPluginResourceIdentifier().GetText();
            executeTask.FullPath = IdentifierHelper.ParsePathIdentifier(executeTask.FullSyneryPath);

            if (context.setCommand() != null)
            {
                IDictionary<string[], IValue> setParameters = Controller.Interpret<SyneryParser.SetCommandContext, IDictionary<string[], IValue>>(context.setCommand());

                foreach (var param in setParameters)
                {
                    // add the pramater with the key and the value (not IValue !)
                    executeTask.Parameters.Add(param.Key, param.Value.Value);
                }
            }

            if (context.getCommand() != null)
            {
                executeTask.GetValues = Controller.Interpret<SyneryParser.GetCommandContext, IList<ProviderPluginGetValue>>(context.getCommand());

                // check whether variables with the specified names exists
                // this prevents errors after sending the request to the provider plugin

                foreach (var getValue in executeTask.GetValues)
                {
                    if (!Memory.CurrentScope.DoesVariableExists(getValue.SyneryVariableName))
                    {
                        throw new SyneryInterpretationException(context, string.Format(
                            "Local variable with name '{0}' was not found.", getValue.SyneryVariableName));
                    }
                }
            }

            executeTask.OnFinishedSuccessfully += ExecuteTask_OnFinishedSuccessfully;

            return executeTask;
        }

        private void ExecuteTask_OnFinishedSuccessfully(ProviderPluginTask task)
        {
            if (task is ProviderPluginExecuteTask)
            {
                ProviderPluginExecuteTask executeTask = (ProviderPluginExecuteTask)task;

                // read the get values back into the synery memory

                if (executeTask.GetValues != null)
                {
                    foreach (var getValue in executeTask.GetValues)
                    {
                        Memory.CurrentScope.AssignVariable(getValue.SyneryVariableName, getValue.Value);
                    }
                }
            }
        }

        #endregion
    }
}
