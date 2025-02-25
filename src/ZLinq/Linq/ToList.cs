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
                    var list = new List<TSource>(span.Length);
#if NET8_0_OR_GREATER
                    CollectionsMarshal.SetCount(list, span.Length);
#else
                    CollectionsMarshal.UnsafeSetCount(list, span.Length);
#endif
                    span.CopyTo(CollectionsMarshal.AsSpan(list));
                    return list;
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
                    UnsafeSlowCopyTo(ref source, ref MemoryMarshal.GetReference(dest));
                    return list;
                }

                var arrayBuilder = new SegmentedArrayBuilder<TSource>();
                try
                {
                    while (source.TryGetNext(out var item))
                    {
                        arrayBuilder.Add(item);
                    }

                    count = arrayBuilder.Count;
                    var list = new List<TSource>(count);
#if NET8_0_OR_GREATER
                    CollectionsMarshal.SetCount(list, count);
#else
                    CollectionsMarshal.UnsafeSetCount(list, count);
#endif
                    var dest = CollectionsMarshal.AsSpan(list);
                    arrayBuilder.CopyTo(dest);
                    return list;
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
