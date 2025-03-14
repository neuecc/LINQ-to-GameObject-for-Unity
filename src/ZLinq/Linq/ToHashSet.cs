#nullable disable

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
            return ToHashSet<TEnumerable, TSource>(source, null!);
        }

        public static HashSet<TSource> ToHashSet<TEnumerable, TSource>(this TEnumerable source, IEqualityComparer<TSource> comparer)
            where TEnumerable : struct, IValueEnumerable<TSource>
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
