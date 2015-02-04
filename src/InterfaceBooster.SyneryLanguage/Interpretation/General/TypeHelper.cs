using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;

namespace InterfaceBooster.SyneryLanguage.Interpretation.General
{
    public static class TypeHelper
    {
        public static SyneryType STRING_TYPE = new SyneryType(typeof(string));
        public static SyneryType BOOL_TYPE = new SyneryType(typeof(bool));
        public static SyneryType INT_TYPE = new SyneryType(typeof(int));
        public static SyneryType DECIMAL_TYPE = new SyneryType(typeof(decimal));
        public static SyneryType DOUBLE_TYPE = new SyneryType(typeof(double));
        public static SyneryType CHAR_TYPE = new SyneryType(typeof(char));
        public static SyneryType DATETIME_TYPE = new SyneryType(typeof(DateTime));

        public static SyneryType GetSyneryType(Type type, string name = null)
        {
            if (type == typeof(string)) return STRING_TYPE;
            if (type == typeof(bool)) return BOOL_TYPE;
            if (type == typeof(int)) return INT_TYPE;
            if (type == typeof(decimal)) return DECIMAL_TYPE;
            if (type == typeof(double)) return DOUBLE_TYPE;
            if (type == typeof(char)) return CHAR_TYPE;
            if (type == typeof(DateTime)) return DATETIME_TYPE;

            return new SyneryType(type, name);
        }
    }
}
