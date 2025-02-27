namespace ZLinq
{
    public static partial class ValueEnumerable
    {
        public static RepeatValueEnumerable<T> Repeat<T>(T element, int count)
        {
            if (count < 0)
            {
                Throws.ArgumentOutOfRangeException(nameof(count));
            }

            return new(element, count);
        }
    }
}

namespace ZLinq.Linq
{
    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct RepeatValueEnumerable<T>(T _element, int _count) : IValueEnumerable<T>
    {
        int index;

        public ValueEnumerator<RepeatValueEnumerable<T>, T> GetEnumerator()
        {
            return new(this);
        }

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = _count;
            return true;
        }

        public bool TryGetSpan(out ReadOnlySpan<T> span)
        {
            span = default!;
            return false;
        }

        public bool TryGetNext(out T current)
        {
            if (index++ < _count)
            {
                current = _element;
                return true;
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
        }

        public T[] ToArray()
        {
            var array = GC.AllocateUninitializedArray<T>(_count);
            array.AsSpan().Fill(_element); // vectorize fill
            return array;
        }

        public List<T> ToList()
        {
            return ListMarshal.AsList(ToArray());
        }

        public void CopyTo(Span<T> dest)
        {
            dest.Slice(0, _count).Fill(_element);
        }
    }
}