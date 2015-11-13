using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;
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
    /// The base record type for all events in Synery.
    /// In Synery an event can be emited by the system or by the developer. An HANDLE block can catch an event to work with it.
    /// An event doesn't break the execution of the current code block.
    /// </summary>
    public class EventRecord : Record
    {
        public static string RECORD_TYPE_NAME = ".Event";

        #region MEMBERS

        private static IRecordType _RecordType;

        #endregion

        #region PROPERTIES

        public bool IsHandled
        {
            get { return (bool)this.GetFieldValue("IsHandled").Value; }
            set { this.SetFieldValue("IsHandled", new TypedValue(TypeHelper.BOOL_TYPE, value)); }
        }


        #endregion

        #region PUBLIC METHODS

        public EventRecord() : base(GetRecordType()) { }

        public IValue GetAsSyneryValue()
        {
            return new TypedValue(new SyneryType(typeof(IRecord), this.RecordType.Name), this);
        }

        public static IRecordType GetRecordType()
        {
            if (_RecordType != null) return _RecordType;

            _RecordType = new RecordType(RECORD_TYPE_NAME);
            _RecordType.AddField("IsHandled", TypeHelper.BOOL_TYPE, new TypedValue(TypeHelper.BOOL_TYPE, false));

            return _RecordType;
        }

        #endregion
    }
}
