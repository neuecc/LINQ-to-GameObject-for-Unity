namespace ZLinq
{
    public static partial class StructEnumerable
    {
        public static RangeStructEnumerable<T> Range<T>(int start, int count)
        {
            // TODO: varidation
            return new(start, count);
        }
    }
}

namespace ZLinq.Linq
{
    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct RangeStructEnumerable<T>(int start, int count) : IStructEnumerable<int>
    {
        // TODO: impl
        readonly int count = count;
        int start = start;
        int to = start + count;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = this.count;
            return true;
        }

        public bool TryGetSpan(out ReadOnlySpan<int> span)
        {
            span = default;
            return false;
        }

        public bool TryGetNext(out int current)
        {
            if (start < to)
            {
                current = start++;
                return true;
            }

            current = 0;
            return false;
        }

        public void Dispose()
        {
        }
    }
}