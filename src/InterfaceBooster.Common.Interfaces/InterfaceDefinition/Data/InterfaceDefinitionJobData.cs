using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace InterfaceBooster.Common.Interfaces.InterfaceDefinition.Data
{
    /// <summary>
    /// Container for the data of an Import Definition Job. An Import Definition can contain multiple jobs.
    /// </summary>
    [Serializable]
    [XmlRoot("Job")]
    public class InterfaceDefinitionJobData
    {
        #region MEMBERS

        private List<IncludeFile> _IncludeFiles;

        #endregion

        #region PROPERTIES

        [XmlAttribute("id")]
        public Guid Id { get; set; }

        [XmlElement]
        public string Name { get; set; }

        [XmlElement]
        public string Description { get; set; }

        [XmlElement]
        public string EstimatedDurationRemarks { get; set; }

        [XmlIgnore]
        public InterfaceDefinitionData InterfaceDefinition { get; set; }

        [XmlArray("IncludeFiles")]
        [XmlArrayItem("IncludeFile")]
        public List<IncludeFile> IncludeFiles
        {
            get
            {
                if (_IncludeFiles == null) _IncludeFiles = new List<IncludeFile>(); // create new if null
                return _IncludeFiles;
            }
            set { _IncludeFiles = value; }
        }

        #endregion
    }
}
