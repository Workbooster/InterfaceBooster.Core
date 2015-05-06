using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Common.Interfaces.InterfaceDefinition.Data
{
    /// <summary>
    /// Container for a reference to a Provider Plugin Instance. This information is needed to load a Provider Plugin.
    /// </summary>
    public class ProviderPluginInstanceReference
    {
        /// <summary>
        /// the identifier that is used in synery to reference this plugin
        /// </summary>
        public string SyneryIdentifier { get; set; }
        public Guid IdPlugin { get; set; }
        public string PluginName { get; set; }
        public Guid IdPluginInstance { get; set; }
        public string PluginInstanceName { get; set; }
    }
}
