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

        public event BroadcastMessageDelegate OnAnyMessage;

        public event BroadcastMessageDelegate OnDebugMessage;

        public event BroadcastMessageDelegate OnInfoMessage;

        public event BroadcastMessageDelegate OnWarningMessage;

        public event BroadcastMessageDelegate OnErrorMessage;

        #endregion

        #region PUBLIC METHODS

        public Message Broadcast(Message msg)
        {
            string channel = String.IsNullOrEmpty(msg.Channel) ? "" : msg.Channel;

            return Broadcast(null, channel, msg);
        }

        public Message Broadcast(string channel, string message, string source = null, params object[] args)
        {
            return Broadcast(null, channel, message, source, args);
        }

        public Message Broadcast(string channel, Exception ex, string source = null)
        {
            return Broadcast(null, channel, ex, source);
        }

        public Message Debug(Message msg)
        {
            return Broadcast(OnDebugMessage, "debug", msg);
        }

        public Message Debug(string message, string source = null, params object[] args)
        {
            return Broadcast(OnDebugMessage, "debug", message, source, args);
        }

        public Message Debug(Exception ex, string source = null)
        {
            return Broadcast(OnDebugMessage, "debug", ex, source);
        }

        public Message Info(Message msg)
        {
            return Broadcast(OnInfoMessage, "info", msg);
        }

        public Message Info(string message, string source = null, params object[] args)
        {
            return Broadcast(OnInfoMessage, "info", message, source, args);
        }

        public Message Info(Exception ex, string source = null)
        {
            return Broadcast(OnInfoMessage, "info", ex, source);
        }

        public Message Warning(Message msg)
        {
            return Broadcast(OnWarningMessage, "warning", msg);
        }

        public Message Warning(string message, string source = null, params object[] args)
        {
            return Broadcast(OnWarningMessage, "warning", message, source, args);
        }

        public Message Warning(Exception ex, string source = null)
        {
            return Broadcast(OnWarningMessage, "warning", ex, source);
        }

        public Message Error(Message msg)
        {
            return Broadcast(OnErrorMessage, "error", msg);
        }

        public Message Error(string message, string source = null, params object[] args)
        {
            return Broadcast(OnErrorMessage, "error", message, source, args);
        }

        public Message Error(Exception ex, string source = null)
        {
            return Broadcast(OnErrorMessage, "error", ex, source);
        }

        #endregion

        #region INTERNAL METHODS

        private Message Broadcast(BroadcastMessageDelegate broadcastDelegate, string channel, Message msg)
        {
            msg.Channel = channel;
            msg.BroadcastedAt = DateTime.Now;

            if (broadcastDelegate != null)
                broadcastDelegate(msg);

            // broadcast all messages

            if (OnAnyMessage != null)
                OnAnyMessage(msg);

            return msg;
        }

        private Message Broadcast(BroadcastMessageDelegate broadcastDelegate, string channel, string message, string source = null, params object[] args)
        {
            Message data = new Message();
            data.Text = String.Format(message, args);
            data.SourceName = source;
            data.Guid = Guid.NewGuid();

            return Broadcast(broadcastDelegate, channel, data);
        }

        private Message Broadcast(BroadcastMessageDelegate broadcastDelegate, string channel, Exception ex, string source = null)
        {
            Message data = new Message();
            data.Text = ex != null ? ex.Message : null;
            data.SourceName = source;
            data.Guid = Guid.NewGuid();

            return Broadcast(broadcastDelegate, channel, data);
        }

        #endregion
    }
}
