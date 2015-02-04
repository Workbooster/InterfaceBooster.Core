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

namespace InterfaceBooster.Test.SyneryLanguage.Interpretation.ProviderPlugins.ProviderPluginDeleteStatementInterpreter_Test
{
    [TestFixture]
    public class DELETE_Statement_Works : ProviderPluginsTestBase
    {
        [SetUp]
        public void SetupTestWithDummyPlugin()
        {
            SetupDummyPlugin();
            SetupProviderPluginDummyConnection();
        }

        [Test]
        public void Basic_DELETE_Statement_Works()
        {
            // setup article table that contains the articles that should be deleted

            ISchema schema = _Database.NewSchema();
            schema.AddField<string>("ArticleNumber");

            List<object[]> data = new List<object[]>
            {
                new object[] { "Test01", },
                new object[] { "Test03", },
            };

            ITable table = _Database.NewTable(schema, data);

            // load the record set from before the deletion
            RecordSet articlesRecordSetBeforeDeletion = LoadArticlesRecordSetFromDummyPlugin();

            // run synery command

            _Database.CreateTable(@"\ExportTests\Articles", table);
            string code = @"
DELETE \\Connections\DummyConnection\Tables\LAG\Articles 
    FROM \ExportTests\Articles
END";

            _SyneryClient.Run(code);

            // load the record set after the deletion
            RecordSet articlesRecordSetAfterDeletion = LoadArticlesRecordSetFromDummyPlugin();

            // check wheter the articles from the table \ExportTests\Articles have been deleted

            Assert.AreEqual(articlesRecordSetBeforeDeletion.Count - data.Count, articlesRecordSetAfterDeletion.Count);
        }
    }
}
