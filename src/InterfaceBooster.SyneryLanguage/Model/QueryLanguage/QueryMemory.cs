using InterfaceBooster.Database.Interfaces.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.QueryLanguage;

namespace InterfaceBooster.SyneryLanguage.Model.QueryLanguage
{
    public class QueryMemory
    {
        public ITable CurrentTable { get; set; }
        public ISchema NewSchema { get; set; }
        public ParameterExpression RowExpression { get; set; }
        public ParameterExpression IndexExpression { get; set; }
    }
}
