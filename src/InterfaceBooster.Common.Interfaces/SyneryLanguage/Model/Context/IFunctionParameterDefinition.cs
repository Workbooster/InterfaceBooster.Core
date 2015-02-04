using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context
{
    /// <summary>
    /// Contains the signature of a function parameter.
    /// </summary>
    public interface IFunctionParameterDefinition
    {
        string Name { get; set; }
        SyneryType Type { get; set; }
        IValue DefaultValue { get; set; }
    }
}
