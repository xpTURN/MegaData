using System;
using System.Collections;
using System.Collections.Generic;

namespace xpTURN.MegaData
{
    public class ListIntGetter<TValue>
    {
        public delegate TValue ValueReader(int dataId);

        public List<int> RefList { protected get; set; }
        private readonly ValueReader _valueReader;

        public int Count => RefList.Count;
        public TValue ElementAt(int idx) => GetValue(RefList[idx]);
        TValue this[int idx] => ElementAt(idx);

        public ListIntGetter(List<int> list, ValueReader valueReader)
        {
            RefList = list;
            _valueReader = valueReader;
        }

        public TValue GetValue(int dataId)
        {
            return _valueReader(dataId);
        }

        public Enumerator GetEnumerator() => new Enumerator(this);

        public struct Enumerator : IEnumerator<TValue>, IEnumerator
        {
            private readonly ListIntGetter<TValue> _list;
            private int _index;
            private TValue _current;

            private int Count => _list?.Count ?? 0;

            internal Enumerator(ListIntGetter<TValue> list)
            {
                _list = list;
                _index = 0;
                _current = default;
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (_list != null && (uint)_index < (uint)Count)
                {
                    _current = _list.ElementAt(_index);
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

            public TValue Current => _current!;

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
}