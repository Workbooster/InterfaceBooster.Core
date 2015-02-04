using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Common.Interfaces.LibraryPlugin.Information.ReflectionData
{
    public interface IStaticExtensionFunctionParameterData
    {
        string Name { get; set; }
        Type Type { get; set; }
        bool IsOptional { get; set; }
    }
}
