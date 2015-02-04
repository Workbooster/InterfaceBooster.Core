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
    public class ProviderPluginDataExchangeStatementInterpreter : IInterpreter<SyneryParser.ProviderPluginDataExchangeStatementContext, ProviderPluginDataExchangeTask>
    {
        #region PROPERTIES

        public ISyneryMemory Memory { get; set; }
        public IInterpretationController Controller { get; set; }

        #endregion

        #region PUBLIC METHODS

        public ProviderPluginDataExchangeTask Run(SyneryParser.ProviderPluginDataExchangeStatementContext context)
        {
            ProviderPluginDataExchangeTask task;

            if (context.providerPluginReadStatement() != null)
            {
                task = Controller.Interpret<SyneryParser.ProviderPluginReadStatementContext, ProviderPluginReadTask>(context.providerPluginReadStatement());
                
                // interpret the nested provider plugin data exchange statements
                task.NestedTasks = InterpretNestedTasks(context.providerPluginReadStatement().providerPluginDataExchangeStatement());
            }
            else if (context.providerPluginCreateStatement() != null)
            {
                task = Controller.Interpret<SyneryParser.ProviderPluginCreateStatementContext, ProviderPluginCreateTask>(context.providerPluginCreateStatement());

                // interpret the nested provider plugin data exchange statements
                task.NestedTasks = InterpretNestedTasks(context.providerPluginCreateStatement().providerPluginDataExchangeStatement());
            }
            else if (context.providerPluginUpdateStatement() != null)
            {
                task = Controller.Interpret<SyneryParser.ProviderPluginUpdateStatementContext, ProviderPluginUpdateTask>(context.providerPluginUpdateStatement());

                // interpret the nested provider plugin data exchange statements
                task.NestedTasks = InterpretNestedTasks(context.providerPluginUpdateStatement().providerPluginDataExchangeStatement());
            }
            else if (context.providerPluginSaveStatement() != null)
            {
                task = Controller.Interpret<SyneryParser.ProviderPluginSaveStatementContext, ProviderPluginSaveTask>(context.providerPluginSaveStatement());

                // interpret the nested provider plugin data exchange statements
                task.NestedTasks = InterpretNestedTasks(context.providerPluginSaveStatement().providerPluginDataExchangeStatement());
            }
            else if (context.providerPluginDeleteStatement() != null)
            {
                task = Controller.Interpret<SyneryParser.ProviderPluginDeleteStatementContext, ProviderPluginDeleteTask>(context.providerPluginDeleteStatement());

                // interpret the nested provider plugin data exchange statements
                task.NestedTasks = InterpretNestedTasks(context.providerPluginDeleteStatement().providerPluginDataExchangeStatement());
            }
            else if (context.providerPluginExecuteStatement() != null)
            {
                task = Controller.Interpret<SyneryParser.ProviderPluginExecuteStatementContext, ProviderPluginExecuteTask>(context.providerPluginExecuteStatement());

                // interpret the nested provider plugin data exchange statements
                task.NestedTasks = InterpretNestedTasks(context.providerPluginExecuteStatement().providerPluginDataExchangeStatement());
            }
            else
            {
                throw new SyneryInterpretationException(context, "Unknown statement in ProviderPluginDataExchangeStatementInterpreter. No interpreter found for the given context.");
            }

            return task;
        }

        #endregion

        #region INTERNAL METHODS

        private IList<ProviderPluginTask> InterpretNestedTasks(IEnumerable<SyneryParser.ProviderPluginDataExchangeStatementContext> listOfNestedStatements)
        {
            List<ProviderPluginTask> listOfTasks = new List<ProviderPluginTask>();

            if (listOfNestedStatements != null)
            {
                foreach (var statement in listOfNestedStatements)
                {
                    ProviderPluginDataExchangeTask task = Run(statement);

                    // append the current state of the SyneryMemory to the task
                    task.Memory = Memory;

                    listOfTasks.Add(task);
                }
            }

            return listOfTasks;
        }

        #endregion
    }
}
