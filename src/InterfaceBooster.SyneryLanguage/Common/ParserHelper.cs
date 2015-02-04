using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.SyneryLanguage.Common
{
    public static class ParserHelper
    {
        /// <summary>
        /// creates an AST for the the given code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static SyneryParser.ProgramContext GetProgramAstFromCode(string code, IAntlrErrorListener<int> lexerErrorListener = null, IAntlrErrorListener<IToken> parserErrorListener = null)
        {
            // create a token stream from the lexer to start processing the code

            AntlrInputStream stream = new AntlrInputStream(code);

            SyneryLexer lexer = new SyneryLexer(stream);

            if (lexerErrorListener != null)
                lexer.AddErrorListener(lexerErrorListener);

            CommonTokenStream tokenStream = new CommonTokenStream(lexer);

            SyneryParser parser = new SyneryParser(tokenStream);

            if (parserErrorListener != null)
                parser.AddErrorListener(parserErrorListener);

            return parser.program();
        }

    }
}
