using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Common.Interfaces.ErrorHandling
{
    [Serializable]
    public class InterfaceBoosterCoreException : InterfaceBoosterException
    {
        public InterfaceBoosterCoreException(string message) : base(message) { }
        public InterfaceBoosterCoreException(string message, Exception inner) : base(message, inner) { }
        protected InterfaceBoosterCoreException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
