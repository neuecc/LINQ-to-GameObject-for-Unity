namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static HashSet<TSource> ToHashSet<TEnumerable, TSource>(this TEnumerable source)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            throw new NotImplementedException();
        }

        public static HashSet<TSource> ToHashSet<TEnumerable, TSource>(this TEnumerable source, IEqualityComparer<TSource> comparer)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            throw new NotImplementedException();
        }

    }
}
