using xpTURN.Common;
using xpTURN.Protobuf;
using static xpTURN.TableGen.Utils.ProtoTypeUtils;

namespace xpTURN.TableGen
{
    public class FieldDesc
    {
        #region DebugInfo
        public DebugInfo DebugInfo { get;} = new DebugInfo();
        public string File { get { return DebugInfo.File; } set { DebugInfo.File = value; } }
        public string Line { get { return DebugInfo.Line; } set { DebugInfo.Line = value; } }
        #endregion

        #region Sheet Fields
        public int Num = 0;
        public string Name = string.Empty;
        public FieldObsolete Obsolete = FieldObsolete.None;

        public string FType = string.Empty;

        public FieldCollections Collections = FieldCollections.None;

        public xpFieldTypes FKeyType = xpFieldTypes.Type_Unknown;
        public string FKeyTypeName = string.Empty;

        public xpFieldTypes FValueType = xpFieldTypes.Type_Unknown;
        public string FValueTypeName = string.Empty;

        public string Default = string.Empty;
        public string ExtraOptions = string.Empty;

        public string Desc = string.Empty;
        #endregion
    
        //
        public bool NonProtoField { get; set; } = false;
        bool TypeSupportsPacking => FValueType != xpFieldTypes.Type_Unknown &&
                                    FValueType != xpFieldTypes.Type_String &&
                                    FValueType != xpFieldTypes.Type_Guid &&
                                    FValueType != xpFieldTypes.Type_Uri &&
                                    FValueType != xpFieldTypes.Type_Bytes &&
                                    FValueType != xpFieldTypes.Type_Message;

        public uint Tag => WireFormat.MakeTag(Num, WireType);
        public uint KeyTag => WireFormat.MakeTag(1, KeyWireType);
        public uint ValueTag => WireFormat.MakeTag(2, ValueWireType);

        public WireFormat.WireType WireType => Collections != FieldCollections.None ? WireFormat.WireType.LengthDelimited : GetWireTypeByFValueType(FValueType);
        public WireFormat.WireType KeyWireType => GetWireTypeByFValueType(FKeyType);
        public WireFormat.WireType ValueWireType => GetWireTypeByFValueType(FValueType);

        public bool IsPackedRepeatedField => TypeSupportsPacking && Collections == FieldCollections.List;

        public Options ParsedExtraOpts { get; set; } = new Options();

        public class Options
        {
            public string Get { get; set; } = string.Empty;
            public string MinValue { get; set; } = string.Empty;
            public string MaxValue { get; set; } = string.Empty;
        }
    }

}
