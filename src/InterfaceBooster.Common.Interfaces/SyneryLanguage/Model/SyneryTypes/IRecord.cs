using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;

namespace InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.SyneryTypes
{
    /// <summary>
    /// A record is an object composition that can take an indefinite number of fields. 
    /// A field can take a primitive value or another record type. 
    /// </summary>
    public interface IRecord
    {
        /// <summary>
        /// Gets all the field values.
        /// </summary>
        IReadOnlyDictionary<string, IValue> Data { get; }

        /// <summary>
        /// Gets the definition of the current record.
        /// </summary>
        IRecordType RecordType { get; }

        /// <summary>
        /// Tries to set the value of the field with the given <paramref name="name"/>.
        /// Throws an exception if the field doesn't exists.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        void SetFieldValue(string name, IValue value);

        /// <summary>
        /// Tries to get the value of the field with the given <paramref name="name"/>.
        /// Throws an exception if the field doesn't exists.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IValue GetFieldValue(string name);

        /// <summary>
        /// Check whether the field with the given <paramref name="name"/> exists in the record type.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool DoesFieldExists(string name);

        /// <summary>
        /// Checks whether the current record is an instance of the given type (also consider derived type)
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        bool InstanceOf(string typeName);
    }
}
