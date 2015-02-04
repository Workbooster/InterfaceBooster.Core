using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;

namespace InterfaceBooster.SyneryLanguage.Model.Context
{
    /// <summary>
    /// Scope that is used for an OBSERVE block.
    /// </summary>
    public class ObserveScope : BlockScope, IObserveScope
    {
        #region MEMBERS

        private List<IHandleBlockData> _HandleBlocks;

        #endregion

        #region PROPERTIES

        /// <summary>
        /// Gets a list of all available HANDLE blocks in the scope of this OBSERVE block.
        /// </summary>
        public IReadOnlyList<IHandleBlockData> HandleBlocks { get { return _HandleBlocks; } }

        #endregion

        #region PUBLIC METHODS

        public ObserveScope(IScope parent, IEnumerable<IHandleBlockData> listOfHandleBlockData, IDictionary<string, IValue> variables = null)
            : base(parent, variables)
        {
            _HandleBlocks = new List<IHandleBlockData>(listOfHandleBlockData);
        }

        #endregion
    }
}
