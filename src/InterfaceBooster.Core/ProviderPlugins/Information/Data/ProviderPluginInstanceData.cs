using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.ProviderPlugin.Interfaces.Information.Data;

namespace InterfaceBooster.Core.ProviderPlugins.Information.Data
{
    /// <summary>
    /// Container for the information about a Provider Plugin Instance.
    /// An assembly of a Provider Plugin can contain multiple Provider Plugin Instance objects.
    /// </summary>
    public class ProviderPluginInstanceData : IProviderPluginInstanceData
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
