using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.ErrorHandling;
using InterfaceBooster.Common.Interfaces.ProviderPlugin.Control;
using InterfaceBooster.Common.Interfaces.SyneryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;

namespace InterfaceBooster.SyneryLanguage.Interpretation.ProviderPlugins.Statements
{
    public class ProviderPluginStatementInterpreter : IInterpreter<SyneryParser.ProviderPluginStatementContext>
    {
        #region PROPERTIES

        public ISyneryMemory Memory { get; set; }
        public IInterpretationController Controller { get; set; }

        #endregion

        #region PUBLIC METHODS

        public void Run(SyneryParser.ProviderPluginStatementContext context)
        {
            ProviderPluginTask task;

            if (context.providerPluginConnectStatement() != null)
            {
                // interpret the statement and get the task for the ProviderPluginManager

                task = Controller.Interpret<SyneryParser.ProviderPluginConnectStatementContext, ProviderPluginConnectTask>(
                        context.providerPluginConnectStatement());
            }
            else if (context.providerPluginDataExchangeStatement() != null)
            {
                // interpret the statement and get the task for the ProviderPluginManager

                ProviderPluginDataExchangeTask dataExchangeTask = Controller.
                    Interpret<SyneryParser.ProviderPluginDataExchangeStatementContext, ProviderPluginDataExchangeTask>(
                        context.providerPluginDataExchangeStatement());

                task = dataExchangeTask;
            }
            else
            {
                throw new SyneryInterpretationException(context, "Unknown statement in ProviderPluginStatementInterpreter. No interpreter found for the given context.");
            }

            // append the current state of the SyneryMemory to the task

            task.Memory = Memory;

            // execute the task

            Memory.ProviderPluginManager.RunTask(task);
        }

        #endregion
    }
}
