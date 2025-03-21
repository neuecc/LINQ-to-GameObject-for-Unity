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
            var enumerator = source.Enumerator;
            try
            {
                if (enumerator.TryGetNonEnumeratedCount(out var count))
                {
                    var list = new List<TSource>(count);
#if NET8_0_OR_GREATER
                    CollectionsMarshal.SetCount(list, count);
#else
                    CollectionsMarshal.UnsafeSetCount(list, count);
#endif
                    var dest = CollectionsMarshal.AsSpan(list);

                    if (enumerator.TryCopyTo(dest))
                    {
                        return list;
                    }
                    else
                    {
                        UnsafeSlowCopyTo(ref enumerator, ref MemoryMarshal.GetReference(dest));
                        return list;
                    }
                }

                {
                    using var arrayBuilder = new SegmentedArrayBuilder<TSource>();
                    while (enumerator.TryGetNext(out var item))
                    {
                        arrayBuilder.Add(item);
                    }

                    var array = GC.AllocateUninitializedArray<TSource>(arrayBuilder.Count);
                    arrayBuilder.CopyTo(array);
                    return ListMarshal.AsList(array);
                }
            }
            finally
            {
                enumerator.Dispose();
            }
        }
    }
}
