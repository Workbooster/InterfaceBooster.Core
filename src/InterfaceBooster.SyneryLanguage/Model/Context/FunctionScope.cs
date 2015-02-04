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

        #endregion

        #region PUBLIC METHODS

        public FunctionScope(IScope parent, IDictionary<string, IValue> variables = null) : base(parent, variables) {}

        #endregion
    }
}
