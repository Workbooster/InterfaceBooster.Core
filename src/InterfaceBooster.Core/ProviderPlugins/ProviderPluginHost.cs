using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.ProviderPluginApi;
using System.Globalization;

namespace InterfaceBooster.Core.ProviderPlugins
{
    /// <summary>
    /// This connector can be used by the plugin to communicate with the Interface Booster host application.
    /// </summary>
    public class ProviderPluginHost : IHost
    {
        public CultureInfo FrontendCulture { get; set; }
        public string CurrentInterfaceDefinitionDirectoryPath { get; set; }
        public string ProviderPluginDiretcoryPath { get; set; }
        public string RuntimeDirectoryPath { get; set; }
    }
}
