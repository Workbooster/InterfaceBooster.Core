using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context
{
    /// <summary>
    /// A scope for the interpretation of Synery Code. It is used to store and resolve global variables.
    /// </summary>
    public interface IScope
    {
        IDictionary<string, IValue> Variables { get; }

        /// <summary>
        /// Creates a new space in memory for a variable with the given name and type.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        void DeclareVariable(string name, SyneryType type);

        /// <summary>
        /// Stores the given value in the reserverd space in memory with the given name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        void AssignVariable(string name, object value);

        /// <summary>
        /// Tries to resolve the variable with the specified name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IValue ResolveVariable(string name);

        /// <summary>
        /// Gets the information whether a variable with the given name can be resolved.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool DoesVariableExists(string name);
    }
}
