namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static ValueEnumerable<Intersect<TEnumerator, TEnumerator2, TSource>, TSource> Intersect<TEnumerator, TEnumerator2, TSource>(this ValueEnumerable<TEnumerator, TSource> source, ValueEnumerable<TEnumerator2, TSource> second)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            where TEnumerator2 : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, second, null));

        public static ValueEnumerable<Intersect<TEnumerator, TEnumerator2, TSource>, TSource> Intersect<TEnumerator, TEnumerator2, TSource>(this ValueEnumerable<TEnumerator, TSource> source, ValueEnumerable<TEnumerator2, TSource> second, IEqualityComparer<TSource>? comparer)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            where TEnumerator2 : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, second, comparer));



        public static ValueEnumerable<Intersect<TEnumerator, FromEnumerable<TSource>, TSource>, TSource> Intersect<TEnumerator,  TSource>(this ValueEnumerable<TEnumerator, TSource> source, IEnumerable< TSource> second)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, second.AsValueEnumerable(), null));

        public static ValueEnumerable<Intersect<TEnumerator, FromEnumerable<TSource>, TSource>, TSource> Intersect<TEnumerator,  TSource>(this ValueEnumerable<TEnumerator, TSource> source, IEnumerable< TSource> second, IEqualityComparer<TSource>? comparer)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, second.AsValueEnumerable(), comparer));
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
    struct Intersect<TEnumerator, TEnumerator2, TSource>(TEnumerator source, ValueEnumerable<TEnumerator2, TSource> second, IEqualityComparer<TSource>? comparer)
        : IValueEnumerator<TSource>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
        where TEnumerator2 : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
    {
        TEnumerator source = source;
        ValueEnumerable<TEnumerator2, TSource> second = second;
        HashSet<TSource>? set;

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
                set = second.ToHashSet(comparer);
            }

            while (source.TryGetNext(out var value))
            {
                if (set.Remove(value))
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
