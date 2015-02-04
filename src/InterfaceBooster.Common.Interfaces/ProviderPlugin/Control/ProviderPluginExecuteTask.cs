using InterfaceBooster.Database.Interfaces.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Common.Interfaces.ProviderPlugin.Control
{
    public class ProviderPluginExecuteTask : ProviderPluginDataExchangeTask
    {
        #region PROPERTIES

        /// <summary>
        /// Gets or sets the parameters sent with the request to the provider plugin.
        /// Key = path and name of the parameter / Value = the parameter value
        /// </summary>
        public IDictionary<string[], object> Parameters { get; set; }

        /// <summary>
        /// Gets or sets a list of values requested by the interface developer to get them from the provider plugin .
        /// </summary>
        public IList<ProviderPluginGetValue> GetValues { get; set; }

        #endregion

        #region PUBLIC METHODS

        public ProviderPluginExecuteTask()
            : base(ProviderPluginTaskTypeEnum.Execute)
        {
            Parameters = new Dictionary<string[], object>();
            GetValues = new List<ProviderPluginGetValue>();
        }

        #endregion
    }
}
