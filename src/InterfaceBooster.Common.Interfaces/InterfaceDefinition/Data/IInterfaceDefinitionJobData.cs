using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Common.Interfaces.InterfaceDefinition.Data
{
    /// <summary>
    /// Container for the data of an Import Definition Job. An Import Definition can contain multiple jobs.
    /// </summary>
    public interface IInterfaceDefinitionJobData
    {
        Guid Id { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        string EstimatedDurationRemarks { get; set; }
        IInterfaceDefinitionData InterfaceDefinition { get; set; }
        IDictionary<string, string> IncludeFiles { get; set; }
    }
}
