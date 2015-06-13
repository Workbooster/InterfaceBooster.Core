using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.SyneryLanguage.Interpretation.BaseLanguage.Statements;
using InterfaceBooster.SyneryLanguage.Interpretation.QueryLanguage.Statements;
using InterfaceBooster.Common.Interfaces.ErrorHandling;
using InterfaceBooster.Common.Interfaces.SyneryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;

namespace InterfaceBooster.SyneryLanguage.Interpretation.BaseLanguage
{
    public class ProgramUnitInterpreter : IInterpreter<SyneryParser.ProgramUnitContext>
    {
        #region PROPERTIES

        public ISyneryMemory Memory { get; set; }
        public IInterpretationController Controller { get; set; }

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Calls the responsible interpreters for each main programm unit like variable, table, 
        /// connect, import or export statements.
        /// </summary>
        /// <param name="context"></param>
        public void Run(SyneryParser.ProgramUnitContext context)
        {
            foreach (var item in context.children)
            {
                if (item is SyneryParser.VariableStatementContext)
                {
                    Controller.Interpret<SyneryParser.VariableStatementContext>((SyneryParser.VariableStatementContext)item);
                }
                else if (item is SyneryParser.LibraryPluginVariableStatementContext)
                {
                    Controller.Interpret<SyneryParser.LibraryPluginVariableStatementContext>((SyneryParser.LibraryPluginVariableStatementContext)item);
                }
                else if (item is SyneryParser.TableAddStatementContext)
                {
                    Controller.Interpret<SyneryParser.TableAddStatementContext>((SyneryParser.TableAddStatementContext)item);
                }
                else if (item is SyneryParser.TableDropStatementContext)
                {
                    Controller.Interpret<SyneryParser.TableDropStatementContext>((SyneryParser.TableDropStatementContext)item);
                }
                else if (item is SyneryParser.TableStatementContext)
                {
                    Controller.Interpret<SyneryParser.TableStatementContext>((SyneryParser.TableStatementContext)item);
                }
                else if (item is SyneryParser.ProviderPluginStatementContext)
                {
                    Controller.Interpret<SyneryParser.ProviderPluginStatementContext>((SyneryParser.ProviderPluginStatementContext)item);
                }
                else if (item is SyneryParser.FunctionCallContext)
                {
                    Controller.Interpret<SyneryParser.FunctionCallContext, IValue>((SyneryParser.FunctionCallContext)item);
                }
                else if (item is SyneryParser.IfStatementContext)
                {
                    Controller.Interpret<SyneryParser.IfStatementContext>((SyneryParser.IfStatementContext)item);
                }
                else if (item is SyneryParser.ObserveBlockContext)
                {
                    Controller.Interpret<SyneryParser.ObserveBlockContext>((SyneryParser.ObserveBlockContext)item);
                }
                else
                {
                    if (item is ParserRuleContext)
                    {
                        throw new SyneryInterpretationException((ParserRuleContext)item, "Unknown program unit in ProgramUnitInterpreter. No interpreter found for the given context.");   
                    }
                }
            }
        }

        #endregion
    }
}
