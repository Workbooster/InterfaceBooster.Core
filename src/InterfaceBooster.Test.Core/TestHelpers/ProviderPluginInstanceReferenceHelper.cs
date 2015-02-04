using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Core.InterfaceDefinitions.Data;
using InterfaceBooster.Common.Interfaces.InterfaceDefinition.Data;

namespace InterfaceBooster.Core.TestHelpers
{
    public static class ProviderPluginInstanceReferenceHelper
    {
        public static IProviderPluginInstanceReference GetSimpleDummyReference(string syneryIdentifier)
        {
            IProviderPluginInstanceReference simpleDummyReference = new ProviderPluginInstanceReference();
            simpleDummyReference.SyneryIdentifier = syneryIdentifier;
            simpleDummyReference.IdPlugin = new Guid("485eccb4-3920-4dc3-9ed4-27f65e8b3c91");
            simpleDummyReference.PluginName = "ReferencePluginName";
            simpleDummyReference.IdPluginInstance = new Guid("58e6a1b5-9eb5-45ab-8c9b-8b267e2d09e8");
            simpleDummyReference.PluginInstanceName = "ReferencePluginInstanceName";

            return simpleDummyReference;
        }
    }
}
