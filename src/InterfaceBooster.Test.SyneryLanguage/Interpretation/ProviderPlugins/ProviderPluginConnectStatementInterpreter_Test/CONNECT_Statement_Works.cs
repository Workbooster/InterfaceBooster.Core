﻿using InterfaceBooster.ProviderPluginApi;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.ErrorHandling;
using InterfaceBooster.Common.Interfaces.InterfaceDefinition.Data;
using InterfaceBooster.Common.Tools.Data.Array;
using InterfaceBooster.Test.SyneryLanguage.Interpretation.QueryLanguage;

namespace InterfaceBooster.Test.SyneryLanguage.Interpretation.ProviderPlugins.ProviderPluginConnectStatementInterpreter_Test
{
    [TestFixture]
    public class CONNECT_Statement_Works : QueryLanguageTestBase
    {
        [SetUp]
        public void SetupTestWithDummyPlugin()
        {
            SetupDummyPlugin();
        }

        [Test]
        public void Create_Connection_Without_Required_Parameters_Throws_Exception()
        {
            string code = @"CONNECT ""Dummy"" AS \\Connections\DummyConnection END";

            Assert.Throws<SyneryInterpretationException>(delegate { _SyneryClient.Run(code); });
        }

        [Test]
        public void Using_Unknown_Plugin_Throws_ProviderPluginManagerException()
        {
            string code = @"CONNECT ""Unknown Plugin"" AS \\Connections\DummyConnection END";

            Assert.Throws<SyneryInterpretationException>(delegate { _SyneryClient.Run(code); });
        }

        [Test]
        public void Create_Connection_With_Valid_Parameters_Works()
        {
            string code = @"
CONNECT ""Dummy"" 
    AS \\Connections\DummyConnection
    SET (
        Database.Connection.Server = ""Testserver"",
        Database.Connection.Database = ""TestDb"",
        Database.Connection.User = ""TestUser"",
        Database.Connection.Password = ""TestPassword"",
        Proffix.Tables.ShowAdditionalTables = TRUE,
        Proffix.Tables.ShowSystemTables = TRUE
    ) 
END
";

            _SyneryClient.Run(code);

            string[] connectionPath = new string[] { "Connections", "DummyConnection" };

            IProviderConnection connection = _ProviderPluginManager.Connections[connectionPath];

            var serverValue = (from a in connection.Settings.Answers
                               where ArrayEqualityComparer.Equals<string>(a.Question.Path, new string[] { "Database", "Connection" })
                               && a.Question.Name == "Server"
                               select a.Value).First();

            var showAdditionalTablesValue = (from a in connection.Settings.Answers
                                             where ArrayEqualityComparer.Equals<string>(a.Question.Path, new string[] { "Proffix", "Tables" })
                                             && a.Question.Name == "ShowAdditionalTables"
                                             select a.Value).First();

            Assert.AreEqual("Testserver", serverValue);
            Assert.AreEqual(true, showAdditionalTablesValue);
        }

        [Test]
        public void Create_Two_Connections_With_The_Same_ProviderPlugin()
        {
            string code = @"
CONNECT ""Dummy"" 
    AS \\Connections\FirstDummyConnection
    SET (
        Database.Connection.Server = ""FirstTestserver"",
        Database.Connection.Database = ""FirstTestDb"",
        Database.Connection.User = ""FirstTestUser"",
        Database.Connection.Password = ""FirstTestPassword"",
        Proffix.Tables.ShowAdditionalTables = TRUE,
        Proffix.Tables.ShowSystemTables = TRUE
    ) 
END

CONNECT ""Dummy"" 
    AS \\Connections\SecondDummyConnection
    SET (
        Database.Connection.Server = ""SecondTestserver"",
        Database.Connection.Database = ""SecondTestDb"",
        Database.Connection.User = ""SecondTestUser"",
        Database.Connection.Password = ""SecondTestPassword"",
        Proffix.Tables.ShowAdditionalTables = FALSE,
        Proffix.Tables.ShowSystemTables = FALSE
    ) 
END
";

            _SyneryClient.Run(code);

            string[] firstConnectionPath = new string[] { "Connections", "FirstDummyConnection" };

            IProviderConnection firstConnection = _ProviderPluginManager.Connections[firstConnectionPath];

            var firstServerValue = (from a in firstConnection.Settings.Answers
                                    where ArrayEqualityComparer.Equals<string>(a.Question.Path, new string[] { "Database", "Connection" })
                                    && a.Question.Name == "Server"
                                    select a.Value).First();

            string[] secondConnectionPath = new string[] { "Connections", "SecondDummyConnection" };

            IProviderConnection secondConnection = _ProviderPluginManager.Connections[secondConnectionPath];

            var secondServerValue = (from a in secondConnection.Settings.Answers
                                    where ArrayEqualityComparer.Equals<string>(a.Question.Path, new string[] { "Database", "Connection" })
                                    && a.Question.Name == "Server"
                                    select a.Value).First();

            Assert.AreEqual("FirstTestserver", firstServerValue);
            Assert.AreEqual("SecondTestserver", secondServerValue);
        }
    }
}
