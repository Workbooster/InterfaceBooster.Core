using Ctrl = InterfaceBooster.Common.Interfaces.ProviderPlugin.Control.Filter;
using InterfaceBooster.ProviderPluginApi.Data.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.ErrorHandling;

namespace InterfaceBooster.Core.ProviderPlugins
{
    /// <summary>
    /// Builds a Provider Plugin filter from the filter commands.
    /// </summary>
    public class FilterBuilder
    {
        #region PROPERTIES

        public IEnumerable<FilterDefinition> AvailableDefinitions { get; private set; }

        #endregion

        #region PUBLIC METHODS

        public FilterBuilder(IEnumerable<FilterDefinition> availableFilterDefinitions)
        {
            AvailableDefinitions = availableFilterDefinitions;
        }

        public Filter ConvertToProviderPluginFilter(Ctrl.IFilter filter)
        {
            if (AvailableDefinitions == null
                || AvailableDefinitions.Count() == 0
                || filter == null)
                return null;

            if (filter is Ctrl.FilterGroup)
            {
                return Convert((Ctrl.FilterGroup)filter);
            }
            else if (filter is Ctrl.FilterComparison)
            {
                return Convert((Ctrl.FilterComparison)filter);
            }

            throw new InterfaceBoosterCoreException(
                String.Format("Unknown filter type: '{0}'", filter.GetType().Name));
        }

        #endregion

        #region INTERNAL METHODS

        private Filter Convert(Ctrl.FilterGroup group)
        {
            FilterGroup newGroup;

            if (group.Operatror == Ctrl.FilterGroupOperatorEnum.And)
            {
                newGroup = new AndFilterGroup();
            }
            else
            {
                newGroup = new OrFilterGroup();
            }

            foreach (var filter in group.Filters)
            {
                newGroup.FilterItems.Add(ConvertToProviderPluginFilter(filter));
            }

            return newGroup;
        }

        private Filter Convert(Ctrl.FilterComparison filter)
        {
            FilterTypeEnum filterType;
            FilterDefinition definition;
            object value = filter.Value.Value;

            definition = AvailableDefinitions.Where(d => d.Name == filter.FieldName).FirstOrDefault();

            if (definition == null)
                throw new InterfaceBoosterCoreException(
                    String.Format("No filter field with the name '{0}' is available.", filter.FieldName));

            filterType = GetFilterType(filter.Operator);

            return new SingleValueFilterCondition(filterType, definition, value);
        }

        private FilterTypeEnum GetFilterType(Ctrl.FilterComparisonOperatorEnum type)
        {
            switch (type)
            {
                case InterfaceBooster.Common.Interfaces.ProviderPlugin.Control.Filter.FilterComparisonOperatorEnum.Equal:
                    return FilterTypeEnum.Equal;
                case InterfaceBooster.Common.Interfaces.ProviderPlugin.Control.Filter.FilterComparisonOperatorEnum.NotEqual:
                    return FilterTypeEnum.NotEqual;
                case InterfaceBooster.Common.Interfaces.ProviderPlugin.Control.Filter.FilterComparisonOperatorEnum.ExactlyEqual:
                    return FilterTypeEnum.ExactlyEqual;
                case InterfaceBooster.Common.Interfaces.ProviderPlugin.Control.Filter.FilterComparisonOperatorEnum.ExactlyNotEqual:
                    return FilterTypeEnum.ExactlyNotEqual;
                case InterfaceBooster.Common.Interfaces.ProviderPlugin.Control.Filter.FilterComparisonOperatorEnum.GreaterThan:
                    return FilterTypeEnum.GreaterThan;
                case InterfaceBooster.Common.Interfaces.ProviderPlugin.Control.Filter.FilterComparisonOperatorEnum.GreaterThanOrEqual:
                    return FilterTypeEnum.GreaterThanOrEqual;
                case InterfaceBooster.Common.Interfaces.ProviderPlugin.Control.Filter.FilterComparisonOperatorEnum.LessThan:
                    return FilterTypeEnum.LessThan;
                case InterfaceBooster.Common.Interfaces.ProviderPlugin.Control.Filter.FilterComparisonOperatorEnum.LessThanOrEqual:
                    return FilterTypeEnum.LessThanOrEqual;
                case InterfaceBooster.Common.Interfaces.ProviderPlugin.Control.Filter.FilterComparisonOperatorEnum.Like:
                    return FilterTypeEnum.Like;
            }

            throw new InterfaceBoosterCoreException(
                String.Format("The filter type '{0}' is not implemented.", Enum.GetName(typeof(Ctrl.FilterComparisonOperatorEnum), type)));
        }

        #endregion
    }
}
