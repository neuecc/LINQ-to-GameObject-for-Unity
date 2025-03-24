namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static HashSet<TSource> ToHashSet<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            return ToHashSet<TEnumerator, TSource>(source, null);
        }

        public static HashSet<TSource> ToHashSet<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source, IEqualityComparer<TSource>? comparer)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            using var enumerator = source.Enumerator;
            if (enumerator.TryGetSpan(out var span))
            {
#if NETSTANDARD2_0
                var hashSet = new HashSet<TSource>(comparer);
#else
                var hashSet = new HashSet<TSource>(span.Length, comparer);
#endif
                foreach (var item in span)
                {
                    hashSet.Add(item);
                }
                return hashSet;
            }
            else
            {
#if NETSTANDARD2_0
                var hashSet = new HashSet<TSource>(comparer);
#else
                var hashSet = enumerator.TryGetNonEnumeratedCount(out var count)
                    ? new HashSet<TSource>(count, comparer)
                    : new HashSet<TSource>(comparer);
#endif
                while (enumerator.TryGetNext(out var item))
                {
                    hashSet.Add(item);
                }
                return hashSet;
            }
        }

        internal static HashSetSlim<TSource> ToHashSetSlim<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source, IEqualityComparer<TSource>? comparer)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            using var enumerator = source.Enumerator;
            if (enumerator.TryGetSpan(out var span))
            {
                var hashSet = new HashSetSlim<TSource>(span.Length, comparer);
                foreach (var item in span)
                {
                    hashSet.Add(item);
                }
                return hashSet;
            }
            else
            {
                var hashSet = enumerator.TryGetNonEnumeratedCount(out var count)
                    ? new HashSetSlim<TSource>(count, comparer)
                    : new HashSetSlim<TSource>(comparer);
                while (enumerator.TryGetNext(out var item))
                {
                    hashSet.Add(item);
                }
                return hashSet;
            }
        }
    }
}
