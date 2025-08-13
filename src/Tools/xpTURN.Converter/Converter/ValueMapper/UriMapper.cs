using System;

using xpTURN.Common;

namespace xpTURN.Converter.Mapper
{
    public class UriMapper : IValueMapper
    {
        public static UriMapper Default = new UriMapper();

        public object MapValue(object value)
        {
            if (value is Uri uriValue)
            {
                return uriValue;
            }

            if (value is string strValue && Uri.TryCreate(strValue, UriKind.RelativeOrAbsolute, out var parsedUri))
            {
                return parsedUri;
            }

            Logger.Log.Tool.Error($"Failed to parse value \"{value}\" to Uri.");
            return null; // Return null if parsing fails
        }
    }
}
