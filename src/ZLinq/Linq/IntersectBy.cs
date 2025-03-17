namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static IntersectBy<TEnumerator, TSource, TKey> IntersectBy<TEnumerator, TSource, TKey>(in this ValueEnumerable<TEnumerator, TSource> source, IEnumerable<TKey> second, Func<TSource, TKey> keySelector)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(source, second, keySelector, null);

        public static IntersectBy<TEnumerator, TSource, TKey> IntersectBy<TEnumerator, TSource, TKey>(in this ValueEnumerable<TEnumerator, TSource> source, IEnumerable<TKey> second, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
            where TEnumerator : struct, IValueEnumerator<TSource>
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
    struct IntersectBy<TEnumerator, TSource, TKey>(TEnumerator source, IEnumerable<TKey> second, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer)
        : IValueEnumerator<TSource>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerator source = source;
        HashSet<TKey>? set;

        public ValueEnumerator<IntersectBy<TEnumerator, TSource, TKey>, TSource> GetEnumerator() => new(this);

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
