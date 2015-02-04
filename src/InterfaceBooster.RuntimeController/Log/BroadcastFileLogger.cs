using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.RuntimeController.Broadcasting;

namespace InterfaceBooster.RuntimeController.Log
{
    public class BroadcastFileLogger
    {
        #region CONSTANTES

        const string DIVIDING_RULE = "------------------------------------------";
        const int BUFFER_SIZE = 20000;

        #endregion

        #region MEMBERS

        private Broadcaster _Broadcaster;
        private string _LogFilePath;
        private StreamWriter _StreamWriter;
        private StringBuilder _Messages;

        #endregion

        #region PUBLIC METHODS

        public BroadcastFileLogger(Broadcaster broadcaster, string logFileDirectoryPath)
        {
            _Broadcaster = broadcaster;
            _LogFilePath = BuildLogFilePath(logFileDirectoryPath);
            _StreamWriter = new StreamWriter(_LogFilePath);
            _Messages = new StringBuilder();

            // register for log events
            _Broadcaster.OnInfoMessage += Broadcaster_OnInfoMessage;
            _Broadcaster.OnErrorMessage += Broadcaster_OnErrorMessage;
        }

        public void Finish()
        {
            // remove registrations
            _Broadcaster.OnErrorMessage -= Broadcaster_OnInfoMessage;
            _Broadcaster.OnErrorMessage -= Broadcaster_OnErrorMessage;

            // write the remaining messages to the file
            WriteToFile();

            // close the writer and relese the file
            _StreamWriter.Close();
        }

        #endregion

        #region INTERNAL METHODS

        private void Broadcaster_OnInfoMessage(string message)
        {
            AppendLogMessage(message, "Info");
        }

        private void Broadcaster_OnErrorMessage(string message)
        {
            AppendLogMessage(message, "Error");
        }

        private void AppendLogMessage(string message, string type)
        {
            // the line between the log entries
            string dividingRule = "-------------------------------------------";

            // concatenate the log entry
            string logEntry = String.Format("[{0} - {1}] {2}",
                DateTime.Now.ToString("HH:mm:ss"),
                type.ToUpper(),
                message);

            lock (_Messages)
            {
                // append the entry to the string builder
                _Messages.AppendLine(logEntry);
                _Messages.AppendLine(dividingRule);
            }

            if (_Messages.Length > BUFFER_SIZE)
            {
                WriteToFile();
            }
        }

        private string BuildLogFilePath(string logFileDirectoryPath)
        {
            if (Directory.Exists(logFileDirectoryPath) == false)
            {
                Directory.CreateDirectory(logFileDirectoryPath);
            }

            string fileName = String.Format("{0}.log", DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));

            return Path.Combine(logFileDirectoryPath, fileName);
        }

        private void WriteToFile()
        {
            try
            {
                lock (_Messages)
                {
                    string messages = _Messages.ToString();

                    _StreamWriter.Write(messages);

                    _Messages.Clear();
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
            }
        }

        #endregion
    }
}
