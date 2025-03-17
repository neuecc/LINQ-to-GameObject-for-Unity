namespace ZLinq
{
    public static partial class ValueEnumerable
    {
        public static FromEmpty<T> Empty<T>()
        {
            return default;
        }
    }
}

namespace ZLinq.Linq
{
    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct FromEmpty<T> : IValueEnumerator<T>
    {
        public ValueEnumerator<FromEmpty<T>, T> GetEnumerator()
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

        public bool TryCopyTo(Span<T> dest) => true;

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