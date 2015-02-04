using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Common.Interfaces.LibraryPlugin.Information.XmlData
{
    public interface ILibraryPluginAssemblyData
    {
        string Path { get; set; }
        Version RequiredInterfaceVersion { get; set; }
    }
}
