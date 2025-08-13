using System;
using System.Collections.Generic;
using System.Reflection;

using xpTURN.Common;
using xpTURN.MegaData;

namespace xpTURN.Converter
{
    public class TableDesc : IDesc
    {
        public string Name { get; }
        public IDesc Owner { get; }

        public string OwnerName => string.Empty;
        public string FullName => string.Empty;

        public Type TableType { get; set; }
        public Table Table { get; set; }

        public FieldDesc NestFieldDesc { get; private set; } = null;
        public Type NestedDataType => NestFieldDesc?.NestedType;
        public DataDesc NestedDataDesc { get; set; }

        public List<FieldDesc> ListFieldDesc { get; } = new();

        public TableDesc(Type tableType)
        {
            Name = tableType.Name;
            Owner = null;
            TableType = tableType;

            var found = TableType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            for (int i = 0; i < found.Length; i++)
            {
                var fieldInfo = found[i];
                var fieldType = fieldInfo.FieldType;

                if (fieldType.IsDictionary() && typeof(Data).IsAssignableFrom(fieldType.GetCollectionElementType()))
                {
                    NestFieldDesc = new FieldDesc(fieldInfo, this, true);

                    NestedDataDesc = NestFieldDesc.NestedDataDesc;
                }
            }

            if (NestedDataDesc != null)
            {
                NestedDataDesc.GetFieldDesc(ListFieldDesc);
            }
        }

        public FieldDesc FindFieldDesc(string ownerName, string fieldName)
        {
            // ShortName
            if (fieldName.StartsWith("/"))
            {
                var fieldDesc = ListFieldDesc.Find(fieldDesc => fieldDesc.ShortName == fieldName);
                return fieldDesc;
            }
            //  FullName
            else if (fieldName.Contains('/'))
            {
                var fieldDesc = ListFieldDesc.Find(fieldDesc => fieldDesc.FullName == fieldName);
                return fieldDesc;
            }
            //  OwnerName, FieldName
            else
            {
                var fieldDesc = ListFieldDesc.Find(fieldDesc => fieldDesc.OwnerName == ownerName && fieldDesc.Name == fieldName);
                return fieldDesc;
            }
        }

        public Table CreateTable(TableSet tableSet)
        {
            Table = tableSet.CreateTableByName(TableType.Name);
            Table.DebugInfo = new DebugInfo
            {
                File = Logger.Log.Tool.GetFile(),
                Line = Logger.Log.Tool.GetLine(),
            };

            NestFieldDesc.Data = Table;
            return Table;
        }
    }
}
