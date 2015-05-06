using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Common.Interfaces.Broadcasting
{
    public class StandardBroadcaster : IBroadcaster
    {
        #region EVENTS
        
        public event BroadcastMessageDelegate OnDebugMessage;

        public event BroadcastMessageDelegate OnInfoMessage;

        public event BroadcastMessageDelegate OnWarningMessage;

        public event BroadcastMessageDelegate OnErrorMessage;

        #endregion

        #region PUBLIC METHODS

        public Message Debug(Message msg)
        {
            return Broadcast(OnDebugMessage, msg);
        }

        public Message Debug(string message, string source = null, params object[] args)
        {
            return Broadcast(OnDebugMessage, message, source, args);
        }

        public Message Debug(Exception ex, string source = null)
        {
            return Broadcast(OnDebugMessage, ex, source);
        }

        public Message Info(Message msg)
        {
            return Broadcast(OnInfoMessage, msg);
        }

        public Message Info(string message, string source = null, params object[] args)
        {
            return Broadcast(OnInfoMessage, message, source, args);
        }

        public Message Info(Exception ex, string source = null)
        {
            return Broadcast(OnInfoMessage, ex, source);
        }

        public Message Warning(Message msg)
        {
            return Broadcast(OnWarningMessage, msg);
        }

        public Message Warning(string message, string source = null, params object[] args)
        {
            return Broadcast(OnWarningMessage, message, source, args);
        }

        public Message Warning(Exception ex, string source = null)
        {
            return Broadcast(OnWarningMessage, ex, source);
        }

        public Message Error(Message msg)
        {
            return Broadcast(OnErrorMessage, msg);
        }

        public Message Error(string message, string source = null, params object[] args)
        {
            return Broadcast(OnErrorMessage, message, source, args);
        }

        public Message Error(Exception ex, string source = null)
        {
            return Broadcast(OnErrorMessage, ex, source);
        }

        #endregion

        #region INTERNAL METHODS

        private Message Broadcast(BroadcastMessageDelegate broadcastDelegate, Message msg)
        {
            if (broadcastDelegate != null)
                broadcastDelegate(msg);

            return msg;
        }

        private Message Broadcast(BroadcastMessageDelegate broadcastDelegate, string message, string source = null, params object[] args)
        {
            Message data = new Message();
            data.Text = String.Format(message, args);
            data.SourceName = source;
            data.Guid = Guid.NewGuid();
            data.BroadcastedAt = DateTime.Now;

            return Broadcast(broadcastDelegate, data);
        }

        private Message Broadcast(BroadcastMessageDelegate broadcastDelegate, Exception ex, string source = null)
        {
            Message data = new Message();
            data.Text = ex != null ? ex.Message : null;
            data.SourceName = source;
            data.Guid = Guid.NewGuid();
            data.BroadcastedAt = DateTime.Now;

            return Broadcast(broadcastDelegate, data);
        }

        #endregion
    }
}
