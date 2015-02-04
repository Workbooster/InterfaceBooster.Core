using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.LibraryPlugin.Information.XmlData;

namespace InterfaceBooster.Core.LibraryPlugins.Information.XmlData
{
    public class LibraryPluginAssemblyData : ILibraryPluginAssemblyData
    {
        public string Path { get; set; }
        public Version RequiredInterfaceVersion { get; set; }
    }
}
