using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.ErrorHandling;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.Utilities.SyneryLanguage;

namespace InterfaceBooster.SyneryLanguage.Interpretation.General
{
    public static class LiteralHelper
    {
        public static IValue STRING_DEFAULT_VALUE = new TypedValue(TypeHelper.STRING_TYPE, "");
        public static IValue BOOL_DEFAULT_VALUE = new TypedValue(TypeHelper.BOOL_TYPE, default(bool));
        public static IValue INT_DEFAULT_VALUE = new TypedValue(TypeHelper.INT_TYPE, default(int));
        public static IValue DECIMAL_DEFAULT_VALUE = new TypedValue(TypeHelper.DECIMAL_TYPE, default(decimal));
        public static IValue DOUBLE_DEFAULT_VALUE = new TypedValue(TypeHelper.DOUBLE_TYPE, default(double));
        public static IValue CHAR_DEFAULT_VALUE = new TypedValue(TypeHelper.CHAR_TYPE, default(char));
        public static IValue DATETIME_DEFAULT_VALUE = new TypedValue(TypeHelper.DATETIME_TYPE, default(DateTime));

        public static string ParseStringLiteral(ITerminalNode stringNote)
        {
            return stringNote.GetText().TrimEnd(new char[] { '"' }).TrimStart(new char[] { '"' });
        }

        public static string ParseVerbatimStringLiteral(ITerminalNode stringNote)
        {
            return stringNote.GetText().TrimStart(new char[] { '@' }).TrimEnd(new char[] { '"' }).TrimStart(new char[] { '"' });
        }

        public static IValue GetDefaultValue(SyneryType type)
        {
            return GetDefaultValue(type.UnterlyingDotNetType);
        }

        public static IValue GetDefaultValue(Type type)
        {
            if (type == typeof(string)) return STRING_DEFAULT_VALUE;
            if (type == typeof(bool)) return BOOL_DEFAULT_VALUE;
            if (type == typeof(int)) return INT_DEFAULT_VALUE;
            if (type == typeof(decimal)) return DECIMAL_DEFAULT_VALUE;
            if (type == typeof(double)) return DOUBLE_DEFAULT_VALUE;
            if (type == typeof(char)) return CHAR_DEFAULT_VALUE;
            if (type == typeof(DateTime)) return DATETIME_DEFAULT_VALUE;

            throw new SyneryException(String.Format("No default value specified for type '{0}'", type.Name));
            
        }
    }
}
