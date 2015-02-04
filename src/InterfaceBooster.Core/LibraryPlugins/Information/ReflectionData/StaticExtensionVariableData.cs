using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.LibraryPluginApi.Static;
using InterfaceBooster.Common.Interfaces.LibraryPlugin.Information.ReflectionData;

namespace InterfaceBooster.Core.LibraryPlugins.Information.ReflectionData
{
    public class StaticExtensionVariableData : IStaticExtensionVariableData
    {
        /// <summary>
        /// gets or sets the instance that contains this variable property.
        /// </summary>
        public IStaticExtension StaticExtension { get; set; }

        /// <summary>
        /// gets or sets the identifier used to access this variable from synery code.
        /// </summary>
        public string SyneryIdentifier { get; set; }

        /// <summary>
        /// gets the full identifier that is used in synery code including the namespace of the StaticExtension
        /// </summary>
        public string FullSyneryIdentifier
        {
            get
            {
                if (StaticExtension != null && StaticExtension.Namespace != null)
                {
                    return String.Format("{0}.{1}", StaticExtension.Namespace, SyneryIdentifier);
                }
                else
                {
                    return SyneryIdentifier;
                }
            }
        }

        /// <summary>
        /// gets or sets the name of the property that is related to this variable.
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// gets or sets the type of the variable value.
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// gets or sets a flag that indicates whether the variable can be set.
        /// </summary>
        public bool HasSetter { get; set; }
    }
}
