namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static TSource Last<TEnumerable, TSource>(this TEnumerable source)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            if (source.TryGetSpan(out var span))
            {
                return span[^1];
            }

            Unsafe.SkipInit<TSource>(out var last);
            if (!source.TryGetNext(out last))
            {
                throw new InvalidOperationException("no element.");
            }

            while (source.TryGetNext(out last))
            {
            }
            return last;
        }

        public static TSource Last<TEnumerable, TSource>(this TEnumerable source, Func<TSource, Boolean> predicate)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            if (source.TryGetSpan(out var span))
            {
                for (var i = span.Length - 1; i >= 0; i--)
                {
                    ref readonly var v = ref span[i];
                    if (predicate(v))
                    {
                        return v;
                    }
                }
                throw new InvalidOperationException("no matched.");
            }

            Unsafe.SkipInit<TSource>(out var last);
            while (source.TryGetNext(out last))
            {
                if (predicate(last))
                {
                    while (source.TryGetNext(out var value))
                    {
                        if (predicate(value))
                        {
                            last = value;
                        }
                    }
                    return last;
                }
            }
            throw new InvalidOperationException("no matched.");
        }
    }
}
