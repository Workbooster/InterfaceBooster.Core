using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.SyneryLanguage.Common;
using InterfaceBooster.SyneryLanguage.Model.Validation;
using InterfaceBooster.Common.Interfaces.SyneryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Validation;

namespace InterfaceBooster.SyneryLanguage.Validation
{
    /// <summary>
    /// Is used to run some code validations on Synery code.
    /// </summary>
    public class ValidationClient : ISyneryClient<IValidationResult>, IAntlrErrorListener<IToken>, IAntlrErrorListener<int>
    {
        #region MEMBERS

        private IValidationResult _ValidationResult;

        #endregion

        #region PROPERTIES

        public IInterpretationController Controller { get; private set; }

        public ISyneryMemory Memory { get; set; }

        #endregion

        #region PUBLIC METHODS
        
        /// <summary>
        /// Runs some code validations and returns a result that contains some information about potential problems or errors.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="includeFiles"></param>
        /// <returns></returns>
        public IValidationResult Run(string code, IDictionary<string, string> includeFiles = null)
        {
            _ValidationResult = new ValidationResult();
            _ValidationResult.IsValid = true;

            // create a lexer/parser and listen for some errors
            SyneryParser.ProgramContext programContext = ParserHelper.GetProgramAstFromCode(code, this, this);

            if (_ValidationResult.Messages.Where(m => m.Category == ValidationResultMessageCategoryEnum.Error).Count() > 0)
            {
                // errors found
                _ValidationResult.IsValid = false;
            }

            return _ValidationResult;
        }

        public void Cancel()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Listens to some parser errors
        /// </summary>
        /// <param name="recognizer"></param>
        /// <param name="offendingSymbol"></param>
        /// <param name="line"></param>
        /// <param name="charPositionInLine"></param>
        /// <param name="msg"></param>
        /// <param name="e"></param>
        public void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            string message = "";

            if (e == null)
            {
                message = String.Format("Unexpected token '{0}' detected.", offendingSymbol.Text);
            }
            else if (e is NoViableAltException)
            {
                NoViableAltException noViableAltException = (NoViableAltException)e;
                message = String.Format("No viable alternative for '{1}' at input '{0}' followed by '{2}'", noViableAltException.StartToken.Text, offendingSymbol.Text, offendingSymbol.TokenSource.NextToken().Text);

                // set the line where the problem begins as the affected line
                line = noViableAltException.StartToken.Line;
            }
            else if (e is InputMismatchException)
            {
                InputMismatchException noViableAltException = (InputMismatchException)e;

                // prepare a list of suggested alternatives of the given tokens

                string expectedTokens = "(no suggestions)";

                if (noViableAltException.GetExpectedTokens().Count > 0)
                {
                    List<string> listOfExpectedTokens = new List<string>();

                    foreach (var i in noViableAltException.GetExpectedTokens().ToArray())
                    {
                        try
                        {
                            string tokenName = SyneryParser.DefaultVocabulary.GetDisplayName(i);

                            if (tokenName != null)
                                listOfExpectedTokens.Add(tokenName);
                        }
                        catch (Exception) { /* do nothing */ }
                    }

                    expectedTokens = String.Join(" or ", listOfExpectedTokens);
                }

                message = String.Format("Mismatched input '{0}'. Expected tokens {1}", offendingSymbol.Text, expectedTokens);
            }

            _ValidationResult.AddMessage(ValidationResultMessageCategoryEnum.Error, message, line, charPositionInLine, null);
        }

        /// <summary>
        /// Listens to some lexer errors
        /// </summary>
        /// <param name="recognizer"></param>
        /// <param name="offendingSymbol"></param>
        /// <param name="line"></param>
        /// <param name="charPositionInLine"></param>
        /// <param name="msg"></param>
        /// <param name="e"></param>
        public void SyntaxError(IRecognizer recognizer, int offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            string message = "";

            if (e.OffendingToken != null)
            {
                message = String.Format("Lexical error next to '{0}' detected.", e.OffendingToken.Text);
            }
            else
            {
                message = String.Format("Lexical error next to {0} detected.", SyneryParser.tokenNames[offendingSymbol]);
            }

            _ValidationResult.AddMessage(ValidationResultMessageCategoryEnum.Error, message, line, charPositionInLine, null);
        }

        #endregion
    }
}
