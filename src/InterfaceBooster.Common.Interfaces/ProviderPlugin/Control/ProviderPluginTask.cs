using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;

namespace InterfaceBooster.Common.Interfaces.ProviderPlugin.Control
{
    public delegate void ProviderPluginTaskStateChangedEventHandler(ProviderPluginTask task);

    public abstract class ProviderPluginTask
    {
        #region EVENTS

        public event ProviderPluginTaskStateChangedEventHandler OnFinishedSuccessfully;
        public event ProviderPluginTaskStateChangedEventHandler OnFinishedWithError;

        #endregion

        #region PROPERTIES

        public ISyneryMemory Memory { get; set; }
        public ProviderPluginTaskTypeEnum Type { get; private set; }
        public ProviderPluginTaskStateEnum State { get; private set; }
        public string ErrorMessage { get; set; }
        public IList<ProviderPluginTask> NestedTasks { get; set; }

        #endregion

        #region PUBLIC METHODS

        public ProviderPluginTask(ProviderPluginTaskTypeEnum type)
        {
            Type = type;
            State = ProviderPluginTaskStateEnum.New;
            NestedTasks = new List<ProviderPluginTask>();
        }

        /// <summary>
        /// Changes the state and informs all available event listeners.
        /// </summary>
        /// <param name="state">the new state</param>
        public void SetNewState(ProviderPluginTaskStateEnum state)
        {
            State = state;

            // inform the event listeners about the state change

            if (State == ProviderPluginTaskStateEnum.FinishedSuccessfully
                && OnFinishedSuccessfully != null)
            {
                OnFinishedSuccessfully(this);
            }

            if (State == ProviderPluginTaskStateEnum.FinishedWithError
                && OnFinishedWithError != null)
            {
                OnFinishedWithError(this);
            }
        }

        #endregion
    }
}
