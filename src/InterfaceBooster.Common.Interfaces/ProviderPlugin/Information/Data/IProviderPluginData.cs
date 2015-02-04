using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Common.Interfaces.ProviderPlugin.Interfaces.Information.Data
{
    /// <summary>
    /// Container for some information about a Provider Plugin.
    /// </summary>
    public interface IProviderPluginData
    {
        Guid Id { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        IList<IProviderPluginAssemblyData> Assemblies { get; set; }
        string PluginXmlFilePath { get; set; }
        string PluginDirectoryPath { get; set; }
    }
}
