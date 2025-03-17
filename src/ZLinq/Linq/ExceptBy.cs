namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static ValueEnumerable<ExceptBy<TEnumerator, TEnumerator2, TSource, TKey>, TSource> ExceptBy<TEnumerator, TEnumerator2, TSource, TKey>(in this ValueEnumerable<TEnumerator, TSource> source, in ValueEnumerable<TEnumerator2, TKey> second, Func<TSource, TKey> keySelector)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            where TEnumerator2 : struct, IValueEnumerator<TKey>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, second, keySelector, null));

        public static ValueEnumerable<ExceptBy<TEnumerator, TEnumerator2, TSource, TKey>, TSource> ExceptBy<TEnumerator, TEnumerator2, TSource, TKey>(in this ValueEnumerable<TEnumerator, TSource> source, in ValueEnumerable<TEnumerator2, TKey> second, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            where TEnumerator2 : struct, IValueEnumerator<TKey>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, second, keySelector, comparer));

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
    struct ExceptBy<TEnumerator, TEnumerator2, TSource, TKey>(in TEnumerator source, in ValueEnumerable<TEnumerator2, TKey> second, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer)
        : IValueEnumerator<TSource>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
        where TEnumerator2 : struct, IValueEnumerator<TKey>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerator source = source;
        ValueEnumerable<TEnumerator2, TKey> second = second;
        HashSet<TKey>? set;

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
                set = second.ToHashSet(comparer ?? EqualityComparer<TKey>.Default);
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
