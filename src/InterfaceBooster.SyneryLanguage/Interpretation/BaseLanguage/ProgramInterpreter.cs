using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.SyneryLanguage.Interpretation.BaseLanguage.Statements;
using InterfaceBooster.SyneryLanguage.Interpretation.QueryLanguage.Statements;
using InterfaceBooster.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.SyneryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;

namespace InterfaceBooster.SyneryLanguage.Interpretation.BaseLanguage
{
    /// <summary>
    /// Interprets the whole code of a Synery program. It calls the responsible interpreters for each main programm unit.
    /// </summary>
    public class ProgramInterpreter : IInterpreter<SyneryParser.ProgramContext>
    {
        #region PROPERTIES

        public ISyneryMemory Memory { get; set; }
        public IInterpretationController Controller { get; set; }

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Interprets the whole code of a Synery program.
        /// </summary>
        /// <param name="context"></param>
        public void Run(SyneryParser.ProgramContext context)
        {
            foreach (var item in context.children)
            {
                if (item is SyneryParser.ProgramUnitContext)
                {
                    Controller.Interpret<SyneryParser.ProgramUnitContext>((SyneryParser.ProgramUnitContext)item);
                }
            }
        }

        #endregion
    }
}
