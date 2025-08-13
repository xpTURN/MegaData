using System;
using System.Collections.Generic;
using System.Reflection;

using xpTURN.Common;
using xpTURN.MegaData;

namespace xpTURN.Converter.AssginValue
{
    public class MapFieldAssignField : IAssignField
    {
        public static MapFieldAssignField Default = new MapFieldAssignField();

        public bool AssignValue(FieldInfo fieldInfo, object target, object value)
        {
            try
            {
                object key = null, val = null;
                var valueType = value.GetType();

                // If value inherits from Data
                if (typeof(Data).IsAssignableFrom(valueType))
                {
                    val = value;

                    // Reflection call to GetId() method
                    key = InvokeUtils.InvokeFunc<int>(value, "GetId");

                    // If GetId() returns 0, the type does not use Id, so call GetAlias() method
                    if ((int)key == 0)
                    {
                        // Reflection call to GetAlias() method
                        key = InvokeUtils.InvokeFunc<string>(value, "GetAlias");
                    }
                }
                // Handle KeyValuePair
                else if (valueType.IsGenericType && valueType.GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
                {
                    key = valueType.GetProperty("Key")!.GetValue(value);
                    val = valueType.GetProperty("Value")!.GetValue(value);
                }
                // Handle (object, object) tuple
                else if (valueType == typeof(ValueTuple<object, object>))
                {
                    key = valueType.GetField("Item1")!.GetValue(value);
                    val = valueType.GetField("Item2")!.GetValue(value);
                }
                else if (value is Tuple<object, object> tuple)
                {
                    key = tuple.Item1;
                    val = tuple.Item2;
                }
                else
                {
                    throw new ArgumentException("Value must be Data, KeyValuePair, or (key, value) tuple for Dictionary field.");
                }

                Type keyType = fieldInfo.FieldType.GetGenericArguments()[0];
                Type valType = fieldInfo.FieldType.GetGenericArguments()[1];

                object fieldObject = fieldInfo.GetValue(target);
                object convertedKey = ConvertTypeUtils.ConvertToType(key, keyType);

                if (InvokeUtils.InvokeFunc<bool>(fieldObject, "ContainsKey", new object[] { convertedKey }))
                {
                    Logger.Log.Tool.Error($"Key '{convertedKey}' already exists in the map field '{fieldInfo.Name}'.");
                    return false;
                }

                InvokeUtils.InvokeFunc(fieldObject, "Add", new Type[] { keyType, valType }, new object[] { convertedKey, val });
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
