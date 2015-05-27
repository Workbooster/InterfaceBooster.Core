using InterfaceBooster.Common.Interfaces.InterfaceDefinition.Data;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Common.Interfaces.Runtime.Model
{
    public class RuntimeResult
    {
        public bool IsSuccess { get; set; }
        public EnvironmentVariables EnvironmentVariables { get; set; }
        public InterfaceDefinitionData InterfaceDefinitionData { get; set; }
        public ISyneryMemory SyneryMemory { get; set; }
    }
}
