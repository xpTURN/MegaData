using System;
using System.Collections.Generic;
using System.Linq;

using xpTURN.Common;
using static xpTURN.Tool.Common.ExcelReader;

namespace xpTURN.Converter
{   
    public class DataMapping
    {
        public SortedList<int, FieldMapping> ListFieldMapper { get; } = new();

        public void Clear()
        {
            ListFieldMapper.Clear();
        }

        public FieldMapping Find(DataDesc dataDesc, string fieldName)
        {
            var fieldMapper = ListFieldMapper.Values.FirstOrDefault(fm => fm.OwnerDataDesc == dataDesc && fm.Name == fieldName);
            return fieldMapper;
        }

        public bool Add(FieldMapping fieldMapper)
        {
            bool isCollection = fieldMapper.FieldType.IsList() || fieldMapper.FieldType.IsDictionary();
            bool duplicated = !isCollection && null != ListFieldMapper.Values.FirstOrDefault(fm => fm.FullName == fieldMapper.FullName);
            if (duplicated)
            {
                return false;
            }

            ListFieldMapper.Add(fieldMapper.CellX, fieldMapper);
            return true;
        }

        public void SetOrAddValue(int curX, string text)
        {
            ListFieldMapper.TryGetValue(curX, out FieldMapping fieldMapper);
            if (fieldMapper == null)
            {
                Logger.Log.Tool.Warn($"FieldMapper not found for cell {CellName(curX)}");
                return;
            }

            // If the field is a NestedType, we need to create a new Data instance
            if (fieldMapper.HasNested)
            {
                if (text != fieldMapper.FieldDesc.NestedType.Name)
                {
                    Logger.Log.Tool.Error($"Nested field type mismatch: {text} != {fieldMapper.FieldDesc.NestedType.Name}");
                    return;
                }

                // Create a new Data
                fieldMapper.CreateNestedData();
                return;
            }

            // A NestedType mismatch may result in a null value
            if (fieldMapper.FieldDesc.Data == null)
            {
                Logger.Log.Tool.Error($"Data is null for field {fieldMapper.FullName}");
                return;
            }

            // Set or add value to the field
            fieldMapper.SetOrAddValue(text);
        }

        public void ResetData(int afterX)
        {
            // Reset all data after the specified cell
            var found = ListFieldMapper.Values.Where(fm =>
                                                        fm.CellX >= afterX &&
                                                        fm.HasNested &&
                                                        fm.FieldDesc.NestedDataDesc.Data != null).Reverse().ToList();
            foreach (var fieldMapper in found)
            {
                PostProcess(fieldMapper.FieldDesc.NestedDataDesc);
                fieldMapper.SetOrAddData();
                fieldMapper.FieldDesc.NestedDataDesc.Data = null;
            }
        }

        public void PostProcess(DataDesc dataDesc)
        {
            var found = ListFieldMapper.Values.Where(fm =>
                                                        fm.OwnerDataDesc == dataDesc).ToList();
            foreach (var fieldMapper in found)
            {
                fieldMapper.PostProcess(this);
            }
        }

        public void SetOrAddData()
        {
            var found = ListFieldMapper.Values.Where(fm => fm.FieldDesc.HasNested &&
                                                            fm.FieldDesc.NestedDataDesc.Data != null).Reverse().ToList();
            foreach (var fieldMapper in found)
            {
                PostProcess(fieldMapper.FieldDesc.NestedDataDesc);
                fieldMapper.SetOrAddData();
            }
        }
    }
}
