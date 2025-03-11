namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static TSource Aggregate<TEnumerable, TSource>(this TEnumerable source, Func<TSource, TSource, TSource> func)
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
                        return Internal.Throws.NoElements<TSource>();
                    }

                    var result = span[0];
                    for (int i = 1; i < span.Length; i++)
                    {
                        result = func(result, span[i]);
                    }

                    return result;
                }
                else
                {
                    if (!source.TryGetNext(out var result))
                    {
                        return Internal.Throws.NoElements<TSource>();
                    }

                    while (source.TryGetNext(out var current))
                    {
                        result = func(result, current);
                    }

                    return result;
                }
            }
        }

        public static TAccumulate Aggregate<TEnumerable, TSource, TAccumulate>(this TEnumerable source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            using (source)
            {
                if (source.TryGetSpan(out var span))
                {
                    var result = seed;
                    foreach (var item in span)
                    {
                        result = func(result, item);
                    }

                    return result;
                }
                else
                {

                    var result = seed;
                    while (source.TryGetNext(out var current))
                    {
                        result = func(result, current);
                    }

                    return result;
                }
            }
        }

        public static TResult Aggregate<TEnumerable, TSource, TAccumulate, TResult>(this TEnumerable source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func, Func<TAccumulate, TResult> resultSelector)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            using (source)
            {
                if (source.TryGetSpan(out var span))
                {
                    var result = seed;
                    foreach (var item in span)
                    {
                        result = func(result, item);
                    }

                    return resultSelector(result);
                }
                else
                {
                    var result = seed;
                    while (source.TryGetNext(out var current))
                    {
                        result = func(result, current);
                    }

                    return resultSelector(result);
                }
            }
        }
    }
}
