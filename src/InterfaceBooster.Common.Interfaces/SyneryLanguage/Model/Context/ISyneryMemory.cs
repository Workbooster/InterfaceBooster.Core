using InterfaceBooster.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.LibraryPlugin;
using InterfaceBooster.Common.Interfaces.ProviderPlugin.Interfaces;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.SyneryTypes;

namespace InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context
{
    /// <summary>
    /// The memory that is used while interpreting synery code.
    /// </summary>
    public interface ISyneryMemory
    {
        /// <summary>
        /// Gets or sets a flag that indicates whether the memory is ready to be used for the interpretation.
        /// </summary>
        bool IsInitialized { get; set; }

        /// <summary>
        /// Gets a list with a scopes on the stack.
        /// </summary>
        IEnumerable<IScope> Scopes { get; }

        /// <summary>
        /// Gets the active scope.
        /// </summary>
        IScope CurrentScope { get; }

        /// <summary>
        /// Gets the global scope of the program (if available; else NULL).
        /// </summary>
        IScope GlobalScope { get; }

        /// <summary>
        /// Gets or sets a list of functions that are available in the current synery code.
        /// </summary>
        IList<IFunctionData> Functions { get; set; }

        /// <summary>
        /// Gets or sets a list of record types that are available in the current synery code.
        /// </summary>
        IDictionary<SyneryType, IRecordType> RecordTypes { get; set; }

        /// <summary>
        /// Gets or sets a SyneryDB instance.
        /// </summary>
        IDatabase Database { get; set; }

        /// <summary>
        /// Gets or sets the manager that handles the communication with Provider Plugins.
        /// </summary>
        IProviderPluginManager ProviderPluginManager { get; set; }

        /// <summary>
        /// Gets or sets the manager that handles the communication with Library Plugins.
        /// </summary>
        ILibraryPluginManager LibraryPluginManager { get; set; }

        /// <summary>
        /// Inserts a new scope at the top of the stack.
        /// </summary>
        /// <param name="scope"></param>
        void PushScope(IScope scope);

        /// <summary>
        /// Removes and returns the scope on the top of the stack.
        /// </summary>
        /// <returns></returns>
        IScope PopScope();

        /// <summary>
        /// Tries to get the value of the variable with the specified name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IValue ResolveVariable(string name);
    }
}
