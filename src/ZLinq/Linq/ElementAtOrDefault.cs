namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static TSource ElementAtOrDefault<TEnumerable, TSource>(this TEnumerable source, Int32 index)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            throw new NotImplementedException();
        }

        public static TSource ElementAtOrDefault<TEnumerable, TSource>(this TEnumerable source, Index index)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            throw new NotImplementedException();
        }

    }
}
