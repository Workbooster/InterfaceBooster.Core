using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context
{
    /// <summary>
    /// Contains the data of a function written in synery code.
    /// </summary>
    public interface IFunctionData
    {
        /// <summary>
        /// Gets the local identifier of the function (without alias)
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the full name of the function (FullName = CodeFileAlias.Name)
        /// </summary>
        string FullName { get; }

        /// <summary>
        /// Gets or sets the alias for an external code file
        /// (= null if its an internal function from the same synery code file)
        /// </summary>
        string CodeFileAlias { get; set; }

        /// <summary>
        /// Gets or sets the AST context for the function block
        /// </summary>
        ParserRuleContext Context { get; set; }

        /// <summary>
        /// Gets or sets the definition of the function
        /// </summary>
        IFunctionDefinition FunctionDefinition { get; set; }
    }
}
