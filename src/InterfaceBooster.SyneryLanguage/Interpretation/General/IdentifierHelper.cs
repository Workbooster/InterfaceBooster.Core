using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.SyneryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;

namespace InterfaceBooster.SyneryLanguage.Interpretation.General
{
    /// <summary>
    /// Provides some static helper methods for handling the complex identifiers of the Synery language.
    /// </summary>
    public static class IdentifierHelper
    {
        /// <summary>
        /// If an identifier is used in a function of an IncludeFile and references another 
        /// Object (e.g. function or record type) from within the file the identifier must 
        /// be prepended with the CodeFileAlias of that IncludeFile.
        /// This method checks whether we are currently in the scope of a function from an
        /// IncludeFile and if so it prepends the CodeFileAlias of the IncludeFile.
        /// </summary>
        /// <param name="memory"></param>
        /// <param name="identifier">The name of the object (without alias)</param>
        /// <returns></returns>
        public static string GetIdentifierBasedOnFunctionScope(ISyneryMemory memory, string identifier)
        {
            // check whether the function call comes from a function scope that sits inside of an include file
            IFunctionScope surroundingFunctionScope = memory.CurrentScope.ResolveFunctionScope();

            if (surroundingFunctionScope != null
                && String.IsNullOrEmpty(surroundingFunctionScope.FunctionData.CodeFileAlias) == false)
            {
                // prepend the CodeFileAlias of the current function scope to get the correct FullName of the requested function
                return GetFullName(identifier, surroundingFunctionScope.FunctionData.CodeFileAlias);
            }

            return identifier;
        }

        /// <summary>
        /// Checks whether a CodeFileAlias is given. If yes, the alias is prepended (e.g. "Alias.Identifier")
        /// </summary>
        /// <param name="identifier">The name of the object</param>
        /// <param name="codeFileAlias">The alias of the IncludeFile</param>
        /// <returns>Returns either "Alias.Identifier" or only the identifier if no alias is specified.</returns>
        public static string GetFullName(string identifier, string codeFileAlias = null)
        {
            if (String.IsNullOrEmpty(codeFileAlias))
                return identifier;

            return codeFileAlias + "." + identifier;
        }

        public static string[] ParsePathIdentifier(string pathIdentifier)
        {
            string timmedPath = pathIdentifier.Trim(new char[] { '\\' });
            string[] path = timmedPath.Split(new char[] { '\\' });

            return path;
        }

        public static string[] ParseComplexIdentifier(string complexIdentifier)
        {
            string[] path = complexIdentifier.Split(new char[] { '.' });

            return path;
        }
    }
}
