using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;

namespace InterfaceBooster.Common.Interfaces.SyneryLanguage.QueryLanguage
{
    public interface IExpressionValue
    {
        Expression Expression { get; set; }
        SyneryType ResultType { get; set; }
    }
}
