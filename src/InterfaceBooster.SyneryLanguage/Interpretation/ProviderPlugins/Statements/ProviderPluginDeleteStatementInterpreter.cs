﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.SyneryLanguage.Interpretation.General;
using InterfaceBooster.Common.Interfaces.ProviderPlugin.Control;
using InterfaceBooster.Common.Interfaces.SyneryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.ProviderPlugin.Control.Filter;

namespace InterfaceBooster.SyneryLanguage.Interpretation.ProviderPlugins.Statements
{
    public class ProviderPluginDeleteStatementInterpreter : IInterpreter<SyneryParser.ProviderPluginDeleteStatementContext, ProviderPluginDeleteTask>
    {
        #region PROPERTIES

        public ISyneryMemory Memory { get; set; }
        public IInterpretationController Controller { get; set; }

        #endregion

        #region PUBLIC METHODS

        public ProviderPluginDeleteTask RunWithResult(SyneryParser.ProviderPluginDeleteStatementContext context)
        {
            ProviderPluginDeleteTask deleteTask = new ProviderPluginDeleteTask();

            // get the full path consisting of the connection identifier, the provider plugin endpoint path and the endpoint name. 

            deleteTask.FullSyneryPath = context.providerPluginResourceIdentifier().GetText();
            deleteTask.FullPath = IdentifierHelper.ParsePathIdentifier(deleteTask.FullSyneryPath);

            if (context.fromCommand() != null)
            {
                deleteTask.SourceTableName = Controller.Interpret<SyneryParser.FromCommandContext, string>(context.fromCommand());
            }

            if (context.setCommand() != null)
            {
                IDictionary<string[], IValue> setParameters = Controller.Interpret<SyneryParser.SetCommandContext, IDictionary<string[], IValue>>(context.setCommand());

                foreach (var param in setParameters)
                {
                    // add the pramater with the key and the value (not IValue !)
                    deleteTask.Parameters.Add(param.Key, param.Value.Value);
                }
            }

            if (context.filterCommand() != null)
            {
                deleteTask.Filter = Controller.Interpret<SyneryParser.FilterCommandContext, IFilter>(context.filterCommand());
            }

            return deleteTask;
        }

        #endregion
    }
}
