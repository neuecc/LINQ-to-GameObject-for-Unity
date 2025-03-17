namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static TSource MaxBy<TEnumerator, TSource, TKey>(in this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TKey> keySelector)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            throw new NotImplementedException();
        }

        public static TSource MaxBy<TEnumerator, TSource, TKey>(in this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            throw new NotImplementedException();
        }

    }
}
