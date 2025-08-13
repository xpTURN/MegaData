using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using xpTURN.Common;
using xpTURN.MegaData;

using xpTURN.Converter.AssginValue;
using xpTURN.Converter.Mapper;

namespace xpTURN.Converter
{
    public class FieldDesc : IDesc
    {
        public string Name { get; }
        public IDesc Owner { get; }
        public string OwnerName => GetOwnerName();
        public string FullName => string.IsNullOrEmpty(OwnerName) ? Name : $"{OwnerName}/{Name}";
        public string ShortName => new string('/', Depth) + FullName.Split('/').Last();

        public Type FieldType => FieldInfo.FieldType;
        public int Depth => FullName.Count(c => c == '/');
        public bool OwnerIsTable { get; } = false;
        public bool IsCollection => FieldType.IsList() || FieldType.IsDictionary();
        public bool IsDictionary => FieldType.IsDictionary();
        public bool IsList => FieldType.IsList();
        public Type KeyType { get; set; }
        public Type NestedType { get; set; }
        public bool HasNested => NestedType != null && typeof(Data).IsAssignableFrom(NestedType);
        public DataDesc NestedDataDesc { get; set; } = null;

        public FieldInfo FieldInfo { get; private set; }

        public Data Data { get; set; }
        public object Key { get; protected set; }
        public object Value { get; protected set; }

        IValueMapper _keyMapper = null;
        IValueMapper _valueMapper = null;
        IAssignField _assignValue = null;

        public FieldDesc(FieldInfo fieldInfo, IDesc parentDesc, bool ownerIsTable = false)
        {
            Name = fieldInfo.Name;
            Owner = parentDesc;
            OwnerIsTable = ownerIsTable;
            FieldInfo = fieldInfo;

            KeyType = FieldType.GetCollectionKeyType();
            NestedType = FieldType.GetCollectionElementType();

            if (typeof(Data).IsAssignableFrom(FieldType))
                NestedType = FieldType;

            NestedDataDesc = HasNested ? new DataDesc(NestedType, this) : null;

            if (KeyType != null)
                _keyMapper = MapperFactory.GetMapper(KeyType);

            _valueMapper = MapperFactory.GetMapper(fieldInfo.FieldType);
            _assignValue = AssignFieldFactory.GetAssignValue(fieldInfo.FieldType);
        }

        public void SetOrAddValue(object valueObject)
        {
            if (Data == null || valueObject == null)
            {
                throw new ArgumentNullException("FieldObject or value cannot be null.");
            }

            // Convert the value to the appropriate type for the field
            object convertedValue;
            if (_valueMapper != null)
            {
                convertedValue = _valueMapper.MapValue(valueObject);
                Value = convertedValue; // Store the value for later use
            }
            else
            {
                throw new InvalidOperationException($"Cannot map value of type {valueObject.GetType()} to field of type {FieldType}.");
            }

            // Assign the mapped value to the field
            if (_assignValue != null)
            {
                bool result = _assignValue.AssignValue(FieldInfo, Data, convertedValue);
                if (!result)
                {
                    Logger.Log.Tool.Error($"Failed to assign value to field '{Name}': {convertedValue}");
                }
            }
            else
            {
                throw new InvalidOperationException($"Cannot assign value to field of type {FieldType}.");
            }
        }

        public void SetOrAddValue(object keyObject, object valueObject)
        {
            if (Data == null || valueObject == null)
            {
                throw new ArgumentNullException("FieldObject or value cannot be null.");
            }

            // Convert the key to the appropriate type for the field
            object convertedKey = null;
            if (_keyMapper != null)
            {
                convertedKey = _keyMapper.MapValue(keyObject);
                Key = convertedKey; // Store the key for later use
            }
            else
            {
                throw new InvalidOperationException($"Cannot map key of type {keyObject.GetType()} to field of type {FieldType}.");
            }

            // Convert the value to the appropriate type for the field
            object convertedValue = null;
            if (_valueMapper != null)
            {
                convertedValue = _valueMapper.MapValue(valueObject);
                Value = convertedValue; // Store the value for later use
            }
            else
            {
                throw new InvalidOperationException($"Cannot map value of type {valueObject.GetType()} to field of type {FieldType}.");
            }

            // Assign the mapped value to the field
            if (_assignValue != null)
            {
                object value = convertedKey != null ? new KeyValuePair<object, object>(convertedKey, convertedValue) : convertedValue;
                bool result = _assignValue.AssignValue(FieldInfo, Data, value);
                if (!result)
                {
                    Logger.Log.Tool.Error($"Failed to assign value to field '{Name}' with key '{convertedKey}'.");
                }
            }
            else
            {
                throw new InvalidOperationException($"Cannot assign value to field of type {FieldType}.");
            }
        }
        
        private string _parentName = null;
        private string GetOwnerName()
        {
            if (_parentName != null)
            {
                return _parentName;
            }

            string parentName = string.Empty;
            IDesc parent = Owner;
            while (parent != null)
            {
                FieldDesc parentDesc = parent as FieldDesc;
                if (parentDesc != null)
                {
                    if (parentDesc.Owner is not TableDesc)
                        parentName = string.IsNullOrEmpty(parentName) ? parentDesc.Name : $"{parentDesc.Name}/{parentName}";
                }

                parent = parent.Owner;
            }

            _parentName = parentName;
            return _parentName;
        }
    }
}
