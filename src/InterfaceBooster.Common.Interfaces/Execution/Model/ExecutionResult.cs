using InterfaceBooster.Common.Interfaces.InterfaceDefinition.Data;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Common.Interfaces.Execution.Model
{
    public class ExecutionResult
    {
        public bool IsSuccess { get; set; }
        public ExecutionVariables EnvironmentVariables { get; set; }
        public InterfaceDefinitionData InterfaceDefinitionData { get; set; }
        public ISyneryMemory SyneryMemory { get; set; }
    }
}
