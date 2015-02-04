using InterfaceBooster.Database.Interfaces.Structure;
using InterfaceBooster.ProviderPluginApi;
using InterfaceBooster.ProviderPluginApi.Service;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.ErrorHandling;
using InterfaceBooster.Common.Tools.Data.Array;
using InterfaceBooster.Test.SyneryLanguage.Interpretation.QueryLanguage;

namespace InterfaceBooster.Test.SyneryLanguage.Interpretation.ProviderPlugins.ProviderPluginReadStatementInterpreter_Test
{
    [TestFixture]
    public class READ_Statement_Works : QueryLanguageTestBase
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
READ \\Connections\DummyConnection\Tables\LAG\Articles 
    TO \imported\Articles
END";

            _SyneryClient.Run(code);

            // load the imported table
            ITable table = _Database.LoadTable(@"\imported\Articles");

            // load the provider plugin connection
            string[] connectionPath = new string[] { "Connections", "DummyConnection" };
            IProviderConnection connection = _ProviderPluginManager.Connections[connectionPath];

            string[] endpointPath = new string[] { "Tables", "LAG" };
            IReadEndpoint articlesEndpoint = (from e in connection.Endpoints
                                              where e is IReadEndpoint
                                              && ArrayEqualityComparer.Equals(e.Path, endpointPath)
                                              && e.Name == "Articles"
                                              select (IReadEndpoint)e).FirstOrDefault();

            // check whether the table is available
            Assert.IsInstanceOf<ITable>(table);

            // check whether the number of fiels is equal in the endpoint's schema and in the imported table's schema
            Assert.AreEqual(articlesEndpoint.GetReadResource().Schema.Fields.Count, table.Schema.Fields.Count);
        }

        [Test]
        public void READ_Statement_With_FIELDS_Works()
        {
            string code = @"
READ \\Connections\DummyConnection\Tables\LAG\Articles 
    TO \imported\Articles
    FIELDS (ArticleNumber, Name1, Price1)
END";

            _SyneryClient.Run(code);

            // load the imported table
            ITable table = _Database.LoadTable(@"\imported\Articles");

            // check whether the table is available
            Assert.IsInstanceOf<ITable>(table);

            // check whether the number of fiels in the imported table is equal to the requested fields
            Assert.AreEqual(3, table.Schema.Fields.Count);

            // check whether all selected fields are available
            Assert.IsTrue(table.Schema.Fields.Count(f => f.Name == "ArticleNumber") == 1);
            Assert.IsTrue(table.Schema.Fields.Count(f => f.Name == "Name1") == 1);
            Assert.IsTrue(table.Schema.Fields.Count(f => f.Name == "Price1") == 1);
        }

        [Test]
        public void READ_Statement_With_FILTER_Works()
        {
            string code = @"
READ \\Connections\DummyConnection\Tables\LAG\Articles 
    TO \imported\Articles
    FILTER (ArticleNumber == ""Test02"")
END";

            _SyneryClient.Run(code);

            // load the imported table
            ITable table = _Database.LoadTable(@"\imported\Articles");

            // check whether the table is available
            Assert.IsInstanceOf<ITable>(table);

            // check whether the number of fiels in the imported table is equal to the requested fields
            Assert.AreEqual(1, table.Count);
        }

        [Test]
        public void Nested_READ_Statement_Works()
        {
            string code = @"
READ \\Connections\DummyConnection\Tables\LAG\Articles 
    TO \imported\Articles
    READ Manufacturer 
        TO \imported\Manufacturer 
    END
END";

            _SyneryClient.Run(code);

            // load the imported tables
            ITable articleTable = _Database.LoadTable(@"\imported\Articles");
            ITable manufacturerTable = _Database.LoadTable(@"\imported\Manufacturer");

            // check whether the tables are available
            Assert.IsInstanceOf<ITable>(articleTable);
            Assert.IsInstanceOf<ITable>(manufacturerTable);
        }

        [Test]
        public void READ_Statement_To_Unknown_Endpoint_Throws_Exception()
        {
            string code = @"
READ \\Connections\DummyConnection\Tables\LAG\Unknown 
    TO \imported\Uknown
END";

            Exception ex = Assert.Throws<ProviderPluginManagerException>(delegate { _SyneryClient.Run(code); });
            
            // check whether the exception message contains the endpoint path so it is possible to identifiy the error in synery code
            Assert.IsTrue(ex.Message.Contains(@"\\Connections\DummyConnection\Tables\LAG\Unknown"));
        }

    }
}
