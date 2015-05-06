using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Common.Interfaces.Broadcasting
{
    /// <summary>
    /// can be used to broadcast a message up to the view
    /// </summary>
    /// <param name="message"></param>
    public delegate void BroadcastMessageDelegate(Message message);
}
