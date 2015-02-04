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
    public interface IProviderPluginInstanceReference
    {
        /// <summary>
        /// the identifier that is used in synery to reference this plugin
        /// </summary>
        string SyneryIdentifier { get; set; }
        Guid IdPlugin { get; set; }
        string PluginName { get; set; }
        Guid IdPluginInstance { get; set; }
        string PluginInstanceName { get; set; }
    }
}
