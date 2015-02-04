using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using InterfaceBooster.Core.Common.Xml;
using InterfaceBooster.Core.LibraryPlugins.Information.XmlData;
using InterfaceBooster.Common.Interfaces.ErrorHandling;
using InterfaceBooster.Common.Interfaces.LibraryPlugin.Information.XmlData;
using InterfaceBooster.Common.Tools.Data.Xml;

namespace InterfaceBooster.Core.LibraryPlugins.Information
{
    public class LibraryPluginDataController
    {
        protected string _PluginXmlFilePath;

        public static ILibraryPluginData Load(string pluginXmlFilePath)
        {
            LibraryPluginDataController controller = new LibraryPluginDataController(pluginXmlFilePath);

            ILibraryPluginData data = controller.LoadData();

            return data;
        }

        #region INTERNAL METHODS

        private LibraryPluginDataController(string pluginXmlFilePath)
        {
            _PluginXmlFilePath = pluginXmlFilePath;
        }

        private ILibraryPluginData LoadData()
        {
            ILibraryPluginData data = new LibraryPluginData();

            XDocument doc = XDocument.Load(_PluginXmlFilePath);
            XElement xmlLibraryPlugin = doc.Element("LibraryPlugin");
            XElement xmlAssembliesRoot = xmlLibraryPlugin.Element("Assemblies");

            data.Id = new Guid(GetRequiredAttributeValue(xmlLibraryPlugin, "id"));
            data.Name = GetRequiredElementValue(xmlLibraryPlugin, "Name");
            data.Description = GetRequiredElementValue(xmlLibraryPlugin, "Description");

            data.Assemblies = LoadAssemblies(xmlAssembliesRoot);

            data.PluginXmlFilePath = _PluginXmlFilePath;
            data.PluginDirectoryPath = System.IO.Directory.GetParent(_PluginXmlFilePath).ToString();

            return data;
        }

        private IList<ILibraryPluginAssemblyData> LoadAssemblies(XElement xmlAssembliesRoot)
        {
            List<ILibraryPluginAssemblyData> assemblyData = new List<ILibraryPluginAssemblyData>();

            if (xmlAssembliesRoot != null)
            {
                foreach (XElement xmlItem in xmlAssembliesRoot.Elements("Assembly"))
                {
                    ILibraryPluginAssemblyData data = new LibraryPluginAssemblyData();
                    data.Path = GetRequiredAttributeValue(xmlItem, "path");
                    data.RequiredInterfaceVersion = new Version(GetRequiredAttributeValue(xmlItem, "requiredInterfaceVersion"));

                    assemblyData.Add(data);
                }
            }

            return assemblyData;
        }

        #region SINGLE VALUE HELPERS

        private string GetRequiredElementValue(XElement root, string name)
        {
            string value = XmlHelper.GetElementValue(root, name);

            if (value != null)
            {
                return value;
            }
            else
            {
                string msg = String.Format("XML-node '{0}' not found in '{1}'.", name, PathHelper.GetElementFullPath(root, "\\"));
                throw new XmlLoadingException("Library Plugin", _PluginXmlFilePath, msg);
            }
        }

        private string GetRequiredAttributeValue(XElement root, string name)
        {
            string value = XmlHelper.GetAttributeValue(root, name);

            if (value != null)
            {
                return value;
            }
            else
            {
                string msg = String.Format("XML-attribute '{0}' not found in '{1}'.", name, PathHelper.GetElementFullPath(root, "\\"));
                throw new XmlLoadingException("Library Plugin", _PluginXmlFilePath, msg);
            }
        }

        #endregion

        #endregion
    }
}
