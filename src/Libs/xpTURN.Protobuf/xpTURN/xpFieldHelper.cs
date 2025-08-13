#region Copyright notice and license
// Protocol Buffers - Google's data interchange format
// Copyright 2008 Google Inc.  All rights reserved.
//
// Use of this source code is governed by a BSD-style
// license that can be found in the LICENSE file or at
// https://developers.google.com/open-source/licenses/bsd
#endregion
using System;

using static xpTURN.Protobuf.CodedOutputStream;
using static xpTURN.Protobuf.xpDateTimeUtils;

namespace xpTURN.Protobuf
{
    public static class xpFieldHelper
    {
        public static xpFieldCodec<bool> BoolCodec = new(
            xpFieldTypes.Type_Bool,
            (ref WriteContext ctx, bool value) => ctx.WriteBool(value),
            (ref ParseContext ctx) => ctx.ReadBool(),
            (bool value) => ComputeBoolSize(value),
            BoolSize
        );

        public static xpFieldCodec<int> Int32Codec = new(
            xpFieldTypes.Type_Int32,
            (ref WriteContext ctx, int value) => ctx.WriteInt32(value),
            (ref ParseContext ctx) => ctx.ReadInt32(),
            (int value) => ComputeInt32Size(value),
            0 // Variable size
        );

        public static xpFieldCodec<int> SInt32Codec = new(
            xpFieldTypes.Type_SInt32,
            (ref WriteContext ctx, int value) => ctx.WriteSInt32(value),
            (ref ParseContext ctx) => ctx.ReadSInt32(),
            (int value) => ComputeSInt32Size(value),
            0 // Variable size
        );

        public static xpFieldCodec<int> SFixed32Codec = new(
            xpFieldTypes.Type_SFixed32,
            (ref WriteContext ctx, int value) => ctx.WriteSFixed32(value),
            (ref ParseContext ctx) => ctx.ReadSFixed32(),
            (int value) => ComputeSFixed32Size(value),
            FloatSize // Fixed size
        );

        public static xpFieldCodec<uint> UInt32Codec = new(
            xpFieldTypes.Type_UInt32,
            (ref WriteContext ctx, uint value) => ctx.WriteUInt32(value),
            (ref ParseContext ctx) => ctx.ReadUInt32(),
            (uint value) => ComputeUInt32Size(value),
            0 // Variable size
        );

        public static xpFieldCodec<uint> Fixed32Codec = new(
            xpFieldTypes.Type_Fixed32,
            (ref WriteContext ctx, uint value) => ctx.WriteFixed32(value),
            (ref ParseContext ctx) => ctx.ReadFixed32(),
            (uint value) => ComputeFixed32Size(value),
            FloatSize // Fixed size
        );

        public static xpFieldCodec<long> Int64Codec = new(
            xpFieldTypes.Type_Int64,
            (ref WriteContext ctx, long value) => ctx.WriteInt64(value),
            (ref ParseContext ctx) => ctx.ReadInt64(),
            (long value) => ComputeInt64Size(value),
            0 // Variable size
        );

        public static xpFieldCodec<long> SInt64Codec = new(
            xpFieldTypes.Type_SInt64,
            (ref WriteContext ctx, long value) => ctx.WriteSInt64(value),
            (ref ParseContext ctx) => ctx.ReadSInt64(),
            (long value) => ComputeSInt64Size(value),
            0 // Variable size
        );

        public static xpFieldCodec<long> SFixed64Codec = new(
            xpFieldTypes.Type_SFixed64,
            (ref WriteContext ctx, long value) => ctx.WriteSFixed64(value),
            (ref ParseContext ctx) => ctx.ReadSFixed64(),
            (long value) => ComputeSFixed64Size(value),
            DoubleSize // Fixed size
        );

        public static xpFieldCodec<ulong> UInt64Codec = new(
            xpFieldTypes.Type_UInt64,
            (ref WriteContext ctx, ulong value) => ctx.WriteUInt64(value),
            (ref ParseContext ctx) => ctx.ReadUInt64(),
            (ulong value) => ComputeUInt64Size(value),
            0 // Variable size
        );

        public static xpFieldCodec<ulong> Fixed64Codec = new(
            xpFieldTypes.Type_Fixed64,
            (ref WriteContext ctx, ulong value) => ctx.WriteFixed64(value),
            (ref ParseContext ctx) => ctx.ReadFixed64(),
            (ulong value) => ComputeFixed64Size(value),
            DoubleSize // Fixed size
        );

        public static xpFieldCodec<float> FloatCodec = new(
            xpFieldTypes.Type_Float,
            (ref WriteContext ctx, float value) => ctx.WriteFloat(value),
            (ref ParseContext ctx) => ctx.ReadFloat(),
            (float value) => ComputeFloatSize(value),
            FloatSize // Fixed size
        );

        public static xpFieldCodec<double> DoubleCodec = new(
            xpFieldTypes.Type_Double,
            (ref WriteContext ctx, double value) => ctx.WriteDouble(value),
            (ref ParseContext ctx) => ctx.ReadDouble(),
            (double value) => ComputeDoubleSize(value),
            DoubleSize // Fixed size
        );

        public static xpFieldCodec<DateTime> DateTimeCodec = new(
            xpFieldTypes.Type_DateTime,
            (ref WriteContext ctx, DateTime value) => ctx.WriteUInt64(value.GetDateData()),
            (ref ParseContext ctx) => ToDateTime(ctx.ReadUInt64()),
            (DateTime value) => ComputeUInt64Size(value.GetDateData()),
            0 // Variable size
        );

        public static xpFieldCodec<TimeSpan> TimeSpanCodec = new(
            xpFieldTypes.Type_TimeSpan,
            (ref WriteContext ctx, TimeSpan value) => ctx.WriteInt64(value.Ticks),
            (ref ParseContext ctx) => TimeSpan.FromTicks(ctx.ReadInt64()),
            (TimeSpan value) => ComputeInt64Size(value.Ticks),
            0 // Variable size
        );

        public static xpFieldCodec<Guid> GuidCodec = new(
            xpFieldTypes.Type_Guid,
            (ref WriteContext ctx, Guid value) => ctx.WriteString(value.ToString("D")),
            (ref ParseContext ctx) => Guid.Parse(ctx.ReadString()),
            (Guid value) => ComputeStringSize(value.ToString("D")),
            0 // Variable size
        );

        public static xpFieldCodec<string> StringCodec = new(
            xpFieldTypes.Type_String,
            (ref WriteContext ctx, string value) => ctx.WriteString(value),
            (ref ParseContext ctx) => ctx.ReadString(),
            (string value) => ComputeStringSize(value),
            0 // Variable size
        );

        public static xpFieldCodec<ByteString> BytesCodec = new(
            xpFieldTypes.Type_Bytes,
            (ref WriteContext ctx, ByteString value) => ctx.WriteBytes(value),
            (ref ParseContext ctx) => ctx.ReadBytes(),
            (ByteString value) => ComputeBytesSize(value),
            0 // Variable size
        );

        public static xpFieldCodec<Uri> UriCodec = new(
            xpFieldTypes.Type_Uri,
            (ref WriteContext ctx, Uri value) => ctx.WriteString(value.ToString()),
            (ref ParseContext ctx) => new Uri(ctx.ReadString()),
            (Uri value) => ComputeStringSize(value.ToString()),
            0 // Variable size
        );
    }
}
