using System;
using System.Collections;
using System.Reflection;

namespace xpTURN.Common
{
    public static class InvokeUtils
    {
        public static void SetPropValue(object target, string propName, object value)
        {
            var property = target.GetType().GetProperty(propName);
            if (property != null)
            {
                property.SetValue(target, value);
                return;
            }
        
            throw new InvalidOperationException($"Property '{propName}' not found in type '{target.GetType()}'.");
        }

        public static object GetPropValue(object target, string propName)
        {
            var property = target.GetType().GetProperty(propName);
            if (property == null)
            {
                throw new InvalidOperationException($"Property '{propName}' not found in type '{target.GetType()}'.");
            }

            return property.GetValue(target);
        }
    
        public static void SetFieldValue(object target, string fieldName, object value)
        {
            var field = target.GetType().GetField(fieldName);
            if (field != null)
            {
                field.SetValue(target, value);
                return;
            }

            throw new InvalidOperationException($"Field '{fieldName}' not found in type '{target.GetType()}'.");
        }

        public static object GetFieldValue(object target, string fieldName)
        {
            var field = target.GetType().GetField(fieldName);
            if (field == null)
            {
                throw new InvalidOperationException($"Field '{fieldName}' not found in type '{target.GetType()}'.");
            }

            return field.GetValue(target);
        }

        public static IEnumerable GetFieldListEnumerable<TArg>(FieldInfo fieldInfo, object target)
        {
            if (fieldInfo.IsListArg<TArg>())
            {
                return fieldInfo.GetValue(target) as IEnumerable;
            }

            return null;
        }

        public static void InvokeFunc(object target, string methodName, params object[] parameters)
        {
            var method = target.GetType().GetMethod(methodName);
            if (method == null)
            {
                throw new InvalidOperationException($"Public Method '{methodName}' not found in type '{target.GetType()}'.");
            }

            method.Invoke(target, parameters);
        }

        public static void InvokeFunc(object target, string methodName, Type[] parameterTypes, params object[] parameters)
        {
            var method = target.GetType().GetMethod(methodName, types: parameterTypes);
            if (method == null)
            {
                throw new InvalidOperationException($"Public Method '{methodName}' not found in type '{target.GetType()}'.");
            }

            method.Invoke(target, parameters);
        }
    
        public static T InvokeFunc<T>(object target, string methodName, params object[] parameters)
        {
            var method = target.GetType().GetMethod(methodName);
            if (method == null)
            {
                throw new InvalidOperationException($"Public Method '{methodName}' not found in type '{target.GetType()}'.");
            }

            return (T)method.Invoke(target, parameters);
        }
    
        public static T InvokeFunc<T>(object target, string methodName, Type[] parameterTypes, params object[] parameters)
        {
            var method = target.GetType().GetMethod(methodName, types: parameterTypes);
            if (method == null)
            {
                throw new InvalidOperationException($"Public Method '{methodName}' not found in type '{target.GetType()}'.");
            }

            return (T)method.Invoke(target, parameters);
        }
    }
}
