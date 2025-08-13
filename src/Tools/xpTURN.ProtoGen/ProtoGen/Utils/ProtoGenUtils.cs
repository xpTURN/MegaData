using System;
using System.Collections.Generic;

using Google.Protobuf.Reflection;

namespace xpTURN.ProtoGen
{
    public static class ProtoGenUtils
    {
        public static bool IsRepeated(this FieldDescriptorProto fieldDescriptorProto) => fieldDescriptorProto.label == FieldDescriptorProto.Label.LabelRepeated;
        public static uint Tag(this FieldDescriptorProto fieldDescriptorProto) => MakeTag(fieldDescriptorProto.Number, fieldDescriptorProto.WireType());
        public static ProtoBuf.WireType WireType(this FieldDescriptorProto fieldDescriptorProto) => GetWireType(fieldDescriptorProto.type, fieldDescriptorProto.IsRepeated());
        public static string WireTypeString(this FieldDescriptorProto fieldDescriptorProto)
        {
            var wireType = fieldDescriptorProto.WireType();
            switch (wireType)
            {
                case ProtoBuf.WireType.Varint:
                    return "Varint";
                case ProtoBuf.WireType.Fixed64:
                    return "Fixed64";
                case ProtoBuf.WireType.Fixed32:
                    return "Fixed32";
                case ProtoBuf.WireType.String:
                    return "LengthDelimited";
                case ProtoBuf.WireType.StartGroup:
                    return "StartGroup";
                case ProtoBuf.WireType.EndGroup:
                    return "EndGroup";

                default:
                case ProtoBuf.WireType.None:
                    return "None";
            }
        }

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

        private const int TagTypeBits = 3;
        public static uint MakeTag(int fieldNumber, ProtoBuf.WireType wireType)
        {
            return (uint)(fieldNumber << TagTypeBits) | (uint)wireType;
        }

        public static ProtoBuf.WireType GetWireType(FieldDescriptorProto.Type type, bool isRepeated)
        {
            if (isRepeated)
            {
                return ProtoBuf.WireType.String;
            }

            switch (type)
            {
                case FieldDescriptorProto.Type.TypeDouble:
                    return ProtoBuf.WireType.Fixed64;
                case FieldDescriptorProto.Type.TypeFloat:
                    return ProtoBuf.WireType.Fixed32;
                case FieldDescriptorProto.Type.TypeInt64:
                    return ProtoBuf.WireType.Varint;
                case FieldDescriptorProto.Type.TypeUint64:
                    return ProtoBuf.WireType.Varint;
                case FieldDescriptorProto.Type.TypeInt32:
                    return ProtoBuf.WireType.Varint;
                case FieldDescriptorProto.Type.TypeFixed64:
                    return ProtoBuf.WireType.Fixed64;
                case FieldDescriptorProto.Type.TypeFixed32:
                    return ProtoBuf.WireType.Fixed32;
                case FieldDescriptorProto.Type.TypeBool:
                    return ProtoBuf.WireType.Varint;
                case FieldDescriptorProto.Type.TypeString:
                    return ProtoBuf.WireType.String;
                case FieldDescriptorProto.Type.TypeMessage:
                    return ProtoBuf.WireType.String;
                case FieldDescriptorProto.Type.TypeBytes:
                    return ProtoBuf.WireType.String;
                case FieldDescriptorProto.Type.TypeUint32:
                    return ProtoBuf.WireType.Varint;
                case FieldDescriptorProto.Type.TypeEnum:
                    return ProtoBuf.WireType.Varint;
                case FieldDescriptorProto.Type.TypeSfixed32:
                    return ProtoBuf.WireType.Fixed32;
                case FieldDescriptorProto.Type.TypeSfixed64:
                    return ProtoBuf.WireType.Fixed64;
                case FieldDescriptorProto.Type.TypeSint32:
                    return ProtoBuf.WireType.Varint;
                case FieldDescriptorProto.Type.TypeSint64:
                    return ProtoBuf.WireType.Varint;

                case FieldDescriptorProto.Type.TypeGroup:
                    return ProtoBuf.WireType.None;

                case FieldDescriptorProto.Type.TypeDateTime:
                    return ProtoBuf.WireType.Varint;
                case FieldDescriptorProto.Type.TypeTimeSpan:
                    return ProtoBuf.WireType.Varint;
                case FieldDescriptorProto.Type.TypeGuid:
                    return ProtoBuf.WireType.String;
                case FieldDescriptorProto.Type.TypeUri:
                    return ProtoBuf.WireType.String;
            }
            return ProtoBuf.WireType.None;
        }

        public static string GetCSharpTypeName(FieldDescriptorProto.Type type, string typeName)
        {
            switch (type)
            {
                case FieldDescriptorProto.Type.TypeDouble:
                    return "double";
                case FieldDescriptorProto.Type.TypeFloat:
                    return "float";
                case FieldDescriptorProto.Type.TypeInt64:
                    return "long";
                case FieldDescriptorProto.Type.TypeUint64:
                    return "ulong";
                case FieldDescriptorProto.Type.TypeInt32:
                    return "int";
                case FieldDescriptorProto.Type.TypeFixed64:
                    return "ulong";
                case FieldDescriptorProto.Type.TypeFixed32:
                    return "uint";
                case FieldDescriptorProto.Type.TypeBool:
                    return "bool";
                case FieldDescriptorProto.Type.TypeString:
                    return "string";
                case FieldDescriptorProto.Type.TypeMessage:
                    return Escape(typeName);
                case FieldDescriptorProto.Type.TypeBytes:
                    return "ByteString";
                case FieldDescriptorProto.Type.TypeUint32:
                    return "uint";
                case FieldDescriptorProto.Type.TypeEnum:
                    return Escape(typeName);
                case FieldDescriptorProto.Type.TypeSfixed32:
                    return "int";
                case FieldDescriptorProto.Type.TypeSfixed64:
                    return "long";
                case FieldDescriptorProto.Type.TypeSint32:
                    return "int";
                case FieldDescriptorProto.Type.TypeSint64:
                    return "long";

                case FieldDescriptorProto.Type.TypeDateTime:
                    return "DateTime";
                case FieldDescriptorProto.Type.TypeTimeSpan:
                    return "TimeSpan";
                case FieldDescriptorProto.Type.TypeGuid:
                    return "Guid";
                case FieldDescriptorProto.Type.TypeUri:
                    return "Uri";
            }

            return Escape(typeName);
        }

        public static string GetGoogleSubFuncName(FieldDescriptorProto.Type type)
        {
            switch (type)
            {
                case FieldDescriptorProto.Type.TypeDouble:
                    return "Double";
                case FieldDescriptorProto.Type.TypeFloat:
                    return "Float";
                case FieldDescriptorProto.Type.TypeInt64:
                    return "Int64";
                case FieldDescriptorProto.Type.TypeUint64:
                    return "UInt64";
                case FieldDescriptorProto.Type.TypeInt32:
                    return "Int32";
                case FieldDescriptorProto.Type.TypeFixed64:
                    return "Fixed64";
                case FieldDescriptorProto.Type.TypeFixed32:
                    return "Fixed32";
                case FieldDescriptorProto.Type.TypeBool:
                    return "Bool";
                case FieldDescriptorProto.Type.TypeString:
                    return "String";
                case FieldDescriptorProto.Type.TypeMessage:
                    return "Message";
                case FieldDescriptorProto.Type.TypeBytes:
                    return "Bytes";
                case FieldDescriptorProto.Type.TypeUint32:
                    return "UInt32";
                case FieldDescriptorProto.Type.TypeEnum:
                    return "Enum";
                case FieldDescriptorProto.Type.TypeSfixed32:
                    return "SFixed32";
                case FieldDescriptorProto.Type.TypeSfixed64:
                    return "SFixed64";
                case FieldDescriptorProto.Type.TypeSint32:
                    return "SInt32";
                case FieldDescriptorProto.Type.TypeSint64:
                    return "SInt64";

                case FieldDescriptorProto.Type.TypeDateTime:
                    return "DateTime";
                case FieldDescriptorProto.Type.TypeTimeSpan:
                    return "TimeSpan";
                case FieldDescriptorProto.Type.TypeGuid:
                    return "Guid";
                case FieldDescriptorProto.Type.TypeUri:
                    return "Uri";

                default:
                    throw new ArgumentException($"Unsupported field type: {type}");
            }
        }

        public static string GetCSharpDefaultValue(bool isRepeated, FieldDescriptorProto.Type type, string defaultString)
        {
            if (isRepeated)
            {
                return "new ()";
            }

            switch (type)
            {
                case FieldDescriptorProto.Type.TypeInt32:
                case FieldDescriptorProto.Type.TypeSint32:
                case FieldDescriptorProto.Type.TypeSfixed32:
                    if (string.IsNullOrEmpty(defaultString))
                    {
                        return null;
                    }
                    else
                    {
                        int output;
                        if (int.TryParse(defaultString, out output))
                        {
                            return $"{defaultString}";
                        }
                        else
                        {
                            return null;
                        }
                    }

                case FieldDescriptorProto.Type.TypeInt64:
                case FieldDescriptorProto.Type.TypeSint64:
                case FieldDescriptorProto.Type.TypeSfixed64:
                    if (string.IsNullOrEmpty(defaultString))
                    {
                        return null;
                    }
                    else
                    {
                        long output;
                        if (long.TryParse(defaultString, out output))
                        {
                            return $"{defaultString}";
                        }
                        else
                        {
                            return null;
                        }
                    }

                case FieldDescriptorProto.Type.TypeFixed32:
                case FieldDescriptorProto.Type.TypeUint32:
                    if (string.IsNullOrEmpty(defaultString))
                    {
                        return null;
                    }
                    else
                    {
                        uint output;
                        if (uint.TryParse(defaultString, out output))
                        {
                            return $"{defaultString}";
                        }
                        else
                        {
                            return null;
                        }
                    }

                case FieldDescriptorProto.Type.TypeFixed64:
                case FieldDescriptorProto.Type.TypeUint64:
                    if (string.IsNullOrEmpty(defaultString))
                    {
                        return null;
                    }
                    else
                    {
                        ulong output;
                        if (ulong.TryParse(defaultString, out output))
                        {
                            return $"{defaultString}";
                        }
                        else
                        {
                            return null;
                        }
                    }

                case FieldDescriptorProto.Type.TypeDouble:
                    if (string.IsNullOrEmpty(defaultString))
                    {
                        return null;
                    }
                    else
                    {
                        double output;
                        if (double.TryParse(defaultString, out output))
                        {
                            return $"{defaultString}";
                        }
                        else
                        {
                            return null;
                        }
                    }

                case FieldDescriptorProto.Type.TypeFloat:
                    if (string.IsNullOrEmpty(defaultString))
                    {
                        return null;
                    }
                    else
                    {
                        float output;
                        if (float.TryParse(defaultString, out output))
                        {
                            return $"{defaultString}f";
                        }
                        else
                        {
                            return null;
                        }
                    }

                case FieldDescriptorProto.Type.TypeBool:
                    if (string.IsNullOrEmpty(defaultString))
                    {
                        return null;
                    }
                    else
                    {
                        bool output;
                        if (bool.TryParse(defaultString.ToLower(), out output))
                        {
                            return output.ToString().ToLower();
                        }
                        else
                        {
                            return null;
                        }
                    }

                case FieldDescriptorProto.Type.TypeString:
                    if (string.IsNullOrEmpty(defaultString))
                    {
                        return $"\"\""; ;
                    }
                    else
                    {
                        return $"\"{defaultString}\"";
                    }

                case FieldDescriptorProto.Type.TypeBytes:
                    return "ByteString.Empty";

                case FieldDescriptorProto.Type.TypeMessage:
                    return "null";

                case FieldDescriptorProto.Type.TypeEnum:
                    return null;

                case FieldDescriptorProto.Type.TypeDateTime:
                    if (!string.IsNullOrEmpty(defaultString))
                    {
                        DateTime output;
                        if (DateTime.TryParse(defaultString, out output))
                        {
                            return $"new DateTime({output})";
                        }
                    }
                    return null;

                case FieldDescriptorProto.Type.TypeTimeSpan:
                    if (!string.IsNullOrEmpty(defaultString))
                    {
                        TimeSpan output;
                        if (TimeSpan.TryParse(defaultString, out output))
                        {
                            return $"new TimeSpan({output.Ticks})";
                        }
                    }
                    return null;

                case FieldDescriptorProto.Type.TypeUri:
                    if (!string.IsNullOrEmpty(defaultString))
                    {
                        Uri output;
                        if (Uri.TryCreate(defaultString, UriKind.RelativeOrAbsolute, out output))
                        {
                            return $"new Uri(\"{output}\")";
                        }
                    }
                    return null;

                case FieldDescriptorProto.Type.TypeGuid:
                    if (!string.IsNullOrEmpty(defaultString))
                    {
                        Guid output;
                        if (Guid.TryParse(defaultString, out output))
                        {
                            return $"new Guid(\"{output}\")";
                        }
                    }
                    return $"Guid.Empty";
            }

            return null;
        }

        public static string GetCSharpDefaultValueByType(FieldDescriptorProto.Type type, string typeName)
        {
            switch (type)
            {
                case FieldDescriptorProto.Type.TypeInt32:
                case FieldDescriptorProto.Type.TypeSint32:
                case FieldDescriptorProto.Type.TypeSfixed32:
                    return "0";

                case FieldDescriptorProto.Type.TypeInt64:
                case FieldDescriptorProto.Type.TypeSint64:
                case FieldDescriptorProto.Type.TypeSfixed64:
                    return "0L";

                case FieldDescriptorProto.Type.TypeFixed32:
                case FieldDescriptorProto.Type.TypeUint32:
                    return "0U";

                case FieldDescriptorProto.Type.TypeFixed64:
                case FieldDescriptorProto.Type.TypeUint64:
                    return "0UL";

                case FieldDescriptorProto.Type.TypeDouble:
                    return "0D";

                case FieldDescriptorProto.Type.TypeFloat:
                    return "0F";

                case FieldDescriptorProto.Type.TypeBool:
                    return "false";

                case FieldDescriptorProto.Type.TypeString:
                    return $"\"\"";

                case FieldDescriptorProto.Type.TypeBytes:
                    return $"ByteString.Empty";

                case FieldDescriptorProto.Type.TypeMessage:
                    return "null";

                case FieldDescriptorProto.Type.TypeEnum:
                    return $"default({typeName})";

                case FieldDescriptorProto.Type.TypeDateTime:
                    return "default(DateTime)";
                case FieldDescriptorProto.Type.TypeTimeSpan:
                    return "default(TimeSpan)";
                case FieldDescriptorProto.Type.TypeUri:
                    return "default(Uri)";
                case FieldDescriptorProto.Type.TypeGuid:
                    return "default(Guid)";
            }

            return null;
        }

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
