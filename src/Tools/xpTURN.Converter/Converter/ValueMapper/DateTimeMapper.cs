using System;

using xpTURN.Common;

namespace xpTURN.Converter.Mapper
{
    public class DateTimeMapper : IValueMapper
    {
        public static DateTimeMapper Default = new DateTimeMapper();

        public object MapValue(object value)
        {
            if (value is DateTime dateTimeValue)
            {
                return dateTimeValue;
            }

            if (value is string strValue && DateTime.TryParse(strValue, out var parsedDateTime))
            {
                return parsedDateTime;
            }

            Logger.Log.Tool.Error($"Failed to parse value \"{value}\" to DateTime.");
            return DateTime.MinValue; // Return a default DateTime value if parsing fails
        }
    }
}
