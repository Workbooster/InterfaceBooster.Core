using InterfaceBooster.ProviderPluginApi;
using InterfaceBooster.ProviderPluginApi.Data;
using InterfaceBooster.ProviderPluginApi.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Test.Dummy.ProviderPluginDummy.V1.Endpoints;

namespace InterfaceBooster.Test.Dummy.ProviderPluginDummy.V1
{
    public class DummyProviderConnection : IProviderConnection
    {
        #region PROPERTIES

        public IEnumerable<IEndpoint> Endpoints { get; private set; }
        public ConnectionSettings Settings { get; private set; }

        #endregion

        #region PUBLIC METHODS

        public DummyProviderConnection(ConnectionSettings settings)
        {
            Settings = settings;
            Endpoints = CreateEndpoints();
        }

        #endregion

        #region INTERNAL METHODS

        private IEnumerable<IEndpoint> CreateEndpoints()
        {
            List<IEndpoint> listOfEndpoints = new List<IEndpoint>();

            DummyData data = new DummyData();

            listOfEndpoints.Add(new ArticleEndpoint(data));
            listOfEndpoints.Add(new ManufacturerEndpoint(data));
            listOfEndpoints.Add(new WebShopEndpoint(data));

            return listOfEndpoints;
        }

        #endregion

    }
}
