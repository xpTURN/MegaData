using System;

using xpTURN.Common;

namespace xpTURN.Converter.Mapper
{
    public class ByteStringMapper : IValueMapper
    {
        public static ByteStringMapper Default = new ByteStringMapper();

        public object MapValue(object value)
        {
            if (value is xpTURN.Protobuf.ByteString byteStringValue)
            {
                return byteStringValue;
            }

            if (value is string strValue)
            {
                return xpTURN.Protobuf.ByteString.FromBase64(strValue);
            }

            Logger.Log.Tool.Error($"Failed to convert value \"{value}\" to ByteString.");
            return xpTURN.Protobuf.ByteString.Empty; // Default value for unsupported types
        }
    }
}
