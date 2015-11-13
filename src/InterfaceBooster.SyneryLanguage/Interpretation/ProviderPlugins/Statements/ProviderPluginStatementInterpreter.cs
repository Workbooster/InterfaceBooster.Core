using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.ErrorHandling;
using InterfaceBooster.Common.Interfaces.ProviderPlugin.Control;
using InterfaceBooster.Common.Interfaces.SyneryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;
using InterfaceBooster.SyneryLanguage.Model.SyneryTypes;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.SyneryTypes;
using InterfaceBooster.SyneryLanguage.Common;
using InterfaceBooster.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.Utilities.SyneryLanguage;
using InterfaceBooster.SyneryLanguage.Model.SyneryTypes.SyneryRecords;
using InterfaceBooster.Common.Tools.Data.ExceptionHandling;

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

            try
            {
                // execute the task

                Memory.ProviderPluginManager.RunTask(task);
            }
            catch (Exception ex)
            {
                // handle exception according to the task type

                ProviderPluginConnectTask connectTask = task as ProviderPluginConnectTask;
                ProviderPluginDataExchangeTask dataTask = task as ProviderPluginDataExchangeTask;

                // create a Synery exception that can be caught by the Synery developer

                if (connectTask != null)
                {
                    ProviderPluginConnectionExceptionRecord syneryException = new ProviderPluginConnectionExceptionRecord();
                    syneryException.Message = ExceptionHelper.GetNestedExceptionMessages(ex);
                    syneryException.ConnectionPath = connectTask.SyneryConnectionPath;
                    syneryException.PluginInstanceReferenceIdentifier = connectTask.InstanceReferenceSyneryIdentifier;

                    Controller.HandleSyneryEvent(context, syneryException.GetAsSyneryValue());
                }
                else if (dataTask != null)
                {
                    ProviderPluginDataExchangeExceptionRecord syneryException = new ProviderPluginDataExchangeExceptionRecord();
                    syneryException.Message = ExceptionHelper.GetNestedExceptionMessages(ex);
                    syneryException.DataCommandType = dataTask.Type.ToString().ToUpper();
                    syneryException.FullPath = dataTask.FullSyneryPath;

                    Controller.HandleSyneryEvent(context, syneryException.GetAsSyneryValue());
                }
            }
        }

        #endregion
    }
}
