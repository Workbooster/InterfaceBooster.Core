using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Common.Interfaces.ErrorHandling
{
    /// <summary>
    /// Is thrown when there is a problem loading data from a XML file.
    /// The property "FileType" contains the name of the type of XML file (e.g. "Interface Definition")
    /// </summary>
    [Serializable]
    public class XmlLoadingException : InterfaceBoosterCoreException
    {
        /// <summary>
        /// Gets or sets the name of the name of the XML type (e.g. "Interface Definition")
        /// </summary>
        public string XmlFileType { get; set; }
        public string XmlFilePath { get; set; }

        public XmlLoadingException(string xmlFileType, string xmlFilePath, string message)
            : base(message)
        {
            XmlFileType = xmlFileType;
            XmlFilePath = xmlFilePath;
        }

        public XmlLoadingException(string xmlFileType, string xmlFilePath, string message, Exception inner)
            : base(message, inner)
        {
            XmlFileType = xmlFileType;
            XmlFilePath = xmlFilePath;
        }
    }
}
