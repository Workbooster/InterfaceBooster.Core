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

namespace InterfaceBooster.Test.SyneryLanguage.Interpretation.ProviderPlugins.ProviderPluginSaveStatementInterpreter_Test
{
    public class SAVE_Statement_Works : ProviderPluginsTestBase
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
                new object[] { "SaveTest01", "SaveTest01-Name1", 15.2M, "PCS", "PCS"  },
                new object[] { "SaveTest02", "SaveTest02-Name1", 17.95M, "PCS", "PCS"  },
                new object[] { "SaveTest03", "SaveTest03-Name1", 9.90M, "PCS", "PCS"  },
            };

            ITable table = _Database.NewTable(schema, data);

            _Database.CreateTable(@"\ExportTests\Articles", table);
        }

        [Test]
        public void Basic_SAVE_Statement_Works()
        {
            string code = @"
SAVE \\Connections\DummyConnection\Tables\LAG\Articles 
    FROM \ExportTests\Articles
END";

            _SyneryClient.Run(code);

            // load the articles record set after the update of the changed articles
            RecordSet articlesRecordSetAfterExecutionOfSyneryCode = LoadArticlesRecordSetFromDummyPlugin();

            // create a query function that looks for the first record created in the SAVE statement

            Func<Record, bool> firstRecordQuery = (Record r) =>
            {
                return (string)r["ArticleNumber"] == "SaveTest01"
                    && (string)r["Name1"] == "SaveTest01-Name1"
                    && (decimal)r["Price1"] == 15.2M
                    && (string)r["UnitInternal"] == "PCS"
                    && (string)r["UnitInvoice"] == "PCS";
            };

            // check wheter one record matching the query conditions exists

            Assert.AreEqual(1, articlesRecordSetAfterExecutionOfSyneryCode.Count(firstRecordQuery));
        }
    }
}
