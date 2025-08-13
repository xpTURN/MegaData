using System;
using System.Linq;

using xpTURN.Common;
using xpTURN.MegaData;

using static xpTURN.Converter.Utils.ConverterUtils;

namespace xpTURN.Converter
{
    public class FieldMapping
    {
        public int CellX { get; set; }
        public string Key { get; set; } = null;
        public FieldDesc FieldDesc { get; set; }
        public DataDesc OwnerDataDesc { get; set; }

        public string CellName => xpTURN.Tool.Common.ExcelReader.CellName(CellX);
        public string Name => FieldDesc?.Name ?? string.Empty;
        public string OwnerName => FieldDesc?.OwnerName ?? string.Empty;
        public string FullName => FieldDesc?.FullName ?? string.Empty;
        public Type ParentDataType => OwnerDataDesc?.DataType ?? null;
        public Type FieldType => FieldDesc?.FieldType ?? null;
        public bool HasNested => FieldDesc?.HasNested ?? false;

        public FieldMapping()
        {
        }

        public FieldMapping(int cellX, string key, FieldDesc fieldDesc)
        {
            CellX = cellX;
            Key = key;
            FieldDesc = fieldDesc;
            OwnerDataDesc = fieldDesc.Owner as DataDesc;
        }

        public Data CreateNestedData()
        {
            if (FieldDesc == null || !HasNested)
            {
                return null;
            }

            var nestedDataDesc = FieldDesc.NestedDataDesc;
            if (nestedDataDesc == null)
            {
                return null;
            }

            nestedDataDesc.CreateData();
            return nestedDataDesc.Data;
        }

        public void SetOrAddValue(object value)
        {
            if (FieldDesc == null)
            {
                return;
            }

            if (FieldDesc.Data == null)
            {
                Logger.Log.Tool.Error($"FieldDesc.Data cannot be null for field '{FullName}'.");
                return;
            }

            // Map<key> field
            if (!string.IsNullOrEmpty(Key))
            {
                FieldDesc.SetOrAddValue(Key, value);
                return;
            }

            // Normal field
            FieldDesc.SetOrAddValue(value);
        }

        public void SetOrAddData()
        {
            if (FieldDesc == null)
            {
                return;
            }

            if (FieldDesc.Data == null)
            {
                Logger.Log.Tool.Error($"FieldDesc.Data cannot be null for field '{FullName}'.");
                return;
            }

            var nestedData = FieldDesc.NestedDataDesc.Data;
            if (nestedData != null)
            {
                if (FieldDesc.FieldType.IsDictionary())
                {
                    if (nestedData.GetId() != 0)
                        FieldDesc.SetOrAddValue(nestedData.GetId(), nestedData);
                    else
                        FieldDesc.SetOrAddValue(nestedData.GetAlias(), nestedData);
                }
                else
                {
                    FieldDesc.SetOrAddValue(nestedData);
                }
            }
        }

        public void PostProcess(DataMapping dataMapper)
        {
            // If the field is IdAlias but the Id field does not exist in the sheet, run the auto-generation logic
            if (FieldDesc.Name.EndsWith(sIdAlias) && !FieldDesc.Name.EndsWith(sRefIdAlias))
            {
                var valueString = FieldDesc.Value as string ?? string.Empty;
                var idFieldName = FieldDesc.Name.Substring(0, FieldDesc.Name.Length - sAlias.Length);
                var idFieldDesc = OwnerDataDesc.GetFieldDesc(idFieldName);
                var idFieldMapper = dataMapper.ListFieldMapper.Values.FirstOrDefault(fm => fm.FieldDesc == idFieldDesc);
                if (idFieldMapper == null)
                {
                    idFieldDesc.SetOrAddValue(Crc32Helper.ComputeInt32(valueString));
                }
            }
        }
    }
}