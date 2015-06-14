using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.SyneryLanguage.Interpretation.BaseLanguage.Expressions;
using InterfaceBooster.Common.Interfaces.SyneryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;

namespace InterfaceBooster.SyneryLanguage.Interpretation.General
{
    /// <summary>
    /// Interprets an single key-value combination.
    /// </summary>
    public class KeyValueAssignmentInterpreter : IInterpreter<SyneryParser.KeyValueAssignmentContext, KeyValuePair<string[], IValue>>
    {
        #region PROPERTIES

        public ISyneryMemory Memory { get; set; }
        public IInterpretationController Controller { get; set; }

        #endregion

        #region PUBLIC METHODS

        public KeyValuePair<string[], IValue> RunWithResult(SyneryParser.KeyValueAssignmentContext context)
        {
            string identifier = context.keyValueAssignmentIdentifier().GetText();
            string[] key = IdentifierHelper.ParseComplexIdentifier(identifier);
            IValue value = Controller.Interpret<SyneryParser.ExpressionContext, IValue>(context.keyValueInitializer().expression());

            return new KeyValuePair<string[], IValue>(key, value);
        }

        #endregion
    }
}
