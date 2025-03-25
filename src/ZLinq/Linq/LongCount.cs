namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static Int64 LongCount<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            using (var enumerator = source.Enumerator)
            {
                if (enumerator.TryGetNonEnumeratedCount(out var count))
                {
                    return count;
                }

                var longCount = 0L;
                while (enumerator.TryGetNext(out _))
                {
                    checked { longCount++; }
                }
                return longCount;
            }
        }

        public static Int64 LongCount<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, Boolean> predicate)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            ArgumentNullException.ThrowIfNull(predicate);
            using (var enumerator = source.Enumerator)
            {
                if (enumerator.TryGetSpan(out var span))
                {
                    var longCount = 0L;
                    foreach (var current in span)
                    {
                        if (predicate(current))
                        {
                            checked { longCount++; }
                        }
                    }
                    return longCount;
                }
                else
                {
                    var longCount = 0L;
                    while (enumerator.TryGetNext(out var current))
                    {
                        if (predicate(current))
                        {
                            checked { longCount++; }
                        }
                    }
                    return longCount;
                }
            }
        }

    }
}
