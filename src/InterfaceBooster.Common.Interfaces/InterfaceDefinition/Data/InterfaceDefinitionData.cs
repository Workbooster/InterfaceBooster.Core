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
        public Guid Id { get; set; }
        public string RootDirectoryPath { get; set; }
        public InterfaceDefinitionDetailData Details { get; set; }
        public IList<InterfaceDefinitionJobData> Jobs { get; set; }
        public IList<ProviderPluginInstanceReference> RequiredProviderPluginInstances { get; set; }
        public IList<LibraryPluginReference> RequiredLibraryPlugins { get; set; }
    }
}
