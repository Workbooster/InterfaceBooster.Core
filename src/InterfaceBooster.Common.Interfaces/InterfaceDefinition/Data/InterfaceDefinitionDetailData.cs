using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace InterfaceBooster.Common.Interfaces.InterfaceDefinition.Data
{
    /// <summary>
    /// Container for the detail information of an Import Definition.
    /// </summary>
    [Serializable]
    [XmlRoot("Details")]
    public class InterfaceDefinitionDetailData
    {
        [XmlElement]
        public string Name { get; set; }

        [XmlElement]
        public string Description { get; set; }

        [XmlElement]
        public string Author { get; set; }

        [XmlElement]
        public DateTime DateOfCreation { get; set; }

        [XmlElement]
        public DateTime DateOfLastChange { get; set; }

        [XmlIgnore]
        public Version Version { get; set; }

        [XmlIgnore]
        public Version RequiredRuntimeVersion { get; set; }

        /// <summary>
        /// Only used for XML Serialization of Version property
        /// </summary>
        [XmlElement("Version")]
        public string VersionAsText
        {
            get { return Version.ToString(); }
            set { Version = new Version(value); }
        }

        /// <summary>
        /// Only used for XML Serialization of RequiredRuntimeVersion property
        /// </summary>
        [XmlElement("RequiredRuntimeVersion")]
        public string RequiredRuntimeVersionAsText
        {
            get { return RequiredRuntimeVersion.ToString(); }
            set { RequiredRuntimeVersion = new Version(value); }
        }
    }
}
