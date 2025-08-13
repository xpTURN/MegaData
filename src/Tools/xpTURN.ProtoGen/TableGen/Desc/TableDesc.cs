using System.Collections.Generic;
using System.Linq;

using xpTURN.Common;
using xpTURN.Protobuf;
using xpTURN.MegaData;
using static xpTURN.TableGen.Utils.TableGenUtils;

namespace xpTURN.TableGen
{
    public class TableDesc
    {
        #region DebugInfo
        public DebugInfo DebugInfo { get;} = new DebugInfo();
        public string File { get { return DebugInfo.File; } set { DebugInfo.File = value; } }
        public string Line { get { return DebugInfo.Line; } set { DebugInfo.Line = value; } }
        #endregion

        #region Sheet Fields
        public TableType Type = TableType.Table;
        public string Name = string.Empty;
        public FieldObsolete Obsolete = FieldObsolete.None;

        public string FType = string.Empty;
        public string ExtraOptions = string.Empty;

        public string Desc = string.Empty;
        #endregion

        private List<FieldDesc> listField = new List<FieldDesc>();
        public void Add(FieldDesc value) => listField.Add(value);

        public Options ParsedExtraOpts { get; set; } = new Options();
        public bool IsOnDemand() => IsTable && ParsedExtraOpts.OnDemand && GetNestedField() != null;
        public bool IsWeakRef() => IsTable && ParsedExtraOpts.WeakRef && GetNestedField() != null;

        public bool IsEmpty => listField.Count == 0;

        public bool IsMessage => Type == TableType.Table || Type == TableType.Data;
        public bool IsTable => Type == TableType.Table;
        public bool IsData => Type == TableType.Data;
        public bool IsEnum => Type == TableType.Enum;
        
        public bool IsNestedData { get; set; } = false;
        public bool IsKeyType(xpFieldTypes type) =>
                   TableTypeUtils.IsAcceptKeyType(type);

        public class Options
        {
            public string Key { get; set; } = sID;
            public bool Hide { get; set; } = false;
            public bool OnDemand { get; set; } = false;
            public bool WeakRef { get; set; } = false;
        }

        public FieldDesc FindField(string Name)
        {
            foreach (var item in listField)
            {
                if (item.Name == Name)
                {
                    return item;
                }
            }

            return null;
        }

        public FieldDesc GetKeyField()
        {
            if (ParsedExtraOpts.Key != sID)
            {
                var keyField = listField.Find(field => field.Name == ParsedExtraOpts.Key);
                if (keyField == null)
                {
                    Logger.Log.Tool.Error(DebugInfo, $"Data '{Name}' has an invalid key field: '{ParsedExtraOpts.Key}'");
                    return null;
                }

                if (IsKeyType(keyField.FValueType))
                {
                    return keyField;
                }
                else
                {
                    // The specified key field is not a valid key type.
                    Logger.Log.Tool.Error(keyField.DebugInfo, $"Data '{Name}' has an invalid key field: '{keyField.Name}' (Type: {keyField.FValueTypeName})");
                    Logger.Log.Tool.Error(keyField.DebugInfo, $"Only Int32, SInt32, SFixed32, Enum, and String types are allowed as key fields.");
                    return null;
                }
            }

            List<FieldDesc> listID = new();
            foreach (var item in listField)
            {
                if (item.Name.EndsWith(sID) && !item.Name.EndsWith(sRefID))
                {
                    if (IsKeyType(item.FValueType))
                    {
                        listID.Add(item);
                    }
                    else
                    {
                        // The specified key field is not a valid key type.
                        Logger.Log.Tool.Error(item.DebugInfo, $"Data '{Name}' has an invalid key field: '{item.Name}' (Type: {item.FValueTypeName})");
                        Logger.Log.Tool.Error(item.DebugInfo, $"Only Int32, SInt32, SFixed32, Enum, and String types are allowed as key fields.");
                        return null;
                    }
                }
            }

            if (listID.Count == 0)
            {
                foreach (var item in listField)
                {
                    if (item.Name.EndsWith(sALIAS) && !item.Name.EndsWith(sID + sALIAS))
                    {
                        if (IsKeyType(item.FValueType))
                        {
                            listID.Add(item);
                        }
                        else
                        {
                            // The specified key field is not a valid key type.
                            Logger.Log.Tool.Error(item.DebugInfo, $"Data '{Name}' has an invalid key field: '{item.Name}' (Type: {item.FValueTypeName})");
                            Logger.Log.Tool.Error(item.DebugInfo, $"Only Int32, SInt32, SFixed32, Enum, and String types are allowed as key fields.");
                            return null;
                        }
                    }
                }
            }

            if (listID.Count == 1)
            {
                return listID[0];
            }
            else if (listID.Count > 1)
            {
                // If there are multiple ID fields, we cannot determine the key field.
                // This is an error condition.
                Logger.Log.Tool.Error(DebugInfo, $"Data '{Name}' has multiple key fields: {string.Join(", ", listID.Select(f => f.Name))}");
                Logger.Log.Tool.Error(DebugInfo, $"Please specify a single key field using the ExtraOptions.Key.");
            }

            return null;
        }

        public FieldDesc GetAliasField()
        {
            var keyField = GetKeyField();
            if (keyField == null)
                return null;

            if (keyField.FValueType == xpFieldTypes.Type_String)
                return null; // String type Id does not have an Alias field.

            string idAliasName = keyField.Name + sALIAS;
            var aliasField = listField.FirstOrDefault(item => item.Name == idAliasName && item.FValueType == xpFieldTypes.Type_String);
            if (aliasField != null)
            {
                return aliasField;
            }

            return null;
        }

        public bool IsOnDemandField(FieldDesc field)
        {
            var nestedField = GetNestedField();
            if (nestedField != null && nestedField.Name == field.Name)
            {
                return IsOnDemand() || IsWeakRef();
            }
            return false;
        }

        public bool IsWeakField(FieldDesc field)
        {
            var nestedField = GetNestedField();
            if (nestedField != null && nestedField.Name == field.Name)
            {
                return IsWeakRef();
            }

            return false;
        }

        public bool IsNonSerializedField(FieldDesc field)
        {
            var nestedField = GetNestedField();
            if (nestedField != null)
            {
                return (IsOnDemand() || IsWeakRef()) && nestedField.Name == field.Name;
            }

            return false;
        }

        public bool IsNestedField(FieldDesc field)
        {
            var nestedField = GetNestedField();
            return nestedField != null && nestedField.Name == field.Name;
        }

        public string GetNestedDataName()
        {
            return GetNestedField()?.FValueTypeName ?? string.Empty;
        }

        public FieldDesc GetNestedField()
        {
            return listField.FirstOrDefault(item => item.Collections == FieldCollections.Map &&
                                                        item.FValueType == xpFieldTypes.Type_Message &&
                                                        IsKeyType(item.FKeyType));
        }

        public List<FieldDesc> GetListField()
        {
            return listField;
        }
    }
}
