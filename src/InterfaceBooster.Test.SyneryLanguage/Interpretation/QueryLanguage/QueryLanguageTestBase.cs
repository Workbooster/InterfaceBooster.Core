using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Core.LibraryPlugins;
using InterfaceBooster.Core.ProviderPlugins;
using InterfaceBooster.SyneryLanguage.Interpretation;
using InterfaceBooster.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.InterfaceDefinition.Data;
using InterfaceBooster.Common.Interfaces.LibraryPlugin;
using InterfaceBooster.Common.Interfaces.ProviderPlugin.Interfaces;
using InterfaceBooster.Common.Interfaces.SyneryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;
using InterfaceBooster.Database.Interfaces.Structure;
using InterfaceBooster.Database.Interfaces;
using InterfaceBooster.Database.Core;
using InterfaceBooster.Common.Interfaces.ProviderPlugin.Control;
using InterfaceBooster.Common.Interfaces.Broadcasting;

namespace InterfaceBooster.Test.SyneryLanguage.Interpretation.QueryLanguage
{
    public class QueryLanguageTestBase
    {
        protected string _PluginMainDirectoryPath;
        protected string _LibraryPluginMainDirectoryPath;
        protected string _DatabaseWorkingDirectoryPath;
        protected ISyneryClient<bool> _SyneryClient;
        protected IDatabase _Database;
        protected IBroadcaster _Broadcaster;
        protected IProviderPluginManager _ProviderPluginManager;
        protected ILibraryPluginManager _LibraryPluginManager;
        protected ISyneryMemory _SyneryMemory;

        [SetUp]
        public void SetupTest()
        {
            string solutionDirectoryPath = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())));
            _PluginMainDirectoryPath = Path.Combine(solutionDirectoryPath, @"_TestData\InterfaceBooster\ProviderPluginDirectory");
            _LibraryPluginMainDirectoryPath = Path.Combine(solutionDirectoryPath, @"_TestData\InterfaceBooster\LibraryPluginDirectory");
            _DatabaseWorkingDirectoryPath = Path.Combine(solutionDirectoryPath, @"_TestData\InterfaceBooster\SyneryDB", Guid.NewGuid().ToString());

            if (Directory.Exists(_DatabaseWorkingDirectoryPath))
            {
                Directory.Delete(_DatabaseWorkingDirectoryPath, true);
            }

            Directory.CreateDirectory(_DatabaseWorkingDirectoryPath);

            _Database = new SyneryDB(_DatabaseWorkingDirectoryPath);
            _Broadcaster = new DefaultBroadcaster();

            _ProviderPluginManager = new ProviderPluginManager(_PluginMainDirectoryPath);
            _LibraryPluginManager = new LibraryPluginManager(_LibraryPluginMainDirectoryPath);

            _SyneryMemory = new SyneryMemory(_Database, _Broadcaster, _ProviderPluginManager, _LibraryPluginManager);

            _SyneryClient = new InterpretationClient(_SyneryMemory);
        }

        [TearDown]
        public void TearDown()
        {
            _Database.Dispose();

            if (Directory.Exists(_DatabaseWorkingDirectoryPath))
            {
                Directory.Delete(_DatabaseWorkingDirectoryPath, true);
            }
        }

        #region PLUGIN TEST

        /// <summary>
        /// Activates a dummy ProviderPlugin on the current instance of the ProviderPluginManager
        /// </summary>
        /// <param name="pluginInstanceSyneryIdentifier"></param>
        public void SetupDummyPlugin(string pluginInstanceSyneryIdentifier = "Dummy")
        {
            ProviderPluginInstanceReference simpleDummyReference = new ProviderPluginInstanceReference();
            simpleDummyReference.SyneryIdentifier = pluginInstanceSyneryIdentifier;
            simpleDummyReference.IdPlugin = new Guid("485eccb4-3920-4dc3-9ed4-27f65e8b3c91");
            simpleDummyReference.PluginName = "ReferencePluginName";
            simpleDummyReference.IdPluginInstance = new Guid("58e6a1b5-9eb5-45ab-8c9b-8b267e2d09e8");
            simpleDummyReference.PluginInstanceName = "ReferencePluginInstanceName";

            // activate provider plugin instance
            _ProviderPluginManager.Activate(simpleDummyReference);
        }

        /// <summary>
        /// Creates a ProviderPlugin connection on the current instance of the ProviderPluginManager.
        /// Therefor all required parameters (answers) are set.
        /// </summary>
        /// <param name="pluginInstanceSyneryIdentifier"></param>
        public void SetupProviderPluginDummyConnection(string pluginInstanceSyneryIdentifier = "Dummy", string[] connectionPath = null)
        {
            if (connectionPath == null)
            {
                // set default
                connectionPath = new string[] { "Connections", "DummyConnection" };
            }

            ProviderPluginConnectTask task = new ProviderPluginConnectTask();

            task.InstanceReferenceSyneryIdentifier = pluginInstanceSyneryIdentifier;
            task.ConnectionPath = connectionPath;
            task.Memory = _SyneryMemory;

            // add the parameters needed by the provider plugin instance to create a connection
            task.Parameters.Add(new string[] { "Database", "Connection", "Server" }, "someServer");
            task.Parameters.Add(new string[] { "Database", "Connection", "Database" }, "someDb");
            task.Parameters.Add(new string[] { "Database", "Connection", "User" }, "SomeUser");
            task.Parameters.Add(new string[] { "Database", "Connection", "Password" }, "somePassword");
            task.Parameters.Add(new string[] { "Proffix", "Tables", "ShowAdditionalTables" }, true);
            task.Parameters.Add(new string[] { "Proffix", "Tables", "ShowSystemTables" }, true);

            _ProviderPluginManager.RunTask(task);
        }

        public void SetupLibraryPlugin(string syneryIdentifier = "Dummy")
        {
            LibraryPluginReference simpleDummyReference = new LibraryPluginReference();
            simpleDummyReference.SyneryIdentifier = syneryIdentifier;
            simpleDummyReference.IdPlugin = new Guid("74A8005D-C9F3-455F-94FC-04846493AB7B");
            simpleDummyReference.PluginName = "ReferencePluginName";

            // activate library plugin instance
            _LibraryPluginManager.Activate(simpleDummyReference);
        }

        #endregion

        #region DATABASE SETUP

        // These methods can be called from a test (or SetUp) to get a set of dummy data

        public static void CreatePeopleTable(IDatabase db)
        {
            Random random = new Random();

            ISchema schema = db.NewSchema();
            schema.AddField<int>("Id");
            schema.AddField<string>("Firstname");
            schema.AddField<string>("Lastname");
            schema.AddField<string>("Title");
            schema.AddField<bool>("IsMale");
            schema.AddField<int>("NumberOfChildren");
            schema.AddField<double>("Size");
            schema.AddField<decimal>("Weight");
            schema.AddField<DateTime>("DateOfBirth");
            schema.AddField<char>("FirstLetter");
            schema.AddField<DateTime>("DateOfDeath");
            schema.AddField<string>("Notes");

            List<object[]> data = new List<object[]>
            {
                new object[] { 1, "Roger", "Guillet", "Mr.", true, 2, 1.70, 81.3M, new DateTime(1987, 12, 15), 'R', null, "Some text..." },
                new object[] { 3, "Sandra", "Bloch", "Mrs.", false, 1, 1.67, 68.9M, new DateTime(1989, 6, 2), 'S', null, "Some notes..." },
                new object[] { 5, "Pete", "Rhubarb", "Mr.", true, 0, 1.75, 85.1M, new DateTime(1981, 3, 17), 'P', null, null },
                new object[] { 7, "Kate", "Price", "Mrs.", false, 3, 1.70, 73.8M, new DateTime(1976, 9, 13), 'K', new DateTime(2014, 6, 22), null },
                new object[] { 9, "Silvia", "Meyer", "Mrs.", false, 1, 1.63, 62.7M, new DateTime(1992, 1, 22), 'S', null, "Simething important..." },
                new object[] { 11, "Petra", "Stalone", "Mrs.", false, 2, 1.7, 73.5M, new DateTime(1984, 6, 9), 'P', null, null },
            };

            ITable table = db.NewTable(schema, data);

            db.CreateTable(@"\QueryLanguageTests\People", table);
        }

        public static void CreateCoursesTable(IDatabase db)
        {
            ISchema schema = db.NewSchema();
            schema.AddField<int>("Id");
            schema.AddField<string>("Name");
            schema.AddField<string>("Description");
            schema.AddField<decimal>("Price");
            schema.AddField<DateTime>("DateOfStart");
            schema.AddField<DateTime>("DateOfEnd");

            List<object[]> data = new List<object[]>
            {
                new object[] { 2, "How to eat lobster", "You learn how to eas a lobster without getting dirty.", 199.90M, new DateTime(2015, 5, 1), new DateTime(2015, 5, 8) },
                new object[] { 4, "Lobster cooking for dummies", "Don't be anxious to cook a lobster. In this course you learn how to do it right.", 279.50M, new DateTime(2015, 9, 5), new DateTime(2015, 9, 13) },
                new object[] { 6, "Rhubarb cakes for beginners", "Learn how to bake a rhubarb cake without getting poisoned.", 99.95M, new DateTime(2015, 7, 23), new DateTime(2015, 7, 30) },
                new object[] { 8, "Rhubarb cakes for professionals", "Learn how to bake like your grandmother.", 529.10M, new DateTime(2015, 11, 12), new DateTime(2015, 11, 17) },
                new object[] { 10, "Blowfish", "Learn how to poison your guests with a blowfish.", 789.25M, new DateTime(2015, 12, 3), new DateTime(2015, 12, 10) },
            };

            ITable table = db.NewTable(schema, data);

            db.CreateTable(@"\QueryLanguageTests\Courses", table);
        }

        public static void CreateRegistrationsTable(IDatabase db)
        {
            ISchema schema = db.NewSchema();
            schema.AddField<int>("Id");
            schema.AddField<int>("IdPerson");
            schema.AddField<int>("IdCourse");
            schema.AddField<DateTime>("DateOfRegistration");

            List<object[]> data = new List<object[]>
            {
                new object[] { 5, 1, 2, new DateTime(2015, 2, 3) },
                new object[] { 10, 1, 4, new DateTime(2015, 2, 3) },
                new object[] { 15, 1, 6, new DateTime(2015, 2, 3) },
                new object[] { 20, 3, 2, new DateTime(2015, 2, 3) },
                new object[] { 25, 3, 6, new DateTime(2015, 2, 3) },
                new object[] { 30, 5, 6, new DateTime(2015, 2, 3) },
                new object[] { 55, 7, 6, new DateTime(2015, 2, 3) },
                new object[] { 35, 7, 8, new DateTime(2015, 2, 3) },
                new object[] { 40, 7, 10, new DateTime(2015, 2, 3) },
                new object[] { 50, 9, 6, new DateTime(2015, 2, 3) },
                new object[] { 45, 9, 10, new DateTime(2015, 2, 3) },
            };

            ITable table = db.NewTable(schema, data);

            db.CreateTable(@"\QueryLanguageTests\Registrations", table);
        }

        /// <summary>
        /// creates a large test table in the given database
        /// the IdJoin can be used to run some join tests with two larg tables
        /// </summary>
        /// <param name="db"></param>
        /// <param name="numberOfRows"></param>
        /// <param name="tableName"></param>
        public static void CreateLargeTestTable(IDatabase db, int numberOfRows = 50000, string tableName = @"\QueryLanguageTests\Logs")
        {
            Random random = new Random();

            ISchema schema = db.NewSchema();
            schema.AddField<int>("Id");
            schema.AddField<int>("IdPerson");
            schema.AddField<int>("IdCourse");
            schema.AddField<int>("IdJoin");
            schema.AddField<string>("First");
            schema.AddField<string>("Second");
            schema.AddField<string>("Third");
            schema.AddField<string>("Fourth");
            schema.AddField<string>("Fifth");
            schema.AddField<bool>("IsActive");

            List<object[]> data = new List<object[]>();

            for (int i = 0; i < numberOfRows; i++)
            {
                data.Add(
                    new object[] {
                        random.Next(), //Id
                        random.Next(5) * 2 + 1, // IdPerson
                        random.Next(5) * 2, // IdCourse
                        random.Next(numberOfRows), // IdJoin
                        GetRandomSentence(random), // First
                        GetRandomSentence(random), // Second
                        GetRandomSentence(random), // Third
                        GetRandomSentence(random), // Fourth
                        GetRandomSentence(random), // Fifth
                        random.Next(1) == 1 ? true : false // IsActive
                    });
            }

            ITable table = db.NewTable(schema, data);

            db.CreateTable(tableName, table);
        }

        public static string GetRandomSentence(Random random = null)
        {
            if (random == null)
                random = new Random();

            StringBuilder sb = new StringBuilder();
            string[] dummyWords = { "Dummy", "table", "content", "Bla!", "aha?", "Rhubarb" };
            int dummyWordsCount = dummyWords.Count();

            for (int i = 0; i < 10; i++)
            {
                sb.Append(dummyWords[random.Next(dummyWordsCount - 1)]);
                sb.Append(" ");
            }

            return sb.ToString();
        }


        #endregion
    }
}
