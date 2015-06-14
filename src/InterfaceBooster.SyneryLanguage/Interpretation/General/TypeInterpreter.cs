using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.ErrorHandling;
using InterfaceBooster.Common.Interfaces.SyneryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.SyneryTypes;
using InterfaceBooster.Common.Interfaces.Utilities.SyneryLanguage;

namespace InterfaceBooster.SyneryLanguage.Interpretation.General
{
    /// <summary>
    /// Interprets the types in Synery and translates them into .NET types.
    /// </summary>
    public class TypeInterpreter : IInterpreter<SyneryParser.TypeContext, SyneryType>, IInterpreter<SyneryParser.PrimitiveTypeContext, SyneryType>
    {
        #region PROPERTIES

        public ISyneryMemory Memory { get; set; }
        public IInterpretationController Controller { get; set; }

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Gets an object that contains the corresponding .NET Type for the given type in Synery.
        /// </summary>
        /// <exception cref="SyneryException">Thrown when no matching type was found</exception>
        /// <param name="context"></param>
        /// <returns>the representation of the .Net Type</returns>
        public SyneryType RunWithResult(SyneryParser.TypeContext context)
        {
            if (context.primitiveType() != null)
            {
                return RunWithResult(context.primitiveType());
            }
            else if (context.recordType() != null)
            {
                // it's a record type

                string recordTypeName = RecordHelper.ParseRecordTypeName(context.recordType().GetText());

                if (Memory.IsInitialized != true)
                {
                    // suppose the record types haven't been initialized yet
                    // but this method is also used during the initialization of record types
                    // for that reason we need to create a new type references

                    return TypeHelper.GetSyneryType(typeof(IRecord), recordTypeName);
                }
                else
                {
                    // search for an existing record type in the memory to work with the same instance as much as possible

                    SyneryType recordType = (from t in Memory.RecordTypes
                                             where t.Key.Name == recordTypeName
                                             select t.Key).FirstOrDefault();

                    // check whether the record type was found

                    if (recordType != null)
                    {
                        return recordType;
                    }
                }

                throw new SyneryInterpretationException(context, string.Format(
                    "Record type with name '{0}' was not found.", recordTypeName));
            }

            throw new SyneryInterpretationException(context, string.Format(
                "Unknown type expression in {0}: '{1}'", this.GetType().Name, context.GetText()));
        }

        public SyneryType RunWithResult(SyneryParser.PrimitiveTypeContext context)
        {
            if (context.STRING() != null)
                return TypeHelper.STRING_TYPE;
            if (context.BOOL() != null)
                return TypeHelper.BOOL_TYPE;
            if (context.INT() != null)
                return TypeHelper.INT_TYPE;
            if (context.DECIMAL() != null)
                return TypeHelper.DECIMAL_TYPE;
            if (context.DOUBLE() != null)
                return TypeHelper.DOUBLE_TYPE;
            if (context.CHAR() != null)
                return TypeHelper.CHAR_TYPE;
            if (context.DATETIME() != null)
                return TypeHelper.DATETIME_TYPE;

            return null;
        }

        #endregion
    }
}
