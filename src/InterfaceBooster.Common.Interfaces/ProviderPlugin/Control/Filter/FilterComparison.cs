using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InterfaceBooster.Common.Interfaces.ProviderPlugin.Control.Filter
{
    public class FilterComparison : IFilter
    {
        public string FieldName;
        public IValue Value;
        public FilterComparisonOperatorEnum Operator;

        public FilterComparison() {}

        public FilterComparison(string fieldName, FilterComparisonOperatorEnum op, IValue value)
        {
            FieldName = fieldName;
            Operator = op;
            Value = value;
        }
    }
}
