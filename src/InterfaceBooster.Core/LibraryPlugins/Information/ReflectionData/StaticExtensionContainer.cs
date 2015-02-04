using InterfaceBooster.LibraryPluginApi.Static;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.LibraryPlugin.Information.ReflectionData;

namespace InterfaceBooster.Core.LibraryPlugins.Information.ReflectionData
{
    public class StaticExtensionContainer : IStaticExtensionContainer
    {
        #region PROPERTIES
        
        /// <summary>
        /// gets a list of all StaticExtension instances of the LibraryPlugins
        /// </summary>
        public IList<IStaticExtension> Extensions { get; private set; }

        /// <summary>
        /// gets a list of all static functions that are available threw this LibraryPlugin.
        /// </summary>
        public IList<IStaticExtensionFunctionData> Functions { get; private set; }

        /// <summary>
        /// gets a list of all static variables that are available threw this LibraryPlugin.
        /// </summary>
        public IList<IStaticExtensionVariableData> Variables { get; private set; }

        #endregion

        #region PUBLIC METHODS

        public StaticExtensionContainer()
        {
            Extensions = new List<IStaticExtension>();
            Functions = new List<IStaticExtensionFunctionData>();
            Variables = new List<IStaticExtensionVariableData>();
        }

        #endregion
    }
}
