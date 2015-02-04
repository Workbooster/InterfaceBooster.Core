using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.RuntimeController.Broadcasting
{
    /// <summary>
    /// can be used to broadcast a message up to the view
    /// </summary>
    /// <param name="message"></param>
    public delegate void BroadcastMessageDelegate(string message);
}
