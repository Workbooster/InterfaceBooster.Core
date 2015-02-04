using InterfaceBooster.LibraryPluginApi.Static;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.LibraryPlugin.Information.ReflectionData;

namespace InterfaceBooster.Core.LibraryPlugins.Information.ReflectionData
{
    public class StaticExtensionFunctionData : IStaticExtensionFunctionData
    {
        #region PROPERTIES

        /// <summary>
        /// gets or sets the instance that contains the method this function is related to.
        /// </summary>
        public IStaticExtension StaticExtension { get; set; }

        /// <summary>
        /// gets or sets the identifier used to calls this function from synery code.
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
        /// gets or sets the name of the method that is related to this function.
        /// </summary>
        public string MethodName { get; set; }

        /// <summary>
        /// gets or sets the Type the function returns. The value is null if the function has no return.
        /// </summary>
        public Type ReturnType { get; set; }

        /// <summary>
        /// gets a list of parameters that the function takes.
        /// </summary>
        public IList<IStaticExtensionFunctionParameterData> Parameters { get; private set; }

        #endregion

        #region PUBLIC METHODS

        public StaticExtensionFunctionData()
        {
            Parameters = new List<IStaticExtensionFunctionParameterData>();
        }

        #endregion
    }
}
