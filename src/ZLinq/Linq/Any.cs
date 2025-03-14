namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static Boolean Any<TEnumerable, TSource>(this TEnumerable source)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            using (source)
            {
                if (source.TryGetNonEnumeratedCount(out var count))
                {
                    return count > 0;
                }

                return source.TryGetNext(out _);
            }
        }

        public static Boolean Any<TEnumerable, TSource>(this TEnumerable source, Func<TSource, Boolean> predicate)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            using (source)
            {
                if (source.TryGetSpan(out var span))
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
                    while (source.TryGetNext(out var item))
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
