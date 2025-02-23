namespace ZLinq
{
    partial class StructEnumerableExtensions
    {
        public static WhereStructEnumerable<TEnumerable, T> Where<TEnumerable, T>(this TEnumerable source, Func<T, bool> predicate)
            where TEnumerable : struct, IStructEnumerable<T>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            return new(source, predicate);
        }

        public static StructEnumerator<WhereStructEnumerable<TEnumerable, T>, T> GetEnumerator<TEnumerable, T>(
            this WhereStructEnumerable<TEnumerable, T> source)
            where TEnumerable : struct, IStructEnumerable<T>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            return new(source);
        }
    }
}

namespace ZLinq.Linq
{
    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
#if NET9_0_OR_GREATER
    public ref
#else
    public
#endif
    struct WhereStructEnumerable<TEnumerable, T>(TEnumerable source, Func<T, bool> predicate) : IStructEnumerable<T>
        where TEnumerable : struct, IStructEnumerable<T>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerable source = source;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = default;
            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<T> span)
        {
            span = default;
            return false;
        }

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