namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        // .NET has IEnumerable<KeyValuePair<>> method but we can not infer it so not implemented.

        //public static Dictionary<TKey, TValue> ToDictionary<TEnumerator, TKey, TValue>(in this ValueEnumerable<TEnumerator, TSource> source)
        //    where TKey : notnull
        //    where TEnumerator : struct, IValueEnumerable<KeyValuePair<TKey, TValue>>
        //public static Dictionary<TKey, TValue> ToDictionary<TEnumerator, TKey, TValue>(in this ValueEnumerable<TEnumerator, TSource> source, IEqualityComparer<TKey> comparer)
        //    where TKey : notnull
        //    where TEnumerator : struct, IValueEnumerable<KeyValuePair<TKey, TValue>>

        public static Dictionary<TKey, TSource> ToDictionary<TEnumerator, TSource, TKey>(in this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TKey> keySelector)
            where TKey : notnull
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            return ToDictionary(source, keySelector, null!);
        }

        public static Dictionary<TKey, TSource> ToDictionary<TEnumerator, TSource, TKey>(in this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
            where TKey : notnull
            where TEnumerator : struct, IValueEnumerator<TSource>
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
                    dict.Add(keySelector(item), item);
                }
                return dict;
            }
        }

        public static Dictionary<TKey, TElement> ToDictionary<TEnumerator, TSource, TKey, TElement>(in this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
            where TKey : notnull
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            return ToDictionary(source, keySelector, elementSelector, null!);
        }

        public static Dictionary<TKey, TElement> ToDictionary<TEnumerator, TSource, TKey, TElement>(in this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
            where TKey : notnull
            where TEnumerator : struct, IValueEnumerator<TSource>
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
