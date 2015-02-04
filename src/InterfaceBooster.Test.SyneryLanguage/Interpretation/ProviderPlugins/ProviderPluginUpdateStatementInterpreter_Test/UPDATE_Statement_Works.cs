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

namespace InterfaceBooster.Test.SyneryLanguage.Interpretation.ProviderPlugins.ProviderPluginUpdateStatementInterpreter_Test
{
    [TestFixture]
    public class UPDATE_Statement_Works : ProviderPluginsTestBase
    {

        [SetUp]
        public void SetupTestWithDummyPlugin()
        {
            SetupDummyPlugin();
            SetupProviderPluginDummyConnection();
        }

        [Test]
        public void UPDATE_Statement_From_Table_With_Different_Structure_Works()
        {
            // setup article table with the same structure for export

            ISchema schema = _Database.NewSchema();
            schema.AddField<string>("ArticleNumber");
            schema.AddField<string>("Name1");
            schema.AddField<decimal>("Price1");
            schema.AddField<string>("UnitInternal");
            schema.AddField<string>("UnitInvoice");

            List<object[]> data = new List<object[]>
            {
                new object[] { "Test01", "UpdateTest01-Name1", 1.1M, "PCS", "PCS"  },
                new object[] { "Test03", "UpdateTest03-Name1", 3.1M, "PCS", "PCS"  },
            };

            ITable table = _Database.NewTable(schema, data);

            _Database.CreateTable(@"\ExportTests\Articles", table);

            // run synery command

            string code = @"
UPDATE \\Connections\DummyConnection\Tables\LAG\Articles 
    FROM \ExportTests\Articles
END";

            _SyneryClient.Run(code);

            // check result

            // load the articles record set after the update of the changed articles
            RecordSet articlesRecordSetAfterExecutionOfSyneryCode = LoadArticlesRecordSetFromDummyPlugin();

            // create a query function that looks for the first record updated in the UPDATE statement

            Func<Record, bool> firstRecordQuery = (Record r) =>
            {
                return (string)r["ArticleNumber"] == "Test01"
                    && (string)r["Name1"] == "UpdateTest01-Name1"
                    && (decimal)r["Price1"] == 1.1M
                    && (string)r["UnitInternal"] == "PCS"
                    && (string)r["UnitInvoice"] == "PCS";
            };

            // create a query function that looks for the second record that is expected to be untouched

            Func<Record, bool> secondRecordQuery = (Record r) =>
            {
                return (string)r["ArticleNumber"] == "Test02"
                    && (string)r["Name1"] == "Test02-Name1"
                    && (decimal)r["Price1"] == 2M
                    && (string)r["UnitInternal"] == "PCS"
                    && (string)r["UnitInvoice"] == "PCS";
            };

            // create a query function that looks for the third record updated in the UPDATE statement

            Func<Record, bool> thirdRecordQuery = (Record r) =>
            {
                return (string)r["ArticleNumber"] == "Test03"
                    && (string)r["Name1"] == "UpdateTest03-Name1"
                    && (decimal)r["Price1"] == 3.1M
                    && (string)r["UnitInternal"] == "PCS"
                    && (string)r["UnitInvoice"] == "PCS";
            };

            // check wheter one record exists that matches the query conditions

            Assert.AreEqual(1, articlesRecordSetAfterExecutionOfSyneryCode.Count(firstRecordQuery));
            Assert.AreEqual(1, articlesRecordSetAfterExecutionOfSyneryCode.Count(secondRecordQuery));
            Assert.AreEqual(1, articlesRecordSetAfterExecutionOfSyneryCode.Count(thirdRecordQuery));
        }
    }
}
