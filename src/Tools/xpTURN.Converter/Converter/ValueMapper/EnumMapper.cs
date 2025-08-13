using System;

using xpTURN.Common;

namespace xpTURN.Converter.Mapper
{
    public class EnumMapper : IValueMapper
    {
        public Type EnumType { get; }
        public bool IgnoreCase { get; } = true;

        public EnumMapper(Type enumType, bool ignoreCase = true)
        {
            if (enumType == null)
            {
                Logger.Log.Tool.Error("Enum type cannot be null.");
                throw new ArgumentNullException(nameof(enumType), "Enum type cannot be null.");
            }
            if (!enumType.IsEnum)
            {
                Logger.Log.Tool.Error($"Provided type {enumType.FullName} is not an enum type.");
                throw new ArgumentException($"Provided type {enumType.FullName} is not an enum type.", nameof(enumType));
            }

            EnumType = enumType;
            IgnoreCase = ignoreCase;
        }

        public object MapValue(object value)
        {
            try
            {
                // Discarding readResult.StringValue nullability warning.
                // If null - CellValueMapperResult.Invalid with ArgumentNullException will be returned
                object result = Enum.Parse(EnumType, value.ToString(), IgnoreCase);
                return result;
            }
            catch
            {
                Logger.Log.Tool.Error($"Failed to parse value \"{value}\" to enum type \"{EnumType}\".");
                return EnumType.DefaultValue();
            }
        }
    }
}
