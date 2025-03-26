using System.Collections;
using System.Collections.Generic;

namespace ZLinq.Tests;

public static class ExtensionMethods
{
    public static IEnumerable<T> RunOnce<TEnumerator, T>(this ValueEnumerable<TEnumerator, T> source)
       where TEnumerator : struct, IValueEnumerator<T>
#if NET9_0_OR_GREATER
       , allows ref struct
#endif
    {
        return new RunOnceList<T>(source.ToArray());
    }

    private class RunOnceList<T> : IList<T>
    {
        private readonly IList<T> _source;
        private readonly HashSet<int> _called = [];

        private void AssertAll()
        {
            Assert.Empty(_called);
            _called.Add(-1);
        }

        private void AssertIndex(int index)
        {
            Assert.False(_called.Contains(-1));
            Assert.True(_called.Add(index));
        }

        public RunOnceList(IList<T> source)
        {
            _source = source;
        }

        public IEnumerator<T> GetEnumerator()
        {
            AssertAll();
            return _source.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(T item)
        {
            throw new NotSupportedException();
        }

        public void Clear()
        {
            throw new NotSupportedException();
        }

        public bool Contains(T item)
        {
            AssertAll();
            return _source.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            AssertAll();
            _source.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            throw new NotSupportedException();
        }

        public int Count => _source.Count;

        public bool IsReadOnly => true;

        public int IndexOf(T item)
        {
            AssertAll();
            return _source.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            throw new NotSupportedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        public T this[int index]
        {
            get
            {
                AssertIndex(index);
                return _source[index];
            }
            set { throw new NotSupportedException(); }
        }
    }
}
