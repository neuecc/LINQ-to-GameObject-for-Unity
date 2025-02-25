namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static Boolean SequenceEqual<TEnumerable, TSource>(this TEnumerable source, IEnumerable<TSource> second)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            throw new NotImplementedException();
        }

        public static Boolean SequenceEqual<TEnumerable, TSource>(this TEnumerable source, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            throw new NotImplementedException();
        }

    }
}
