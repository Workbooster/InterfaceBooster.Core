using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.ProviderPlugin.Interfaces;

namespace InterfaceBooster.Common.Interfaces.ErrorHandling
{
    [Serializable]
    public class ProviderPluginManagerException : InterfaceBoosterCoreException
    {
        public IProviderPluginManager ProviderPluginManager { get; set; }

        public ProviderPluginManagerException(IProviderPluginManager instance, string message)
            : base(message)
        {
            ProviderPluginManager = instance;
        }

        public ProviderPluginManagerException(IProviderPluginManager instance, string message, Exception inner)
            : base(message, inner)
        {
            ProviderPluginManager = instance;
        }
    }
}
