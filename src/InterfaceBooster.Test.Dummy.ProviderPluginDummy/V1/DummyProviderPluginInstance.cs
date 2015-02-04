using InterfaceBooster.ProviderPluginApi;
using InterfaceBooster.ProviderPluginApi.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Test.Dummy.ProviderPluginDummy.V1
{
    public class DummyProviderPluginInstance : IProviderPluginInstance
    {
        public IList<Question> ConnectionSettingQuestions { get; private set; }

        public IHost Host { get; private set; }

        public DummyProviderPluginInstance(IHost host)
        {
            Host = host;
            ConnectionSettingQuestions = CreateQuestions();
        }

        public IProviderConnection CreateProviderConnection(ConnectionSettings settings)
        {
            DummyProviderConnection connection = new DummyProviderConnection(settings);

            return connection;
        }

        private IList<Question> CreateQuestions()
        {
            IList<Question> listOfQuestions = new List<Question>();

            string[] dbPath = new string[] { "Database", "Connection", };
            listOfQuestions.Add(Question.New<string>("Server", dbPath, true));
            listOfQuestions.Add(Question.New<string>("Database", dbPath, true));
            listOfQuestions.Add(Question.New<int>("Port", dbPath, false));
            listOfQuestions.Add(Question.New<string>("User", dbPath, true));
            listOfQuestions.Add(Question.New<string>("Password", dbPath, true));

            string[] pxPath = new string[] { "Proffix", "Tables", };
            listOfQuestions.Add(Question.New<bool>("ShowAdditionalTables", pxPath, true, null, "Should the additional tables be displayed?"));
            listOfQuestions.Add(Question.New<bool>("ShowSystemTables", pxPath, true, null, "Should the system tables be displayed?"));

            return listOfQuestions;
        }
    }
}
