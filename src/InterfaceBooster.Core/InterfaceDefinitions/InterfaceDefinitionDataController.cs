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
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace InterfaceBooster.Core.InterfaceDefinitions
{
    /// <summary>
    /// Handels the loading of the Interface Definition XML file.
    /// </summary>
    public class InterfaceDefinitionDataController
    {
        #region CONSTANTS

        public static readonly Encoding USED_ENCODING = Encoding.UTF8;

        #endregion

        #region MEMBERS

        protected string _InterfaceDefinitionFilePath;

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Loads the data of an Interface definition.
        /// Can throw exceptions if the loading fails.
        /// </summary>
        /// <param name="interfaceDefinitionFilePath">the path of the XML file</param>
        /// <returns></returns>
        public static InterfaceDefinitionData Load(string interfaceDefinitionFilePath)
        {
            // Validate parameters
            if (interfaceDefinitionFilePath == null)
                throw new ArgumentNullException("interfaceDefinitionFilePath", "The path of the interface definition file is required.");
            if (File.Exists(interfaceDefinitionFilePath) == false)
                throw new ArgumentException(String.Format("No interface definition found at {0}.", interfaceDefinitionFilePath), "interfaceDefinitionFilePath");

            InterfaceDefinitionDataController controller = new InterfaceDefinitionDataController(interfaceDefinitionFilePath);

            InterfaceDefinitionData data = controller.LoadDefinition();
            
            // store the interface definition root directory path
            data.RootDirectoryPath = Path.GetDirectoryName(interfaceDefinitionFilePath);

            return data;
        }

        public static void Save(string interfaceDefinitionFilePath, InterfaceDefinitionData definitionData)
        {
            // Validate parameters
            if (interfaceDefinitionFilePath == null)
                throw new ArgumentNullException("interfaceDefinitionFilePath", "The path of the interface definition file is required.");
            if (definitionData == null)
                throw new ArgumentNullException("definitionData", "The Interface Defintion data is required.");

            InterfaceDefinitionDataController controller = new InterfaceDefinitionDataController(interfaceDefinitionFilePath);
            
            controller.SaveDefintion(interfaceDefinitionFilePath, definitionData);
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

        #region SAVING

        private void SaveDefintion(string interfaceDefinitionFilePath, InterfaceDefinitionData data)
        {
            using (StreamWriter writer = new StreamWriter(interfaceDefinitionFilePath, false, USED_ENCODING))
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(writer, new XmlWriterSettings() { Indent = true, NewLineHandling = NewLineHandling.Entitize }))
                {
                    XmlSerializer s = new XmlSerializer(data.GetType());
                    s.Serialize(xmlWriter, data);
                }

            }
        }

        #endregion

        #region LOADING

        private InterfaceDefinitionData LoadDefinition()
        {
            InterfaceDefinitionData data = new InterfaceDefinitionData();

            using (StreamReader reader = new StreamReader(_InterfaceDefinitionFilePath, USED_ENCODING))
            {
                XDocument doc = XDocument.Load(_InterfaceDefinitionFilePath);
                XElement root = doc.Element("InterfaceDefinition");

                data.Id = new Guid(GetRequiredAttributeValue(root, "id"));
                data.Details = LoadDetails(root.Element("Details"));
                data.Jobs = LoadJobs(root.Element("Jobs"));

                LoadRequiredPlugins(data, root.Element("RequiredPlugins"));
            }

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
            data.Description = GetOptionalElementValue(root, "Description");
            data.Author = GetRequiredElementValue(root, "Author");
            data.DateOfCreation = GetRequiredDateTimeElementValue(root, "DateOfCreation");
            data.DateOfLastChange = GetRequiredDateTimeElementValue(root, "DateOfLastChange");
            data.Version = new Version(GetRequiredElementValue(root, "Version"));
            data.RequiredRuntimeVersion = new Version(GetRequiredElementValue(root, "RequiredRuntimeVersion"));

            return data;
        }

        private List<InterfaceDefinitionJobData> LoadJobs(XElement root)
        {
            List<InterfaceDefinitionJobData> list = new List<InterfaceDefinitionJobData>();

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
            data.Description = GetOptionalElementValue(root, "Description");
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
            data.RequiredPlugins.ProviderPluginInstances = LoadProviderPluginInstances(root.Element("ProviderPlugins"));
            data.RequiredPlugins.LibraryPlugins = LoadLibraryPlugins(root.Element("LibraryPlugins"));
        }

        private List<ProviderPluginInstanceReference> LoadProviderPluginInstances(XElement root)
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

        private List<LibraryPluginReference> LoadLibraryPlugins(XElement root)
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

        private List<IncludeFile> LoadIncludeFiles(XElement root)
        {
            List<IncludeFile> list = new List<IncludeFile>();

            if (root != null)
            {
                foreach (var xmlItem in root.Elements("IncludeFile"))
                {
                    list.Add(LoadIncludeFile(xmlItem));
                }
            }

            return list;
        }

        private IncludeFile LoadIncludeFile(XElement root)
        {
            return new IncludeFile()
            {
                Alias = GetRequiredAttributeValue(root, "alias"),
                RelativePath = GetRequiredAttributeValue(root, "relativePath"),
            };
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

        private string GetOptionalElementValue(XElement root, string name)
        {
            return XmlHelper.GetElementValue(root, name);
        }

        #endregion

        #endregion

        #endregion
    }
}
