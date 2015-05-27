using InterfaceBooster.Common.Interfaces.Broadcasting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Common.Interfaces.Execution.Model
{
    public class ExecutionVariables
    {
        #region PROPERTIES

        public IBroadcaster Broadcaster { get; set; }

        /// <summary>
        /// Gets or sets the absolute path to the interface definition main directory.
        /// </summary>
        public string InterfaceDefinitionDirectoryPath { get; set; }

        /// <summary>
        /// Gets or sets the absolute path to the directory where the Synery code files are stored.
        /// (default: [InterfaceDefinitionDirectoryPath]\code\)
        /// </summary>
        public string InterfaceDefinitionCodeDirectoryPath { get; set; }

        /// <summary>
        /// Gets or sets the absolute path to the synery database.
        /// (default: [InterfaceDefinitionDirectoryPath]\db\)
        /// </summary>
        public string DatabaseDirectoryPath { get; set; }

        /// <summary>
        /// Gets or sets the absolute path to the directory where all provider plugins are stored.
        /// (default: [InterfaceDefinitionDirectoryPath]\plugins\provider_plugins\)
        /// </summary>
        public string ProviderPluginDirectoryPath { get; set; }

        /// <summary>
        /// Gets or sets the absolute path to the directory where all library plugins are stored.
        /// (default: [InterfaceDefinitionDirectoryPath]\plugins\library_plugins\)
        /// </summary>
        public string LibraryPluginDirectoryPath { get; set; }


        #endregion
    }
}
