using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using InterfaceBooster.Core.Common.Xml;
using InterfaceBooster.Common.Interfaces.ErrorHandling;
using InterfaceBooster.Common.Interfaces.InterfaceDefinition.Data;
using InterfaceBooster.Common.Tools.Data.Xml;

namespace InterfaceBooster.Core.InterfaceDefinitions
{
    /// <summary>
    /// Handels the loading of the Import Definition XML file.
    /// </summary>
    public class InterfaceDefinitionDataController
    {
        #region MEMBERS
        
        protected string _InterfaceDefinitionFilePath;

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Loads the data of an import definition.
        /// Can throw exceptions if the loading fails.
        /// </summary>
        /// <param name="interfaceDefinitionFilePath">the path of the XML file</param>
        /// <returns></returns>
        public static InterfaceDefinitionData Load(string interfaceDefinitionFilePath)
        {
            InterfaceDefinitionDataController controller = new InterfaceDefinitionDataController(interfaceDefinitionFilePath);

            InterfaceDefinitionData data = controller.LoadDefinition();

            return data;
        }

        #endregion

        #region INTERNAL METHODS

        /// <summary>
        /// Constructor may only be used by the static method "Load".
        /// </summary>
        /// <param name="interfaceDefinitionFilePath"></param>
        private InterfaceDefinitionDataController(string interfaceDefinitionFilePath)
        {
            _InterfaceDefinitionFilePath = interfaceDefinitionFilePath;
        }

        private InterfaceDefinitionData LoadDefinition()
        {
            InterfaceDefinitionData data = new InterfaceDefinitionData();

            XDocument doc = XDocument.Load(_InterfaceDefinitionFilePath);
            XElement root = doc.Element("ImportDefinition");

            data.Id = new Guid(GetRequiredAttributeValue(root, "id"));
            data.Details = LoadDetails(root.Element("Details"));
            data.Jobs = LoadJobs(root.Element("Jobs"));

            LoadRequiredPlugins(data, root.Element("RequiredPlugins"));

            return data;
        }

        private InterfaceDefinitionDetailData LoadDetails(XElement root)
        {
            if (root == null)
                throw new XmlLoadingException(
                    "Interface Definition",
                    _InterfaceDefinitionFilePath, 
                    "The Definition must contain a 'Details' XML-Node.");

            InterfaceDefinitionDetailData data = new InterfaceDefinitionDetailData();

            data.Name = GetRequiredElementValue(root, "Name");
            data.Description = GetRequiredElementValue(root, "Description");
            data.Author = GetRequiredElementValue(root, "Author");
            data.DateOfCreation = GetRequiredDateTimeElementValue(root, "DateOfCreation");
            data.DateOfLastChange = GetRequiredDateTimeElementValue(root, "DateOfLastChange");
            data.Version = new Version(GetRequiredElementValue(root, "Version"));
            data.RequiredRuntimeVersion = new Version(GetRequiredElementValue(root, "RequiredRuntimeVersion"));

            return data;
        }

        private IList<InterfaceDefinitionJobData> LoadJobs(XElement root)
        {
            IList<InterfaceDefinitionJobData> list = new List<InterfaceDefinitionJobData>();

            if (root != null)
            {
                foreach (var xmlItem in root.Elements("Job"))
                {
                    list.Add(LoadJob(xmlItem));
                }
            }

            return list;
        }

        private InterfaceDefinitionJobData LoadJob(XElement root)
        {
            InterfaceDefinitionJobData data = new InterfaceDefinitionJobData();

            data.Id = new Guid(GetRequiredAttributeValue(root, "id"));
            data.Name = GetRequiredElementValue(root, "Name");
            data.Description = GetRequiredElementValue(root, "Description");
            data.EstimatedDurationRemarks = GetRequiredElementValue(root, "EstimatedDurationRemarks");
            data.IncludeFiles = LoadIncludeFiles(root.Element("IncludeFiles"));

            return data;
        }

        /// <summary>
        /// loads the data of all required plugins and appends them to the given <paramref name="data"/>.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="root"></param>
        private void LoadRequiredPlugins(InterfaceDefinitionData data, XElement root)
        {
            data.RequiredProviderPluginInstances = LoadProviderPluginInstances(root.Element("ProviderPlugins"));
            data.RequiredLibraryPlugins = LoadLibraryPlugins(root.Element("LibraryPlugins"));
        }

        private IList<ProviderPluginInstanceReference> LoadProviderPluginInstances(XElement root)
        {
            List<ProviderPluginInstanceReference> list = new List<ProviderPluginInstanceReference>();

            if (root != null)
            {
                foreach (var xmlItem in root.Elements("ProviderPluginInstance"))
                {
                    list.Add(LoadProviderPluginInstance(xmlItem));
                }
            }

            return list;
        }

        private IList<LibraryPluginReference> LoadLibraryPlugins(XElement root)
        {
            List<LibraryPluginReference> list = new List<LibraryPluginReference>();

            if (root != null)
            {
                foreach (var xmlItem in root.Elements("LibraryPlugin"))
                {
                    list.Add(LoadLibraryPlugin(xmlItem));
                }
            }

            return list;
        }

        private IDictionary<string, string> LoadIncludeFiles(XElement root)
        {
            IDictionary<string, string> list = new Dictionary<string, string>();

            if (root != null)
            {
                foreach (var xmlItem in root.Elements("IncludeFile"))
                {
                    list.Add(LoadIncludeFile(xmlItem));
                }
            }

            return list;
        }

        private KeyValuePair<string, string> LoadIncludeFile(XElement root)
        {
            return new KeyValuePair<string, string>(
                GetRequiredAttributeValue(root, "alias"),
                GetRequiredAttributeValue(root, "relativePath")
                );
        }

        private ProviderPluginInstanceReference LoadProviderPluginInstance(XElement root)
        {
            ProviderPluginInstanceReference data = new ProviderPluginInstanceReference();

            data.SyneryIdentifier = GetRequiredAttributeValue(root, "syneryIdentifier");
            data.IdPlugin = new Guid(GetRequiredAttributeValue(root, "idPlugin"));
            data.PluginName = GetRequiredAttributeValue(root, "pluginName");
            data.IdPluginInstance = new Guid(GetRequiredAttributeValue(root, "idPluginInstance"));
            data.PluginInstanceName = GetRequiredAttributeValue(root, "pluginInstanceName");

            return data;
        }

        private LibraryPluginReference LoadLibraryPlugin(XElement root)
        {
            LibraryPluginReference data = new LibraryPluginReference();

            data.SyneryIdentifier = GetRequiredAttributeValue(root, "syneryIdentifier");
            data.IdPlugin = new Guid(GetRequiredAttributeValue(root, "idPlugin"));
            data.PluginName = GetRequiredAttributeValue(root, "pluginName");

            return data;
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
                throw new XmlLoadingException("Interface Definition", _InterfaceDefinitionFilePath, msg);
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
                throw new XmlLoadingException("Interface Definition", _InterfaceDefinitionFilePath, msg);
            }
        }

        private DateTime GetRequiredDateTimeElementValue(XElement root, string name)
        {
            DateTime? value = XmlHelper.GetDateTimeElementValue(root, name);

            if (value != null)
            {
                return (DateTime)value;
            }
            else
            {
                string msg = String.Format("XML-node '{0}' is not available or does not contain a valid DateTime value in '{1}'.", name, PathHelper.GetElementFullPath(root, "\\"));
                throw new XmlLoadingException("Interface Definition", _InterfaceDefinitionFilePath, msg);
            }
        }

        #endregion

        #endregion
    }
}
