using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Common.Interfaces.Broadcasting
{
    public interface IBroadcaster
    {
        #region EVENTS

        /// <summary>
        /// occurs when a debug message appears
        /// </summary>
        event BroadcastMessageDelegate OnDebugMessage;

        /// <summary>
        /// occurs when a information appears
        /// </summary>
        event BroadcastMessageDelegate OnInfoMessage;

        /// <summary>
        /// occurs when a warning message appears
        /// </summary>
        event BroadcastMessageDelegate OnWarningMessage;

        /// <summary>
        /// occures when a error message appears
        /// </summary>
        event BroadcastMessageDelegate OnErrorMessage;

        #endregion

        #region METHODS

        Message Debug(Message msg);
        Message Debug(string message, string source = null, params object[] args);
        Message Debug(Exception ex, string source = null);

        Message Info(Message msg);
        Message Info(string message, string source = null, params object[] args);
        Message Info(Exception ex, string source = null);

        Message Warning(Message msg);
        Message Warning(string message, string source = null, params object[] args);
        Message Warning(Exception ex, string source = null);

        Message Error(Message msg);
        Message Error(string message, string source = null, params object[] args);
        Message Error(Exception ex, string source = null);

        #endregion
    }
}
