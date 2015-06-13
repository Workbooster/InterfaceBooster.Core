using InterfaceBooster.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.LibraryPlugin;
using InterfaceBooster.Common.Interfaces.ProviderPlugin.Interfaces;
using InterfaceBooster.Common.Interfaces.SyneryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.SyneryTypes;
using InterfaceBooster.Common.Interfaces.Broadcasting;

namespace InterfaceBooster.SyneryLanguage.Model.Context
{
    /// <summary>
    /// The memory that is used while interpreting synery code.
    /// </summary>
    public class SyneryMemory : ISyneryMemory
    {
        #region MEMBERS

        private Stack<IScope> _Scopes = new Stack<IScope>();

        #endregion

        #region PROPERTIES

        /// <summary>
        /// Gets or sets a flag that indicates whether the memory is ready to be used for the interpretation.
        /// </summary>
        public bool IsInitialized { get; set; }

        /// <summary>
        /// Gets a list with a scopes on the stack.
        /// </summary>
        public IEnumerable<IScope> Scopes
        {
            get
            {
                return _Scopes;
            }
        }

        /// <summary>
        /// Gets the active scope.
        /// </summary>
        public IScope CurrentScope
        {
            get
            {
                if (_Scopes.Count > 0)
                {
                    return _Scopes.Peek();
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the global scope of the program (if available; else NULL).
        /// </summary>
        public IScope GlobalScope
        {
            get
            {
                return _Scopes.LastOrDefault();
            }
        }

        /// <summary>
        /// Gets or sets a list of functions that are available in the current synery code.
        /// </summary>
        public IList<IFunctionData> Functions { get; set; }

        /// <summary>
        /// Gets or sets a list of record types that are available in the current synery code.
        /// </summary>
        public IDictionary<SyneryType, IRecordType> RecordTypes { get; set; }

        /// <summary>
        /// Gets or sets a SyneryDB instance.
        /// </summary>
        public IDatabase Database { get; set; }

        /// <summary>
        /// Gets or sets a broadcaster that handles log messages.
        /// </summary>
        public IBroadcaster Broadcaster { get; set; }

        /// <summary>
        /// Gets or sets the manager that handles the communication with Provider Plugins
        /// </summary>
        public IProviderPluginManager ProviderPluginManager { get; set; }

        /// <summary>
        /// Gets or sets the manager that handles the communication with Library Plugins.
        /// </summary>
        public ILibraryPluginManager LibraryPluginManager { get; set; }

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// The memory that is used while interpreting synery code.
        /// </summary>
        /// <param name="database"></param>
        /// <param name="providerPluginManager"></param>
        public SyneryMemory(IDatabase database, IBroadcaster broadcaster, IProviderPluginManager providerPluginManager, ILibraryPluginManager libraryPluginManager)
        {
            IsInitialized = false;
            Database = database;
            Broadcaster = broadcaster;
            ProviderPluginManager = providerPluginManager;
            LibraryPluginManager = libraryPluginManager;
        }

        /// <summary>
        /// Inserts a new scope at the top of the stack.
        /// </summary>
        /// <param name="scope"></param>
        public void PushScope(IScope scope)
        {
            _Scopes.Push(scope);
        }

        /// <summary>
        /// Removes and returns the scope on the top of the stack.
        /// </summary>
        /// <returns></returns>
        public IScope PopScope()
        {
            return _Scopes.Pop();
        }

        /// <summary>
        /// Tries to get the value of the variable with the specified name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IValue ResolveVariable(string name)
        {
            return CurrentScope.ResolveVariable(name);
        }
        
        #endregion
    }
}
