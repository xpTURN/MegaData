using System;
using System.Linq;
using System.Collections.Generic;

using xpTURN.Common;
using static xpTURN.Common.ConvertTypeUtils;

namespace xpTURN.MegaData
{
    public class MapEnumWrapper<TKey, TValue> : IMapIntWrapper where TKey : struct where TValue : Data
    {
        private readonly Dictionary<TKey, TValue> _mapField;

        public MapEnumWrapper(Dictionary<TKey, TValue> mapField) => _mapField = mapField;

        public int Count => _mapField.Count;

        public TKey ConvertKey(int key) => ConvertToEnum<TKey>(key);

        public Data ElementAt(int idx) => _mapField.ElementAt(idx).Value;

        public Data Get(int key) => _mapField.TryGetValue(ConvertKey(key), out var value) ? value : null;

        public void Set(int key, Data value) => _mapField[ConvertKey(key)] = value as TValue;

        public void Remove(int key) => _mapField.Remove(ConvertKey(key));

        public void Clear() => _mapField.Clear();
    }

    public class WeakMapEnumWrapper<TKey, TValue> : IMapIntWrapper where TKey : struct where TValue : Data
    {
        private readonly Dictionary<TKey, WeakReference<TValue>> _mapField;

        public WeakMapEnumWrapper(Dictionary<TKey, WeakReference<TValue>> mapField) => _mapField = mapField;

        public int Count => _mapField.Count;

        public TKey ConvertKey(int key) => ConvertToEnum<TKey>(key);

        public Data ElementAt(int idx)
        {
            var weakRef = _mapField.ElementAt(idx).Value;

            if (!weakRef.TryGetTarget(out var target))
                return null;

            return target;
        }

        public Data Get(int key)
        {
            if (!_mapField.TryGetValue(ConvertKey(key), out var weakRef))
                return null;

            if (!weakRef.TryGetTarget(out var target))
            {
                Logger.Log.Debug($"Weak reference for key '{ConvertKey(key)}' has been collected.");
                return null;
            }

            return target;
        }

        public void Set(int key, Data value)
        {
            var eKey = ConvertKey(key);
            if (!_mapField.TryGetValue(eKey, out var weakRef))
            {
                _mapField[eKey] = new WeakReference<TValue>(value as TValue);
                return;
            }

            weakRef.SetTarget(value as TValue);
        }

        public void Remove(int key) => _mapField.Remove(ConvertKey(key));

        public void Clear() => _mapField.Clear();
    }
}