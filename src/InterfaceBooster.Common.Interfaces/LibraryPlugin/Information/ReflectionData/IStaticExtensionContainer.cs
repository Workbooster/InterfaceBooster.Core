using InterfaceBooster.LibraryPluginApi.Static;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Common.Interfaces.LibraryPlugin.Information.ReflectionData
{
    public interface IStaticExtensionContainer
    {
        /// <summary>
        /// gets a list of all StaticExtension instances of the LibraryPlugins
        /// </summary>
        IList<IStaticExtension> Extensions { get; }

        /// <summary>
        ///  gets a list of all static functions that are available threw this LibraryPlugin.
        /// </summary>
        IList<IStaticExtensionFunctionData> Functions { get; }

        /// <summary>
        /// gets a list of all static variables that are available threw this LibraryPlugin.
        /// </summary>
        IList<IStaticExtensionVariableData> Variables { get; }
    }
}
