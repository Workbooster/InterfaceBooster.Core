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

namespace InterfaceBooster.SyneryLanguage.Interpretation
{
    public class SyneryFunctionDeclarationInterpretationClient : ISyneryClient<IList<IFunctionData>>
    {
        #region PROPERTIES

        public IInterpretationController Controller { get; private set; }

        public ISyneryMemory Memory { get; set; }

        public IAntlrErrorListener<int> LexerErrorListener { get; set; }

        public IAntlrErrorListener<IToken> ParserErrorListener { get; set; }

        #endregion

        #region PUBLIC METHODS

        public SyneryFunctionDeclarationInterpretationClient(ISyneryMemory memory, IAntlrErrorListener<int> lexerErrorListener = null, IAntlrErrorListener<IToken> parserErrorListener = null)
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
        /// execute the given code
        /// </summary>
        /// <param name="code"></param>
        /// <param name="includeCode"></param>
        /// <returns></returns>
        public IList<IFunctionData> Run(string code, IDictionary<string, string> includeCode = null)
        {
            // extract the functions and add them to the memory

            return ExtractFunctions(code, includeCode);
        }

        public void Cancel()
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion



        #region INTERNAL METHODS

        private IList<IFunctionData> ExtractFunctions(string code, IDictionary<string, string> includeCode = null)
        {
            List<IFunctionData> listOfFunctionDeclarations = new List<IFunctionData>();

            // extract functions from main code
            listOfFunctionDeclarations.AddRange(ExtractFunctionsAndAppendThemToMemory(code));

            if (includeCode != null)
            {
                // extract functions from included code files

                foreach (var item in includeCode)
                {
                    listOfFunctionDeclarations.AddRange(ExtractFunctionsAndAppendThemToMemory(item.Value, item.Key));
                }
            }

            return listOfFunctionDeclarations;
        }

        private IList<IFunctionData> ExtractFunctionsAndAppendThemToMemory(string code, string alias = null)
        {
            List<IFunctionData> listOfFunctionDeclarations = new List<IFunctionData>();

            SyneryParser.ProgramContext programContexts = ParserHelper.GetProgramAstFromCode(code, LexerErrorListener, ParserErrorListener);

            if (programContexts != null && programContexts.syneryFunctionBlock() != null)
            {
                foreach (SyneryParser.SyneryFunctionBlockContext context in programContexts.syneryFunctionBlock())
                {
                    IFunctionData functionData = Controller.Interpret<SyneryParser.SyneryFunctionBlockContext, IFunctionData, string>(context, alias);
                    listOfFunctionDeclarations.Add(functionData);
                }
            }

            return listOfFunctionDeclarations;
        }

        #endregion
    }
}
