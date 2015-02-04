using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context
{
    /// <summary>
    /// Contains the signature of a function.
    /// </summary>
    public interface IFunctionDefinition
    {
        string Name { get; set; }
        bool HasReturnValue { get; set; }
        SyneryType ReturnType { get; set; }
        IList<IFunctionParameterDefinition> Parameters { get; set; }
    }
}
