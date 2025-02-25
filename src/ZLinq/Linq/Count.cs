namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static Int32 Count<TEnumerable, TSource>(this TEnumerable source)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            if (source.TryGetNonEnumeratedCount(out var count))
            {
                return count;
            }

            count = 0;
            try
            {
                while (source.TryGetNext(out _))
                {
                    count++;
                }
            }
            finally
            {
                source.Dispose();
            }
            return count;
        }

        public static Int32 Count<TEnumerable, TSource>(this TEnumerable source, Func<TSource, Boolean> predicate)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            if (source.TryGetNonEnumeratedCount(out var count))
            {
                return count;
            }

            count = 0;
            try
            {
                while (source.TryGetNext(out var current))
                {
                    if (predicate(current))
                    {
                        count++;
                    }
                }
            }
            finally
            {
                source.Dispose();
            }
            return count;
        }

    }
}
