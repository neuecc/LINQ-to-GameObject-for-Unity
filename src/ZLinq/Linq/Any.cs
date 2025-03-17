namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static Boolean Any<TEnumerator, TSource>(in this ValueEnumerable<TEnumerator, TSource> source)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            using (var enumerator = source.Enumerator)
            {
                if (enumerator.TryGetNonEnumeratedCount(out var count))
                {
                    return count > 0;
                }

                return enumerator.TryGetNext(out _);
            }
        }

        public static Boolean Any<TEnumerator, TSource>(in this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, Boolean> predicate)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            using (var enumerator = source.Enumerator)
            {
                if (enumerator.TryGetSpan(out var span))
                {
                    foreach (var item in span)
                    {
                        if (predicate(item))
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    while (enumerator.TryGetNext(out var item))
                    {
                        if (predicate(item))
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }
    }
}
