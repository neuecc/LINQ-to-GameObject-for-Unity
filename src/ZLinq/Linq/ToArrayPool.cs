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
        if (enumerator.TryGetSpan(out var span))
        {
            var array = ArrayPool<TSource>.Shared.Rent(span.Length);
            span.CopyTo(array);
            return (array, span.Length);
        }
        else if (enumerator.TryGetNonEnumeratedCount(out var count))
        {
            if (count == 0)
            {
                return (Array.Empty<TSource>(), 0);
            }

            var i = 0;
            var array = ArrayPool<TSource>.Shared.Rent(count);

            if (enumerator.TryCopyTo(array.AsSpan(0, count)))
            {
                return (array, count);
            }

            while (enumerator.TryGetNext(out var item))
            {
                array[i++] = item;
            }

            return (array, i);
        }
        else
        {
            using var arrayBuilder = new SegmentedArrayBuilder<TSource>();
            while (enumerator.TryGetNext(out var item))
            {
                arrayBuilder.Add(item);
            }

            var array = ArrayPool<TSource>.Shared.Rent(arrayBuilder.Count);
            arrayBuilder.CopyTo(array);
            return (array, arrayBuilder.Count);

        }
    }
}
