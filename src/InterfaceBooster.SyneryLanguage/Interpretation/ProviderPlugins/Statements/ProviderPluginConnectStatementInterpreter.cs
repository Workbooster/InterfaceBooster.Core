using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.SyneryLanguage.Interpretation.BaseLanguage.Expressions;
using InterfaceBooster.SyneryLanguage.Interpretation.General;
using InterfaceBooster.Common.Interfaces.ProviderPlugin.Control;
using InterfaceBooster.Common.Interfaces.SyneryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;

namespace InterfaceBooster.SyneryLanguage.Interpretation.ProviderPlugins.Statements
{
    /// <summary>
    /// Interprets a connect statement that instructs the Interface Booster to open a connection of a Provider Plugin.
    /// </summary>
    public class ProviderPluginConnectStatementInterpreter : IInterpreter<SyneryParser.ProviderPluginConnectStatementContext, ProviderPluginConnectTask>
    {
        #region PROPERTIES

        public ISyneryMemory Memory { get; set; }
        public IInterpretationController Controller { get; set; }

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Interprets a connect statement that instructs the Interface Booster to open a connection of a Provider Plugin.
        /// </summary>
        /// <param name="context"></param>
        public ProviderPluginConnectTask RunWithResult(SyneryParser.ProviderPluginConnectStatementContext context)
        {
            // get the instance reference name and the connection identifier from the Synery code
            // prepare the ConnectTask for the ProviderPluginManager

            ProviderPluginConnectTask connectTask = new ProviderPluginConnectTask();
            connectTask.SyneryConnectionPath = context.ExternalPathIdentifier().GetText();                          // e.g. \\csv\articleFile
            connectTask.ConnectionPath = IdentifierHelper.ParsePathIdentifier(connectTask.SyneryConnectionPath);    // e.g. new string[] { "csv", "articleFile" }
            connectTask.InstanceReferenceSyneryIdentifier = LiteralHelper.ParseStringLiteral(context.providerPluginConnectStatementProviderPluginInstance().StringLiteral());

            if (context.setCommand() != null)
            {
                // get all connection parameters from the synery code and append them to the ConnectCommand

                IDictionary<string[], IValue> listOfKeyValues = Controller.Interpret<SyneryParser.SetCommandContext, IDictionary<string[], IValue>>(context.setCommand());

                foreach (var keyValue in listOfKeyValues)
                {
                    connectTask.Parameters.Add(keyValue.Key, keyValue.Value.Value);
                }
            }

            return connectTask;
        }

        #endregion
    }
}
