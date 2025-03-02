namespace ZLinq
{
    public static partial class ValueEnumerable
    {
        public static Empty<T> Empty<T>()
        {
            return default;
        }
    }
}

namespace ZLinq.Linq
{
    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct Empty<T> : IValueEnumerable<T>
    {
        public ValueEnumerator<Empty<T>, T> GetEnumerator()
        {
            return new(this);
        }

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = 0;
            return true;
        }

        public bool TryGetSpan(out ReadOnlySpan<T> span)
        {
            span = [];
            return true;
        }

        public bool TryGetNext(out T current)
        {
            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
        }
    }
}