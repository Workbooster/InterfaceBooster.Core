using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace InterfaceBooster.Common.Interfaces.InterfaceDefinition.Data
{
    [Serializable]
    [XmlRoot("LibraryPlugin")]
    public class LibraryPluginReference
    {
        [XmlAttribute("syneryIdentifier")]
        public string SyneryIdentifier { get; set; }

        [XmlAttribute("idPlugin")]
        public Guid IdPlugin { get; set; }

        [XmlAttribute("pluginName")]
        public string PluginName { get; set; }
    }
}
