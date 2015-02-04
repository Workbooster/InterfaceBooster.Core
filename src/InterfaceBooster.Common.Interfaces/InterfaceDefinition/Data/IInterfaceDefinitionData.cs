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
    public interface IInterfaceDefinitionData
    {
        Guid Id { get; set; }
        string RootDirectoryPath { get; set; }
        IInterfaceDefinitionDetailData Details { get; set; }
        IList<IInterfaceDefinitionJobData> Jobs { get; set; }
        IList<IProviderPluginInstanceReference> RequiredProviderPluginInstances { get; set; }
        IList<ILibraryPluginReference> RequiredLibraryPlugins { get; set; }
    }
}
