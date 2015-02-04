using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.ProviderPlugin.Interfaces.Information.Data;

namespace InterfaceBooster.Core.ProviderPlugins.Information.Data
{
    /// <summary>
    /// Container for the information of a Provider Plugin assembly (.dll file).
    /// A Provider Plugin can contain multiple assemblies. An assembly can have many instances.
    /// </summary>
    public class ProviderPluginAssemblyData : IProviderPluginAssemblyData
    {
        public string Path { get; set; }
        public Version RequiredInterfaceVersion { get; set; }
        public IList<IProviderPluginInstanceData> Instances { get; set; }
    }
}
