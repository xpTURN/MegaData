#region Copyright notice and license
// Protocol Buffers - Google's data interchange format
// Copyright 2008 Google Inc.  All rights reserved.
//
// Use of this source code is governed by a BSD-style
// license that can be found in the LICENSE file or at
// https://developers.google.com/open-source/licenses/bsd
#endregion
using System;
using System.Collections.Generic;

using xpTURN.Protobuf.Collections;
using static xpTURN.Protobuf.CodedOutputStream;

namespace xpTURN.Protobuf
{
    public class xpFieldCodec<T>
    {
        public delegate TValue ValueReader<out TValue>(ref ParseContext ctx);
        public delegate void ValueWriter<TValue>(ref WriteContext ctx, TValue value);

        public xpFieldTypes Type { get; protected set; }
        public ValueWriter<T> Writer { get; protected set; }
        public Func<T, int> SizeCalculator { get; protected set; }
        public ValueReader<T> Reader { get; protected set; }
        public int FixedSize { get; protected set; }
        public T DefaultValue { get; protected set; }

        private static readonly EqualityComparer<T> EqualityComparer = ProtobufEqualityComparers.GetEqualityComparer<T>();

        public bool TypeSupportsPacking => Type != xpFieldTypes.Type_Unknown &&
                            Type != xpFieldTypes.Type_String &&
                            Type != xpFieldTypes.Type_Guid &&
                            Type != xpFieldTypes.Type_Uri &&
                            Type != xpFieldTypes.Type_Bytes &&
                            Type != xpFieldTypes.Type_Message;

        internal xpFieldCodec() { }

        internal xpFieldCodec(xpFieldTypes type, ValueWriter<T> writer, ValueReader<T> reader, Func<T, int> sizeCalculator, int fixedSize)
        {
            Type = type;
            Writer = writer;
            Reader = reader;
            SizeCalculator = sizeCalculator;
            FixedSize = fixedSize;
            DefaultValue = default(T);
        }

        private bool IsDefault(T value) => EqualityComparer.Equals(value, DefaultValue);

        /// <summary>
        /// Write a tag and the given value, *if* the value is not the default.
        /// </summary>
        public void WriteTagAndValue(ref WriteContext ctx, T value, uint fTag, uint fEndTag = 0)
        {
            if (!IsDefault(value))
            {
                ctx.WriteTag(fTag);
                Writer(ref ctx, value);
                if (fEndTag != 0)
                {
                    ctx.WriteTag(fEndTag);
                }
            }
        }

        /// <summary>
        /// Reads a value of the codec type from the given <see cref="CodedInputStream"/>.
        /// </summary>
        /// <param name="input">The input stream to read from.</param>
        /// <returns>The value read from the stream.</returns>
        public T Read(CodedInputStream input)
        {
            ParseContext.Initialize(input, out ParseContext ctx);
            try
            {
                return Reader(ref ctx);
            }
            finally
            {
                ctx.CopyStateTo(input);
            }
        }

        /// <summary>
        /// Reads a value of the codec type from the given <see cref="ParseContext"/>.
        /// </summary>
        /// <param name="ctx">The parse context to read from.</param>
        /// <returns>The value read.</returns>
        public T Read(ref ParseContext ctx)
        {
            return Reader(ref ctx);
        }

        /// <summary>
        /// Calculates the size required to write the given value, with a tag,
        /// if the value is not the default.
        /// </summary>
        public int CalculateSizeWithTag(T value, uint fTag)
        {
            if (IsDefault(value))
                return 0;

            return SizeCalculator(value) + ComputeRawVarint32Size(fTag);
        }
    }
}
