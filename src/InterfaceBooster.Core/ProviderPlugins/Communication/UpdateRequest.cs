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
    public class UpdateRequest : Request, IUpdateRequest
    {
        #region PROPERTIES

        public AnswerList Answers { get; set; }
        public Filter Filters { get; set; }
        public RecordSet RecordSet { get; set; }
        new public UpdateResource Resource { get; set; }

        #endregion

        #region PUBLIC METHODS

        public UpdateRequest() : base(RequestTypeEnum.Update) { }

        #endregion
    }
}
