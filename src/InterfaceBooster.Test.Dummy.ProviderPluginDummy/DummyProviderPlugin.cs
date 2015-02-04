using InterfaceBooster.ProviderPluginApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using V1 = InterfaceBooster.Test.Dummy.ProviderPluginDummy.V1;

namespace InterfaceBooster.Test.Dummy.ProviderPluginDummy
{
    public class DummyProviderPlugin : IProviderPlugin
    {
        public IProviderPluginInstance CreateProviderPluginInstance(Guid id, IHost host)
        {
            if (id.ToString() == "58e6a1b5-9eb5-45ab-8c9b-8b267e2d09e8")
            {
                return new V1.DummyProviderPluginInstance(host);
            }

            return null;
        }
    }
}
