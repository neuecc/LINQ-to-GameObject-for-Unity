namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static IntersectBy<TEnumerable, TSource, TKey> IntersectBy<TEnumerable, TSource, TKey>(this TEnumerable source, IEnumerable<TKey> second, Func<TSource, TKey> keySelector)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(source, second, keySelector, null);

        public static IntersectBy<TEnumerable, TSource, TKey> IntersectBy<TEnumerable, TSource, TKey>(this TEnumerable source, IEnumerable<TKey> second, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(source, second, keySelector, comparer);

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
    struct IntersectBy<TEnumerable, TSource, TKey>(TEnumerable source, IEnumerable<TKey> second, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer)
        : IValueEnumerable<TSource>
        where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerable source = source;
        HashSet<TKey>? set;

        public ValueEnumerator<IntersectBy<TEnumerable, TSource, TKey>, TSource> GetEnumerator() => new(this);

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = 0;
            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<TSource> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(Span<TSource> dest) => false;

        public bool TryGetNext(out TSource current)
        {
            if (set == null)
            {
                set = new HashSet<TKey>(second, comparer ?? EqualityComparer<TKey>.Default);
            }

            if (source.TryGetNext(out var value) && set.Remove(keySelector(value)))
            {
                current = value;
                return true;
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
