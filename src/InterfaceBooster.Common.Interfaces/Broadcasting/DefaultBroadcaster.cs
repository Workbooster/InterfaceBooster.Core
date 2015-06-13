using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Common.Interfaces.Broadcasting
{
    public class DefaultBroadcaster : IBroadcaster
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

        public Message Broadcast(string channel, string message, params object[] args)
        {
            return Broadcast(null, channel, message, null, args);
        }

        public Message Broadcast(string channel, Exception ex, string source = null)
        {
            return Broadcast(null, channel, ex, source);
        }

        public Message Debug(Message msg)
        {
            return Broadcast(OnDebugMessage, "debug", msg);
        }

        public Message Debug(string message, params object[] args)
        {
            return Broadcast(OnDebugMessage, "debug", message, null, args);
        }

        public Message Debug(Exception ex, string source = null)
        {
            return Broadcast(OnDebugMessage, "debug", ex, source);
        }

        public Message Info(Message msg)
        {
            return Broadcast(OnInfoMessage, "info", msg);
        }

        public Message Info(string message, params object[] args)
        {
            return Broadcast(OnInfoMessage, "info", message, null, args);
        }

        public Message Info(Exception ex, string source = null)
        {
            return Broadcast(OnInfoMessage, "info", ex, source);
        }

        public Message Warning(Message msg)
        {
            return Broadcast(OnWarningMessage, "warning", msg);
        }

        public Message Warning(string message, params object[] args)
        {
            return Broadcast(OnWarningMessage, "warning", message, null, args);
        }

        public Message Warning(Exception ex, string source = null)
        {
            return Broadcast(OnWarningMessage, "warning", ex, source);
        }

        public Message Error(Message msg)
        {
            return Broadcast(OnErrorMessage, "error", msg);
        }

        public Message Error(string message, params object[] args)
        {
            return Broadcast(OnErrorMessage, "error", message, null, args);
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
            {
                broadcastDelegate(msg);
            }
            else
            {
                // no broadcast-delegate given means that the message came over the neutral Broadcast()-method
                // if it is a default channel (e.g. "info") call the specific delegates manually

                if (OnDebugMessage != null && msg.Channel.ToLower() == "debug")
                    OnDebugMessage(msg);
                if (OnInfoMessage != null && msg.Channel.ToLower() == "info")
                    OnInfoMessage(msg);
                if (OnWarningMessage != null && msg.Channel.ToLower() == "warning")
                    OnWarningMessage(msg);
                if (OnErrorMessage != null && msg.Channel.ToLower() == "error")
                    OnErrorMessage(msg);
            }
                
            // broadcast all messages

            if (OnAnyMessage != null)
                OnAnyMessage(msg);

            return msg;
        }

        private Message Broadcast(BroadcastMessageDelegate broadcastDelegate, string channel, string message, string source = null, params object[] args)
        {
            Message data = new Message();
            data.SourceName = source;
            data.Guid = Guid.NewGuid();

            if (args == null || args.Length == 0)
            {
                data.Text = message;
            }
            else
            {
                // it's a formated message
                data.Text = String.Format(message, args);
            }

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
