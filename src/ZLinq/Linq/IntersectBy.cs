namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static ValueEnumerable<IntersectBy<TEnumerator, TEnumerator2, TSource, TKey>, TSource> IntersectBy<TEnumerator, TEnumerator2, TSource, TKey>(this ValueEnumerable<TEnumerator, TSource> source, ValueEnumerable<TEnumerator2, TKey> second, Func<TSource, TKey> keySelector)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            where TEnumerator2 : struct, IValueEnumerator<TKey>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, second, Throws.IfNull(keySelector), null));

        public static ValueEnumerable<IntersectBy<TEnumerator, TEnumerator2, TSource, TKey>, TSource> IntersectBy<TEnumerator, TEnumerator2, TSource, TKey>(this ValueEnumerable<TEnumerator, TSource> source, ValueEnumerable<TEnumerator2, TKey> second, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            where TEnumerator2 : struct, IValueEnumerator<TKey>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, second, Throws.IfNull(keySelector), comparer));

        public static ValueEnumerable<IntersectBy<TEnumerator, FromEnumerable<TKey>, TSource, TKey>, TSource> IntersectBy<TEnumerator, TSource, TKey>(this ValueEnumerable<TEnumerator, TSource> source, IEnumerable<TKey> second, Func<TSource, TKey> keySelector)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, second.AsValueEnumerable(), Throws.IfNull(keySelector), null));

        public static ValueEnumerable<IntersectBy<TEnumerator, FromEnumerable<TKey>, TSource, TKey>, TSource> IntersectBy<TEnumerator, TSource, TKey>(this ValueEnumerable<TEnumerator, TSource> source, IEnumerable<TKey> second, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, second.AsValueEnumerable(), Throws.IfNull(keySelector), comparer));
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
    struct IntersectBy<TEnumerator, TEnumerator2, TSource, TKey>(TEnumerator source, ValueEnumerable<TEnumerator2, TKey> second, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer)
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
        HashSetSlim<TKey>? set;

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

        public bool TryCopyTo(Span<TSource> destination, Index offset) => false;

        public bool TryGetNext(out TSource current)
        {
            if (set == null)
            {
                set = second.ToHashSetSlim(comparer);
            }

            while (source.TryGetNext(out var value))
            {
                if (set.Remove(keySelector(value)))
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
            set?.Dispose();
            source.Dispose();
        }
    }

}
