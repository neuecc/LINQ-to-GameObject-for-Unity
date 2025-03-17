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
            return TryGetFirst<TEnumerable, TSource>(ref source, out var value)
                ? value
                : Throws.NoElements<TSource>();
        }

        public static TSource First<TEnumerable, TSource>(this TEnumerable source, Func<TSource, Boolean> predicate)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            return TryGetFirst<TEnumerable, TSource>(ref source, predicate, out var value)
                ? value
                : Throws.NoMatch<TSource>();
        }

        public static TSource? FirstOrDefault<TEnumerable, TSource>(this TEnumerable source)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            return TryGetFirst<TEnumerable, TSource>(ref source, out var value)
                ? value
                : default;
        }

        public static TSource FirstOrDefault<TEnumerable, TSource>(this TEnumerable source, TSource defaultValue)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            return TryGetFirst<TEnumerable, TSource>(ref source, out var value)
                ? value
                : defaultValue;
        }

        public static TSource? FirstOrDefault<TEnumerable, TSource>(this TEnumerable source, Func<TSource, Boolean> predicate)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            return TryGetFirst<TEnumerable, TSource>(ref source, predicate, out var value)
                ? value
                : default;
        }

        public static TSource FirstOrDefault<TEnumerable, TSource>(this TEnumerable source, Func<TSource, Boolean> predicate, TSource defaultValue)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            return TryGetFirst<TEnumerable, TSource>(ref source, predicate, out var value)
                ? value
                : defaultValue;
        }

        static bool TryGetFirst<TEnumerable, TSource>(ref TEnumerable source, out TSource value)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            using (source)
            {
                if (source.TryGetSpan(out var span))
                {
                    if (span.Length > 0)
                    {
                        value = span[0];
                        return true;
                    }

                    value = default!;
                    return false;
                }

                if (source.TryGetNext(out value))
                {
                    return true;
                }

                value = default!;
                return false;
            }
        }

        static bool TryGetFirst<TEnumerable, TSource>(ref TEnumerable source, Func<TSource, Boolean> predicate, out TSource value)
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
                            value = item;
                            return true;
                        }
                    }

                    value = default!;
                    return false;
                }

                while (source.TryGetNext(out var current))
                {
                    if (predicate(current))
                    {
                        value = current;
                        return true;
                    }
                }

                value = default!;
                return false;
            }
        }
    }
}
