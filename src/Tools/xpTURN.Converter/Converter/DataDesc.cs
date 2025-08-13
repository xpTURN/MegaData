using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using xpTURN.Common;
using xpTURN.MegaData;

namespace xpTURN.Converter
{
    public interface IDesc
    {
        string Name { get; }
        IDesc Owner { get; }

        string OwnerName { get; }
        string FullName { get; }
    }

    public class DataDesc : IDesc
    {
        public string Name { get; }
        public IDesc Owner { get; }
        public string OwnerName => string.Empty;
        public string FullName => string.Empty;

        public Type DataType { get; private set; }
        public Data Data { get; set; }

        public List<FieldDesc> listFieldDesc = new();

        public DataDesc(Type dataType, IDesc ownerDesc)
        {
            Name = dataType.Name;
            Owner = ownerDesc;
            DataType = dataType;

            var found = DataType.GetFields(BindingFlags.Public | BindingFlags.Instance);
            for (int i = 0; i < found.Length; i++)
            {
                var fieldInfo = found[i];

                var fieldDesc = new FieldDesc(fieldInfo, this);
                if (fieldDesc != null)
                {
                    listFieldDesc.Add(fieldDesc);
                }
            }
        }

        public void GetFieldDesc(List<FieldDesc> listFullFieldDesc)
        {
            foreach (var fieldDesc in listFieldDesc)
            {
                listFullFieldDesc.Add(fieldDesc);
            }

            foreach (var fieldDesc in listFieldDesc)
            {
                if (fieldDesc.NestedDataDesc != null)
                {
                    fieldDesc.NestedDataDesc.GetFieldDesc(listFullFieldDesc);
                }
            }
        }

        public Data CreateData()
        {
            var data = (Data)Activator.CreateInstance(DataType);
            data.DebugInfo = new DebugInfo
            {
                File = Logger.Log.Tool.GetFile(),
                Line = Logger.Log.Tool.GetLine(),
            };

            SetData(data);

            return data;
        }

        public void SetData(Data data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data), "Data cannot be null.");
            }

            Data = data;
            foreach (var field in listFieldDesc)
            {
                field.Data = data;
            }
        }

        public FieldDesc GetFieldDesc(string fieldName)
        {
            if (string.IsNullOrEmpty(fieldName))
            {
                return null;
            }

            var field = listFieldDesc.FirstOrDefault(f => f.Name == fieldName);
            if (field == null)
            {
                return null;
            }

            return field;
        }
    }
}
