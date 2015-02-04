using InterfaceBooster.ProviderPluginApi.Communication;
using InterfaceBooster.ProviderPluginApi.Data;
using InterfaceBooster.ProviderPluginApi.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Core.ProviderPlugins.Communication
{
    public class SaveRequest : Request, ISaveRequest
    {
        #region PROPERTIES

        public IEnumerable<Answer> Answers { get; set; }
        public RecordSet RecordSet { get; set; }
        new public SaveResource Resource { get; set; }

        #endregion

        #region PUBLIC METHODS

        public SaveRequest() : base(RequestTypeEnum.Save) { }

        #endregion
    }
}
