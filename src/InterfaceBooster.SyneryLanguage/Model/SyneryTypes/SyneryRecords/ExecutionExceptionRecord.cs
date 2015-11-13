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
    /// Occurs during the execution of the Synery script. Therefore it holds information about the position of the statement that causes the exception.
    /// </summary>
    public class ExecutionExceptionRecord : ExceptionRecord
    {        
        public static new string RECORD_TYPE_NAME = ".ExecutionException";

        #region MEMBERS

        private static IRecordType _RecordType;

        #endregion

        #region PROPERTIES

        public int? Line
        {
            get { return this.GetFieldValue("Line").Value as int?; }
            set { this.SetFieldValue("Line", new TypedValue(TypeHelper.INT_TYPE, value)); }
        }

        public int? CharPosition
        {
            get { return this.GetFieldValue("CharPosition").Value as int?; }
            set { this.SetFieldValue("CharPosition", new TypedValue(TypeHelper.INT_TYPE, value)); }
        }

        #endregion

        #region PUBLIC METHODS

        public ExecutionExceptionRecord()
            : base()
        {
            this.RecordType = GetRecordType();
        }

        public static new IRecordType GetRecordType()
        {
            if (_RecordType != null) return _RecordType;

            _RecordType = new RecordType(RECORD_TYPE_NAME, ExceptionRecord.GetRecordType());
            _RecordType.AddField("Line", TypeHelper.INT_TYPE);
            _RecordType.AddField("CharPosition", TypeHelper.INT_TYPE);

            return _RecordType;
        }

        #endregion
    }
}
