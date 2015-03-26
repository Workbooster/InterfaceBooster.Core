using InterfaceBooster.ProviderPluginApi.Communication;
using InterfaceBooster.ProviderPluginApi.Data;
using InterfaceBooster.ProviderPluginApi.Data.Filter;
using InterfaceBooster.ProviderPluginApi.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Core.ProviderPlugins.Communication
{
    public class ExecuteRequest : Request, IExecuteRequest
    {
        #region PROPERTIES

        public AnswerList Answers { get; set; }
        public IEnumerable<string> RequestedValues { get; set; }
        new public ExecuteResource Resource { get; set; }

        #endregion

        #region PUBLIC METHODS

        public ExecuteRequest() : base(RequestTypeEnum.Execute) { }

        #endregion
    }
}
