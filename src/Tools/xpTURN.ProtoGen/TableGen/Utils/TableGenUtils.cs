using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text.RegularExpressions;

using xpTURN.Common;
using xpTURN.Protobuf;

namespace xpTURN.TableGen.Utils
{
    public static class TableGenUtils
    {
        public static readonly string sTABLE = "Table";
        public static readonly string sID = "Id";
        public static readonly string sRefID = "RefId";
        public static readonly string sALIAS = "Alias";


        public static int GetTagByteCount(uint tag)
        {
            int byteCount = 0;
            while (tag > 127)
            {
                tag >>= 7;
                byteCount++;
            }

            // for the last byte
            byteCount++;
            return byteCount;
        }

        public static int GetTagBytes(uint tag, out List<byte> tagBytes)
        {
            tagBytes = new List<byte>(5);
            while (tag > 127)
            {
                var b = (byte)((tag & 0x7F) | 0x80);
                tag >>= 7;

                tagBytes.Add(b);
            }

            // for the last byte
            tagBytes.Add((byte)tag);
            return tagBytes.Count;
        }

        public static string ToCamelCase(string name)
        {
            if (string.IsNullOrEmpty(name) || char.IsLower(name[0]))
                return name;
            if (name.Length == 1)
                return name.ToLower();
            return char.ToLower(name[0]) + name.Substring(1);
        }

        public static string ConvertSpecialCharacters(string input)
        {
            var specialCharsSet = new List<(string, string)>
            {
                ("<",   "\u27E8"), //  [ <, "\u27E8" ] to ⟨ (Mathematical Left Angle Bracket)
                (">",   "\u27E9"), //  [ >, "\u27E9" ] to ⟩ (Mathematical Right Angle Bracket)
                ("&",   "\u0026"), //  [ &, "\u0026" ] to ＆ (Fullwidth Ampersand)
                ("\"",  "\u201C"), //  [ ", "\u201C" ] to “ (Left Double Quotation Mark)
                ("\'",  "\u2018"), //  [ ', "\u2018" ] to ‘ (Left Single Quotation Mark)
            };

            foreach (var item in specialCharsSet)
            {
                input = input.Replace(item.Item1.ToString(), item.Item2);
            }

            return input;
        }

        public static void Matching<T>(Dictionary<int, FieldInfo> matching, List<string> list) where T : class
        {
            //
            matching.Clear();

            //
            var type = typeof(T);
            List<FieldInfo> fields = new List<FieldInfo>();
            fields.AddRange(type.GetFields());

            //
            for (int i = 0; i < list.Count; ++i)
            {
                var item = list[i];
                if (item.StartsWith("#")) // Ignore comments
                    continue;

                var fieldInfo = fields.Find(x => x.Name == item);
                if (fieldInfo != null && fieldInfo.IsPublic && !fieldInfo.IsStatic)
                {
                    matching.Add(i, fieldInfo);
                }
                else
                {
                    Logger.Log.Tool.Error($"Field '{item}' not found in type '{type.Name}'");
                }
            }
        }

        private static void SetValue(object obj, string fieldName, object value)
        {
            var type = obj.GetType();
            var fieldInfo = type.GetField(fieldName);
            if (fieldInfo != null)
            {
                fieldInfo.SetValue(obj, value);
            }

            var propertyInfo = type.GetProperty(fieldName);
            if (propertyInfo != null)
            {
                propertyInfo.SetValue(obj, value);
            }
        }

        private static bool TryParse(Type type, string input, out object value)
        {
            value = type.DefaultValue();

            try
            {
                var converter = TypeDescriptor.GetConverter(type);
                if (converter != null)
                {
                    value = converter.ConvertFromString(input);
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private static bool FTypeForMergeTo<T>(T obj, string strValue)
        {
            var keyType = xpFieldTypes.Type_Unknown;
            var keyTypeName = string.Empty;

            var valueType = xpFieldTypes.Type_Unknown;
            var valueTypeName = string.Empty;

            var match = Regex.Match(strValue, @"^Map<\s*([\w]+)\s*,\s*([\w]+)\s*>$");
            if (match.Success)
            {
                SetValue(obj, "Collections", FieldCollections.Map);

                keyTypeName = match.Groups[1].Value;
                if (Enum.TryParse("Type_" + keyTypeName, true, out keyType))
                    SetValue(obj, "FKeyType", keyType);
                SetValue(obj, "FKeyTypeName", keyTypeName);

                valueTypeName = match.Groups[2].Value;
                if (Enum.TryParse("Type_" + valueTypeName, true, out valueType))
                    SetValue(obj, "FValueType", valueType);

                SetValue(obj, "FValueTypeName", valueTypeName);
                return true;
            }

            match = Regex.Match(strValue, @"^List<\s*([\w]+)\s*>$");
            if (match.Success)
            {
                SetValue(obj, "Collections", FieldCollections.List);

                valueTypeName = match.Groups[1].Value;
                if (Enum.TryParse("Type_" + valueTypeName, true, out valueType))
                    SetValue(obj, "FValueType", valueType);

                SetValue(obj, "FValueTypeName", valueTypeName);
                return true;
            }

            valueTypeName = strValue;
            if (Enum.TryParse("Type_" + valueTypeName, true, out valueType))
            {
                SetValue(obj, "FValueType", valueType);
            }

            SetValue(obj, "FValueTypeName", valueTypeName);

            return true;
        }

        public static T MergeTo<T>(Dictionary<int, FieldInfo> memberByIndex, List<string> list, T obj) where T : class
        {
            //
            SetValue(obj, "File", Logger.Log.Tool.GetFile());
            SetValue(obj, "Line", Logger.Log.Tool.GetLine());

            //
            var objField = obj as FieldDesc;

            //
            for (int i = 0; i < list.Count; ++i)
            {
                if (memberByIndex.ContainsKey(i) == false)
                    continue;

                var strValue = list[i].Trim();
                if (string.IsNullOrEmpty(strValue))
                    continue;

                FieldInfo fieldInfo = memberByIndex[i] as FieldInfo;
                if (fieldInfo == null)
                    continue;

                //
                if (fieldInfo.Name == "FType")
                {
                    if (!FTypeForMergeTo(obj, strValue))
                    {
                        Logger.Log.Tool.Error(objField.DebugInfo, $"Unknown Type '{strValue}'");
                    }
                }
                else
                {
                    object value;
                    if (!TryParse(fieldInfo.FieldType, strValue, out value))
                    {
                        Logger.Log.Tool.Error(objField.DebugInfo, $"Cannot parse '{strValue}' to type '{fieldInfo.FieldType.Name}'");
                    }

                    fieldInfo.SetValue(obj, value);
                }
            }

            return obj;
        }

        public static bool IsReservedType(string identifier)
        {
            if (string.IsNullOrEmpty(identifier))
                return false;

            return ReservedTypes.Contains(identifier);
        }

        public static bool IsReservedTableType(string identifier)
        {
            if (string.IsNullOrEmpty(identifier))
                return false;

            return ReservedTableTypes.Contains(identifier);
        }

        private static readonly HashSet<string> ReservedTypes = new HashSet<string>
        {
            "Bool",
            "Int32", "SInt32", "SFixed32",
            "UInt32", "Fixed32",
            "Int64", "SInt64", "SFixed64",
            "UInt64", "Fixed64",
            "Float", "Double",
            "String",
            "Bytes",
            "ByteString",

            "DateTime", "TimeSpan", "Guid", "Uri",
        };

        private static readonly HashSet<string> ReservedTableTypes = new HashSet<string>
        {
            "Data", "Table",
            "AliasData", "Header",
            "MetaData", "MetaDataTable", "MetaNestedData",
        };

        public static string Escape(string identifier)
        {
            switch (identifier)
            {
                case "abstract":
                case "event":
                case "new":
                case "struct":
                case "as":
                case "explicit":
                case "null":
                case "switch":
                case "base":
                case "extern":
                case "object":
                case "this":
                case "bool":
                case "false":
                case "operator":
                case "throw":
                case "break":
                case "finally":
                case "out":
                case "true":
                case "byte":
                case "fixed":
                case "override":
                case "try":
                case "case":
                case "float":
                case "params":
                case "typeof":
                case "catch":
                case "for":
                case "private":
                case "uint":
                case "char":
                case "foreach":
                case "protected":
                case "ulong":
                case "checked":
                case "goto":
                case "public":
                case "unchecked":
                case "class":
                case "if":
                case "readonly":
                case "unsafe":
                case "const":
                case "implicit":
                case "ref":
                case "ushort":
                case "continue":
                case "in":
                case "return":
                case "using":
                case "decimal":
                case "int":
                case "sbyte":
                case "virtual":
                case "default":
                case "interface":
                case "sealed":
                case "volatile":
                case "delegate":
                case "internal":
                case "short":
                case "void":
                case "do":
                case "is":
                case "sizeof":
                case "while":
                case "double":
                case "lock":
                case "stackalloc":
                case "else":
                case "long":
                case "static":
                case "enum":
                case "namespace":
                case "string":
                    return "@" + identifier;
                default:
                    return identifier;
            }
        }
    }
}
