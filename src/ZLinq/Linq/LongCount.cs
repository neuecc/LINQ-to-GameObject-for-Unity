namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static Int64 LongCount<TEnumerable, TSource>(this TEnumerable source)
            where TEnumerable : struct, IValueEnumerable<TSource>
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

        public static Int64 LongCount<TEnumerable, TSource>(this TEnumerable source, Func<TSource, Boolean> predicate)
            where TEnumerable : struct, IValueEnumerable<TSource>
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
