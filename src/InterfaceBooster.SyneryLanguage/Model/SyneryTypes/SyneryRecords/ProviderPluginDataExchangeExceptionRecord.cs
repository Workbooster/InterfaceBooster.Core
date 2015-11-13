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
    /// Occurs if there was an error while exchanging data with a Provider Plugin. This exception is usually related to a data commands like READ, CREATE, EXECUTE etc.
    /// </summary>
    public class ProviderPluginDataExchangeExceptionRecord : ExecutionExceptionRecord
    {        
        public static new string RECORD_TYPE_NAME = ".ProviderPluginDataExchangeException";

        #region MEMBERS

        private static IRecordType _RecordType;

        #endregion

        #region PROPERTIES

        public string DataCommandType
        {
            get { return this.GetFieldValue("DataCommandType").Value as string; }
            set { this.SetFieldValue("DataCommandType", new TypedValue(TypeHelper.STRING_TYPE, value)); }
        }

        public string FullPath
        {
            get { return this.GetFieldValue("FullPath").Value as string; }
            set { this.SetFieldValue("FullPath", new TypedValue(TypeHelper.STRING_TYPE, value)); }
        }

        #endregion

        #region PUBLIC METHODS

        public ProviderPluginDataExchangeExceptionRecord()
            : base()
        {
            this.RecordType = GetRecordType();
        }

        public static new IRecordType GetRecordType()
        {
            if (_RecordType != null) return _RecordType;

            _RecordType = new RecordType(RECORD_TYPE_NAME, ExecutionExceptionRecord.GetRecordType());
            _RecordType.AddField("DataCommandType", TypeHelper.STRING_TYPE);
            _RecordType.AddField("FullPath", TypeHelper.STRING_TYPE);

            return _RecordType;
        }

        #endregion
    }
}
