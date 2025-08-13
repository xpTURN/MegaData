using System;

using xpTURN.Common;

namespace xpTURN.Converter.Mapper
{
    public class GuidMapper : IValueMapper
    {
        public static GuidMapper Default = new GuidMapper();

        public object MapValue(object value)
        {
            if (value is Guid guidValue)
            {
                return guidValue;
            }

            if (value is string strValue && Guid.TryParse(strValue, out var parsedGuid))
            {
                return parsedGuid;
            }

            Logger.Log.Tool.Error($"Failed to parse value \"{value}\" to Guid.");
            return Guid.Empty; // Return a default Guid value if parsing fails
        }
    }
}
