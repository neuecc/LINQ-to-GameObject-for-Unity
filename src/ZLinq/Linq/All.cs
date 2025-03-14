namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static Boolean All<TEnumerable, TSource>(this TEnumerable source, Func<TSource, Boolean> predicate)
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
                        if (!predicate(item))
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    while (source.TryGetNext(out var item))
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
