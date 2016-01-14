using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Common.Interfaces.ErrorHandling
{
    [Serializable]
    public class SyneryQueryInterpretationException : SyneryInterpretationException
    {
        public int RecordIndex { get; set; }
        public object[] Record { get; private set; }

        public SyneryQueryInterpretationException(Antlr4.Runtime.ParserRuleContext tree, int recordIndex, object[] record, string message)
            : base(tree, message)
        {
            ParseTree = tree;
            RecordIndex = recordIndex;
            Record = record;
        }

        public SyneryQueryInterpretationException(Antlr4.Runtime.ParserRuleContext tree, int recordIndex, object[] record, string message, Exception inner)
            : base(tree, message, inner)
        {
            ParseTree = tree;
            RecordIndex = recordIndex;
            Record = record;
        }

        protected SyneryQueryInterpretationException(
            Antlr4.Runtime.ParserRuleContext tree,
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(tree, info, context)
        {
            ParseTree = tree;
        }
    }
}
