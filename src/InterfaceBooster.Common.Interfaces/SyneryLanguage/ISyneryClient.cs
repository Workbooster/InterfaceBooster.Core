using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;

namespace InterfaceBooster.Common.Interfaces.SyneryLanguage
{
    public interface ISyneryClient<resultT>
    {
        IInterpretationController Controller { get; }
        ISyneryMemory Memory { get; set; }
        resultT Run(string code, IDictionary<string, string> includeFiles = null);
        void Cancel();
    }
}
