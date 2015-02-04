using InterfaceBooster.LibraryPluginApi.Static;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Common.Interfaces.LibraryPlugin.Information.ReflectionData
{
    public interface IStaticExtensionFunctionData
    {
        /// <summary>
        /// gets or sets the instance that contains the method this function is related to.
        /// </summary>
        IStaticExtension StaticExtension { get; set; }
        
        /// <summary>
        /// gets or sets the identifier used to calls this function from synery code.
        /// </summary>
        string SyneryIdentifier { get; set; }

        /// <summary>
        /// gets the full identifier that is used in synery code including the namespace of the StaticExtension
        /// </summary>
        string FullSyneryIdentifier { get; }

        /// <summary>
        /// gets or sets the name of the method that is related to this function.
        /// </summary>
        string MethodName { get; set; }

        /// <summary>
        /// gets or sets the Type the function returns. The value is null if the function has no return.
        /// </summary>
        Type ReturnType { get; set; }

        /// <summary>
        /// gets a list of parameters that the function takes
        /// </summary>
        IList<IStaticExtensionFunctionParameterData> Parameters { get; }
    }
}
