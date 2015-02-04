using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.InterfaceDefinition.Data;

namespace InterfaceBooster.Core.InterfaceDefinitions.Data
{
    /// <summary>
    /// Container for Import Definition data.
    /// </summary>
    public class InterfaceDefinitionData : IInterfaceDefinitionData
    {
        public Guid Id { get; set; }
        public string RootDirectoryPath { get; set; }
        public IInterfaceDefinitionDetailData Details { get; set; }
        public IList<IInterfaceDefinitionJobData> Jobs { get; set; }
        public IList<IProviderPluginInstanceReference> RequiredProviderPluginInstances { get; set; }
        public IList<ILibraryPluginReference> RequiredLibraryPlugins { get; set; }
    }
}
