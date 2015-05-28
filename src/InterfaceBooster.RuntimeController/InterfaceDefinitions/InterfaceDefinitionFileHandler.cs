using InterfaceBooster.Common.Interfaces.InterfaceDefinition.Data;
using InterfaceBooster.Core.InterfaceDefinitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.RuntimeController.InterfaceDefinitions
{
    public static class InterfaceDefinitionFileHandler
    {
        public static InterfaceDefinitionData Load(string interfaceDefinitionFilePath)
        {
            return InterfaceDefinitionDataController.Load(interfaceDefinitionFilePath);
        }

        public static void Save(string interfaceDefinitionFilePath, InterfaceDefinitionData definitionData)
        {
            InterfaceDefinitionDataController.Save(interfaceDefinitionFilePath, definitionData);
        }
    }
}
