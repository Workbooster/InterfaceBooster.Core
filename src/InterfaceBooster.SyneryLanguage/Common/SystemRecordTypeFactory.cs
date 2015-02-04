using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.SyneryLanguage.Interpretation.General;
using InterfaceBooster.SyneryLanguage.Model.SyneryTypes;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.SyneryTypes;

namespace InterfaceBooster.SyneryLanguage.Common
{
    public static class SystemRecordTypeFactory
    {
        #region INTERNAL STRUCTS

        private struct SystemRecordTypeDefinition
        {
            public string Name { get; set; }
            public string BaseTypeName { get; set; }
            public List<SystemRecordTypeFieldDefinition> Fields { get; set; }

            public SystemRecordTypeDefinition(string name, string baseTypeName = null)
                : this()
            {
                Name = name;
                BaseTypeName = baseTypeName;
            }
        }

        private struct SystemRecordTypeFieldDefinition
        {
            public string Name { get; set; }
            public SyneryType Type { get; set; }
            public IValue DefaultValue { get; set; }

            public SystemRecordTypeFieldDefinition(string name, SyneryType type, IValue defaultValue = null)
                : this()
            {
                Name = name;
                Type = type;
                DefaultValue = defaultValue;
            }
        }

        #endregion

        #region CONSTANT VALUES

        public static string EVENT_NAME = ".Event";
        public static string EXCEPTION_NAME = ".Exception";

        #endregion

        #region MEMBERS

        /// <summary>
        /// Contains the definition for all system record types.
        /// </summary>
        private static List<SystemRecordTypeDefinition> _SystemRecordTypeDefinitions =
            new List<SystemRecordTypeDefinition>() {
                new SystemRecordTypeDefinition(EVENT_NAME) {
                    Fields = new List<SystemRecordTypeFieldDefinition>() {
                        new SystemRecordTypeFieldDefinition("IsHandled", TypeHelper.BOOL_TYPE),
                    }
                },
                new SystemRecordTypeDefinition(EXCEPTION_NAME) {
                    Fields = new List<SystemRecordTypeFieldDefinition>() {
                        new SystemRecordTypeFieldDefinition("Message", TypeHelper.STRING_TYPE),
                    }
                },
            };

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Get a dictinary with all record types provided by Interface Booster.
        /// </summary>
        /// <returns></returns>
        public static IDictionary<SyneryType, IRecordType> GetSystemRecordTypes()
        {
            Dictionary<SyneryType, IRecordType> listOfRecordTypes = new Dictionary<SyneryType, IRecordType>();

            // dynamically build the record type list by using the internal definitions

            foreach (var definition in _SystemRecordTypeDefinitions)
            {
                var baseType = (from t in listOfRecordTypes
                                where t.Value.FullName == definition.BaseTypeName
                                select t.Value).FirstOrDefault();

                KeyValuePair<SyneryType, IRecordType> data = GetRecordType(definition.Name, baseType);

                foreach (var field in definition.Fields)
                {
                    data.Value.AddField(field.Name, field.Type, field.DefaultValue);
                }

                listOfRecordTypes.Add(data.Key, data.Value);
            }


            return listOfRecordTypes;
        }

        #endregion

        #region INTERNAL METHODS

        private static KeyValuePair<SyneryType, IRecordType> GetRecordType(string name, IRecordType baseRecordType)
        {
            SyneryType syneryType = new SyneryType(typeof(IRecord), name);
            IRecordType recordType = new RecordType(name, baseRecordType);

            return new KeyValuePair<SyneryType, IRecordType>(syneryType, recordType);
        }

        #endregion
    }
}
