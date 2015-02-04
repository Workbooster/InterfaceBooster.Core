using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context
{
    /// <summary>
    /// A scope that has a parent.
    /// </summary>
    public interface INestedScope : IScope
    {
        /// <summary>
        /// Gets the scope from the next higher level
        /// </summary>
        IScope Parent { get; }

        /// <summary>
        /// Gets or sets a flag that idicates whether the current block is terminated.
        /// For example by a RETURN or a THROW statement.
        /// </summary>
        bool IsTerminated { get; set; }
    }
}
