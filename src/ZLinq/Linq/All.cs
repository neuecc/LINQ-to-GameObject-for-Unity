namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static Boolean All<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, Boolean> predicate)
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
                        if (!predicate(item))
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    while (enumerator.TryGetNext(out var item))
                    {
                        if (!predicate(item))
                        {
                            return false;
                        }
                    }
                }

                return true;
            }
        }
    }
}
