﻿using InterfaceBooster.Common.Interfaces.ProviderPlugin.Control.Filter;
using InterfaceBooster.Database.Interfaces.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Common.Interfaces.ProviderPlugin.Control
{
    public class ProviderPluginUpdateTask : ProviderPluginDataExchangeTask
    {
        #region PROPERTIES

        /// <summary>
        /// Gets or sets the parameters sent with the request to the provider plugin.
        /// Key = path and name of the parameter / Value = the parameter value
        /// </summary>
        public IDictionary<string[], object> Parameters { get; set; }

        /// <summary>
        /// Gets or sets the name of the table where the data that are sent with the request are comming from.
        /// </summary>
        public string SourceTableName { get; set; }

        /// <summary>
        /// Gets or sets the filter definition sent with the request to the provider plugin.
        /// </summary>
        public IFilter Filter { get; set; }

        #endregion

        #region PUBLIC METHODS

        public ProviderPluginUpdateTask()
            : base(ProviderPluginTaskTypeEnum.Update)
        {
            Parameters = new Dictionary<string[], object>();
        }

        #endregion
    }
}
