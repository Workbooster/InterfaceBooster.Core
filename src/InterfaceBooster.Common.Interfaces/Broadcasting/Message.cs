using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Common.Interfaces.Broadcasting
{
    public class Message
    {
        /// <summary>
        /// Gets or sets an unique identifier for the message
        /// </summary>
        public Guid Guid { get; set; }

        /// <summary>
        /// Gets or sets the message content.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the name of the channel this message is broadcasted.
        /// </summary>
        public string Channel { get; set; }

        /// <summary>
        /// Gets or sets the name of the component the message is comming from.
        /// </summary>
        public string SourceName { get; set; }

        /// <summary>
        /// Gets or sets an Exception linkt with the message (on error).
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// Gets or sets the the date and time the message was added on broadcast.
        /// </summary>
        public DateTime BroadcastedAt { get; set; }
    }
}
