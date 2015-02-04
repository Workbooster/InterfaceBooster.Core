using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.SyneryTypes;

namespace InterfaceBooster.SyneryLanguage.Model.SyneryTypes
{
    /// <summary>
    /// This is the conceptual declaration of a value field for a record type.
    /// </summary>
    public class RecordTypeField : IRecordTypeField
    {
        #region PROPERTIES

        /// <summary>
        /// Gets the recory type that contains this field.
        /// </summary>
        public IRecordType RecordType { get; private set; }
        
        /// <summary>
        /// Gets or sets the identifier for the field.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value type of the field.
        /// </summary>
        public SyneryType Type { get; set; }
        
        /// <summary>
        /// Gets or sets a default value that is returned when the field value is NULL.
        /// </summary>
        public IValue DefaultValue { get; set; }

        #endregion

        #region PUBLIC METHODS

        public RecordTypeField(IRecordType recordType)
        {
            RecordType = recordType;
        }

        #endregion
    }
}
