using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.InterfaceDefinition.Data;
using InterfaceBooster.Common.Interfaces.LibraryPlugin.Information.ReflectionData;
using InterfaceBooster.Common.Interfaces.LibraryPlugin.Information.XmlData;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;

namespace InterfaceBooster.Common.Interfaces.LibraryPlugin
{
    public interface ILibraryPluginManager
    {
        string PluginMainDirectoryPath { get; set; }
        IReadOnlyDictionary<ILibraryPluginReference, ILibraryPluginData> AvailablePlugins { get; }
        
        void Activate(ILibraryPluginReference reference);
        void Activate(IList<ILibraryPluginReference> references);
        IStaticExtensionFunctionData GetStaticFunctionDataBySignature(string libraryPluginIdentifier, string functionIdentifier, Type[] parameterTypes);
        IStaticExtensionVariableData GetStaticVariableDataByIdentifier(string libraryPluginIdentifier, string variableIdentifier);
        object CallStaticFunctionWithPrimitiveReturn(IStaticExtensionFunctionData functionData, object[] listOfParameters);
        object GetStaticVariableWithPrimitiveReturn(IStaticExtensionVariableData variableData);
        void SetStaticVariableWithPrimitiveReturn(IStaticExtensionVariableData variableData, IValue value);
    }
}
