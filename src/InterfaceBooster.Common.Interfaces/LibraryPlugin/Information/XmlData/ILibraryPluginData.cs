using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.LibraryPlugin.Information.ReflectionData;

namespace InterfaceBooster.Common.Interfaces.LibraryPlugin.Information.XmlData
{
    /// <summary>
    /// Container for some information about a Library Plugin.
    /// </summary>
    public interface ILibraryPluginData
    {
        Guid Id { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        IList<ILibraryPluginAssemblyData> Assemblies { get; set; }
        IStaticExtensionContainer StaticExtensionContainer { get; set; }
        string PluginXmlFilePath { get; set; }
        string PluginDirectoryPath { get; set; }
    }
}
