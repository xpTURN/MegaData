using System;
using System.Collections;
using System.Reflection;

namespace xpTURN.Common
{
    /// <summary>
    /// Reflection helpers for getting/setting properties and fields and invoking methods. Only public instance members are considered.
    /// </summary>
    public static class InvokeUtils
    {
        public static void SetPropValue(object target, string propName, object value)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
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
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            var property = target.GetType().GetProperty(propName);
            if (property == null)
            {
                throw new InvalidOperationException($"Property '{propName}' not found in type '{target.GetType()}'.");
            }

            return property.GetValue(target);
        }
    
        public static void SetFieldValue(object target, string fieldName, object value)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
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
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            var field = target.GetType().GetField(fieldName);
            if (field == null)
            {
                throw new InvalidOperationException($"Field '{fieldName}' not found in type '{target.GetType()}'.");
            }

            return field.GetValue(target);
        }

        public static IEnumerable GetFieldListEnumerable<TArg>(FieldInfo fieldInfo, object target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (fieldInfo.IsListArg<TArg>())
            {
                return fieldInfo.GetValue(target) as IEnumerable;
            }

            return null;
        }

        public static void InvokeFunc(object target, string methodName, params object[] parameters)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            var method = target.GetType().GetMethod(methodName);
            if (method == null)
            {
                throw new InvalidOperationException($"Public Method '{methodName}' not found in type '{target.GetType()}'.");
            }

            method.Invoke(target, parameters);
        }

        public static void InvokeFunc(object target, string methodName, Type[] parameterTypes, params object[] parameters)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            var method = target.GetType().GetMethod(methodName, types: parameterTypes);
            if (method == null)
            {
                throw new InvalidOperationException($"Public Method '{methodName}' not found in type '{target.GetType()}'.");
            }

            method.Invoke(target, parameters);
        }
    
        public static T InvokeFunc<T>(object target, string methodName, params object[] parameters)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            var method = target.GetType().GetMethod(methodName);
            if (method == null)
            {
                throw new InvalidOperationException($"Public Method '{methodName}' not found in type '{target.GetType()}'.");
            }

            return (T)method.Invoke(target, parameters);
        }
    
        public static T InvokeFunc<T>(object target, string methodName, Type[] parameterTypes, params object[] parameters)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            var method = target.GetType().GetMethod(methodName, types: parameterTypes);
            if (method == null)
            {
                throw new InvalidOperationException($"Public Method '{methodName}' not found in type '{target.GetType()}'.");
            }

            return (T)method.Invoke(target, parameters);
        }
    }
}
