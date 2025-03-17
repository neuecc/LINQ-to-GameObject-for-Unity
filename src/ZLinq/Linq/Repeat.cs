namespace ZLinq
{
    public static partial class ValueEnumerable
    {
        public static ValueEnumerable<FromRepeat<T>, T> Repeat<T>(T element, int count)
        {
            if (count < 0)
            {
                Throws.ArgumentOutOfRange(nameof(count));
            }

            return new(new(element, count));
        }
    }
}

namespace ZLinq.Linq
{
    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct FromRepeat<T>(T _element, int _count) : IValueEnumerator<T>
    {
        int index;

        public ValueEnumerator<FromRepeat<T>, T> GetEnumerator()
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

        public bool TryCopyTo(Span<T> dest)
        {
            // TODO: range validation?
            dest.Slice(0, _count).Fill(_element);
            return true;
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
    }
}