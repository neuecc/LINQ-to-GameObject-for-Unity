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
                if (!enumerator.TryCopyTo(span, 0))
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
                var initialBuffer = default(InlineArray16<TSource>);
#if NET8_0_OR_GREATER
                Span<TSource> initialBufferSpan = initialBuffer;
#else
                Span<TSource> initialBufferSpan = InlineArrayMarshal.AsSpan<InlineArray16<TSource>, TSource>(ref initialBuffer, 16);
#endif
                var arrayBuilder = new SegmentedArrayBuilder<TSource>(initialBufferSpan);
                var span = arrayBuilder.GetSpan();
                var i = 0;
                while (enumerator.TryGetNext(out var item))
                {
                    if (i == span.Length)
                    {
                        arrayBuilder.Advance(i);
                        span = arrayBuilder.GetSpan();
                        i = 0;
                    }

                    span[i++] = item;
                }
                arrayBuilder.Advance(i);

                count = arrayBuilder.Count;

                var list = new List<TSource>(count);
#if NET8_0_OR_GREATER
                CollectionsMarshal.SetCount(list, count);
#else
                CollectionsMarshal.UnsafeSetCount(list, count);
#endif

                var listSpan = CollectionsMarshal.AsSpan(list);
                arrayBuilder.CopyToAndClear(listSpan);
                return list;
            }
        }
    }
}
