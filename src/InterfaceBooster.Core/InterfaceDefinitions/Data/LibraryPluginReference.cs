using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.InterfaceDefinition.Data;

namespace InterfaceBooster.Core.InterfaceDefinitions.Data
{
    public class LibraryPluginReference : ILibraryPluginReference
    {
        public string SyneryIdentifier { get; set; }
        public Guid IdPlugin { get; set; }
        public string PluginName { get; set; }
    }
}
