using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;

namespace InterfaceBooster.Core.Common.Model
{
    /// <summary>
    /// Contains the value and the type of the value. These information is stored in two separate variables so the 
    /// type information doesn't get lost. This class is used by some Synery Interpreters.
    /// </summary>
    public class TypedValue : IValue
    {
        public object Value { get; set; }
        public SyneryType Type { get; private set; }

        public TypedValue(SyneryType syneryType, object value = null)
        {
            Value = value;
            Type = syneryType;
        }
    }
}
