using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace System
{
    public static class TypeUtils
    {
        public static bool ImplementsInterface(this Type type, Type interfaceType)
        {
            return type.GetTypeInfo().ImplementedInterfaces.Any(t => t == interfaceType);
        }

        public static object DefaultValue(this Type type)
        {
            if (type.GetTypeInfo().IsValueType)
            {
                return Activator.CreateInstance(type);
            }

            return null;
        }

        public static bool IsList(this Type type) => type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);
        public static bool IsList(this FieldInfo fieldInfo) => fieldInfo.FieldType.IsList();
        public static bool IsListArg<TArg>(this FieldInfo fieldInfo) => fieldInfo.IsList() &&
                                                            typeof(TArg).IsAssignableFrom(fieldInfo.FieldType.GetGenericArguments()[0]);

        public static bool IsDictionary(this Type type) => type.GetInterfaces().Any(i =>
                                                            i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDictionary<,>));
        public static bool IsDictionary(this FieldInfo fieldInfo) => fieldInfo.FieldType.IsDictionary();

        public static Type GetCollectionKeyType(this Type type)
        {
            if (type.IsDictionary())
            {
                return type.GetGenericArguments()[0];
            }
            else
            {
                return null;
            }
        }

        public static Type GetCollectionElementType(this Type type)
        {
            if (type.IsList())
            {
                return type.GetGenericArguments()[0];
            }
            else if (type.IsDictionary())
            {
                return type.GetGenericArguments()[1];
            }
            else
            {
                return null;
            }
        }
    }
}
