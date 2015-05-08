using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace InterfaceBooster.Common.Interfaces.InterfaceDefinition.Data
{
    [Serializable]
    [XmlRoot("RequiredPlugins")]
    public class InterfaceDefinitionRequiredPlugins
    {
        #region MEMBERS

        private List<ProviderPluginInstanceReference> _ProviderPluginInstances;
        private List<LibraryPluginReference> _LibraryPlugins;

        #endregion

        #region PROPERTIES

        [XmlArray("ProviderPlugins")]
        [XmlArrayItem("ProviderPluginInstance")]
        public List<ProviderPluginInstanceReference> ProviderPluginInstances
        {
            get
            {
                if (_ProviderPluginInstances == null) _ProviderPluginInstances = new List<ProviderPluginInstanceReference>(); // create new if null
                return _ProviderPluginInstances;
            }
            set { _ProviderPluginInstances = value; }
        }

        [XmlArray("LibraryPlugins")]
        [XmlArrayItem("LibraryPlugin")]
        public List<LibraryPluginReference> LibraryPlugins
        {
            get
            {
                if (_LibraryPlugins == null) _LibraryPlugins = new List<LibraryPluginReference>(); // create new if null
                return _LibraryPlugins;
            }
            set { _LibraryPlugins = value; }
        }

        #endregion
    }
}
