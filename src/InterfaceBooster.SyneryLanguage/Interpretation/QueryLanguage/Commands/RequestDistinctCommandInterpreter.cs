using InterfaceBooster.Database.Interfaces.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.SyneryLanguage.Model.QueryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Tools.Data.Array;

namespace InterfaceBooster.SyneryLanguage.Interpretation.QueryLanguage.Commands
{
    public class RequestDistinctCommandInterpreter : IInterpreter<SyneryParser.RequestDistinctCommandContext, ITable, QueryMemory>
    {
        #region PROPERTIES

        public ISyneryMemory Memory { get; set; }
        public IInterpretationController Controller { get; set; }

        #endregion

        #region PUBLIC METHODS

        public ITable RunWithResult(SyneryParser.RequestDistinctCommandContext context, QueryMemory queryMemory)
        {
            ObjectArrayEqualityComparer equalityComparer = new ObjectArrayEqualityComparer();

            // perform the distinct action
            IEnumerable<object[]> data = queryMemory.CurrentTable.Distinct(equalityComparer);

            // overwrite the data of the current table and return it

            queryMemory.CurrentTable.SetData(data);

            return queryMemory.CurrentTable;
        }

        #endregion
    }
}
