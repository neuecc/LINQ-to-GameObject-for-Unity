namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static Int64 LongCount<TEnumerator, TSource>(in this ValueEnumerable<TEnumerator, TSource> source)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            using (source)
            {
                if (source.TryGetNonEnumeratedCount(out var count))
                {
                    return count;
                }

                var longCount = 0L;
                while (source.TryGetNext(out _))
                {
                    checked { longCount++; }
                }
                return longCount;
            }
        }

        public static Int64 LongCount<TEnumerator, TSource>(in this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, Boolean> predicate)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            using (source)
            {
                if (source.TryGetNonEnumeratedCount(out var count))
                {
                    return count;
                }

                var longCount = 0L;
                while (source.TryGetNext(out var current))
                {
                    if (predicate(current))
                    {
                        checked { longCount++; }
                    }
                }
                return count;
            }
        }

    }
}
