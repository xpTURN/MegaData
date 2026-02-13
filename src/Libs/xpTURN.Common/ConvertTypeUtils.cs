using System;
using System.Runtime.CompilerServices;

namespace xpTURN.Common
{
    public static class ConvertTypeUtils
    {
        public static TEnum ConvertToEnum<TEnum>(int value) where TEnum : struct, Enum
        {
            if (!Enum.IsDefined(typeof(TEnum), value))
            {
                Logger.Log.Tool.Error(DebugInfo.Empty, $"Value {value} is not defined for enum {typeof(TEnum).Name}");
                return default;
            }

            // int-backed enum: no allocation. other backing types: use Enum.ToObject (allocates once).
            if (Enum.GetUnderlyingType(typeof(TEnum)) == typeof(int))
                return Unsafe.As<int, TEnum>(ref value);
            return (TEnum)Enum.ToObject(typeof(TEnum), value);
        }

        public static object ConvertToType(object value, Type targetType)
        {
            if (value == null || targetType.IsInstanceOfType(value))
                return value;

            if (targetType.IsEnum)
            {
                if (value is sbyte || value is byte ||
                    value is short || value is ushort ||
                    value is int || value is uint ||
                    value is long || value is ulong)
                    return Enum.ToObject(targetType, value);

                return Enum.Parse(targetType, value.ToString()!);
            }

            return Convert.ChangeType(value, targetType);
        }
    }
}
