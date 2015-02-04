using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.ErrorHandling;

namespace InterfaceBooster.Common.Interfaces.ErrorHandling
{
    [Serializable]
    public class SyneryInterpretationException : SyneryException
    {
        public Antlr4.Runtime.Tree.IParseTree ParseTree { get; private set; }

        public SyneryInterpretationException(Antlr4.Runtime.ParserRuleContext tree, string message)
            : base(message)
        {
            ParseTree = tree;
        }
        public SyneryInterpretationException(Antlr4.Runtime.ParserRuleContext tree, string message, Exception inner)
            : base(message, inner)
        {
            ParseTree = tree;
        }
        protected SyneryInterpretationException(Antlr4.Runtime.ParserRuleContext tree,
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
            ParseTree = tree;
        }
    }
}
