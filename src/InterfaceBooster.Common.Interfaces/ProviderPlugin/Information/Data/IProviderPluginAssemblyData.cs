using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Common.Interfaces.ProviderPlugin.Interfaces.Information.Data
{
    /// <summary>
    /// Container for the information of a Provider Plugin assembly (.dll file).
    /// A Provider Plugin can contain multiple assemblies. An assembly can have many instances.
    /// </summary>
    public interface IProviderPluginAssemblyData
    {
        string Path { get; set; }
        Version RequiredInterfaceVersion { get; set; }
        IList<IProviderPluginInstanceData> Instances { get; set; }
    }
}
