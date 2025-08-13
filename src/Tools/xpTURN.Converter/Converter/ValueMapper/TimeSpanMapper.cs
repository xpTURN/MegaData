using System;

using xpTURN.Common;

namespace xpTURN.Converter.Mapper
{
    public class TimeSpanMapper : IValueMapper
    {
        public static TimeSpanMapper Default = new TimeSpanMapper();

        public object MapValue(object value)
        {
            if (value is TimeSpan timeSpanValue)
            {
                return timeSpanValue;
            }

            if (value is string strValue && TimeSpan.TryParse(strValue, out var parsedTimeSpan))
            {
                return parsedTimeSpan;
            }

            Logger.Log.Tool.Error($"Failed to parse value \"{value}\" to TimeSpan.");
            return TimeSpan.Zero; // Return a default TimeSpan value if parsing fails
        }
    }
}
