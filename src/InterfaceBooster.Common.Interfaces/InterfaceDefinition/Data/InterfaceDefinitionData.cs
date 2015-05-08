using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace InterfaceBooster.Common.Interfaces.InterfaceDefinition.Data
{
    /// <summary>
    /// Container for Import Definition data.
    /// </summary>
    [Serializable]
    [XmlRoot("InterfaceDefinition")]
    public class InterfaceDefinitionData
    {
        #region MEMBERS

        private InterfaceDefinitionDetailData _Details;
        private List<InterfaceDefinitionJobData> _Jobs;
        private InterfaceDefinitionRequiredPlugins _InterfaceDefinitionRequiredPlugins;

        #endregion

        #region PROPERTIES

        [XmlAttribute("id")]
        public Guid Id { get; set; }

        [XmlIgnore]
        public string RootDirectoryPath { get; set; }

        [XmlElement]
        public InterfaceDefinitionDetailData Details
        {
            get
            {
                if (_Details == null) _Details = new InterfaceDefinitionDetailData(); // create new if null
                return _Details;
            }
            set { _Details = value; }
        }

        [XmlArray("Jobs")]
        [XmlArrayItem("Job")]
        public List<InterfaceDefinitionJobData> Jobs
        {
            get
            {
                if (_Jobs == null) _Jobs = new List<InterfaceDefinitionJobData>(); // create new if null
                return _Jobs;
            }
            set { _Jobs = value; }
        }

        [XmlElement]
        public InterfaceDefinitionRequiredPlugins RequiredPlugins
        {
            get
            {
                if (_InterfaceDefinitionRequiredPlugins == null) _InterfaceDefinitionRequiredPlugins = new InterfaceDefinitionRequiredPlugins(); // create new if null
                return _InterfaceDefinitionRequiredPlugins;
            }
            set { _InterfaceDefinitionRequiredPlugins = value; }
        }

        #endregion
    }
}
