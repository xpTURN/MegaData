using System;

using xpTURN.Common;

namespace xpTURN.Converter.Mapper
{
    public class ChangeTypeMapper : IValueMapper
    {
        public static ChangeTypeMapper Int32Mapper = new ChangeTypeMapper(typeof(int));
        public static ChangeTypeMapper UInt32Mapper = new ChangeTypeMapper(typeof(uint));
        public static ChangeTypeMapper Int64Mapper = new ChangeTypeMapper(typeof(long));
        public static ChangeTypeMapper UInt64Mapper = new ChangeTypeMapper(typeof(ulong));
        public static ChangeTypeMapper FloatMapper = new ChangeTypeMapper(typeof(float));
        public static ChangeTypeMapper DoubleMapper = new ChangeTypeMapper(typeof(double));

        public Type TargetType { get; private set; }

        public ChangeTypeMapper(Type targetType)
        {
            if (targetType == null)
            {
                throw new ArgumentNullException(nameof(targetType));
            }

            if (!targetType.ImplementsInterface(typeof(IConvertible)))
            {
                throw new ArgumentException($"Type \"{targetType}\" must implement IConvertible to support Convert.ChangeType.", nameof(targetType));
            }

            TargetType = targetType;
        }

        public object MapValue(object value)
        {
            try
            {
                object result = ConvertTypeUtils.ConvertToType(value.ToString(), TargetType);
                return result;
            }
            catch (Exception exception)
            {
                Logger.Log.Tool.Error($"Failed to convert value \"{value}\" to type \"{TargetType}\": {exception.Message}");
                return TargetType.DefaultValue();
            }
        }
    }
}
