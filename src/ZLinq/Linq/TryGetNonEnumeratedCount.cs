namespace ZLinq;

partial class ValueEnumerableExtensions
{
    public static bool TryGetNonEnumeratedCount<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source, out int count)
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        using (var enumerator = source.Enumerator)
        {
            return enumerator.TryGetNonEnumeratedCount(out count);
        }
    }
}