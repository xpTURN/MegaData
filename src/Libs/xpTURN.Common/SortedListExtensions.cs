using System;
using System.Collections.Generic;
using System.Linq;

namespace xpTURN.Common
{
    public static class SortedListExtensions
    {
        /// <summary>
        /// Extension method to convert IEnumerable<KeyValuePair<TKey, TValue>> to SortedList<TKey, TValue>
        /// </summary>
        public static SortedList<TKey, TValue> ToSortedList<TKey, TValue>(
            this IEnumerable<KeyValuePair<TKey, TValue>> source)
            where TKey : notnull
        {
            var sortedList = new SortedList<TKey, TValue>();
            foreach (var kv in source)
            {
                sortedList[kv.Key] = kv.Value;
            }
            return sortedList;
        }

        /// <summary>
        /// Extension method to convert IEnumerable<T> to SortedList<TKey, TValue>
        /// </summary>
        public static SortedList<TKey, TValue> ToSortedList<T, TKey, TValue>(
            this IEnumerable<T> source,
            Func<T, TKey> keySelector,
            Func<T, TValue> valueSelector)
            where TKey : notnull
        {
            var sortedList = new SortedList<TKey, TValue>();
            foreach (var item in source)
            {
                sortedList[keySelector(item)] = valueSelector(item);
            }
            return sortedList;
        }
    }
}
