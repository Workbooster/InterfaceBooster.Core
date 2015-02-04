using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Common.Interfaces.ProviderPlugin.Control
{
    public class ProviderPluginConnectTask : ProviderPluginTask
    {
        #region PROPERTIES

        /// <summary>
        /// Gets or sets the Synery Identifier from the Interface Definition.
        /// </summary>
        public string InstanceReferenceSyneryIdentifier { get; set; }

        /// <summary>
        /// Gets or sets the parameters sent to the provider plugin for opening the conneciton.
        /// Key = parameter path / Value = parameter value
        /// </summary>
        public IDictionary<string[], object> Parameters { get; set; }

        /// <summary>
        /// Gets or sets the identifier used to identify the connection.
        /// </summary>
        public string[] ConnectionPath { get; set; }

        #endregion

        #region PUBLIC METHODS

        public ProviderPluginConnectTask()
            : base(ProviderPluginTaskTypeEnum.Connect)
        {
            Parameters = new Dictionary<string[], object>();
        }

        #endregion
    }
}
