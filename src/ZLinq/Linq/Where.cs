namespace ZLinq
{
    partial class StructEnumerableExtensions
    {
        public static WhereStructEnumerable<TEnumerable, T> Where<TEnumerable, T>(this TEnumerable source, Func<T, bool> predicate)
            where TEnumerable : struct, IStructEnumerable<T>
        {
            return new(source, predicate);
        }

        public static StructEnumerator<WhereStructEnumerable<TEnumerable, T>, T> GetEnumerator<TEnumerable, T>(
            this WhereStructEnumerable<TEnumerable, T> source)
            where TEnumerable : struct, IStructEnumerable<T>
        {
            return new(source);
        }
    }
}

namespace ZLinq.Linq
{
    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct WhereStructEnumerable<TEnumerable, T>(TEnumerable source, Func<T, bool> predicate) : IStructEnumerable<T>
        where TEnumerable : struct, IStructEnumerable<T>
    {
        public bool TryGetNonEnumeratedCount(out int count) => source.TryGetNonEnumeratedCount(out count);

        public bool TryGetNext(out T current)
        {
            while (source.TryGetNext(out var value))
            {
                if (predicate(value))
                {
                    current = value;
                    return true;
                }
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
            source.Dispose();
        }
    }
}