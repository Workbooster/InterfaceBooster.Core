using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Common.Interfaces.InterfaceDefinition.Data
{
    /// <summary>
    /// Container for Import Definition data.
    /// </summary>
    public class InterfaceDefinitionData
    {
        #region MEMBERS

        private InterfaceDefinitionDetailData _Details;
        private List<InterfaceDefinitionJobData> _Jobs;
        private List<ProviderPluginInstanceReference> _RequiredProviderPluginInstances;
        private List<LibraryPluginReference> _RequiredLibraryPlugins;

        #endregion

        #region PROPERTIES

        public Guid Id { get; set; }
        public string RootDirectoryPath { get; set; }

        public InterfaceDefinitionDetailData Details
        {
            get
            {
                if (_Details == null) _Details = new InterfaceDefinitionDetailData(); // create new if null
                return _Details;
            }
            set { _Details = value; }
        }

        public List<InterfaceDefinitionJobData> Jobs
        {
            get
            {
                if (_Jobs == null) _Jobs = new List<InterfaceDefinitionJobData>(); // create new if null
                return _Jobs;
            }
            set { _Jobs = value; }
        }

        public List<ProviderPluginInstanceReference> RequiredProviderPluginInstances
        {
            get
            {
                if (_RequiredProviderPluginInstances == null) _RequiredProviderPluginInstances = new List<ProviderPluginInstanceReference>(); // create new if null
                return _RequiredProviderPluginInstances;
            }
            set { _RequiredProviderPluginInstances = value; }
        }

        public List<LibraryPluginReference> RequiredLibraryPlugins
        {
            get
            {
                if (_RequiredLibraryPlugins == null) _RequiredLibraryPlugins = new List<LibraryPluginReference>(); // create new if null
                return _RequiredLibraryPlugins;
            }
            set { _RequiredLibraryPlugins = value; }
        }

        #endregion
    }
}
