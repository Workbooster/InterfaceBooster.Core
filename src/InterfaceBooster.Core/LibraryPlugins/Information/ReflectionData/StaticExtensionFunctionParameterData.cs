using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.LibraryPlugin.Information.ReflectionData;

namespace InterfaceBooster.Core.LibraryPlugins.Information.ReflectionData
{
    public class StaticExtensionFunctionParameterData : IStaticExtensionFunctionParameterData
    {
        public string Name { get; set; }
        public Type Type { get; set; }
        public bool IsOptional { get; set; }
    }
}
