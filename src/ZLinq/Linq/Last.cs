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
            return TryGetLast<TEnumerable, TSource>(ref source, out var value)
                ? value
                : Throws.NoElements<TSource>();
        }

        public static TSource Last<TEnumerable, TSource>(this TEnumerable source, Func<TSource, Boolean> predicate)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            return TryGetLast<TEnumerable, TSource>(ref source, predicate, out var value)
                ? value
                : Throws.NoMatch<TSource>();
        }

        public static TSource? LastOrDefault<TEnumerable, TSource>(this TEnumerable source)
    where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
    , allows ref struct
#endif
        {
            return TryGetLast<TEnumerable, TSource>(ref source, out var value)
                ? value
                : default;
        }

        public static TSource LastOrDefault<TEnumerable, TSource>(this TEnumerable source, TSource defaultValue)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            return TryGetLast<TEnumerable, TSource>(ref source, out var value)
                ? value
                : defaultValue;
        }

        public static TSource? LastOrDefault<TEnumerable, TSource>(this TEnumerable source, Func<TSource, Boolean> predicate)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            return TryGetLast<TEnumerable, TSource>(ref source, predicate, out var value)
                ? value
                : default;
        }

        public static TSource LastOrDefault<TEnumerable, TSource>(this TEnumerable source, Func<TSource, Boolean> predicate, TSource defaultValue)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            return TryGetLast<TEnumerable, TSource>(ref source, predicate, out var value)
                ? value
                : defaultValue;
        }

        public static bool TryGetLast<TEnumerable, TSource>(ref TEnumerable source, out TSource value)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            using (source)
            {
                if (source.TryGetSpan(out var span))
                {
                    if (span.Length == 0)
                    {
                        value = default!;
                        return false;
                    }

                    value = span[^1];
                    return true;
                }

                if (!source.TryGetNext(out value))
                {
                    return false;
                }

                while (source.TryGetNext(out value))
                {
                }

                return true;
            }
        }

        public static bool TryGetLast<TEnumerable, TSource>(ref TEnumerable source, Func<TSource, Boolean> predicate, out TSource value)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            using (source)
            {
                if (source.TryGetSpan(out var span))
                {
                    // search from last
                    for (var i = span.Length - 1; i >= 0; i--)
                    {
                        ref readonly var v = ref span[i];
                        if (predicate(v))
                        {
                            value = v;
                            return true;
                        }
                    }

                    value = default!;
                    return false;
                }

                while (source.TryGetNext(out var last))
                {
                    if (predicate(last)) // found
                    {
                        while (source.TryGetNext(out var current))
                        {
                            if (predicate(current))
                            {
                                last = current;
                            }
                        }

                        value = last;
                        return true;
                    }
                }

                value = default!;
                return false;
            }
        }
    }
}
