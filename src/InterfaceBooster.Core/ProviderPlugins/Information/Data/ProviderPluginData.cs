using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.ProviderPlugin.Interfaces.Information.Data;

namespace InterfaceBooster.Core.ProviderPlugins.Information.Data
{
    /// <summary>
    /// Container for some information about a Provider Plugin.
    /// </summary>
    public class ProviderPluginData : IProviderPluginData
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public IList<IProviderPluginAssemblyData> Assemblies { get; set; }
        public string PluginXmlFilePath { get; set; }
        public string PluginDirectoryPath { get; set; }
    }
}
