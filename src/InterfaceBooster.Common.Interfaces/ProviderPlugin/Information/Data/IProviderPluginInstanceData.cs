using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Common.Interfaces.ProviderPlugin.Interfaces.Information.Data
{
    /// <summary>
    /// Container for the information about a Provider Plugin Instance.
    /// An assembly of a Provider Plugin can contain multiple Provider Plugin Instance objects.
    /// </summary>
    public interface IProviderPluginInstanceData
    {
        Guid Id { get; set; }
        string Name { get; set; }
        string Description { get; set; }
    }
}
