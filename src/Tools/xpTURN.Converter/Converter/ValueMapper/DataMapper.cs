using System;

using xpTURN.Common;
using xpTURN.MegaData;

namespace xpTURN.Converter.Mapper
{
    public class DataMapper : IValueMapper
    {
        public static DataMapper Default = new DataMapper();

        public object MapValue(object value)
        {
            if (value is Data dataValue)
            {
                return dataValue;
            }

            Logger.Log.Tool.Error($"Failed to convert value \"{value}\" to Data.");
            return null;
        }

    }
}
