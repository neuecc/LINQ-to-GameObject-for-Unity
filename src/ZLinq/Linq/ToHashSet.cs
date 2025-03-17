#nullable disable

namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static HashSet<TSource> ToHashSet<TEnumerator, TSource>(in this ValueEnumerable<TEnumerator, TSource> source)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            return ToHashSet<TEnumerator, TSource>(source, null!);
        }

        public static HashSet<TSource> ToHashSet<TEnumerator, TSource>(in this ValueEnumerable<TEnumerator, TSource> source, IEqualityComparer<TSource> comparer)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            if (source.TryGetSpan(out var span))
            {
                var hashSet = new HashSet<TSource>(span.Length, comparer);
                foreach (var item in span)
                {
                    hashSet.Add(item);
                }
                return hashSet;
            }
            else
            {
                var hashSet = source.TryGetNonEnumeratedCount(out var count)
                    ? new HashSet<TSource>(count, comparer)
                    : new HashSet<TSource>(comparer);
                while (source.TryGetNext(out var item))
                {
                    hashSet.Add(item);
                }
                return hashSet;
            }
        }
    }
}
