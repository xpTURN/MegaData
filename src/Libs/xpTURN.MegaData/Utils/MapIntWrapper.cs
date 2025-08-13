using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using xpTURN.Common;

namespace xpTURN.MegaData
{
    public interface IMapIntWrapper
    {
        int Count { get; }
        Data ElementAt(int idx);
        Data Get(int key);
        void Set(int key, Data value);
        void Add(int key, Data value) => Set(key, value);
        void Remove(int key);
        void Clear();

        Data this[int key]
        {
            get => Get(key);
            set => Set(key, value);
        }
    
        public Enumerator GetEnumerator() => new Enumerator(this);

        public struct Enumerator : IEnumerator<Data>, IEnumerator
        {
            private readonly IMapIntWrapper _map;
            private int _index;
            private Data _current;

            private int Count => _map?.Count ?? 0;

            internal Enumerator(IMapIntWrapper map)
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

    public class NullMapIntWrapper : IMapIntWrapper
    {
        public static NullMapIntWrapper Default { get; } = new NullMapIntWrapper();

        public int Count { get; } = 0;
        public Data ElementAt(int idx) => throw new ArgumentOutOfRangeException(nameof(NullMapIntWrapper));
        public Data Get(int key) => null;
        public void Set(int key, Data value) { throw new NotSupportedException(nameof(NullMapIntWrapper)); }
        public void Remove(int key) { throw new ArgumentOutOfRangeException(nameof(NullMapIntWrapper)); }
        public void Clear() { }
    }

    public class MapIntWrapper<TValue> : IMapIntWrapper where TValue : Data
    {
        private readonly Dictionary<int, TValue> _mapField;

        public MapIntWrapper(Dictionary<int, TValue> mapField) => _mapField = mapField;

        public int Count => _mapField.Count;

        public Data ElementAt(int idx) => _mapField.ElementAt(idx).Value;

        public Data Get(int key) => _mapField.TryGetValue(key, out var value) ? value : null;

        public void Set(int key, Data value) => _mapField[key] = value as TValue;

        public void Remove(int key) => _mapField.Remove(key);

        public void Clear() => _mapField.Clear();
    }

    public class WeakMapIntWrapper<TValue> : IMapIntWrapper where TValue : Data
    {
        private readonly Dictionary<int, WeakReference<TValue>> _mapField;

        public WeakMapIntWrapper(Dictionary<int, WeakReference<TValue>> mapField) => _mapField = mapField;

        public int Count => _mapField.Count;

        public Data ElementAt(int idx)
        {
            var weakRef = _mapField.ElementAt(idx).Value;

            if (!weakRef.TryGetTarget(out var target))
                return null;

            return target;
        }

        public Data Get(int key)
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

        public void Set(int key, Data value)
        {
            if (!_mapField.TryGetValue(key, out var weakRef))
            {
                _mapField[key] = new WeakReference<TValue>(value as TValue);
                return;
            }

            weakRef.SetTarget(value as TValue);
        }

        public void Remove(int key) => _mapField.Remove(key);

        public void Clear() => _mapField.Clear();
    }
}