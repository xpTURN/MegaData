#region Copyright notice and license
// Protocol Buffers - Google's data interchange format
// Copyright 2008 Google Inc.  All rights reserved.
//
// Use of this source code is governed by a BSD-style
// license that can be found in the LICENSE file or at
// https://developers.google.com/open-source/licenses/bsd
#endregion
using System;
using System.Runtime.CompilerServices;

using static xpTURN.Protobuf.CodedOutputStream;

namespace xpTURN.Protobuf
{
    public class xpFieldCodecForEnum<T> : xpFieldCodec<T> where T : struct, Enum
    {
        public static xpFieldCodecForEnum<T> EnumCodec = new(
            (ref WriteContext ctx, T value) => ctx.WriteEnum(Convert<T, int>(value)),
            (ref ParseContext ctx) => Convert<int, T>(ctx.ReadEnum()),
            (T value) => ComputeEnumSize(Convert<T, int>(value)),
            0 // Variable size
        );

        internal xpFieldCodecForEnum() { }

        internal xpFieldCodecForEnum(ValueWriter<T> writer, ValueReader<T> reader, Func<T, int> sizeCalculator, int fixedSize)
        {
            Type = xpFieldTypes.Type_Enum;
            Writer = writer;
            Reader = reader;
            SizeCalculator = sizeCalculator;
            FixedSize = fixedSize;
        }

        // The generic types 'From' and 'To' are specified to be the same, but this cannot be verified by syntax, so a forced conversion is used. 
        // Other methods would involve boxing/unboxing.
        public static To Convert<From, To>(From value)
        {
            return Unsafe.As<From, To>(ref value);
        }
    }
}