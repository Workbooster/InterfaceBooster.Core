using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.SyneryTypes;

namespace InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context
{
    public interface IHandleBlockData
    {
        /// <summary>
        /// Gets or sets the definition of the record type that is handled by this HANDLE block.
        /// </summary>
        IRecordType HandledRecordType { get; set; }

        /// <summary>
        /// Gets or sets the local variable name the given event/exception record will get.
        /// </summary>
        string ParameterName { get; set; }

        /// <summary>
        /// Gets or sets the block context that should be executed if this HANDLE gets called.
        /// </summary>
        ParserRuleContext Context { get; set; }

        /// <summary>
        /// Gets or sets the parent scope of the OBSERVE block the HANDLE block is nested in.
        /// </summary>
        IScope ParentScope { get; set; }
    }
}
