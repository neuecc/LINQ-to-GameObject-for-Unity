using System.Buffers;

namespace ZLinq;

partial class ValueEnumerableExtensions
{
    public static (TSource[] Array, int Size) ToArrayPool<TEnumerable, TSource>(this TEnumerable source)
        where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        try
        {
            if (source.TryGetSpan(out var span))
            {
                var array = ArrayPool<TSource>.Shared.Rent(span.Length);
                span.CopyTo(array);
                return (array, span.Length);
            }
            else if (source.TryGetNonEnumeratedCount(out var count))
            {
                if (count == 0)
                {
                    return (Array.Empty<TSource>(), 0);
                }

                var i = 0;
                var array = ArrayPool<TSource>.Shared.Rent(count);

                if (source.TryCopyTo(array.AsSpan(0, count)))
                {
                    return (array, count);
                }

                while (source.TryGetNext(out var item))
                {
                    array[i++] = item;
                }

                return (array, i);
            }
            else
            {
                var arrayBuilder = new SegmentedArrayBuilder<TSource>();
                try
                {
                    while (source.TryGetNext(out var item))
                    {
                        arrayBuilder.Add(item);
                    }

                    var array = ArrayPool<TSource>.Shared.Rent(arrayBuilder.Count);
                    arrayBuilder.CopyTo(array);
                    return (array, arrayBuilder.Count);
                }
                finally
                {
                    arrayBuilder.Dispose();
                }
            }
        }
        finally
        {
            source.Dispose();
        }
    }
}
