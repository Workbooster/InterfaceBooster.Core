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
    /// The base record type for all exceptions in Synery.
    /// In Synery an exception can be thrown by the system or by the developer. An HANDLE block can catch an exception to work with it.
    /// An exception inherits from the #.Event record type. But as a difference to events exceptions do break the execution of the current code block.
    /// </summary>
    public class ExceptionRecord : EventRecord
    {
        public static new string RECORD_TYPE_NAME = ".Exception";

        #region MEMBERS

        private static IRecordType _RecordType;

        #endregion

        #region PROPERTIES

        public string Message
        {
            get { return this.GetFieldValue("Message").Value as string; }
            set { this.SetFieldValue("Message", new TypedValue(TypeHelper.STRING_TYPE, value)); }
        }

        #endregion

        #region PUBLIC METHODS

        public ExceptionRecord()
            : base()
        {
            this.RecordType = GetRecordType();
        }

        public static new IRecordType GetRecordType()
        {
            if (_RecordType != null) return _RecordType;

            _RecordType = new RecordType(RECORD_TYPE_NAME, EventRecord.GetRecordType());
            _RecordType.AddField("Message", TypeHelper.STRING_TYPE);

            return _RecordType;
        }

        #endregion
    }
}
