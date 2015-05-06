using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Common.Interfaces.InterfaceDefinition.Data
{
    public class LibraryPluginReference
    {
        public string SyneryIdentifier { get; set; }
        public Guid IdPlugin { get; set; }
        public string PluginName { get; set; }
    }
}
