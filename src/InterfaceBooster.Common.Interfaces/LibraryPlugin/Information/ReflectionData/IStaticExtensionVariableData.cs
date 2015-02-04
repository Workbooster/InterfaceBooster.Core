using InterfaceBooster.LibraryPluginApi.Static;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Common.Interfaces.LibraryPlugin.Information.ReflectionData
{
    public interface IStaticExtensionVariableData
    {
        /// <summary>
        /// gets or sets the instance that contains this variable property.
        /// </summary>
        IStaticExtension StaticExtension { get; set; }
        
        /// <summary>
        /// gets or sets the identifier used to access this variable from synery code.
        /// </summary>
        string SyneryIdentifier { get; set; }

        /// <summary>
        /// gets the full identifier that is used in synery code including the namespace of the StaticExtension.
        /// </summary>
        string FullSyneryIdentifier { get; }
        
        /// <summary>
        /// gets or sets the name of the property that is related to this variable.
        /// </summary>
        string PropertyName { get; set; }
        
        /// <summary>
        /// gets or sets the type of the variable value.
        /// </summary>
        Type Type { get; set; }
        
        /// <summary>
        /// gets or sets a flag that indicates whether the variable can be set.
        /// </summary>
        bool HasSetter { get; set; }
    }
}
