using System;

using xpTURN.Common;

namespace xpTURN.Converter.Mapper
{
    public class BoolMapper : IValueMapper
    {
        public static BoolMapper Default = new BoolMapper();

        public object MapValue(object value)
        {
            if (value is bool boolValue)
            {
                return boolValue;
            }
            else if (value is string strValue)
            {
                if (strValue == "1")
                {
                    return true; // Treat "1" as true
                }
                else if (strValue == "0")
                {
                    return false; // Treat "0" as false
                }

                if (bool.TryParse(strValue.ToLower(), out bool parsedValue))
                {
                    return parsedValue;
                }

                Logger.Log.Tool.Error($"Failed to convert string \"{strValue}\" to boolean. Expected '1', '0', or a valid boolean string (true/false).");
                return false; // Default value for unsupported strings
            }
            else
            {
                Logger.Log.Tool.Error($"Failed to convert string \"{value}\" to boolean. Expected '1', '0', or a valid boolean string (true/false).");
                return false; // Default value for unsupported types
            }
        }
    }
}
