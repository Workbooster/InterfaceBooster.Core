using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace InterfaceBooster.Core.Common.Xml
{
    /// <summary>
    /// Contains some helper methods for working with XML-LINQ objects.
    /// </summary>
    public static class XmlHelper
    {
        /// <summary>
        /// Gets the value of a XML-Node with the given name inside of the given root node.
        /// </summary>
        /// <param name="root"></param>
        /// <param name="name"></param>
        /// <returns>The value or NULL if the node couldn't be resolved.</returns>
        public static string GetElementValue(XElement root, string name)
        {
            if (root != null)
            {
                XElement element = root.Element(name);

                if (element != null)
                {
                    return element.Value;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the value of a XML-Attribute with the given name inside of the given root node.
        /// </summary>
        /// <param name="root"></param>
        /// <param name="name"></param>
        /// <returns>The value or NULL if the attribute couldn't be resolved.</returns>
        public static string GetAttributeValue(XElement root, string name)
        {
            if (root != null)
            {
                XAttribute attribute = root.Attribute(name);

                if (attribute != null)
                {
                    return attribute.Value;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the value of a XML-Node with the given name that contains a DateTime value inside of the given root node.
        /// </summary>
        /// <param name="root"></param>
        /// <param name="name"></param>
        /// <returns>The value or NULL if the node couldn't be resolved or the DateTime value is invalid.</returns>
        public static DateTime? GetDateTimeElementValue(XElement root, string name)
        {
            string value = GetElementValue(root, name);

            if (value == null)
            {
                return null;
            }

            DateTime dtm;
            DateTime.TryParse(value, out dtm);

            if (dtm != null)
            {
                return dtm;
            }
            else
            {
                return null;
            }
        }
    }
}
