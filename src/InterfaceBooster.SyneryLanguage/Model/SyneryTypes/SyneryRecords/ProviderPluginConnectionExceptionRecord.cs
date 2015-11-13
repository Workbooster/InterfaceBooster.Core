using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.SyneryTypes;
using InterfaceBooster.Common.Interfaces.Utilities.SyneryLanguage;
using InterfaceBooster.SyneryLanguage.Model.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.SyneryLanguage.Model.SyneryTypes.SyneryRecords
{
    /// <summary>
    /// Occurs if there was an error while trying to open a new provider plugin connection. This exception is usually related to a CONNECT statement.
    /// </summary>
    public class ProviderPluginConnectionExceptionRecord : ExecutionExceptionRecord
    {        
        public static new string RECORD_TYPE_NAME = ".ProviderPluginConnectionException";

        #region MEMBERS

        private static IRecordType _RecordType;

        #endregion

        #region PROPERTIES

        public string ConnectionPath
        {
            get { return this.GetFieldValue("ConnectionPath").Value as string; }
            set { this.SetFieldValue("ConnectionPath", new TypedValue(TypeHelper.STRING_TYPE, value)); }
        }

        public string PluginInstanceReferenceIdentifier
        {
            get { return this.GetFieldValue("PluginInstanceReferenceIdentifier").Value as string; }
            set { this.SetFieldValue("PluginInstanceReferenceIdentifier", new TypedValue(TypeHelper.STRING_TYPE, value)); }
        }

        #endregion

        #region PUBLIC METHODS

        public ProviderPluginConnectionExceptionRecord()
            : base()
        {
            this.RecordType = GetRecordType();
        }

        public static new IRecordType GetRecordType()
        {
            if (_RecordType != null) return _RecordType;

            _RecordType = new RecordType(RECORD_TYPE_NAME, ExecutionExceptionRecord.GetRecordType());
            _RecordType.AddField("ConnectionPath", TypeHelper.STRING_TYPE);
            _RecordType.AddField("PluginInstanceReferenceIdentifier", TypeHelper.STRING_TYPE);

            return _RecordType;
        }

        #endregion
    }
}
