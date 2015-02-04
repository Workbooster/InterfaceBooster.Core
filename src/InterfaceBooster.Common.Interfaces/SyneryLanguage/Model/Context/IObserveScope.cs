using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context
{
    /// <summary>
    /// Scope that is used for an OBSERVE block.
    /// </summary>
    public interface IObserveScope : INestedScope
    {
        /// <summary>
        /// Gets a list of all available HANDLE blocks in the scope of this OBSERVE block.
        /// </summary>
        IReadOnlyList<IHandleBlockData> HandleBlocks { get; }
    }
}
