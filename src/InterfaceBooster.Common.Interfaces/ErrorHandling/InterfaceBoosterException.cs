using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Common.Interfaces.ErrorHandling
{
    [Serializable]
    public class InterfaceBoosterException : Exception
    {
        public InterfaceBoosterException(string message) : base(message) { }
        public InterfaceBoosterException(string message, Exception inner) : base(message, inner) { }
        protected InterfaceBoosterException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
