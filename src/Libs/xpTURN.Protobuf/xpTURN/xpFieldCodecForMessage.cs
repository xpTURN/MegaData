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
    public class xpFieldCodecForMessage<T> : xpFieldCodec<T> where T : IMessage, new()
    {
        public static xpFieldCodecForMessage<T> MessageCodec = new (
            (ref WriteContext ctx, T value) => ctx.WriteMessage(value),
            (ref ParseContext ctx) =>
            {
                T msg = new T();
                ParsingPrimitivesMessages.ReadMessage(ref ctx, msg);
                return msg;
            },
            (T value) => CodedOutputStream.ComputeMessageSize(value),
            0 // Variable size
        );

        protected xpFieldCodecForMessage() { }

        private xpFieldCodecForMessage(ValueWriter<T> writer, ValueReader<T> reader, Func<T, int> sizeCalculator, int fixedSize)
        {
            Type = xpFieldTypes.Type_Message;
            Writer = writer;
            Reader = reader;
            SizeCalculator = sizeCalculator;
            FixedSize = fixedSize;
        }
    }
}