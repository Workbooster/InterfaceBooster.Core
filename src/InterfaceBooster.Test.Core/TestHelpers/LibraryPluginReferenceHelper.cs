using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Core.InterfaceDefinitions.Data;
using InterfaceBooster.Common.Interfaces.InterfaceDefinition.Data;

namespace InterfaceBooster.Core.TestHelpers
{
    public class LibraryPluginReferenceHelper
    {
        public static ILibraryPluginReference GetSimpleDummyReference(string syneryIdentifier)
        {
            ILibraryPluginReference simpleDummyReference = new LibraryPluginReference();
            simpleDummyReference.SyneryIdentifier = syneryIdentifier;
            simpleDummyReference.IdPlugin = new Guid("74A8005D-C9F3-455F-94FC-04846493AB7B");
            simpleDummyReference.PluginName = "ReferencePluginName";

            return simpleDummyReference;
        }
    }
}
