using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Common.Interfaces.InterfaceDefinition.Data
{
    public interface ILibraryPluginReference
    {
        string SyneryIdentifier { get; set; }
        Guid IdPlugin { get; set; }
        string PluginName { get; set; }
    }
}
