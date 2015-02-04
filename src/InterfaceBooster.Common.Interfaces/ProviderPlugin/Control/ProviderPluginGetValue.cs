using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Common.Interfaces.ProviderPlugin.Control
{
    public class ProviderPluginGetValue
    {
        public string SyneryVariableName { get; set; }
        public string ProviderPluginValueName { get; set; }
        public object Value { get; set; }
    }
}
