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
    /// Interprets a list of key-value pairs from Synery language.
    /// </summary>
    public class KeyValueListInterpreter : IInterpreter<SyneryParser.KeyValueListContext, IDictionary<string[], IValue>>
    {
        #region PROPERTIES

        public ISyneryMemory Memory { get; set; }
        public IInterpretationController Controller { get; set; }

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Interprets a list of key-value pairs from Synery language. It returns the result as a Dictionary containing a path array of a complex identifier as key and the value as value.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public IDictionary<string[], IValue> Run(SyneryParser.KeyValueListContext context)
        {
            Dictionary<string[], IValue> listOfKeyValues = new Dictionary<string[], IValue>();

            foreach (SyneryParser.KeyValueAssignmentContext assignmentContext in context.keyValueAssignment())
            {
                KeyValuePair<string[], IValue> assignmentResult = Controller
                    .Interpret<SyneryParser.KeyValueAssignmentContext,KeyValuePair<string[], IValue>>(assignmentContext);
                listOfKeyValues.Add(assignmentResult.Key, assignmentResult.Value);
            }

            return listOfKeyValues;
        }

        #endregion
    }
}
