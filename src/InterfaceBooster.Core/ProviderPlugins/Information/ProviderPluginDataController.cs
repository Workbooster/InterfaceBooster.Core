using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using InterfaceBooster.Core.Common.Xml;
using InterfaceBooster.Core.ProviderPlugins.Information.Data;
using InterfaceBooster.Common.Interfaces.ErrorHandling;
using InterfaceBooster.Common.Interfaces.ProviderPlugin.Interfaces.Information.Data;
using InterfaceBooster.Common.Tools.Data.Xml;

namespace InterfaceBooster.Core.ProviderPlugins.Information
{
    public class ProviderPluginDataController
    {
        #region MEMBERS

        private string _PluginXmlFilePath;

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// loads a plugin.xml file
        /// </summary>
        /// <param name="pluginXmlFilePath"></param>
        /// <returns></returns>
        public static IProviderPluginData Load(string pluginXmlFilePath)
        {
            ProviderPluginDataController controller = new ProviderPluginDataController(pluginXmlFilePath);

            IProviderPluginData data = controller.LoadData();

            return data;
        }

        #endregion

        #region INTERNAL METHODS

        private ProviderPluginDataController(string pluginXmlFilePath)
        {
            _PluginXmlFilePath = pluginXmlFilePath;
        }

        /// <summary>
        /// loads a plugin.xml file
        /// </summary>
        /// <param name="pluginXmlFilePath"></param>
        /// <returns></returns>
        private IProviderPluginData LoadData()
        {
            IProviderPluginData data = new ProviderPluginData();

            XDocument doc = XDocument.Load(_PluginXmlFilePath);
            XElement xmlProviderPlugin = doc.Element("ProviderPlugin");
            XElement xmlAssembliesRoot = xmlProviderPlugin.Element("Assemblies");

            // load ProviderPlugin data
            data.Id = new Guid(GetRequiredAttributeValue(xmlProviderPlugin, "id"));
            data.Name = GetRequiredElementValue(xmlProviderPlugin, "Name");
            data.Description = GetRequiredElementValue(xmlProviderPlugin, "Description");

            data.Assemblies = LoadAssemblies(xmlAssembliesRoot);

            data.PluginXmlFilePath = _PluginXmlFilePath;
            data.PluginDirectoryPath = System.IO.Directory.GetParent(_PluginXmlFilePath).ToString();

            return data;
        }

        private IList<IProviderPluginAssemblyData> LoadAssemblies(XElement xmlAssembliesRoot)
        {
            List<IProviderPluginAssemblyData> assemblyData = new List<IProviderPluginAssemblyData>();

            if (xmlAssembliesRoot != null)
            {
                foreach (XElement xmlItem in xmlAssembliesRoot.Elements("Assembly"))
                {
                    ProviderPluginAssemblyData data = new ProviderPluginAssemblyData();
                    data.Path = GetRequiredAttributeValue(xmlItem, "path");
                    data.RequiredInterfaceVersion = new Version(GetRequiredAttributeValue(xmlItem, "requiredInterfaceVersion"));
                    data.Instances = LoadInstances(xmlItem.Element("Instances"));

                    assemblyData.Add(data);
                }
            }

            return assemblyData;
        }

        private IList<IProviderPluginInstanceData> LoadInstances(XElement xmlInstancesRoot)
        {
            List<IProviderPluginInstanceData> instanceData = new List<IProviderPluginInstanceData>();

            if (xmlInstancesRoot != null)
            {
                foreach (XElement xmlItem in xmlInstancesRoot.Elements("Instance"))
                {
                    ProviderPluginInstanceData data = new ProviderPluginInstanceData();
                    data.Id = new Guid(GetRequiredAttributeValue(xmlItem, "id"));
                    data.Name = GetRequiredElementValue(xmlItem, "Name");
                    data.Description = GetRequiredElementValue(xmlItem, "Description");

                    instanceData.Add(data);
                }
            }

            return instanceData;
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
                throw new XmlLoadingException("Provider Plugin", _PluginXmlFilePath, msg);
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
                throw new XmlLoadingException("Provider Plugin", _PluginXmlFilePath, msg);
            }
        }

        #endregion

        #endregion
    }
}
