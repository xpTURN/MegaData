using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using xpTURN.Common;


namespace xpTURN.MegaData
{
    public interface IMapStringWrapper
    {
        int Count { get; }
        Data ElementAt(int idx);
        Data Get(string key);
        void Set(string key, Data value);
        void Add(string key, Data value) => Set(key, value);
        void Remove(string key);
        void Clear();

        Data this[string key]
        {
            get => Get(key);
            set => Set(key, value);
        }

        public Enumerator GetEnumerator() => new Enumerator(this);

        public struct Enumerator : IEnumerator<Data>, IEnumerator
        {
            private readonly IMapStringWrapper _map;
            private int _index;
            private Data _current;

            private int Count => _map?.Count ?? 0;

            internal Enumerator(IMapStringWrapper map)
            {
                _map = map;
                _index = 0;
                _current = default;
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (_map != null && (uint)_index < (uint)Count)
                {
                    _current = _map.ElementAt(_index);
                    _index++;
                    return true;
                }
                return MoveNextRare();
            }

            private bool MoveNextRare()
            {
                _index = Count + 1;
                _current = default;
                return false;
            }

            public Data Current => _current!;

            object IEnumerator.Current
            {
                get
                {
                    if (_index == 0 || _index == Count + 1)
                    {
                        throw new InvalidOperationException("Enumeration has either not started or has already finished.");
                    }
                    return Current;
                }
            }

            void IEnumerator.Reset()
            {
                _index = 0;
                _current = default;
            }
        }
    }

    public class NullMapStringWrapper : IMapStringWrapper
    {
        public static NullMapStringWrapper Default { get; } = new NullMapStringWrapper();

        public int Count { get; } = 0;
        public Data ElementAt(int idx) => throw new ArgumentOutOfRangeException(nameof(NullMapStringWrapper));
        public Data Get(string key) => null;
        public void Set(string key, Data value) { throw new NotSupportedException(nameof(NullMapStringWrapper)); }
        public void Remove(string key) { throw new ArgumentOutOfRangeException(nameof(NullMapStringWrapper)); }
        public void Clear() { }
    }

    public class MapStringWrapper<TValue> : IMapStringWrapper where TValue : Data
    {
        private readonly Dictionary<string, TValue> _mapField;

        public MapStringWrapper(Dictionary<string, TValue> mapField) => _mapField = mapField;

        public int Count => _mapField.Count;

        public Data ElementAt(int idx) => _mapField.ElementAt(idx).Value;

        public Data Get(string key) => _mapField.TryGetValue(key, out var value) ? value : null;

        public void Set(string key, Data value) => _mapField[key] = value as TValue;

        public void Remove(string key) => _mapField.Remove(key);

        public void Clear() => _mapField.Clear();
    }

    public class WeakMapStringWrapper<TValue> : IMapStringWrapper where TValue : Data
    {
        private readonly Dictionary<string, WeakReference<TValue>> _mapField;

        public WeakMapStringWrapper(Dictionary<string, WeakReference<TValue>> mapField) => _mapField = mapField;

        public int Count => _mapField.Count;

        public Data ElementAt(int idx)
        {
            var weakRef = _mapField.ElementAt(idx).Value;

            if (!weakRef.TryGetTarget(out var target))
                return null;

            return target;
        }

        public Data Get(string key)
        {
            if (!_mapField.TryGetValue(key, out var weakRef))
                return null;

            if (!weakRef.TryGetTarget(out var target))
            {
                Logger.Log.Debug($"Weak reference for key '{key}' has been collected.");
                return null;
            }

            return target;
        }

        public void Set(string key, Data value)
        {
            if (!_mapField.TryGetValue(key, out var weakRef))
            {
                _mapField[key] = new WeakReference<TValue>(value as TValue);
                return;
            }

            weakRef.SetTarget(value as TValue);
        }

        public void Remove(string key) => _mapField.Remove(key);

        public void Clear() => _mapField.Clear();
    }
}
