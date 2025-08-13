#region Copyright notice and license
// Protocol Buffers - Google's data interchange format
// Copyright 2008 Google Inc.  All rights reserved.
//
// Use of this source code is governed by a BSD-style
// license that can be found in the LICENSE file or at
// https://developers.google.com/open-source/licenses/bsd
#endregion
using System;

namespace xpTURN.Protobuf
{
    public static class xpRepeatedFieldHelper
    {
        internal static xpRepeatedCodec<bool> BoolCodec = new (xpFieldHelper.BoolCodec);
        internal static xpRepeatedCodec<int> Int32Codec = new (xpFieldHelper.Int32Codec);
        internal static xpRepeatedCodec<int> SInt32Codec = new (xpFieldHelper.SInt32Codec);
        internal static xpRepeatedCodec<int> SFixed32Codec = new (xpFieldHelper.SFixed32Codec);
        internal static xpRepeatedCodec<uint> UInt32Codec = new (xpFieldHelper.UInt32Codec);
        internal static xpRepeatedCodec<uint> Fixed32Codec = new (xpFieldHelper.Fixed32Codec);
        internal static xpRepeatedCodec<long> Int64Codec = new (xpFieldHelper.Int64Codec);
        internal static xpRepeatedCodec<long> SInt64Codec = new (xpFieldHelper.SInt64Codec);
        internal static xpRepeatedCodec<long> SFixed64Codec = new (xpFieldHelper.SFixed64Codec);
        internal static xpRepeatedCodec<ulong> UInt64Codec = new (xpFieldHelper.UInt64Codec);
        internal static xpRepeatedCodec<ulong> Fixed64Codec = new (xpFieldHelper.Fixed64Codec);
        internal static xpRepeatedCodec<float> FloatCodec = new (xpFieldHelper.FloatCodec);
        internal static xpRepeatedCodec<double> DoubleCodec = new (xpFieldHelper.DoubleCodec);
        internal static xpRepeatedCodec<DateTime> DateTimeCodec = new (xpFieldHelper.DateTimeCodec);
        internal static xpRepeatedCodec<TimeSpan> TimeSpanCodec = new (xpFieldHelper.TimeSpanCodec);
        internal static xpRepeatedCodec<Guid> GuidCodec = new (xpFieldHelper.GuidCodec);
        internal static xpRepeatedCodec<string> StringCodec = new (xpFieldHelper.StringCodec);
        internal static xpRepeatedCodec<ByteString> BytesCodec = new (xpFieldHelper.BytesCodec);
        internal static xpRepeatedCodec<Uri> UriCodec = new (xpFieldHelper.UriCodec);

        public static xpRepeatedCodec<bool> RepeatedBool() => BoolCodec;
        public static xpRepeatedCodec<int> RepeatedInt32() => Int32Codec;
        public static xpRepeatedCodec<int> RepeatedSInt32() => SInt32Codec;
        public static xpRepeatedCodec<int> RepeatedSFixed32() => SFixed32Codec;
        public static xpRepeatedCodec<uint> RepeatedUInt32() => UInt32Codec;
        public static xpRepeatedCodec<uint> RepeatedFixed32() => Fixed32Codec;
        public static xpRepeatedCodec<long> RepeatedInt64() => Int64Codec;
        public static xpRepeatedCodec<long> RepeatedSInt64() => SInt64Codec;
        public static xpRepeatedCodec<long> RepeatedSFixed64() => SFixed64Codec;
        public static xpRepeatedCodec<ulong> RepeatedUInt64() => UInt64Codec;
        public static xpRepeatedCodec<ulong> RepeatedFixed64() => Fixed64Codec;
        public static xpRepeatedCodec<float> RepeatedFloat() => FloatCodec;
        public static xpRepeatedCodec<double> RepeatedDouble() => DoubleCodec;
        public static xpRepeatedCodec<DateTime> RepeatedDateTime() => DateTimeCodec;
        public static xpRepeatedCodec<TimeSpan> RepeatedTimeSpan() => TimeSpanCodec;
        public static xpRepeatedCodec<Guid> RepeatedGuid() => GuidCodec;
        public static xpRepeatedCodec<string> RepeatedString() => StringCodec;
        public static xpRepeatedCodec<ByteString> RepeatedBytes() => BytesCodec;
        public static xpRepeatedCodec<Uri> RepeatedUri() => UriCodec;

        public static xpRepeatedCodecForEnum<T> RepeatedEnum<T>() where T : struct, Enum => xpRepeatedCodecForEnum<T>.EnumCodec;
        public static xpRepeatedCodecForMessage<T> RepeatedMessage<T>() where T : IMessage, new() => xpRepeatedCodecForMessage<T>.MessageCodec;
    }
}