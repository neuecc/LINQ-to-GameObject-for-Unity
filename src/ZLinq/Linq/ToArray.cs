namespace ZLinq;

partial class ValueEnumerableExtensions
{
    public static TSource[] ToArray<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source)
       where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        using var enumerator = source.Enumerator;

        if (enumerator.TryGetNonEnumeratedCount(out var count))
        {
            if (count == 0)
            {
                return Array.Empty<TSource>();
            }

            var array = GC.AllocateUninitializedArray<TSource>(count);

            if (enumerator.TryCopyTo(array.AsSpan(), 0))
            {
                return array;
            }

            var i = 0;
            while (enumerator.TryGetNext(out var item))
            {
                array[i++] = item;
            }

            return array;
        }
        else
        {
            using var arrayBuilder = new SegmentedArrayBuilder<TSource>();

            while (enumerator.TryGetNext(out var item))
            {
                arrayBuilder.Add(item);
            }

            return arrayBuilder.ToArray();
        }
    }
}
