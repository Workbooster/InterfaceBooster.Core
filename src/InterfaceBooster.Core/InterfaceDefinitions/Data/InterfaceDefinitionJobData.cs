using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.InterfaceDefinition.Data;

namespace InterfaceBooster.Core.InterfaceDefinitions.Data
{
    /// <summary>
    /// Container for the data of an Import Definition Job. An Import Definition can contain multiple jobs.
    /// </summary>
    public class InterfaceDefinitionJobData : IInterfaceDefinitionJobData
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string EstimatedDurationRemarks { get; set; }
        public IInterfaceDefinitionData InterfaceDefinition { get; set; }
        public IDictionary<string, string> IncludeFiles { get; set; }
    }
}
