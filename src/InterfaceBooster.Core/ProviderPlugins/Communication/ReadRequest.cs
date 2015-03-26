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
    public class ReadRequest : Request, IReadRequest
    {
        #region PROPERTIES

        public AnswerList Answers { get; set; }
        public Filter Filters { get; set; }
        public IEnumerable<Field> RequestedFields { get; set; }
        public new ReadResource Resource { get { return base.Resource as ReadResource; } set { base.Resource = value; } }

        #endregion

        #region PUBLIC METHODS

        public ReadRequest() : base(RequestTypeEnum.Read) { }

        #endregion
    }
}
