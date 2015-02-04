using InterfaceBooster.ProviderPluginApi;
using InterfaceBooster.ProviderPluginApi.Communication;
using InterfaceBooster.ProviderPluginApi.Data;
using InterfaceBooster.ProviderPluginApi.Service;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Tools.Data.Array;
using InterfaceBooster.Test.SyneryLanguage.Interpretation.QueryLanguage;

namespace InterfaceBooster.Test.SyneryLanguage.Interpretation.ProviderPlugins
{
    public abstract class ProviderPluginsTestBase : QueryLanguageTestBase
    {
        /// <summary>
        /// Loads the current RecordSet from \\Connections\DummyConnection\Tables\LAG\Articles
        /// This can be used to verify that modifications worked.
        /// </summary>
        /// <returns></returns>
        public RecordSet LoadArticlesRecordSetFromDummyPlugin()
        {
            // load the provider plugin connection
            string[] connectionPath = new string[] { "Connections", "DummyConnection" };
            IProviderConnection connection = _ProviderPluginManager.Connections[connectionPath];

            string[] endpointPath = new string[] { "Tables", "LAG" };
            IReadEndpoint articlesEndpoint = (from e in connection.Endpoints
                                              where e is IReadEndpoint
                                              && ArrayEqualityComparer.Equals(e.Path, endpointPath)
                                              && e.Name == "Articles"
                                              select (IReadEndpoint)e).FirstOrDefault();

            ReadResource resource = articlesEndpoint.GetReadResource();

            // create a mocked read request

            Mock<IReadRequest> mock = new Mock<IReadRequest>();
            mock.Setup(r => r.Resource).Returns(resource);
            mock.Setup(r => r.RequestedFields).Returns(resource.Schema.Fields);

            // request all articles with all fields

            ReadResponse response = articlesEndpoint.RunReadRequest(mock.Object);

            return response.RecordSet;
        }
    }
}
