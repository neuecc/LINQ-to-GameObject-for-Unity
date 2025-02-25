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
            throw new NotImplementedException();
        }

        public static TAccumulate Aggregate<TEnumerable, TSource, TAccumulate>(this TEnumerable source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            throw new NotImplementedException();
        }

        public static TResult Aggregate<TEnumerable, TSource, TAccumulate, TResult>(this TEnumerable source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func, Func<TAccumulate, TResult> resultSelector)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            throw new NotImplementedException();
        }

    }
}
