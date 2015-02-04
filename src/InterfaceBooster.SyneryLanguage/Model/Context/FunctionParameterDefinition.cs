using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;

namespace InterfaceBooster.SyneryLanguage.Model.Context
{
    /// <summary>
    /// Contains the signature of a function parameter.
    /// </summary>
    public class FunctionParameterDefinition : IFunctionParameterDefinition
    {
        public string Name { get; set; }
        public SyneryType Type { get; set; }
        public IValue DefaultValue { get; set; }
    }
}
