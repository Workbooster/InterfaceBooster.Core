using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;

namespace InterfaceBooster.SyneryLanguage.Model.Context
{
    /// <summary>
    /// Contains the signature of a function.
    /// </summary>
    public class FunctionDefinition : IFunctionDefinition
    {
        public string Name { get; set; }
        public bool HasReturnValue
        {
            get
            {
                if (ReturnType == null) return false; 
                else return true;
            }
            set
            {
                if (value == false) ReturnType = null; // remove return type
            }
        }
        public SyneryType ReturnType { get; set; }
        public IList<IFunctionParameterDefinition> Parameters { get; set; }
    }
}
