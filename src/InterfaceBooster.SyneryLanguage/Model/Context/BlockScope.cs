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
    /// A scope of a Synery code block (e.g. function, if-else).
    /// It is used to store and resolve variables.
    /// </summary>
    public class BlockScope : INestedScope
    {
        #region MEMBERS

        private IDictionary<string, IValue> _Variables;

        #endregion

        #region PROPERTIES

        /// <summary>
        /// Gets the scope from the next higher level
        /// </summary>
        public IScope Parent { get; private set; }

        /// <summary>
        /// Gets or sets a flag that idicates whether the current block is terminated.
        /// For example by a RETURN or a THROW statement.
        /// </summary>
        public bool IsTerminated { get; set; }

        /// <summary>
        /// Gets a list of all variables
        /// </summary>
        public IDictionary<string, IValue> Variables
        {
            get
            {
                return _Variables;
            }
        }

        #endregion

        #region PUBLIC METHODS

        public BlockScope(IScope parent, IDictionary<string, IValue> variables = null)
        {
            Parent = parent;
            IsTerminated = false;

            if (variables == null)
            {
                _Variables = new Dictionary<string, IValue>();
            }
            else
            {
                _Variables = variables;
            }
        }

        #region IMPLEMENTATION OF IScope

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
                Parent.AssignVariable(name, value);
            }
        }

        /// <summary>
        /// Tries to resolve the variable with the specified name.
        /// If the current scope doesn't contain the variable the request is forwarded to the parent scope.
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
                return Parent.ResolveVariable(name);
            }
        }

        /// <summary>
        /// Gets the information whether a variable with the given name can be resolved.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool DoesVariableExists(string name)
        {
            if (_Variables.ContainsKey(name))
            {
                return true;
            }
            else
            {
                return Parent.DoesVariableExists(name);
            }
        }

        /// <summary>
        /// Tries to resolve the surrounding function scope that is closest to the current scope (may also be the this scope).
        /// This may be important to know in what scope the current code is running.
        /// </summary>
        /// <returns>The closest function scope or null if the code isn't running inside of a function.</returns>
        public IFunctionScope ResolveFunctionScope()
        {
            return Parent.ResolveFunctionScope();
        }

        #endregion

        #endregion
    }
}
