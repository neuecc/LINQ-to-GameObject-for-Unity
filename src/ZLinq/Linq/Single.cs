namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static TSource Single<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            var enumerator = source.Enumerator;
            try
            {
                return TryGetSingle<TEnumerator, TSource>(ref enumerator, out var value)
                    ? value
                    : Throws.NoElements<TSource>();
            }
            finally
            {
                enumerator.Dispose();
            }
        }

        public static TSource Single<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, Boolean> predicate)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            ArgumentNullException.ThrowIfNull(predicate);
            var enumerator = source.Enumerator;
            try
            {
                return TryGetSingle<TEnumerator, TSource>(ref enumerator, predicate, out var value)
                ? value
                : Throws.NoMatch<TSource>();
            }
            finally
            {
                enumerator.Dispose();
            }
        }

        public static TSource? SingleOrDefault<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source)
    where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
    , allows ref struct
#endif
        {
            var enumerator = source.Enumerator;
            try
            {
                return TryGetSingle<TEnumerator, TSource>(ref enumerator, out var value)
                    ? value
                    : default;
            }
            finally
            {
                enumerator.Dispose();
            }
        }

        public static TSource SingleOrDefault<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source, TSource defaultValue)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            var enumerator = source.Enumerator;
            try
            {
                return TryGetSingle<TEnumerator, TSource>(ref enumerator, out var value)
                    ? value
                    : defaultValue;
            }
            finally
            {
                enumerator.Dispose();
            }
        }

        public static TSource? SingleOrDefault<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, Boolean> predicate)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            ArgumentNullException.ThrowIfNull(predicate);
            var enumerator = source.Enumerator;
            try
            {
                return TryGetSingle<TEnumerator, TSource>(ref enumerator, predicate, out var value)
                ? value
                : default;
            }
            finally
            {
                enumerator.Dispose();
            }
        }

        public static TSource SingleOrDefault<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, Boolean> predicate, TSource defaultValue)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            ArgumentNullException.ThrowIfNull(predicate);
            var enumerator = source.Enumerator;
            try
            {
                return TryGetSingle<TEnumerator, TSource>(ref enumerator, predicate, out var value)
                ? value
                : defaultValue;
            }
            finally
            {
                enumerator.Dispose();
            }
        }

        // Try... but throws exception when there are more than one element.
        static bool TryGetSingle<TEnumerator, TSource>(ref TEnumerator source, out TSource value)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            if (source.TryGetSpan(out var span))
            {
                if (span.Length == 1)
                {
                    value = span[0];
                    return true;
                }
                else if (span.Length != 0)
                {
                    Throws.MoreThanOneElement();
                }
            }
            else
            {
                if (source.TryGetNext(out value))
                {
                    if (source.TryGetNext(out _))
                    {
                        Throws.MoreThanOneElement();
                    }
                    return true;
                }
            }

            value = default!;
            return false;
        }

        static bool TryGetSingle<TEnumerator, TSource>(ref TEnumerator source, Func<TSource, Boolean> predicate, out TSource value)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            value = default!;
            bool found = false;
            if (source.TryGetSpan(out var span))
            {
                foreach (var item in span)
                {
                    if (predicate(item))
                    {
                        if (found)
                        {
                            Throws.MoreThanOneMatch();
                        }
                        found = true;
                        value = item;
                    }
                }
                if (found)
                {
                    return true;
                }
            }
            else
            {
                while (source.TryGetNext(out var item))
                {
                    if (predicate(item))
                    {
                        if (found)
                        {
                            Throws.MoreThanOneMatch();
                        }
                        found = true;
                        value = item;
                    }
                }
                if (found)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
