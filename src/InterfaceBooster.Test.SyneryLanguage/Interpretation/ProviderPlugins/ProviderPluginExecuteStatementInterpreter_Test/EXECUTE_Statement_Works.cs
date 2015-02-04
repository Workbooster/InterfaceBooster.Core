using InterfaceBooster.Database.Interfaces.Structure;
using InterfaceBooster.ProviderPluginApi;
using InterfaceBooster.ProviderPluginApi.Communication;
using InterfaceBooster.ProviderPluginApi.Service;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Tools.Data.Array;
using InterfaceBooster.Test.SyneryLanguage.Interpretation.QueryLanguage;

namespace InterfaceBooster.Test.SyneryLanguage.Interpretation.ProviderPlugins.ProviderPluginExecuteStatementInterpreter_Test
{
    [TestFixture]
    public class EXECUTE_Statement_Works : QueryLanguageTestBase
    {
        [SetUp]
        public void SetupTestWithDummyPlugin()
        {
            SetupDummyPlugin();
            SetupProviderPluginDummyConnection();
        }

        [Test]
        public void Basic_READ_Statement_Works()
        {
            string code = @"
INT nextNumber = 999;
EXECUTE \\Connections\DummyConnection\Tables\LAG\Manufacturers
    GET (NextManufacturerNumber AS nextNumber)
END";

            _SyneryClient.Run(code);

            // resolve the synery variable "nextNumber"
            int testNextNumber = (int)_SyneryMemory.CurrentScope.ResolveVariable("nextNumber").Value;

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

            // calculate the expected number from adding 1 to the highest manufacturer number
            int expectedNextNumber = response.RecordSet.Max(r => (int)r["ManufacturerNumber"]) + 1;

            Assert.AreEqual(expectedNextNumber, testNextNumber);
        }
    }
}
