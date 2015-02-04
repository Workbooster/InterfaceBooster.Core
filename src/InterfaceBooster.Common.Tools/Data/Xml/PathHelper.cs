using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace InterfaceBooster.Common.Tools.Data.Xml
{
    public static class PathHelper
    {
        /// <summary>
        /// Get a path from the names of the parent nodes for the given node.
        /// For example this can be used to create an error message that contains the path of a missing node.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="separator">the separator between the node names</param>
        /// <returns>a list of names separated by the given separator</returns>
        public static string GetElementFullPath(XElement element, string separator = "\\")
        {
            if (element != null)
            {
                int endlessLoopPreventionCounter = 1000;
                StringBuilder sb = new StringBuilder();
                XElement currentElement = element;

                while (currentElement != null)
                {
                    sb.Insert(0, currentElement.Name);
                    sb.Insert(0, separator);

                    currentElement = currentElement.Parent;

                    // prevent endless loop by breaking the loop if the counter reaches 0
                    endlessLoopPreventionCounter--;
                    if (endlessLoopPreventionCounter == 0) break;
                }

                return sb.ToString();
            }

            return "";
        }
    }
}
