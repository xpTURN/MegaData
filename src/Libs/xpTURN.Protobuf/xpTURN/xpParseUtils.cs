#region Copyright notice and license
// Protocol Buffers - Google's data interchange format
// Copyright 2008 Google Inc.  All rights reserved.
//
// Use of this source code is governed by a BSD-style
// license that can be found in the LICENSE file or at
// https://developers.google.com/open-source/licenses/bsd
#endregion
using System;
using System.IO;

namespace xpTURN.Protobuf
{
    public static class xpParseUtils
    {
        /// <summary>
        /// Merges length-delimited data from the given stream into an existing message.
        /// Identical to MessageExtensions.MergeDelimitedFrom, except it returns the size of the data read.
        /// </summary>
        /// <remarks>
        /// The stream is expected to contain a length and then the data. Only the amount of data
        /// specified by the length will be consumed.
        /// </remarks>
        /// <param name="message">The message to merge the data into.</param>
        /// <param name="input">Stream containing the data to merge, which must be protobuf-encoded binary data.</param>
        public static int ReadDelimitedFrom(IMessage message, Stream input)
        {
            ProtoPreconditions.CheckNotNull(message, nameof(message));
            ProtoPreconditions.CheckNotNull(input, nameof(input));
            int size = (int)CodedInputStream.ReadRawVarint32(input);
            xpLimitedInputStream.Default.Setup(input, size);
            MessageExtensions.MergeFrom(message, xpLimitedInputStream.Default, true);
            return size;
        }

        /// <summary>
        /// Skips the last field in the current parse context.
        /// ParsingPrimitivesMessages.SkipLastField() processes Varint types as Int32, which can cause issues
        /// if the skipped field is 64-bit. Therefore, the code below assumes and processes it as a 64-bit type
        /// to handle such cases.
        /// </summary>
        public static void SkipLastField(ref ParseContext ctx)
        {
            uint tag = ctx.LastTag;
            int number = WireFormat.GetTagFieldNumber(tag);
            switch (WireFormat.GetTagWireType(tag))
            {
                case WireFormat.WireType.Varint:
                    ParsingPrimitives.ParseRawVarint64(ref ctx.buffer, ref ctx.state);
                    break;
                case WireFormat.WireType.Fixed32:
                    ParsingPrimitives.ParseRawLittleEndian32(ref ctx.buffer, ref ctx.state);
                    break;
                case WireFormat.WireType.Fixed64:
                    ParsingPrimitives.ParseRawLittleEndian64(ref ctx.buffer, ref ctx.state);
                    break;
                case WireFormat.WireType.LengthDelimited:
                    var length = ParsingPrimitives.ParseLength(ref ctx.buffer, ref ctx.state);
                    ParsingPrimitives.SkipRawBytes(ref ctx.buffer, ref ctx.state, length);
                    break;
                case WireFormat.WireType.StartGroup:
                    ParsingPrimitivesMessages.SkipGroup(ref ctx.buffer, ref ctx.state, ctx.state.lastTag);
                    break;
                case WireFormat.WireType.EndGroup:
                    throw new InvalidProtocolBufferException(
                        "SkipLastField called on an end-group tag, indicating that the corresponding start-group was missing");
                default:
                    throw InvalidProtocolBufferException.InvalidWireType();
            }
        }
    }
}
