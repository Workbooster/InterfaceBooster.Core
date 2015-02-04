using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.QueryLanguage;

namespace InterfaceBooster.SyneryLanguage.Model.QueryLanguage
{
    public class ExpressionValue : IExpressionValue
    {
        public Expression Expression { get; set; }
        public SyneryType ResultType { get; set; }

        public ExpressionValue(Expression expression, SyneryType resultType)
        {
            Expression = expression;
            ResultType = resultType;
        }
    }
}
