using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.ErrorHandling;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.SyneryTypes;

namespace InterfaceBooster.SyneryLanguage.Interpretation.General
{
    public static class RecordHelper
    {
        /// <summary>
        /// Parses the given identifier <paramref name="value"/> from synery code an returns a clean record type name.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ParseRecordTypeName(string value)
        {
            return value.TrimStart('#');
        }

        /// <summary>
        /// Checks whether the record type <paramref name="checkTypeName"/> is derived from or equals to the given
        /// <paramref name="baseTypeName"/> record type.
        /// </summary>
        /// <param name="memory">The current memory state. Is needed to access the list of record types.</param>
        /// <param name="checkTypeName">the full name of the type that should be checked</param>
        /// <param name="baseTypeName">the full name of the potential base type</param>
        /// <returns>true = type is derived from or equal to the base type</returns>
        public static bool IsDerivedType(ISyneryMemory memory, string checkTypeName, string baseTypeName)
        {
            if (checkTypeName == baseTypeName)
            {
                return true;
            }

            IRecordType checkType = (from t in memory.RecordTypes.Values
                                    where t.Name == checkTypeName
                                    select t).FirstOrDefault();

            if (checkType == null)
                throw new SyneryException(String.Format("Record type wiht name='{0}' not found", baseTypeName));

            return checkType.IsType(baseTypeName);
        }
    }
}
