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
            throw new NotImplementedException();
        }

        public static Dictionary<TKey, TValue> ToDictionary<TEnumerable, TKey, TValue>(this TEnumerable source, IEqualityComparer<TKey> comparer)
            where TKey : notnull
            where TEnumerable : struct, IValueEnumerable<KeyValuePair<TKey, TValue>>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            throw new NotImplementedException();
        }

        public static Dictionary<TKey, TSource> ToDictionary<TEnumerable, TSource, TKey>(this TEnumerable source, Func<TSource, TKey> keySelector)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            throw new NotImplementedException();
        }

        public static Dictionary<TKey, TSource> ToDictionary<TEnumerable, TSource, TKey>(this TEnumerable source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            throw new NotImplementedException();
        }

        public static Dictionary<TKey, TElement> ToDictionary<TEnumerable, TSource, TKey, TElement>(this TEnumerable source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            throw new NotImplementedException();
        }

        public static Dictionary<TKey, TElement> ToDictionary<TEnumerable, TSource, TKey, TElement>(this TEnumerable source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            throw new NotImplementedException();
        }

    }
}
