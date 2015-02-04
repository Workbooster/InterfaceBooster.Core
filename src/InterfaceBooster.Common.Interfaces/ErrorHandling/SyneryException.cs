using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Common.Interfaces.ErrorHandling
{
    [Serializable]
    public class SyneryException : InterfaceBoosterException
    {
        public SyneryException(string message) : base(message) { }
        public SyneryException(string message, Exception inner) : base(message, inner) { }
        protected SyneryException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
