namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static TSource First<TEnumerable, TSource>(this TEnumerable source)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            // TODO:TryGetSpan, optimize path

            if (source.TryGetNext(out var current))
            {
                return current;
            }

            throw new InvalidOperationException("Sequence contains no elements");
        }

        public static TSource First<TEnumerable, TSource>(this TEnumerable source, Func<TSource, Boolean> predicate)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            while (source.TryGetNext(out var current))
            {
                if (predicate(current))
                {
                    return current;
                }
            }

            throw new InvalidOperationException("Sequence contains no matching element");
        }

    }
}
