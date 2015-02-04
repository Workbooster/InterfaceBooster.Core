using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context
{
    /// <summary>
    /// Contains the value and the type of the value. These information is stored in two separate variables so the 
    /// type information doesn't get lost.
    /// </summary>
    public interface IValue
    {
        object Value { get; set; }
        SyneryType Type { get; }
    }
}
