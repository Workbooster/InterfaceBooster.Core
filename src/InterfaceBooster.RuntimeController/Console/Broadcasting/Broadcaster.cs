using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.RuntimeController.Broadcasting
{
    /// <summary>
    /// The Broadcaster handels the distribution of messages, errors and questions to all registered listeners
    /// </summary>
    public class Broadcaster
    {
        #region EVENTS

        /// <summary>
        /// occurs when a information appears
        /// </summary>
        public event BroadcastMessageDelegate OnInfoMessage;

        /// <summary>
        /// occures when a error message appears
        /// </summary>
        public event BroadcastMessageDelegate OnErrorMessage;

        /// <summary>
        /// occurs when the user should be asked a question
        /// </summary>
        public event AskQuestionDelegate OnQuestion;

        #endregion

        #region PUBLIC METHODS

        #region MESSAGES

        /// <summary>
        /// broadcasts an info message to all registered listeners at "OnInfoMessage"
        /// </summary>
        /// <param name="formatedMessage"></param>
        /// <param name="args"></param>
        public void Info(string formatedMessage, params object[] args)
        {
            if (OnInfoMessage != null)
            {
                if (args.Length == 0)
                {
                    OnInfoMessage(formatedMessage);
                }
                else
                {
                    OnInfoMessage(String.Format(formatedMessage, args));
                }
            }
        }

        /// <summary>
        /// broadcasts an error message to all registered listeners at "OnErrorMessage"
        /// </summary>
        /// <param name="formatedMessage"></param>
        /// <param name="args"></param>
        public void Error(string formatedMessage, params object[] args)
        {
            if (OnErrorMessage != null)
            {
                if (args.Length == 0)
                {
                    OnErrorMessage(formatedMessage);
                }
                else
                {
                    OnErrorMessage(String.Format(formatedMessage, args));
                }
            }
        }

        #endregion

        #region QUESTIONS

        /// <summary>
        /// broadcasts a question to all registered listeners at "OnQuestion"
        /// </summary>
        /// <param name="question"></param>
        /// <param name="options"></param>
        public object Question(string question, Dictionary<string, object> options = null)
        {
            if (OnQuestion != null)
            {
                return OnQuestion(question, options);
            }

            return null;
        }

        #endregion

        #endregion
    }
}
