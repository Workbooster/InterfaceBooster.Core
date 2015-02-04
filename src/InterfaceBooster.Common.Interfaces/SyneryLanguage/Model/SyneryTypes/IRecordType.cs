using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;

namespace InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.SyneryTypes
{
    /// <summary>
    /// This is a conceptual declaration of a record. It contains the identifier, the fields and the base type.
    /// </summary>
    public interface IRecordType
    {
        /// <summary>
        /// Gets the internal identifier of the record type.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the full identifier of the record type with the format [CodeFileAlias].[Name]
        /// </summary>
        string FullName { get; }

        /// <summary>
        /// Ges or sets the alias of the code file. This is part of the identifier.
        /// (= null if its an internal record type from the same synery code file)
        /// </summary>
        string CodeFileAlias { get; set; }

        /// <summary>
        /// Gets the record type from which the this record type derives.
        /// </summary>
        IRecordType BaseRecordType { get; }

        /// <summary>
        /// Gets the declaration of all fields of that type.
        /// </summary>
        IReadOnlyList<IRecordTypeField> Fields { get; }

        /// <summary>
        /// Creates a new field declaration and adds it to the record type.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        IRecordTypeField AddField(string name, SyneryType type, IValue defaultValue = null);

        /// <summary>
        /// Tries to find the field with the specified <paramref name="name"/>. Returns null if no field was found.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Returns null if no field was found.</returns>
        IRecordTypeField GetField(string name);

        /// <summary>
        /// Removes the given <paramref name="field"/> from this record type.
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        bool DeleteField(IRecordTypeField field);

        /// <summary>
        /// Removes the field with the given <paramref name="nane"/> from this record type.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool DeleteField(string name);

        /// <summary>
        /// Checks whether this record type is the type with the given <paramref name="typeName"/> or derives from the searched type.
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        bool IsType(string typeName);
    }
}
