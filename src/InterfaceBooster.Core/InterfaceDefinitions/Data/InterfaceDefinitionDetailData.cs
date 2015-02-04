using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.InterfaceDefinition.Data;

namespace InterfaceBooster.Core.InterfaceDefinitions.Data
{
    /// <summary>
    /// Container for the detail information of an Import Definition.
    /// </summary>
    public class InterfaceDefinitionDetailData : IInterfaceDefinitionDetailData
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public DateTime DateOfCreation { get; set; }
        public DateTime DateOfLastChange { get; set; }
        public Version Version { get; set; }
        public Version RequiredRuntimeVersion { get; set; }
    }
}
