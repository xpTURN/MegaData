using System;


using xpTURN.Common;
using xpTURN.Protobuf;
using xpTURN.MegaData;

namespace xpTURN.Converter.Mapper
{
    public class MapperFactory
    {
        public static IValueMapper GetMapper(Type type)
        {
            if (type == typeof(string))
            {
                return StringMapper.Default;
            }
            else if (type == typeof(ByteString))
            {
                return ByteStringMapper.Default;
            }
            else if (type == typeof(int))
            {
                return ChangeTypeMapper.Int32Mapper;
            }
            else if (type == typeof(uint))
            {
                return ChangeTypeMapper.UInt32Mapper;
            }
            else if (type == typeof(long))
            {
                return ChangeTypeMapper.Int64Mapper;
            }
            else if (type == typeof(ulong))
            {
                return ChangeTypeMapper.UInt64Mapper;
            }
            else if (type == typeof(float))
            {
                return ChangeTypeMapper.FloatMapper;
            }
            else if (type == typeof(double))
            {
                return ChangeTypeMapper.DoubleMapper;
            }
            else if (type.IsEnum)
            {
                return new EnumMapper(type);
            }
            else if (type == typeof(bool))
            {
                return BoolMapper.Default;
            }
            else if (type == typeof(DateTime))
            {
                return DateTimeMapper.Default;
            }
            else if (type == typeof(TimeSpan))
            {
                return TimeSpanMapper.Default;
            }
            else if (type == typeof(Uri))
            {
                return UriMapper.Default;
            }
            else if (type == typeof(Guid))
            {
                return GuidMapper.Default;
            }
            else if (type.IsList())
            {
                var innerType = type.GetCollectionElementType();
                return GetMapper(innerType);
            }
            else if (type.IsDictionary())
            {
                var innerType = type.GetCollectionElementType();
                return GetMapper(innerType);
            }
            else if (typeof(Data).IsAssignableFrom(type) || typeof(IMessage).IsAssignableFrom(type))
            {
                return DataMapper.Default;
            }
            else
            {
                Logger.Log.Tool.Error($"Unsupported type for value mapping: {type}");
                return null;
            }
        }
    }
}
