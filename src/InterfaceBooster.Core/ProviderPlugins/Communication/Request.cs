using InterfaceBooster.ProviderPluginApi.Communication;
using InterfaceBooster.ProviderPluginApi.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Core.ProviderPlugins.Communication
{
    public abstract class Request : IRequest
    {
        #region PROPERTIES

        public Resource Resource { get; set; }
        public RequestTypeEnum RequestType { get; set; }
        public IList<IRequest> SubRequests { get; set; }

        #endregion

        #region PUBLIC METHODS

        public Request(RequestTypeEnum type)
        {
            RequestType = type;
            SubRequests = new List<IRequest>();
        }

        #endregion
    }
}
