using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;

namespace InterfaceBooster.SyneryLanguage.Model.Context
{
    /// <summary>
    /// Contains the data of a function written in synery code.
    /// </summary>
    public class FunctionData : IFunctionData
    {
        /// <summary>
        /// Gets the local identifier of the function (without alias)
        /// </summary>
        public string Name
        {
            get
            {
                if (FunctionDefinition == null)
                    return null;

                return FunctionDefinition.Name;
            }
        }

        /// <summary>
        /// Gets the full name of the function (FullName = CodeFileAlias.Name)
        /// </summary>
        public string FullName
        {
            get
            {
                // check whether an alias is available -> return CodeFileAlias.Name
                if (!string.IsNullOrEmpty(CodeFileAlias))
                    return String.Format("{0}.{1}", CodeFileAlias, Name);

                // no alias -> only return Name
                return Name;
            }
        }

        /// <summary>
        /// Gets or sets the alias for an external code file
        /// (= null if its an internal function from the same synery code file)
        /// </summary>
        public string CodeFileAlias { get; set; }

        /// <summary>
        /// Gets or sets the AST context for the function block
        /// </summary>
        public ParserRuleContext Context { get; set; }

        /// <summary>
        /// Gets or sets the definition of the function
        /// </summary>
        public IFunctionDefinition FunctionDefinition { get; set; }
    }
}
