using System;

namespace xpTURN.Converter.Mapper
{
    public class StringMapper : IValueMapper
    {
        public static StringMapper Default = new StringMapper();

        public object MapValue(object value)
        {
            if (value is string strValue)
            {
                return strValue;
            }

            return value.ToString();
        }
    }
}
