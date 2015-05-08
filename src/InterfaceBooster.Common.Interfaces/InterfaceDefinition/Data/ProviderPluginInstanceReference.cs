using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace InterfaceBooster.Common.Interfaces.InterfaceDefinition.Data
{
    /// <summary>
    /// Container for a reference to a Provider Plugin Instance. This information is needed to load a Provider Plugin.
    /// </summary>
    [Serializable]
    [XmlRoot("ProviderPluginInstance")]
    public class ProviderPluginInstanceReference
    {
        /// <summary>
        /// the identifier that is used in synery to reference this plugin
        /// </summary>
        [XmlAttribute("syneryIdentifier")]
        public string SyneryIdentifier { get; set; }

        [XmlAttribute("idPlugin")]
        public Guid IdPlugin { get; set; }

        [XmlAttribute("pluginName")]
        public string PluginName { get; set; }

        [XmlAttribute("idPluginInstance")]
        public Guid IdPluginInstance { get; set; }

        [XmlAttribute("pluginInstanceName")]
        public string PluginInstanceName { get; set; }
    }
}
