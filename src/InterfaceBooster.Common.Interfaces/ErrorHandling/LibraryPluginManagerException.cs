using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.LibraryPlugin;

namespace InterfaceBooster.Common.Interfaces.ErrorHandling
{
    public class LibraryPluginManagerException : InterfaceBoosterCoreException
    {
        public ILibraryPluginManager LibraryPluginManager { get; set; }

        public LibraryPluginManagerException(ILibraryPluginManager instance, string message)
            : base(message)
        {
            LibraryPluginManager = instance;
        }

        public LibraryPluginManagerException(ILibraryPluginManager instance, string message, Exception inner)
            : base(message, inner)
        {
            LibraryPluginManager = instance;
        }
    }
}
