#region Copyright notice and license
// Protocol Buffers - Google's data interchange format
// Copyright 2008 Google Inc.  All rights reserved.
//
// Use of this source code is governed by a BSD-style
// license that can be found in the LICENSE file or at
// https://developers.google.com/open-source/licenses/bsd
#endregion
using System;
using System.Buffers;
using System.Security;
using System.Collections.Generic;

using xpTURN.Protobuf.Collections;
using static xpTURN.Protobuf.ParsingPrimitivesMessages;

namespace xpTURN.Protobuf
{
    public class xpMapCodec<TKey, TValue>
    {
        private readonly xpFieldCodec<TKey> keyCodec;
        private readonly xpFieldCodec<TValue> valueCodec;

        private readonly uint fTag;
        private readonly uint keyTag;
        private readonly uint valueTag;

        private static readonly EqualityComparer<TValue> ValueEqualityComparer = ProtobufEqualityComparers.GetEqualityComparer<TValue>();
        private static readonly EqualityComparer<TKey> KeyEqualityComparer = ProtobufEqualityComparers.GetEqualityComparer<TKey>();

        private static readonly byte[] ZeroLengthMessageStreamData = new byte[] { 0 };

        public xpMapCodec(xpFieldCodec<TKey> keyCodec, xpFieldCodec<TValue> valueCodec, uint tag, uint keyTag, uint valueTag)
        {
            this.keyCodec = keyCodec;
            this.valueCodec = valueCodec;
            this.fTag = tag;
            this.keyTag = keyTag;
            this.valueTag = valueTag;
        }

        /// <summary>
        /// Creates a deep clone of this object.
        /// </summary>
        /// <returns>
        /// A deep clone of this object.
        /// </returns>
        public Dictionary<TKey, TValue> Clone(Dictionary<TKey, TValue> source)
        {
            Dictionary<TKey, TValue> clone = new Dictionary<TKey, TValue>(source.Count);
            // Keys are never cloneable. Values might be.
            if (typeof(IDeepCloneable<TValue>).IsAssignableFrom(typeof(TValue)))
            {
                foreach (var pair in source)
                {
                    clone.Add(pair.Key, ((IDeepCloneable<TValue>)pair.Value).Clone());
                }
            }
            else
            {
                // Nothing is cloneable, so we don't need to worry.
                foreach (var pair in source)
                {
                    clone.Add(pair.Key, pair.Value);
                }
            }

            return clone;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public bool AreEqual(Dictionary<TKey, TValue> left, Dictionary<TKey, TValue> right)
        {
            if (left == null || right == null)
            {
                return false;
            }
            if (left == right)
            {
                return true;
            }
            if (left.Count != right.Count)
            {
                return false;
            }
            var valueComparer = ValueEqualityComparer;
            foreach (var pair in left)
            {
                if (!right.TryGetValue(pair.Key, out TValue value))
                {
                    return false;
                }
                if (!valueComparer.Equals(value, pair.Value))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public int GetHashCode(Dictionary<TKey, TValue> source)
        {
            var keyComparer = KeyEqualityComparer;
            var valueComparer = ValueEqualityComparer;
            int hash = 0;
            foreach (var pair in source)
            {
                hash ^= keyComparer.GetHashCode(pair.Key) * 31 + valueComparer.GetHashCode(pair.Value);
            }
            return hash;
        }

        private IEnumerable<KeyValuePair<TKey, TValue>> GetSortedListCopy(IEnumerable<KeyValuePair<TKey, TValue>> listToSort)
        {
            // We can't sort the list in place, as that would invalidate the linked list.
            // Instead, we create a new list, sort that, and then write it out.
            var listToWrite = new List<KeyValuePair<TKey, TValue>>(listToSort);
            listToWrite.Sort((pair1, pair2) =>
            {
                if (typeof(TKey) == typeof(string))
                {
                    // Use Ordinal, otherwise Comparer<string>.Default uses StringComparer.CurrentCulture
                    return StringComparer.Ordinal.Compare(pair1.Key.ToString(), pair2.Key.ToString());
                }
                return Comparer<TKey>.Default.Compare(pair1.Key, pair2.Key);
            });
            return listToWrite;
        }

        /// <summary>
        /// Writes the contents of this map to the given write context, using the specified codec
        /// to encode each entry.
        /// </summary>
        /// <param name="ctx">The write context to write to.</param>
        /// <param name="codec">The codec to use for each entry.</param>
        [SecuritySafeCritical]
        public void WriteTo(ref WriteContext ctx, ref Dictionary<TKey, TValue> source)
        {
            IEnumerable<KeyValuePair<TKey, TValue>> listToWrite = source;
            if (ctx.state.CodedOutputStream?.Deterministic ?? false)
            {
                listToWrite = GetSortedListCopy(source);
            }
            WriteTo(ref ctx, listToWrite);
        }

        [SecuritySafeCritical]
        private void WriteTo(ref WriteContext ctx, IEnumerable<KeyValuePair<TKey, TValue>> listKvp)
        {
            foreach (var entry in listKvp)
            {
                ctx.WriteTag(fTag);

                WritingPrimitives.WriteLength(ref ctx.buffer, ref ctx.state, CalculateEntrySize(entry));
                keyCodec.WriteTagAndValue(ref ctx, entry.Key, keyTag);
                valueCodec.WriteTagAndValue(ref ctx, entry.Value, valueTag);
            }
        }

        private int CalculateEntrySize(KeyValuePair<TKey, TValue> entry)
        {
            return keyCodec.CalculateSizeWithTag(entry.Key, keyTag) + valueCodec.CalculateSizeWithTag(entry.Value, valueTag);
        }

        /// <summary>
        /// Calculates the size of this map based on the given entry codec.
        /// </summary>
        /// <param name="codec">The codec to use to encode each entry.</param>
        /// <returns></returns>
        public int CalculateSize(Dictionary<TKey, TValue> source)
        {
            if (source.Count == 0)
            {
                return 0;
            }
            int size = 0;
            foreach (var entry in source)
            {
                int entrySize = CalculateEntrySize(entry);

                size += CodedOutputStream.ComputeRawVarint32Size(fTag);
                size += CodedOutputStream.ComputeLengthSize(entrySize) + entrySize;
            }
            return size;
        }

        /// <summary>
        /// Adds the specified entries to the map, replacing any existing entries with the same keys.
        /// The keys and values are not automatically cloned.
        /// </summary>
        /// <remarks>This method primarily exists to be called from MergeFrom methods in generated classes for messages.</remarks>
        /// <param name="entries">The entries to add to the map.</param>
        public void Merge(Dictionary<TKey, TValue> source, Dictionary<TKey, TValue> other)
        {
            ProtoPreconditions.CheckNotNull(source, nameof(source));
            ProtoPreconditions.CheckNotNull(other, nameof(other));
            foreach (var pair in other)
            {
                source[pair.Key] = pair.Value;
            }
        }

        /// <summary>
        /// Adds entries to the map from the given parse context.
        /// </summary>
        /// <remarks>
        /// It is assumed that the input is initially positioned after the tag specified by the codec.
        /// This method will continue reading entries from the input until the end is reached, or
        /// a different tag is encountered.
        /// </remarks>
        /// <param name="ctx">Input to read from</param>
        /// <param name="codec">Codec describing how the key/value pairs are encoded</param>
        [SecuritySafeCritical]
        public void Read(ref ParseContext ctx, ref Dictionary<TKey, TValue> source)
        {
            do
            {
                KeyValuePair<TKey, TValue> entry = ReadMapEntry(ref ctx);
                source[entry.Key] = entry.Value;
            } while (ParsingPrimitives.MaybeConsumeTag(ref ctx.buffer, ref ctx.state, fTag));
        }

        private KeyValuePair<TKey, TValue> ReadMapEntry(ref ParseContext ctx)
        {
            int length = ParsingPrimitives.ParseLength(ref ctx.buffer, ref ctx.state);
            if (ctx.state.recursionDepth >= ctx.state.recursionLimit)
            {
                throw InvalidProtocolBufferException.RecursionLimitExceeded();
            }
            int oldLimit = SegmentedBufferHelper.PushLimit(ref ctx.state, length);
            ++ctx.state.recursionDepth;

            TKey key = keyCodec.DefaultValue;
            TValue value = valueCodec.DefaultValue;

            uint tag;
            while ((tag = ctx.ReadTag()) != 0)
            {
                if (tag == keyTag)
                {
                    key = keyCodec.Read(ref ctx);
                }
                else if (tag == valueTag)
                {
                    value = valueCodec.Read(ref ctx);
                }
                else
                {
                    xpParseUtils.SkipLastField(ref ctx);
                }
            }

            // Corner case: a map entry with a key but no value, where the value type is a message.
            // Read it as if we'd seen input with no data (i.e. create a "default" message).
            if (value == null)
            {
                if (ctx.state.CodedInputStream != null)
                {
                    // the decoded message might not support parsing from ParseContext, so
                    // we need to allow fallback to the legacy MergeFrom(CodedInputStream) parsing.
                    value = valueCodec.Read(new CodedInputStream(ZeroLengthMessageStreamData));
                }
                else
                {
                    ParseContext.Initialize(new ReadOnlySequence<byte>(ZeroLengthMessageStreamData), out ParseContext zeroLengthCtx);
                    value = valueCodec.Read(ref zeroLengthCtx);
                }
            }

            CheckReadEndOfStreamTag(ref ctx.state);
            // Check that we've read exactly as much data as expected.
            if (!SegmentedBufferHelper.IsReachedLimit(ref ctx.state))
            {
                throw InvalidProtocolBufferException.TruncatedMessage();
            }
            --ctx.state.recursionDepth;
            SegmentedBufferHelper.PopLimit(ref ctx.state, oldLimit);

            return new KeyValuePair<TKey, TValue>(key, value);
        }
    }
}
