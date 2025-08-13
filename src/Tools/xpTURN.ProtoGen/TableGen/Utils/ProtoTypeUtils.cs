using System;

using xpTURN.Protobuf;
using static xpTURN.TableGen.Utils.TableGenUtils;

namespace xpTURN.TableGen.Utils
{
    public class ProtoTypeUtils
    {
        public static WireFormat.WireType GetWireTypeByFValueType(xpFieldTypes fType)
        {
            switch (fType)
            {
                case xpFieldTypes.Type_Enum:
                    return WireFormat.WireType.Varint;

                case xpFieldTypes.Type_Bool:
                    return WireFormat.WireType.Varint;

                case xpFieldTypes.Type_Int32:
                case xpFieldTypes.Type_SInt32:
                case xpFieldTypes.Type_UInt32:
                case xpFieldTypes.Type_Int64:
                case xpFieldTypes.Type_SInt64:
                case xpFieldTypes.Type_UInt64:
                    return WireFormat.WireType.Varint;

                case xpFieldTypes.Type_SFixed32:
                case xpFieldTypes.Type_Fixed32:
                case xpFieldTypes.Type_Float:
                    return WireFormat.WireType.Fixed32;

                case xpFieldTypes.Type_SFixed64:
                case xpFieldTypes.Type_Fixed64:
                case xpFieldTypes.Type_Double:
                    return WireFormat.WireType.Fixed64;

                case xpFieldTypes.Type_String:
                case xpFieldTypes.Type_Bytes:
                case xpFieldTypes.Type_Message:
                    return WireFormat.WireType.LengthDelimited;

                case xpFieldTypes.Type_DateTime:
                    return WireFormat.WireType.Varint;
                case xpFieldTypes.Type_TimeSpan:
                    return WireFormat.WireType.Varint;

                case xpFieldTypes.Type_Uri:
                    return WireFormat.WireType.LengthDelimited;
                case xpFieldTypes.Type_Guid:
                    return WireFormat.WireType.LengthDelimited;

                case xpFieldTypes.Type_Unknown:
                default:
                    throw new ArgumentException($"Unknown FieldType '{fType}'");
            }
        }

        public static string GetGoogleTypeName(xpFieldTypes type)
        {
            switch (type)
            {
                case xpFieldTypes.Type_Enum:
                    return "Enum";

                case xpFieldTypes.Type_Bool:
                    return "Bool";

                case xpFieldTypes.Type_Int32:
                    return "Int32";
                case xpFieldTypes.Type_SInt32:
                    return "SInt32";
                case xpFieldTypes.Type_SFixed32:
                    return "SFixed32";

                case xpFieldTypes.Type_UInt32:
                    return "UInt32";
                case xpFieldTypes.Type_Fixed32:
                    return "Fixed32";

                case xpFieldTypes.Type_Int64:
                    return "Int64";
                case xpFieldTypes.Type_SInt64:
                    return "SInt64";
                case xpFieldTypes.Type_SFixed64:
                    return "SFixed64";

                case xpFieldTypes.Type_UInt64:
                    return "UInt64";
                case xpFieldTypes.Type_Fixed64:
                    return "Fixed64";

                case xpFieldTypes.Type_Float:
                    return "Float";
                case xpFieldTypes.Type_Double:
                    return "Double";

                case xpFieldTypes.Type_String:
                    return "String";
                case xpFieldTypes.Type_Bytes:
                    return "Bytes";
                case xpFieldTypes.Type_Message:
                    return "Message";

                case xpFieldTypes.Type_DateTime:
                    return "DateTime";
                case xpFieldTypes.Type_TimeSpan:
                    return "TimeSpan";
                case xpFieldTypes.Type_Uri:
                    return "Uri";
                case xpFieldTypes.Type_Guid:
                    return "Guid";

                default:
                    throw new ArgumentException($"Unsupported field type: {type}");
            }
        }

        public static string GetProtoTypeName(xpFieldTypes fType, string typeName)
        {
            switch (fType)
            {
                case xpFieldTypes.Type_Enum:
                    return typeName;

                case xpFieldTypes.Type_Bool:
                    return "bool";

                case xpFieldTypes.Type_Int32:
                    return "int32";
                case xpFieldTypes.Type_SInt32:
                    return "sint32";
                case xpFieldTypes.Type_SFixed32:
                    return "sfixed32";

                case xpFieldTypes.Type_UInt32:
                    return "uint32";
                case xpFieldTypes.Type_Fixed32:
                    return "fixed32";

                case xpFieldTypes.Type_Int64:
                    return "int64";
                case xpFieldTypes.Type_SInt64:
                    return "sint64";
                case xpFieldTypes.Type_SFixed64:
                    return "sfixed64";

                case xpFieldTypes.Type_UInt64:
                    return "uint64";
                case xpFieldTypes.Type_Fixed64:
                    return "fixed64";

                case xpFieldTypes.Type_Float:
                    return "float";
                case xpFieldTypes.Type_Double:
                    return "double";

                case xpFieldTypes.Type_String:
                    return "string";
                case xpFieldTypes.Type_Bytes:
                    return "bytes";
                case xpFieldTypes.Type_Message:
                    return typeName;

                case xpFieldTypes.Type_DateTime:
                    return "DateTime";
                case xpFieldTypes.Type_TimeSpan:
                    return "TimeSpan";
                case xpFieldTypes.Type_Uri:
                    return "Uri";
                case xpFieldTypes.Type_Guid:
                    return "Guid";
            }

            return typeName;
        }

        public static string GetCSharpTypeName(xpFieldTypes fType, string typeName)
        {
            switch (fType)
            {
                case xpFieldTypes.Type_Enum:
                    return Escape(typeName);

                case xpFieldTypes.Type_Bool:
                    return "bool";

                case xpFieldTypes.Type_Int32:
                    return "int";
                case xpFieldTypes.Type_SInt32:
                    return "int";
                case xpFieldTypes.Type_SFixed32:
                    return "int";

                case xpFieldTypes.Type_UInt32:
                    return "uint";
                case xpFieldTypes.Type_Fixed32:
                    return "uint";

                case xpFieldTypes.Type_Int64:
                    return "long";
                case xpFieldTypes.Type_SInt64:
                    return "long";
                case xpFieldTypes.Type_SFixed64:
                    return "long";

                case xpFieldTypes.Type_UInt64:
                    return "ulong";
                case xpFieldTypes.Type_Fixed64:
                    return "ulong";

                case xpFieldTypes.Type_Float:
                    return "float";
                case xpFieldTypes.Type_Double:
                    return "double";

                case xpFieldTypes.Type_String:
                    return "string";
                case xpFieldTypes.Type_Bytes:
                    return "ByteString";
                case xpFieldTypes.Type_Message:
                    return Escape(typeName);

                case xpFieldTypes.Type_DateTime:
                    return "DateTime";
                case xpFieldTypes.Type_TimeSpan:
                    return "TimeSpan";
                case xpFieldTypes.Type_Uri:
                    return "Uri";
                case xpFieldTypes.Type_Guid:
                    return "Guid";
            }

            return Escape(typeName);
        }

        public static bool GetCSharpDefaultValueByType(xpFieldTypes fType, string typeName, out string defaultString)
        {
            switch (fType)
            {
                case xpFieldTypes.Type_Enum:
                    defaultString = $"default({typeName})";
                    return true;

                case xpFieldTypes.Type_Bool:
                    defaultString = "false";
                    return true;

                case xpFieldTypes.Type_Int32:
                case xpFieldTypes.Type_SInt32:
                case xpFieldTypes.Type_SFixed32:
                    defaultString = "0";
                    return true;

                case xpFieldTypes.Type_UInt32:
                case xpFieldTypes.Type_Fixed32:
                    defaultString = "0U";
                    return true;

                case xpFieldTypes.Type_Int64:
                case xpFieldTypes.Type_SInt64:
                case xpFieldTypes.Type_SFixed64:
                    defaultString = "0L";
                    return true;

                case xpFieldTypes.Type_UInt64:
                case xpFieldTypes.Type_Fixed64:
                    defaultString = "0UL";
                    return true;

                case xpFieldTypes.Type_Float:
                    defaultString = "0F";
                    return true;
                case xpFieldTypes.Type_Double:
                    defaultString = "0D";
                    return true;

                case xpFieldTypes.Type_String:
                    defaultString = $"\"\"";
                    return true;
                case xpFieldTypes.Type_Bytes:
                    defaultString = $"\"\"";
                    return true;
                case xpFieldTypes.Type_Message:
                    defaultString = $"\"\"";
                    return true;

                case xpFieldTypes.Type_DateTime:
                    defaultString = "default(DateTime)";
                    return true;
                case xpFieldTypes.Type_TimeSpan:
                    defaultString = "default(TimeSpan)";
                    return true;
                case xpFieldTypes.Type_Uri:
                    defaultString = "default(Uri)";
                    return true;
                case xpFieldTypes.Type_Guid:
                    defaultString = "default(Guid)";
                    return true;
            }

            defaultString = string.Empty;
            return false;
        }

        public static bool GetCSharpDefaultValue(FieldCollections fCollections, xpFieldTypes fType, ref string defaultString)
        {
            if (fCollections != FieldCollections.None)
            {
                defaultString = "new ()";
                return true;
            }

            switch (fType)
            {
                case xpFieldTypes.Type_Unknown:
                    defaultString = string.Empty;
                    return true;

                case xpFieldTypes.Type_Enum:
                    return true;

                case xpFieldTypes.Type_Bool:
                    if (string.IsNullOrEmpty(defaultString))
                    {
                        defaultString = string.Empty;
                        return true;
                    }
                    else
                    {
                        bool output;
                        if (bool.TryParse(defaultString.ToLower(), out output))
                        {
                            defaultString = output.ToString().ToLower();
                            return true;
                        }
                        else
                        {
                            defaultString = string.Empty;
                            return false;
                        }
                    }

                case xpFieldTypes.Type_Int32:
                case xpFieldTypes.Type_SInt32:
                case xpFieldTypes.Type_SFixed32:
                    if (string.IsNullOrEmpty(defaultString))
                    {
                        defaultString = string.Empty;
                        return true;
                    }
                    else
                    {
                        int output;
                        if (int.TryParse(defaultString, out output))
                        {
                            defaultString = $"{defaultString}";
                            return true;
                        }
                        else
                        {
                            defaultString = string.Empty;
                            return false;
                        }
                    }

                case xpFieldTypes.Type_UInt32:
                case xpFieldTypes.Type_Fixed32:
                    if (string.IsNullOrEmpty(defaultString))
                    {
                        defaultString = string.Empty;
                        return true;
                    }
                    else
                    {
                        uint output;
                        if (uint.TryParse(defaultString, out output))
                        {
                            defaultString = $"{defaultString}";
                            return true;
                        }
                        else
                        {
                            defaultString = string.Empty;
                            return false;
                        }
                    }

                case xpFieldTypes.Type_Int64:
                case xpFieldTypes.Type_SInt64:
                case xpFieldTypes.Type_SFixed64:
                    if (string.IsNullOrEmpty(defaultString))
                    {
                        defaultString = string.Empty;
                        return true;
                    }
                    else
                    {
                        long output;
                        if (long.TryParse(defaultString, out output))
                        {
                            defaultString = $"{defaultString}";
                            return true;
                        }
                        else
                        {
                            defaultString = string.Empty;
                            return false;
                        }
                    }

                case xpFieldTypes.Type_UInt64:
                case xpFieldTypes.Type_Fixed64:
                    if (string.IsNullOrEmpty(defaultString))
                    {
                        defaultString = string.Empty;
                        return true;
                    }
                    else
                    {
                        ulong output;
                        if (ulong.TryParse(defaultString, out output))
                        {
                            defaultString = $"{defaultString}";
                            return true;
                        }
                        else
                        {
                            defaultString = string.Empty;
                            return false;
                        }
                    }

                case xpFieldTypes.Type_Float:
                    if (string.IsNullOrEmpty(defaultString))
                    {
                        defaultString = string.Empty;
                        return true;
                    }
                    else
                    {
                        float output;
                        if (float.TryParse(defaultString, out output))
                        {
                            defaultString = $"{defaultString}f";
                            return true;
                        }
                        else
                        {
                            defaultString = string.Empty;
                            return false;
                        }
                    }

                case xpFieldTypes.Type_Double:
                    if (string.IsNullOrEmpty(defaultString))
                    {
                        defaultString = string.Empty;
                        return true;
                    }
                    else
                    {
                        double output;
                        if (double.TryParse(defaultString, out output))
                        {
                            defaultString = $"{defaultString}";
                            return true;
                        }
                        else
                        {
                            defaultString = string.Empty;
                            return false;
                        }
                    }

                case xpFieldTypes.Type_String:
                    if (string.IsNullOrEmpty(defaultString))
                    {
                        defaultString = $"\"\""; ;
                        return true;
                    }
                    else
                    {
                        defaultString = $"\"{defaultString}\"";
                    }
                    return true;

                case xpFieldTypes.Type_Bytes:
                    defaultString = $"ByteString.Empty";
                    return true;

                case xpFieldTypes.Type_Message:
                    defaultString = string.Empty;
                    return true;

                case xpFieldTypes.Type_DateTime:
                    if (!string.IsNullOrEmpty(defaultString))
                    {
                        DateTime output;
                        if (DateTime.TryParse(defaultString, out output))
                        {
                            defaultString = $"new DateTime({output})";
                            return true;
                        }
                    }

                    defaultString = string.Empty;
                    return true;

                case xpFieldTypes.Type_TimeSpan:
                    if (!string.IsNullOrEmpty(defaultString))
                    {
                        TimeSpan output;
                        if (TimeSpan.TryParse(defaultString, out output))
                        {
                            defaultString = $"new TimeSpan({output.Ticks})";
                            return true;
                        }
                    }
                    defaultString = string.Empty;
                    return true;

                case xpFieldTypes.Type_Uri:
                    if (!string.IsNullOrEmpty(defaultString))
                    {
                        Uri output;
                        if (Uri.TryCreate(defaultString, UriKind.RelativeOrAbsolute, out output))
                        {
                            defaultString = $"new Uri(\"{output}\")";
                            return true;
                        }
                    }
                    defaultString = $"";
                    return true;

                case xpFieldTypes.Type_Guid:
                    if (!string.IsNullOrEmpty(defaultString))
                    {
                        Guid output;
                        if (Guid.TryParse(defaultString, out output))
                        {
                            defaultString = $"new Guid(\"{output}\")";
                            return true;
                        }
                    }
                    defaultString = $"Guid.Empty";
                    return true;
            }

            defaultString = string.Empty;
            return false;
        }
    }
}
