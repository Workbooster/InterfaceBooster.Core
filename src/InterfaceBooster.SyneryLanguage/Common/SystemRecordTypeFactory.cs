using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.SyneryLanguage.Interpretation.General;
using InterfaceBooster.SyneryLanguage.Model.SyneryTypes;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.SyneryTypes;
using InterfaceBooster.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.Utilities.SyneryLanguage;
using InterfaceBooster.SyneryLanguage.Model.SyneryTypes.SyneryRecords;

namespace InterfaceBooster.SyneryLanguage.Common
{
    public static class SystemRecordTypeFactory
    {
        #region PUBLIC METHODS

        /// <summary>
        /// Get a dictionary with all the system record types provided by Interface Booster.
        /// </summary>
        /// <returns></returns>
        public static IDictionary<SyneryType, IRecordType> GetSystemRecordTypes()
        {
            IDictionary<SyneryType, IRecordType> listOfRecordTypes = new Dictionary<SyneryType, IRecordType>();

            // get the signatures of the default system record types

            listOfRecordTypes.Add(GetRecordTypeSignature(EventRecord.GetRecordType()));
            listOfRecordTypes.Add(GetRecordTypeSignature(ExceptionRecord.GetRecordType()));
            listOfRecordTypes.Add(GetRecordTypeSignature(ExecutionExceptionRecord.GetRecordType()));
            listOfRecordTypes.Add(GetRecordTypeSignature(LibraryPluginExceptionRecord.GetRecordType()));
            listOfRecordTypes.Add(GetRecordTypeSignature(ProviderPluginConnectionExceptionRecord.GetRecordType()));
            listOfRecordTypes.Add(GetRecordTypeSignature(ProviderPluginDataExchangeExceptionRecord.GetRecordType()));

            return listOfRecordTypes;
        }

        #endregion

        #region INTERNAL METHODS

        private static KeyValuePair<SyneryType, IRecordType> GetRecordTypeSignature(IRecordType recordType)
        {
            SyneryType syneryType = new SyneryType(typeof(IRecord), recordType.Name);

            return new KeyValuePair<SyneryType, IRecordType>(syneryType, recordType);
        }

        #endregion
    }
}
