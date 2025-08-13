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
using System.Runtime.CompilerServices;
using System.Security;

using static xpTURN.Protobuf.CodedOutputStream;

namespace xpTURN.Protobuf
{
    public class xpRepeatedCodec<T>
    {
        protected xpFieldCodec<T> valueCodec;

        public bool TypeSupportsPacking => valueCodec.TypeSupportsPacking;

        internal xpRepeatedCodec() { }

        internal xpRepeatedCodec(xpFieldCodec<T> valueCodec)
        {
            this.valueCodec = valueCodec;
        }

        // The generic types 'From' and 'To' are specified to be the same, but this cannot be verified by syntax, so a forced conversion is used. 
        // Other methods would involve boxing/unboxing.
        private static To Convert<From, To>(From value)
        {
            return Unsafe.As<From, To>(ref value);
        }

        public bool IsPackedRepeatedField(uint tag) =>
            TypeSupportsPacking && WireFormat.GetTagWireType(tag) == WireFormat.WireType.LengthDelimited;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int CalculatePackedDataSize(ref List<T> values, uint fTag)
        {
            if (valueCodec.FixedSize != 0)
            {
                return valueCodec.FixedSize * values.Count;
            }
            else
            {
                int tmp = 0;
                for (int i = 0; i < values.Count; i++)
                {
                    tmp += valueCodec.SizeCalculator(values[i]);
                }
                return tmp;
            }
        }

        [SecuritySafeCritical]
        public void Write(ref WriteContext ctx, ref List<T> values, uint fTag)
        {
            if (values.Count == 0)
            {
                return;
            }

            if (IsPackedRepeatedField(fTag))
            {
                // Packed primitive type
                int size = CalculatePackedDataSize(ref values, fTag);
                ctx.WriteTag(fTag);
                ctx.WriteLength(size);

                for (int i = 0; i < values.Count; i++)
                {
                    valueCodec.Writer(ref ctx, values[i]);
                }
            }
            else
            {
                // Not packed: a simple tag/value pair for each value.
                // Can't use codec.WriteTagAndValue, as that omits default values.
                for (int i = 0; i < values.Count; i++)
                {
                    ctx.WriteTag(fTag);
                    valueCodec.Writer(ref ctx, values[i]);
                }
            }
        }

        [SecuritySafeCritical]
        public void Read(ref ParseContext ctx, ref List<T> values, uint fTag)
        {
            uint tag = ctx.state.lastTag;

            // Non-nullable value types can be packed or not.
            if (IsPackedRepeatedField(tag))
            {
                int length = ctx.ReadLength();
                if (length > 0)
                {
                    int oldLimit = SegmentedBufferHelper.PushLimit(ref ctx.state, length);

                    // If the content is fixed size then we can calculate the length
                    // of the repeated field and pre-initialize the underlying collection.
                    //
                    // Check that the supplied length doesn't exceed the underlying buffer.
                    // That prevents a malicious length from initializing a very large collection.
                    if (valueCodec.FixedSize > 0 && length % valueCodec.FixedSize == 0 && ParsingPrimitives.IsDataAvailable(ref ctx.state, length))
                    {
                        while (!SegmentedBufferHelper.IsReachedLimit(ref ctx.state))
                        {
                            // Only FieldCodecs with a fixed size can reach here, and they are all known
                            // types that don't allow the user to specify a custom reader action.
                            // reader action will never return null.
                            values.Add(valueCodec.Reader(ref ctx));
                        }
                    }
                    else
                    {
                        // Content is variable size so add until we reach the limit.
                        while (!SegmentedBufferHelper.IsReachedLimit(ref ctx.state))
                        {
                            values.Add(valueCodec.Reader(ref ctx));
                        }
                    }
                    SegmentedBufferHelper.PopLimit(ref ctx.state, oldLimit);
                }
                // Empty packed field. Odd, but valid - just ignore.
            }
            else
            {
                // Not packed... (possibly not packable)
                do
                {
                    values.Add(valueCodec.Reader(ref ctx));
                } while (ParsingPrimitives.MaybeConsumeTag(ref ctx.buffer, ref ctx.state, tag));
            }
        }

        public int GetHashCode(List<T> values)
        {
            int hash = 0;
            for (int i = 0; i < values.Count; i++)
            {
                hash = hash * 31 + values[i].GetHashCode();
            }
            return hash;
        }

        public bool AreEqual(List<T> left, List<T> right)
        {
            if (left == null || right == null)
                return left == right;
            if (left.Count != right.Count)
                return false;
            for (int i = 0; i < left.Count; i++)
            {
                if (!EqualityComparer<T>.Default.Equals(left[i], right[i]))
                    return false;
            }
            return true;
        }

        public int CalculateSize(List<T> values, uint tag)
        {
            if (values.Count == 0)
            {
                return 0;
            }

            int size = 0;
            if (TypeSupportsPacking)
            {
                size += ComputeRawVarint32Size(tag); // tag size
                size += ComputeLengthSize(values.Count); // length size
            }
            else
            {
                size += values.Count * ComputeRawVarint32Size(tag); // tag size
            }

            // Calculate the size of each value based on its type
            if (valueCodec.FixedSize != 0)
            {
                size += values.Count * valueCodec.FixedSize;
            }
            else
            {
                for (int i = 0; i < values.Count; i++)
                {
                    size += valueCodec.SizeCalculator(values[i]);
                }
            }

            return size;
        }

        public List<T> Clone(List<T> values)
        {
            List<T> clonedValues = new(values.Count);
            if (typeof(IDeepCloneable<T>).IsAssignableFrom(typeof(T)))
            {
                foreach (var value in values)
                {
                    clonedValues.Add(((IDeepCloneable<T>)value).Clone());
                }
            }
            else
            {
                foreach (var value in values)
                {
                    clonedValues.Add(value);
                }
            }

            return clonedValues;
        }
    }
}