using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.LibraryPlugin.Information.ReflectionData;
using InterfaceBooster.Common.Interfaces.LibraryPlugin.Information.XmlData;

namespace InterfaceBooster.Core.LibraryPlugins.Information.XmlData
{
    public class LibraryPluginData : ILibraryPluginData
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public IList<ILibraryPluginAssemblyData> Assemblies { get; set; }
        public IStaticExtensionContainer StaticExtensionContainer { get; set; }
        public string PluginXmlFilePath { get; set; }
        public string PluginDirectoryPath { get; set; }
    }
}
