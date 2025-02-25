namespace ZLinq;

partial class ValueEnumerableExtensions
{
    public static TSource[] ToArray<TEnumerable, TSource>(this TEnumerable source)
        where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        try
        {
            if (source.TryGetSpan(out var span))
            {
                return span.ToArray(); // fastest copy
            }
            else if (source.TryGetNonEnumeratedCount(out var count))
            {
                var i = 0;
                var array = new TSource[count];

                while (source.TryGetNext(out var item))
                {
                    array[i++] = item;
                }

                return array;
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

                    return arrayBuilder.ToArray();
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
