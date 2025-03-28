using System.Buffers;

namespace ZLinq;

partial class ValueEnumerableExtensions
{
    public static (TSource[] Array, int Size) ToArrayPool<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source)
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        using var enumerator = source.Enumerator;

        if (enumerator.TryGetNonEnumeratedCount(out var count))
        {
            // when count == 0 but always return rental array
            var array = ArrayPool<TSource>.Shared.Rent(count);

            if (enumerator.TryCopyTo(array.AsSpan(0, count), 0))
            {
                return (array, count);
            }

            var i = 0;
            while (enumerator.TryGetNext(out var item))
            {
                array[i++] = item;
            }

            return (array, i);
        }
        else
        {
#if NETSTANDARD2_0
            Span<TSource> initialBufferSpan = default;
#elif NET8_0_OR_GREATER
            var initialBuffer = default(InlineArray16<TSource>);
            Span<TSource> initialBufferSpan = initialBuffer;
#else
            var initialBuffer = default(InlineArray16<TSource>);
            Span<TSource> initialBufferSpan = initialBuffer.AsSpan();
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

            var array = ArrayPool<TSource>.Shared.Rent(arrayBuilder.Count);
            arrayBuilder.CopyToAndClear(array);
            return (array, arrayBuilder.Count);

        }
    }
}
