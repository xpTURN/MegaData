using System;

namespace xpTURN.MegaData
{
    public class JsonWrapper
    {
        public delegate string ToJsonDelegate(object data);
        public delegate object FromJsonDelegate(string json, Type type);

        public static ToJsonDelegate ToJsonMethod { get; set; } = null;
        public static FromJsonDelegate FromJsonMethod { get; set; } = null;

        public static string ToJson(object data)
        {
            if (ToJsonMethod == null)
            {
                throw new InvalidOperationException("ToJsonMethod is not set. Please set it before calling ToJson.");
            }

            if (data == null || ToJsonMethod == null)
            {
                return string.Empty;
            }

            return ToJsonMethod(data);
        }

        public static object FromJson(string json, Type type)
        {
            if (FromJsonMethod == null)
            {
                throw new InvalidOperationException("FromJsonMethod is not set. Please set it before calling FromJson.");
            }

            if (string.IsNullOrEmpty(json) || type == null)
            {
                return null;
            }

            return FromJsonMethod(json, type);
        }
    }
}
