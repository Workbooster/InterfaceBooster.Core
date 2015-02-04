using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Common.Interfaces.ErrorHandling
{
    [Serializable]
    public class SyneryQueryInterpretationException : SyneryException
    {
        public Antlr4.Runtime.Tree.IParseTree ParseTree { get; private set; }
        public int RecordIndex { get; set; }
        public object[] Record { get; private set; }

        public SyneryQueryInterpretationException(Antlr4.Runtime.ParserRuleContext tree, int recordIndex, object[] record, string message)
            : base(message)
        {
            ParseTree = tree;
            RecordIndex = recordIndex;
            Record = record;
        }
        public SyneryQueryInterpretationException(Antlr4.Runtime.ParserRuleContext tree, int recordIndex, object[] record, string message, Exception inner)
            : base(message, inner)
        {
            ParseTree = tree;
            RecordIndex = recordIndex;
            Record = record;
        }
        protected SyneryQueryInterpretationException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
