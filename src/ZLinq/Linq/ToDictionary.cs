namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static Dictionary<TKey, TValue> ToDictionary<TEnumerable, TKey, TValue>(this TEnumerable source)
            where TKey : notnull
            where TEnumerable : struct, IValueEnumerable<KeyValuePair<TKey, TValue>>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            return ToDictionary<TEnumerable, TKey, TValue>(source, null!);
        }

        public static Dictionary<TKey, TValue> ToDictionary<TEnumerable, TKey, TValue>(this TEnumerable source, IEqualityComparer<TKey> comparer)
            where TKey : notnull
            where TEnumerable : struct, IValueEnumerable<KeyValuePair<TKey, TValue>>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            if (source.TryGetSpan(out var span))
            {
                var dict = new Dictionary<TKey, TValue>(span.Length, comparer);
                foreach (var item in span)
                {
                    dict.Add(item.Key, item.Value);
                }
                return dict;
            }
            else
            {
                var dict = source.TryGetNonEnumeratedCount(out var count)
                    ? new Dictionary<TKey, TValue>(count, comparer)
                    : new Dictionary<TKey, TValue>(comparer);
                while (source.TryGetNext(out var item))
                {
                    dict.Add(item.Key, item.Value);
                }
                return dict;
            }
        }

        public static Dictionary<TKey, TSource> ToDictionary<TEnumerable, TSource, TKey>(this TEnumerable source, Func<TSource, TKey> keySelector)
            where TKey : notnull
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            return ToDictionary(source, keySelector, null!);
        }

        public static Dictionary<TKey, TSource> ToDictionary<TEnumerable, TSource, TKey>(this TEnumerable source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
            where TKey : notnull
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            if (source.TryGetSpan(out var span))
            {
                var dict = new Dictionary<TKey, TSource>(span.Length, comparer);
                foreach (var item in span)
                {
                    dict.Add(keySelector(item), item);
                }
                return dict;
            }
            else
            {
                var dict = source.TryGetNonEnumeratedCount(out var count)
                    ? new Dictionary<TKey, TSource>(count, comparer)
                    : new Dictionary<TKey, TSource>(comparer);
                while (source.TryGetNext(out var item))
                {
                    dict.Add(keySelector(item), item))
                }
                return dict;
            }
        }

        public static Dictionary<TKey, TElement> ToDictionary<TEnumerable, TSource, TKey, TElement>(this TEnumerable source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
            where TKey : notnull
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            return ToDictionary(source, keySelector, elementSelector, null!);
        }

        public static Dictionary<TKey, TElement> ToDictionary<TEnumerable, TSource, TKey, TElement>(this TEnumerable source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
            where TKey : notnull
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            if (source.TryGetSpan(out var span))
            {
                var dict = new Dictionary<TKey, TElement>(span.Length, comparer);
                foreach (var item in span)
                {
                    dict.Add(keySelector(item), elementSelector(item));
                }
                return dict;
            }
            else
            {
                var dict = source.TryGetNonEnumeratedCount(out var count)
                    ? new Dictionary<TKey, TElement>(count, comparer)
                    : new Dictionary<TKey, TElement>(comparer);
                while (source.TryGetNext(out var item))
                {
                    dict.Add(keySelector(item), elementSelector(item));
                }
                return dict;
            }
        }

    }
}
