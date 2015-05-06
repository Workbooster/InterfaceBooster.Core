using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Common.Interfaces.Broadcasting
{
    public class Message
    {
        public Guid Guid { get; set; }
        public string Text { get; set; }
        public string SourceName { get; set; }
        public Exception Exception { get; set; }
        public DateTime BroadcastedAt { get; set; }
    }
}
