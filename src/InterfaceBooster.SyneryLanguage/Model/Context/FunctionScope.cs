using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;

namespace InterfaceBooster.SyneryLanguage.Model.Context
{
    public class FunctionScope : BlockScope, IFunctionScope
    {
        #region PROPERTIES

        /// <summary>
        /// Gets or sets value that the current function block returns.
        /// This property may be null if the function returns nothing.
        /// </summary>
        public IValue ReturnValue { get; set; }

        /// <summary>
        /// Gets or sets the the specification of the function this function block is related to.
        /// This may for example be important to get the CodeFileAlias to resolve the Synery file this function belongs to.
        /// </summary>
        public IFunctionData FunctionData { get; set; }

        #endregion

        #region PUBLIC METHODS

        public FunctionScope(IScope parent, IFunctionData functionData, IDictionary<string, IValue> variables = null) : base(parent, variables)
        {
            this.FunctionData = functionData;
        }

        /// <summary>
        /// Tries to resolve the surrounding function scope that is closest to the current scope (may also be the this scope).
        /// This may be important to know in what scope the current code is running.
        /// </summary>
        /// <returns>The closest function scope or null if the code isn't running inside of a function.</returns>
        public new IFunctionScope ResolveFunctionScope()
        {
            return this;
        }

        #endregion
    }
}
