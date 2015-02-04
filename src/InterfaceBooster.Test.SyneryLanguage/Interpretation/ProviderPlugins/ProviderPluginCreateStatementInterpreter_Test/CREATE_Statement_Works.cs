using InterfaceBooster.Database.Interfaces.Structure;
using InterfaceBooster.ProviderPluginApi;
using InterfaceBooster.ProviderPluginApi.Communication;
using InterfaceBooster.ProviderPluginApi.Data;
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

namespace InterfaceBooster.Test.SyneryLanguage.Interpretation.ProviderPlugins.ProviderPluginCreateStatementInterpreter_Test
{
    [TestFixture]
    public class CREATE_Statement_Works : ProviderPluginsTestBase
    {
        [SetUp]
        public void SetupTestWithDummyPlugin()
        {
            SetupDummyPlugin();
            SetupProviderPluginDummyConnection();

            // setup article table for export

            ISchema schema = _Database.NewSchema();
            schema.AddField<string>("ArticleNumber");
            schema.AddField<string>("Name1");
            schema.AddField<decimal>("Price1");
            schema.AddField<string>("UnitInternal");
            schema.AddField<string>("UnitInvoice");

            List<object[]> data = new List<object[]>
            {
                new object[] { "CreateTest01", "CreateTest01-Name1", 15.2M, "PCS", "PCS"  },
                new object[] { "CreateTest02", "CreateTest02-Name1", 17.95M, "PCS", "PCS"  },
                new object[] { "CreateTest03", "CreateTest03-Name1", 9.90M, "PCS", "PCS"  },
            };

            ITable table = _Database.NewTable(schema, data);

            _Database.CreateTable(@"\ExportTests\Articles", table);
        }

        [Test]
        public void Basic_CREATE_Statement_Works()
        {
            string code = @"
CREATE \\Connections\DummyConnection\Tables\LAG\Articles 
    FROM \ExportTests\Articles
END";

            _SyneryClient.Run(code);

            // load the articles record set after the creation of the new articles
            RecordSet articlesRecordSetAfterExecutionOfSyneryCode = LoadArticlesRecordSetFromDummyPlugin();

            // create a query function that looks for the first record created in the CREATE statement

            Func<Record, bool> firstRecordQuery = (Record r) =>
            {
                return (string)r["ArticleNumber"] == "CreateTest01"
                    && (string)r["Name1"] == "CreateTest01-Name1"
                    && (decimal)r["Price1"] == 15.2M
                    && (string)r["UnitInternal"] == "PCS"
                    && (string)r["UnitInvoice"] == "PCS";
            };

            // check wheter one record matching the query conditions exists

            Assert.AreEqual(1, articlesRecordSetAfterExecutionOfSyneryCode.Count(firstRecordQuery));
        }
    }
}
