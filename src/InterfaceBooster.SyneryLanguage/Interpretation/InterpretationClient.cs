using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.SyneryLanguage.Common;
using InterfaceBooster.SyneryLanguage.Interpretation.BaseLanguage;
using InterfaceBooster.SyneryLanguage.Interpretation.BaseLanguage.Blocks;
using InterfaceBooster.SyneryLanguage.Interpretation.BaseLanguage.Expressions;
using InterfaceBooster.SyneryLanguage.Interpretation.BaseLanguage.Functions;
using InterfaceBooster.SyneryLanguage.Interpretation.BaseLanguage.Statements;
using InterfaceBooster.SyneryLanguage.Interpretation.General;
using InterfaceBooster.SyneryLanguage.Interpretation.QueryLanguage.Commands;
using InterfaceBooster.SyneryLanguage.Interpretation.QueryLanguage.Expressions;
using InterfaceBooster.SyneryLanguage.Interpretation.QueryLanguage.Functions;
using InterfaceBooster.SyneryLanguage.Interpretation.QueryLanguage.Statements;
using InterfaceBooster.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.ErrorHandling;
using InterfaceBooster.Common.Interfaces.SyneryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.SyneryTypes;

namespace InterfaceBooster.SyneryLanguage.Interpretation
{
    public class InterpretationClient : ISyneryClient<bool>, IAntlrErrorListener<IToken>, IAntlrErrorListener<int>
    {
        #region PROPERTIES

        public IInterpretationController Controller { get; private set; }

        public ISyneryMemory Memory { get; set; }

        #endregion

        #region PUBLIC METHODS

        public InterpretationClient(ISyneryMemory memory)
        {
            Memory = memory;

            IInterpreterFactory factory = InterpreterFactory.GetDefaultInterpreterFactory();

            Controller = new InterpretationController(factory, memory);
        }

        #region IMPLEMENTATION OF ISyneryClient

        /// <summary>
        /// execute the given code
        /// </summary>
        /// <param name="code"></param>
        /// <param name="includeCode"></param>
        /// <returns></returns>
        public bool Run(string code, IDictionary<string, string> includeCode = null)
        {
            // prepare the memory that is used for the interpreation

            InitializeMemory(code, includeCode);

            // create a parser and start processing the tokenized code

            SyneryParser.ProgramContext programContext = ParserHelper.GetProgramAstFromCode(code, this, this);

            // start executing the code

            Controller.Interpret<SyneryParser.ProgramContext>(programContext);

            return true;
        }

        public void Cancel()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IMPLEMENTATION OF IAntlrErrorListener

        /// <summary>
        /// handle parser error
        /// </summary>
        /// <param name="recognizer"></param>
        /// <param name="offendingSymbol"></param>
        /// <param name="line"></param>
        /// <param name="charPositionInLine"></param>
        /// <param name="msg"></param>
        /// <param name="e"></param>
        public void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            throw new SyneryException(String.Format("Syntax error on line {0} at char {1}: {2}.", line, charPositionInLine, msg));
        }

        /// <summary>
        /// handle lexer error
        /// </summary>
        /// <param name="recognizer"></param>
        /// <param name="offendingSymbol"></param>
        /// <param name="line"></param>
        /// <param name="charPositionInLine"></param>
        /// <param name="msg"></param>
        /// <param name="e"></param>
        public void SyntaxError(IRecognizer recognizer, int offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            throw new SyneryException(String.Format("Syntax error on line {0} at char {1}: {2}.", line, charPositionInLine, msg));
        }

        #endregion

        #endregion

        #region INTERNAL METHODS

        private void InitializeMemory(string code, IDictionary<string, string> includeCode = null)
        {
            Memory.IsInitialized = false;

            // check whether a global scope exists
            if (Memory.CurrentScope == null)
            {
                Memory.PushScope(new GlobalScope());
            }

            // get the default system RecordTypes
            Memory.RecordTypes = SystemRecordTypeFactory.GetSystemRecordTypes();

            // extract the RecordTypes and add them to the memory

            RecordTypeDeclarationInterpretationClient recordTypeClient = new RecordTypeDeclarationInterpretationClient(Memory, this, this);
            recordTypeClient.Run(code, includeCode);

            // extract the functions and add them to the memory

            SyneryFunctionDeclarationInterpretationClient syneryFunctionClient = new SyneryFunctionDeclarationInterpretationClient(Memory, this, this);
            Memory.Functions = syneryFunctionClient.Run(code, includeCode);

            Memory.IsInitialized = true;
        }

        #endregion
    }
}
