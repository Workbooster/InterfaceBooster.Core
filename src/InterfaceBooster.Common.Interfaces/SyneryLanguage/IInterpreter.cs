using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;

namespace InterfaceBooster.Common.Interfaces.SyneryLanguage
{
    /// <summary>
    /// All interpreters that interpret synery code must implement this interface.
    /// </summary>
    public interface IInterpreter
    {
        ISyneryMemory Memory { get; set; }
        IInterpretationController Controller { get; set; }
    }

    public interface IInterpreter<contextT> : IInterpreter
    {
        void Run(contextT context);
    }

    public interface IInterpreter<contextT, resultT> : IInterpreter
    {
        resultT RunWithResult(contextT context);
    }

    public interface IInterpreter<contextT, resultT, paramT> : IInterpreter
    {
        resultT RunWithResult(contextT context, paramT parameter);
    }
}
