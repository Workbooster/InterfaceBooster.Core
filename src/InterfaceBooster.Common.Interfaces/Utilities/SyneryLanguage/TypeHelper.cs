using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Common.Interfaces.Utilities.SyneryLanguage
{
    public static class TypeHelper
    {
        public static Type STRING_DOTNET_TYPE = typeof(string);
        public static Type BOOL_DOTNET_TYPE = typeof(bool);
        public static Type INT_DOTNET_TYPE = typeof(int);
        public static Type DECIMAL_DOTNET_TYPE = typeof(decimal);
        public static Type DOUBLE_DOTNET_TYPE = typeof(double);
        public static Type CHAR_DOTNET_TYPE = typeof(char);
        public static Type DATETIME_DOTNET_TYPE = typeof(DateTime);

        public static SyneryType STRING_TYPE = new SyneryType(STRING_DOTNET_TYPE);
        public static SyneryType BOOL_TYPE = new SyneryType(BOOL_DOTNET_TYPE);
        public static SyneryType INT_TYPE = new SyneryType(INT_DOTNET_TYPE);
        public static SyneryType DECIMAL_TYPE = new SyneryType(DECIMAL_DOTNET_TYPE);
        public static SyneryType DOUBLE_TYPE = new SyneryType(DOUBLE_DOTNET_TYPE);
        public static SyneryType CHAR_TYPE = new SyneryType(CHAR_DOTNET_TYPE);
        public static SyneryType DATETIME_TYPE = new SyneryType(DATETIME_DOTNET_TYPE);

        public static Type[] SUPPORTED_DOTNET_TYPES = new Type[] { STRING_DOTNET_TYPE, BOOL_DOTNET_TYPE, INT_DOTNET_TYPE, DECIMAL_DOTNET_TYPE, DOUBLE_DOTNET_TYPE, CHAR_DOTNET_TYPE, DATETIME_DOTNET_TYPE };

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

        public static bool IsPrimitiveType(SyneryType syneryType)
        {
            return SUPPORTED_DOTNET_TYPES.Contains(syneryType.UnterlyingDotNetType);
        }
    }
}
