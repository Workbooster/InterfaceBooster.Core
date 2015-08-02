using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.SyneryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.ProviderPlugin.Control.Filter;
using InterfaceBooster.Common.Interfaces.ErrorHandling;

namespace InterfaceBooster.SyneryLanguage.Interpretation.ProviderPlugins.Commands
{
    public class FilterCommandInterpreter : IInterpreter<SyneryParser.FilterCommandContext, IFilter>
    {
        #region PROPERTIES

        public ISyneryMemory Memory { get; set; }
        public IInterpretationController Controller { get; set; }

        #endregion

        #region PUBLIC METHODS

        public IFilter RunWithResult(SyneryParser.FilterCommandContext context)
        {
            return InterpretItem(context.filterCommandItem());
        }

        #endregion

        #region INTERNAL METHODS

        public IFilter InterpretItem(SyneryParser.FilterCommandItemContext context)
        {
            if (context.filterCommandItem() != null && context.filterCommandItem().Count == 1)
            {
                // it's a single item group
                // e.g. (myField == "bla")

                FilterGroup group = new FilterGroup();

                IFilter filter = InterpretItem(context.filterCommandItem()[0]);
                group.Filters.Add(filter);

                return group;
            }
            else if (context.filterCommandItem() != null && context.filterCommandItem().Count > 1)
            {
                // it's a group consisting of two conditions
                // e.g. (myField == "bla OR myField = "aha")

                FilterGroup group = new FilterGroup();

                if (context.OR() != null)
                    group.Operatror = FilterGroupOperatorEnum.Or;

                foreach (var item in context.filterCommandItem())
                {
                    IFilter filter = InterpretItem(item);
                    group.Filters.Add(filter);
                }

                return group;
            }
            else if (context.filterCommandConditionItem() != null)
            {
                // it's a condition

                return InterpretConditionItem(context.filterCommandConditionItem());
            }

            throw new SyneryInterpretationException(context, "Unknown filter command in FilterCommandInterpreter. No interpreter found for the given context.");   
        }

        public FilterComparison InterpretConditionItem(SyneryParser.FilterCommandConditionItemContext context)
        {
            FilterComparison filter = new FilterComparison();

            filter.FieldName = context.filterCommandListItemField().GetText();
            filter.Value = Controller.Interpret<SyneryParser.SingleValueContext, IValue>(context.filterCommandListItemExpression().singleValue());


            // equality expressions

            if (context.EQUAL() != null)
                filter.Operator = FilterComparisonOperatorEnum.Equal;
            if (context.NOTEQUAL() != null)
                filter.Operator = FilterComparisonOperatorEnum.NotEqual;

            if (context.EXACTLYEQUAL() != null)
                filter.Operator = FilterComparisonOperatorEnum.ExactlyEqual;
            if (context.EXACTLYNOTEQUAL() != null)
                filter.Operator = FilterComparisonOperatorEnum.ExactlyNotEqual;

            // comparison expressions

            if (context.GT() != null)
                filter.Operator = FilterComparisonOperatorEnum.GreaterThan;
            if (context.LT() != null)
                filter.Operator = FilterComparisonOperatorEnum.LessThan;
            if (context.GE() != null)
                filter.Operator = FilterComparisonOperatorEnum.GreaterThanOrEqual;
            if (context.LE() != null)
                filter.Operator = FilterComparisonOperatorEnum.LessThanOrEqual;

            return filter;
        }

        #endregion
    }
}
