using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Common.Interfaces.SyneryLanguage
{
    public interface IInterpreterFactory
    {
        void SetInterpreter(IInterpreter interpreter);

        IInterpreter<contextT> GetInterpreter<contextT>() where contextT : Antlr4.Runtime.ParserRuleContext;

        IInterpreter<contextT, resultT> GetInterpreter<contextT, resultT>() where contextT : Antlr4.Runtime.ParserRuleContext;

        IInterpreter<contextT, resultT, paramT> GetInterpreter<contextT, resultT, paramT>() where contextT : Antlr4.Runtime.ParserRuleContext;
    }
}
