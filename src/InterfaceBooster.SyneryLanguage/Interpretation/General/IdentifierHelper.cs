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
