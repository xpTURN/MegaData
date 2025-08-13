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
    public class xpRepeatedCodecForEnum<T> : xpRepeatedCodec<T> where T : struct, Enum
    {
        internal static xpRepeatedCodecForEnum<T> EnumCodec = new (xpFieldCodecForEnum<T>.EnumCodec);

        protected xpRepeatedCodecForEnum() { }

        private xpRepeatedCodecForEnum(xpFieldCodec<T> valueCodec)
        {
            this.valueCodec = valueCodec;
        }
    }
}
