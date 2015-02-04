using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Common.Interfaces.ProviderPlugin.Control
{
    public abstract class ProviderPluginDataExchangeTask : ProviderPluginTask
    {
        #region PROPERTIES

        /// <summary>
        /// Gets or sets the full path consisting of the connection identifier, the provider plugin endpoint path and the endpoint name.
        /// 
        /// Example (full Synery path):
        ///     \\someCategory\myConnection\firstSubPath\secondSubPath\endpointName
        /// 
        /// Example (C# string array): 
        ///     FullPath = new string[] { "someCategory", "myConnection", "fistSubPath", "secondSubPath", "endpointName" };
        /// </summary>
        public string[] FullPath { get; set; }

        /// <summary>
        /// Gets or sets the full path as pure text consisting of the connection identifier, the provider plugin endpoint path and the endpoint name.
        /// This value is used to generate error messages if something went wrong with the given path.
        /// 
        /// Example (full Synery path):
        ///     \\someCategory\myConnection\firstSubPath\secondSubPath\endpointName
        /// 
        /// Example (C# string array): 
        ///     FullPath = new string[] { "someCategory", "myConnection", "fistSubPath", "secondSubPath", "endpointName" };
        /// </summary>
        public string FullSyneryPath { get; set; }

        /// <summary>
        /// Gets or sets the path of the provider plugin connection that is part of the <see cref="FullPath"/>.
        /// 
        /// Example (full Synery path):
        ///     \\someCategory\myConnection\firstSubPath\secondSubPath\endpointName
        /// 
        /// Example (C# string array): 
        ///     ConnectionPath = new string[] { "someCategory", "myConnection" };
        /// </summary>
        public string[] ConnectionPath { get; set; }

        /// <summary>
        /// Gets or sets the path of the provider plugin endpoint that is part of the <see cref="FullPath"/>.
        /// 
        /// Example (full Synery path):
        ///     \\someCategory\myConnection\firstSubPath\secondSubPath\endpointName
        /// 
        /// Example (C# string array): 
        ///     EndpointPath = new string[] { "fistSubPath", "secondSubPath" };
        /// </summary>
        public string[] EndpointPath { get; set; }

        /// <summary>
        /// Gets or sets the name of the provider plugin endpoint that is part of the <see cref="FullPath"/>.
        /// 
        /// Example (full Synery path):
        ///     \\someCategory\myConnection\firstSubPath\secondSubPath\endpointName
        /// 
        /// Example (C# string): 
        ///     EndpointName = "endpointName";
        /// </summary>
        public string EndpointName { get; set; }

        /// <summary>
        /// Gets or sets the name of the resource. This is used for addressing SubResources.
        /// </summary>
        public string ResourceName { get; set; }

        #endregion

        #region PUBLIC METHODS

        public ProviderPluginDataExchangeTask(ProviderPluginTaskTypeEnum type) : base(type) { }

        #endregion
    }
}
