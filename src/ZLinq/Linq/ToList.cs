namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static List<TSource> ToList<TEnumerable, TSource>(this TEnumerable source)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            try
            {
                if (source.TryGetSpan(out var span))
                {
                    var array = GC.AllocateUninitializedArray<TSource>(span.Length);
                    span.CopyTo(array);
                    return ListMarshal.AsList(array);
                }

                if (source.TryGetNonEnumeratedCount(out var count))
                {
                    var list = new List<TSource>(count);
#if NET8_0_OR_GREATER
                    CollectionsMarshal.SetCount(list, count);
#else
                    CollectionsMarshal.UnsafeSetCount(list, count);
#endif
                    var dest = CollectionsMarshal.AsSpan(list);

                    if (source.TryCopyTo(dest))
                    {
                        return list;
                    }
                    else
                    {
                        UnsafeSlowCopyTo(ref source, ref MemoryMarshal.GetReference(dest));
                        return list;
                    }
                }

                var arrayBuilder = new SegmentedArrayBuilder<TSource>();
                try
                {
                    while (source.TryGetNext(out var item))
                    {
                        arrayBuilder.Add(item);
                    }

                    var array = GC.AllocateUninitializedArray<TSource>(arrayBuilder.Count);
                    arrayBuilder.CopyTo(array);
                    return ListMarshal.AsList(array);
                }
                finally
                {
                    arrayBuilder.Dispose();
                }
            }
            finally
            {
                source.Dispose();
            }
        }
    }
}
