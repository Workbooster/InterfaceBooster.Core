using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Common.Interfaces.ProviderPlugin.Control
{
    public enum ProviderPluginTaskStateEnum
    {
        /// <summary>
        /// The task is new and the execution hasn't been started yet.
        /// </summary>
        New = 1,
        
        /// <summary>
        /// Started executing the task.
        /// </summary>
        Started = 2,

        /// <summary>
        /// The request to the provider plugin has been sent.
        /// </summary>
        RequestSent = 3,

        /// <summary>
        /// The response from the provider plugin is available and will now be validated.
        /// </summary>
        ResponseReceived = 4,

        /// <summary>
        /// The task finished successfully. All response data are now available.
        /// </summary>
        FinishedSuccessfully = 5,

        /// <summary>
        /// The task finished with errors. An error message is now available.
        /// </summary>
        FinishedWithError = 6,
    }
}
