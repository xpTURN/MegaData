
using System;

using xpTURN.Protobuf;

namespace xpTURN.MegaData
{
    public static class TableTypeUtils
    {
        public static bool IsAcceptableIdType(Type type)
        {
            return type == typeof(int) || type.IsEnum;
        }

        public static bool IsAcceptKeyType(Type type)
        {
            return type == typeof(int) || type.IsEnum || type == typeof(string);
        }

        public static bool IsAcceptKeyType(xpFieldTypes type)
        {
            // int, enum, string
            return IsIntKeyType(type) ||
                type == xpFieldTypes.Type_String ||
                type == xpFieldTypes.Type_Enum;
        }

        public static bool IsIntKeyType(xpFieldTypes type)
        {
            // int, enum
            return type == xpFieldTypes.Type_Int32 || type == xpFieldTypes.Type_SInt32 || type == xpFieldTypes.Type_SFixed32;
        }
    }
}