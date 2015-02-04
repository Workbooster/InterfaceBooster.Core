using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.ErrorHandling;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;

namespace InterfaceBooster.SyneryLanguage.Model.Context
{
    /// <summary>
    /// A root scope of a Synery programm. It is used to store and resolve global variables.
    /// </summary>
    public class GlobalScope : IScope
    {
        private IDictionary<string, IValue> _Variables;

        public IDictionary<string, IValue> Variables
        {
            get
            {
                return _Variables;
            }
        }

        public GlobalScope(IDictionary<string, IValue> variables = null)
        {
            if (variables == null)
            {
                _Variables = new Dictionary<string, IValue>();
            }
            else
            {
                _Variables = variables;
            }
        }

        /// <summary>
        /// Creates a new space in memory for a variable with the given name and type.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        public void DeclareVariable(string name, SyneryType type)
        {
            _Variables[name] = new TypedValue(type);
        }

        /// <summary>
        /// Stores the given value in the reserverd space in memory with the given name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void AssignVariable(string name, object value)
        {
            if (_Variables.ContainsKey(name))
            {
                if (value == null || _Variables[name].Type.UnterlyingDotNetType.IsAssignableFrom(value.GetType()))
                {
                    _Variables[name].Value = value;
                    return;
                }

                throw new SyneryException(String.Format("Variable type '{0}' and value type '{1}' don't match. Variable name='{2}'.",
                    _Variables[name].Type.PublicName, value.GetType().Name, name));
            }
            else
            {
                throw new SyneryException(String.Format("Variable with name='{0}' doesn't exists.", name));
            }
        }

        /// <summary>
        /// Tries to resolve the variable with the specified name.
        /// Throws a SyneryException if the current (global) scope doesn't contain the requested variable. Because this 
        /// scope is the root scope without a parent.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IValue ResolveVariable(string name)
        {
            if (_Variables.ContainsKey(name))
            {
                return _Variables[name];
            }
            else
            {
                throw new SyneryException(String.Format("Variable with name='{0}' doesn't exists.", name));
            }
        }

        /// <summary>
        /// Gets the information whether a variable with the given name can be resolved.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool DoesVariableExists(string name)
        {
            return _Variables.ContainsKey(name);
        }
    }
}
