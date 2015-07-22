using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.ErrorHandling;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.SyneryTypes;

namespace InterfaceBooster.SyneryLanguage.Model.SyneryTypes
{
    public class Record : IRecord
    {
        #region MEMBERS

        /// <summary>
        /// Internal dictionary for storing field values.
        /// </summary>
        private Dictionary<string, IValue> _Data;

        #endregion

        #region PROPERTIES

        /// <summary>
        /// Gets all the field values.
        /// </summary>
        public IReadOnlyDictionary<string, IValue> Data { get { return _Data; } }

        /// <summary>
        /// Gets the definition of the current record.
        /// </summary>
        public IRecordType RecordType { get; private set; }

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// A record is an object composition that can take an indefinite number of fields. 
        /// A field can take a primitive value or another record type.
        /// </summary>
        /// <param name="recordType"></param>
        public Record(IRecordType recordType)
        {
            _Data = new Dictionary<string, IValue>();
            RecordType = recordType;
        }

        /// <summary>
        /// Tries to set the value of the field with the given <paramref name="name"/>.
        /// Throws an exception if the field doesn't exists.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void SetFieldValue(string name, IValue value)
        {
            IRecordTypeField field = RecordType.GetField(name);

            // Validation: Is it a known field?
            if (field == null)
                throw new SyneryException(String.Format("Field with name='{0}' doesn't exists in record '{1}'.", name, RecordType.FullName));

            // Handle NULL values: Create a typed value by taking the SyneryType from the field definition
            if (value.Type == null)
                value = new TypedValue(field.Type);

            // Validation: Do the field type and the type of the given value match?
            if (field.Type != value.Type)
                throw new SyneryException(String.Format("Field type '{0}' and value type '{1}' don't match. Field name='{2}'.", 
                    field.Type.PublicName, value.Type.PublicName, name));

            // OK! Set the field value

            _Data[name] = value;
        }

        /// <summary>
        /// Tries to get the value of the field with the given <paramref name="name"/>.
        /// Throws an exception if the field doesn't exists.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IValue GetFieldValue(string name)
        {
            if (_Data.ContainsKey(name))
            {
                // a value for the field is available

                return _Data[name];
            }
            else
            {
                // try to find the field

                IRecordTypeField field = RecordType.GetField(name);

                if (field == null)
                    throw new SyneryException(String.Format("Field with name='{0}' doesn't exists in record '{1}'.", name, RecordType.FullName));

                // no value has been set yet

                if (field.DefaultValue != null)
                {
                    // the field has a default value
                    return field.DefaultValue;
                }
                else
                {
                    // the field value is still NULL
                    return new TypedValue(field.Type);
                }
            }
        }

        /// <summary>
        /// Check whether the field with the given <paramref name="name"/> exists in the record type.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool DoesFieldExists(string name)
        {
            IRecordTypeField field = RecordType.GetField(name);

            if (field != null)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks whether the current record is an instance of the given type (also consider derived type)
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public bool InstanceOf(string typeName)
        {
            return RecordType.IsType(typeName);
        }

        #endregion
    }
}
