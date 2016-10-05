using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.SyneryLanguage.Common;
using InterfaceBooster.SyneryLanguage.Interpretation.BaseLanguage.Functions;
using InterfaceBooster.Common.Interfaces.SyneryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.ProviderPlugin.Control;

namespace InterfaceBooster.SyneryLanguage.Interpretation
{
    public class ConnectTaskDeclarationInterpretationClient : ISyneryClient<IList<ProviderPluginConnectTask>>
    {
        #region PROPERTIES

        public IInterpretationController Controller { get; private set; }

        public ISyneryMemory Memory { get; set; }

        public IAntlrErrorListener<int> LexerErrorListener { get; set; }

        public IAntlrErrorListener<IToken> ParserErrorListener { get; set; }

        #endregion

        #region PUBLIC METHODS

        public ConnectTaskDeclarationInterpretationClient(ISyneryMemory memory, IAntlrErrorListener<int> lexerErrorListener = null, IAntlrErrorListener<IToken> parserErrorListener = null)
        {
            Memory = memory;
            LexerErrorListener = lexerErrorListener;
            ParserErrorListener = parserErrorListener;

            IInterpreterFactory factory = InterpreterFactory.GetDefaultInterpreterFactory();
            factory.SetInterpreter(new SyneryFunctionBlockInterpreter());

            Controller = new InterpretationController(factory, memory);
        }

        #region IMPLEMENTATION OF ISyneryClient

        /// <summary>
        /// Gets a list of provider plugin connections from the given synery code with all the information that is needed to establish a connection.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="includeCode"></param>
        /// <returns></returns>
        public IList<ProviderPluginConnectTask> Run(string code, IDictionary<string, string> includeCode = null)
        {
            // extract the functions and add them to the memory

            return ExtractConnectTaskData(code, includeCode);
        }

        public void Cancel()
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion



        #region INTERNAL METHODS

        private IList<ProviderPluginConnectTask> ExtractConnectTaskData(string code, IDictionary<string, string> includeCode = null)
        {
            List<ProviderPluginConnectTask> listOfConnectTaskDataObjects = new List<ProviderPluginConnectTask>();

            // extract connect task data from main code
            listOfConnectTaskDataObjects.AddRange(ExtractConnectTaskDataFromCode(code));

            if (includeCode != null)
            {
                // extract connect task data from included code files

                foreach (var item in includeCode)
                {
                    listOfConnectTaskDataObjects.AddRange(ExtractConnectTaskDataFromCode(item.Value));
                }
            }

            return listOfConnectTaskDataObjects;
        }

        private IList<ProviderPluginConnectTask> ExtractConnectTaskDataFromCode(string code)
        {
            List<ProviderPluginConnectTask> listOfConnectTaskDataObjects = new List<ProviderPluginConnectTask>();

            SyneryParser.ProgramContext programContexts = ParserHelper.GetProgramAstFromCode(code, LexerErrorListener, ParserErrorListener);

            if (programContexts != null)
            {
                // load connect statements that are nested like that:
                // programUnit > providerPluginStatement > providerPluginConnectStatement
                foreach (var programUnitContext in programContexts.GetRuleContexts<SyneryParser.ProgramUnitContext>())
                {
                    foreach (var providerPluginStatementContext in programUnitContext.GetRuleContexts<SyneryParser.ProviderPluginStatementContext>())
                    {
                        foreach (var connectStatementContext in providerPluginStatementContext.GetRuleContexts<SyneryParser.ProviderPluginConnectStatementContext>())
                        {
                            ProviderPluginConnectTask connectTask = Controller.Interpret<SyneryParser.ProviderPluginConnectStatementContext, ProviderPluginConnectTask>(connectStatementContext);
                            listOfConnectTaskDataObjects.Add(connectTask);
                        }
                        
                    }
                    
                }
            }

            return listOfConnectTaskDataObjects;
        }

        #endregion
    }
}
