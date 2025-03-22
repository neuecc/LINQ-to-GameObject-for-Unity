namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static List<TSource> ToList<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            using var enumerator = source.Enumerator;

            if (enumerator.TryGetNonEnumeratedCount(out var count))
            {
                var list = new List<TSource>(count); // list with capacity set internal buffer as source size
#if NET8_0_OR_GREATER
                CollectionsMarshal.SetCount(list, count);
#else
                CollectionsMarshal.UnsafeSetCount(list, count);
#endif
                var span = CollectionsMarshal.AsSpan(list);
                if (!enumerator.TryCopyTo(span))
                {
                    var i = 0;
                    while (enumerator.TryGetNext(out var current))
                    {
                        span[i++] = current;
                    }
                }
                return list;
            }
            else
            {
                // list.Add is slow, avoid it.

                using var arrayBuilder = new SegmentedArrayBuilder<TSource>();
                while (enumerator.TryGetNext(out var item))
                {
                    arrayBuilder.Add(item);
                }

                var list = new List<TSource>(arrayBuilder.Count);
#if NET8_0_OR_GREATER
                CollectionsMarshal.SetCount(list, count);
#else
                CollectionsMarshal.UnsafeSetCount(list, count);
#endif

                var span = CollectionsMarshal.AsSpan(list);
                arrayBuilder.CopyTo(span);
                return list;
            }
        }
    }
}
