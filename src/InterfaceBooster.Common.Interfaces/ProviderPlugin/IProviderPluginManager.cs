using InterfaceBooster.ProviderPluginApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.InterfaceDefinition.Data;
using InterfaceBooster.Common.Interfaces.ProviderPlugin.Control;
using InterfaceBooster.Common.Interfaces.ProviderPlugin.Interfaces.Information.Data;

namespace InterfaceBooster.Common.Interfaces.ProviderPlugin.Interfaces
{
    /// <summary>
    /// handels the loading of provider plugins and the communication with them
    /// </summary>
    public interface IProviderPluginManager
    {
        string PluginMainDirectoryPath { get; }
        IReadOnlyDictionary<IProviderPluginData, IProviderPlugin> AvailablePlugins { get; }
        IReadOnlyDictionary<IProviderPluginInstanceReference, IProviderPluginInstance> ProviderPluginInstances { get; }
        IReadOnlyDictionary<string[], IProviderConnection> Connections { get; }
        void Activate(IProviderPluginInstanceReference reference);
        void Activate(IList<IProviderPluginInstanceReference> references);
        void RunTask(ProviderPluginTask task);
    }
}
