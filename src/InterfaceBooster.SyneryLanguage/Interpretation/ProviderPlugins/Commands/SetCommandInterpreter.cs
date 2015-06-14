using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.SyneryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;

namespace InterfaceBooster.SyneryLanguage.Interpretation.ProviderPlugins.Commands
{
    public class SetCommandInterpreter : IInterpreter<SyneryParser.SetCommandContext, IDictionary<string[], IValue>>
    {
        #region PROPERTIES

        public ISyneryMemory Memory { get; set; }
        public IInterpretationController Controller { get; set; }

        #endregion

        #region PUBLIC METHODS

        public IDictionary<string[], IValue> RunWithResult(SyneryParser.SetCommandContext context)
        {
            IDictionary<string[], IValue> values = Controller.Interpret<SyneryParser.KeyValueListContext, IDictionary<string[], IValue>>(context.keyValueList());

            return values;
        }

        #endregion
    }
}
