namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static Boolean Contains<TEnumerable, TSource>(this TEnumerable source, TSource value)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            if (source.TryGetSpan(out var span))
            {
                foreach (var item in span)
                {
                    if (EqualityComparer<TSource>.Default.Equals(item, value))
                    {
                        return true;
                    }
                }
            }
            else
            {
                try
                {
                    while (source.TryGetNext(out var item))
                    {
                        if (EqualityComparer<TSource>.Default.Equals(item, value))
                        {
                            return true;
                        }
                    }
                }
                finally
                {
                    source.Dispose();
                }
            }

            return false;
        }

        public static Boolean Contains<TEnumerable, TSource>(this TEnumerable source, TSource value, IEqualityComparer<TSource> comparer)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            if (source.TryGetSpan(out var span))
            {
                foreach (var item in span)
                {
                    if (comparer.Equals(item, value))
                    {
                        return true;
                    }
                }
            }
            else
            {
                try
                {
                    while (source.TryGetNext(out var item))
                    {
                        if (comparer.Equals(item, value))
                        {
                            return true;
                        }
                    }
                }
                finally
                {
                    source.Dispose();
                }
            }

            return false;
        }
    }
}
