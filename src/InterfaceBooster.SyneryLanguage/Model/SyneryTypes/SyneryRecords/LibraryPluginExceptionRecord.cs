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
    public class LibraryPluginExceptionRecord : ExecutionExceptionRecord
    {
        public static new string RECORD_TYPE_NAME = ".LibraryPluginExceptionRecord";

        #region MEMBERS

        private static IRecordType _RecordType;

        #endregion

        #region PROPERTIES

        public string FullIdentifier
        {
            get { return this.GetFieldValue("FullIdentifier").Value as string; }
            set { this.SetFieldValue("FullIdentifier", new TypedValue(TypeHelper.STRING_TYPE, value)); }
        }

        public string LibraryPluginIdentifier
        {
            get { return this.GetFieldValue("LibraryPluginIdentifier").Value as string; }
            set { this.SetFieldValue("LibraryPluginIdentifier", new TypedValue(TypeHelper.STRING_TYPE, value)); }
        }

        #endregion

        #region PUBLIC METHODS

        public LibraryPluginExceptionRecord()
            : base()
        {
            this.RecordType = GetRecordType();
        }

        public static new IRecordType GetRecordType()
        {
            if (_RecordType != null) return _RecordType;

            _RecordType = new RecordType(RECORD_TYPE_NAME, ExecutionExceptionRecord.GetRecordType());
            _RecordType.AddField("FullIdentifier", TypeHelper.STRING_TYPE);
            _RecordType.AddField("LibraryPluginIdentifier", TypeHelper.STRING_TYPE);

            return _RecordType;
        }

        #endregion
    }
}
