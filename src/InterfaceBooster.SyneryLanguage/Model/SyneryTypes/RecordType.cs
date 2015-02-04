using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.SyneryTypes;

namespace InterfaceBooster.SyneryLanguage.Model.SyneryTypes
{
    /// <summary>
    /// This is a conceptual declaration of a record. It contains the identifier, the fields and the base type.
    /// </summary>
    public class RecordType : IRecordType
    {
        #region MEMBERS

        private List<IRecordTypeField> _Fields = new List<IRecordTypeField>();

        #endregion

        #region PROPERTIES

        /// <summary>
        /// Gets the internal identifier of the record type.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the full identifier of the record type with the format [CodeFileAlias].[Name]
        /// </summary>
        public string FullName
        {
            get
            {
                // check whether an alias is available -> return CodeFileAlias.Name
                if (!string.IsNullOrEmpty(CodeFileAlias))
                    return String.Format("{0}.{1}", CodeFileAlias, Name);

                // no alias -> only return Name
                return Name;
            }
        }

        /// <summary>
        /// Ges or sets the alias of the code file. This is part of the identifier.
        /// (= null if its an internal record type from the same synery code file)
        /// </summary>
        public string CodeFileAlias { get; set; }

        /// <summary>
        /// Gets the record type from which the this record type derives.
        /// </summary>
        public IRecordType BaseRecordType { get; private set; }

        /// <summary>
        /// Gets the declaration of all fields of that type.
        /// </summary>
        public IReadOnlyList<IRecordTypeField> Fields { get { return _Fields; } }

        #endregion

        #region PUBLIC METHODS

        public RecordType(string name, IRecordType baseRecordType = null)
        {
            Name = name;

            if (baseRecordType != null)
            {
                InitializeBaseType(baseRecordType);
            }
        }

        /// <summary>
        /// Creates a new field declaration and adds it to the record type.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public IRecordTypeField AddField(string name, SyneryType type, IValue defaultValue = null)
        {
            return AddFieldWithParentType(name, type, this, defaultValue);
        }

        /// <summary>
        /// Tries to find the field with the specified <paramref name="name"/>. Returns null if no field was found.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IRecordTypeField GetField(string name)
        {
            return _Fields.FirstOrDefault(f => f.Name == name);
        }

        /// <summary>
        /// Removes the given <paramref name="field"/> from this record type.
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public bool DeleteField(IRecordTypeField field)
        {
            return _Fields.Remove(field);
        }

        /// <summary>
        /// Removes the field with the given <paramref name="nane"/> from this record type.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool DeleteField(string name)
        {
            return _Fields.RemoveAll(f => f.Name == name) > 0;
        }

        /// <summary>
        /// Checks whether this record type is the type with the given <paramref name="typeName"/> or derives from the searched type.
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public bool IsType(string typeName)
        {
            if (FullName == typeName)
            {
                return true;
            }
            else
            {
                if (BaseRecordType != null)
                {
                    return BaseRecordType.IsType(typeName);
                }

                return false;
            }
        }

        #endregion

        #region INTERNAL METHODS

        /// <summary>
        /// Creates a field using the given parameters and adds it to the list of fields
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="parentType"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        private IRecordTypeField AddFieldWithParentType(string name, SyneryType type, IRecordType parentType, IValue defaultValue = null)
        {
            IRecordTypeField field = new RecordTypeField(parentType)
            {
                Name = name,
                Type = type,
                DefaultValue = defaultValue,
            };

            _Fields.Add(field);

            return field;
        }

        /// <summary>
        /// Sets the BaseRecordType-Property and copies all the fields from the base type
        /// </summary>
        /// <param name="baseRecordType"></param>
        private void InitializeBaseType(IRecordType baseRecordType)
        {
            BaseRecordType = baseRecordType;

            foreach (var field in baseRecordType.Fields)
            {
                AddFieldWithParentType(field.Name, field.Type, baseRecordType, field.DefaultValue);
            }
        }

        #endregion
    }
}
