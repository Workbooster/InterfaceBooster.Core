using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Common.Interfaces.InterfaceDefinition.Data
{
    /// <summary>
    /// Container for the detail information of an Import Definition.
    /// </summary>
    public interface IInterfaceDefinitionDetailData
    {
        string Name { get; set; }
        string Description { get; set; }
        string Author { get; set; }
        DateTime DateOfCreation { get; set; }
        DateTime DateOfLastChange { get; set; }
        Version Version { get; set; }
        Version RequiredRuntimeVersion { get; set; }
    }
}
