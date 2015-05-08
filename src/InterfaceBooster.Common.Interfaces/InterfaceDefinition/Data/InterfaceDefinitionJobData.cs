using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Common.Interfaces.InterfaceDefinition.Data
{
    /// <summary>
    /// Container for the data of an Import Definition Job. An Import Definition can contain multiple jobs.
    /// </summary>
    public class InterfaceDefinitionJobData
    {
        #region MEMBERS

        private Dictionary<string, string> _IncludeFiles;

        #endregion

        #region PROPERTIES

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string EstimatedDurationRemarks { get; set; }
        public InterfaceDefinitionData InterfaceDefinition { get; set; }

        public Dictionary<string, string> IncludeFiles
        {
            get
            {
                if (_IncludeFiles == null) _IncludeFiles = new Dictionary<string, string>(); // create new if null
                return _IncludeFiles;
            }
            set { _IncludeFiles = value; }
        }

        #endregion
    }
}
