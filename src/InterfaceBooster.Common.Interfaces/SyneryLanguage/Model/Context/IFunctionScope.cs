using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context
{
    public interface IFunctionScope : INestedScope
    {
        /// <summary>
        /// Gets or sets value that the current function block returns.
        /// This property may be null if the function returns nothing.
        /// </summary>
        IValue ReturnValue { get; set; }

        /// <summary>
        /// Gets or sets the the specification of the function this function block is related to.
        /// This may for example be important to get the CodeFileAlias to resolve the Synery file this function belongs to.
        /// </summary>
        IFunctionData FunctionData { get; set; }
    }
}
