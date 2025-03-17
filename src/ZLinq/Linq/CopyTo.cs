namespace ZLinq;

partial class ValueEnumerableExtensions
{
    public static void CopyTo<TEnumerator, T>(in this ValueEnumerable<TEnumerator, T> source, List<T> list)
        where TEnumerator : struct, IValueEnumerator<T>
#if NET9_0_OR_GREATER
    , allows ref struct
#endif
    {
        using (var enumerator = source.Enumerator)
        {
            if (enumerator.TryGetSpan(out var span))
            {
#if NET8_0_OR_GREATER
                CollectionsMarshal.SetCount(list, span.Length);
                span.CopyTo(CollectionsMarshal.AsSpan(list));
                return;
#else
                // NETSTANDARD2_1 has no SetCount(Count + Grow)
                // only use no needs Grow
                var listSpan = CollectionsMarshal.UnsafeAsRawSpan(list);
                if (span.Length < listSpan.Length)
                {
                    if (list.Count < span.Length)
                    {
                        CollectionsMarshal.UnsafeSetCount(list, span.Length);
                        listSpan.Slice(span.Length).Clear(); // clear rest
                    }
                    span.CopyTo(listSpan);
                    return;
                }
#endif
            }

            if (enumerator.TryGetNonEnumeratedCount(out var length))
            {
#if NET8_0_OR_GREATER
                CollectionsMarshal.SetCount(list, length);
                var listSpan = CollectionsMarshal.AsSpan(list);
                if (enumerator.TryCopyTo(listSpan))
                {
                    return;
                }

                var i = 0;
                while (enumerator.TryGetNext(out var current))
                {
                    listSpan[i++] = current;
                }
                return;
#else
            var listSpan = CollectionsMarshal.AsSpan(list);
            if (length < listSpan.Length)
            {
                if (list.Count < length)
                {
                    CollectionsMarshal.UnsafeSetCount(list, length);
                    listSpan.Slice(length).Clear(); // clear rest
                }

                if (enumerator.TryCopyTo(listSpan))
                {
                    return;
                }

                var i = 0;
                while (enumerator.TryGetNext(out var current))
                {
                    listSpan[i++] = current;
                }
                return;
            }
#endif
            }

            {
                var i = 0;
                var count = list.Count;
                while (i < count)
                {
                    if (enumerator.TryGetNext(out var item))
                    {
                        list[i++] = item;
                    }
                    else
                    {
                        list.RemoveRange(i, count - i);
                        return;
                    }
                }

                while (enumerator.TryGetNext(out var item))
                {
                    i++;
                    list.Add(item);
                }
                return;
            }
        }
    }

    public static int CopyTo<TEnumerator, T>(in this ValueEnumerable<TEnumerator, T> source, Span<T> dest)
        where TEnumerator : struct, IValueEnumerator<T>
#if NET9_0_OR_GREATER
    , allows ref struct
#endif
    {
        var enumerator = source.Enumerator;
        try
        {
            if (enumerator.TryGetSpan(out var src))
            {
                src.CopyTo(dest);
                return src.Length;
            }
            else if (enumerator.TryGetNonEnumeratedCount(out var count))
            {
                if (enumerator.TryCopyTo(dest.Slice(0, count)))
                {
                    return count;
                }
            }

            return SlowCopyTo(ref enumerator, dest);
        }
        finally
        {
            enumerator.Dispose();
        }
    }

    /// <summary>
    /// CopyTo but non optimized-path(use for TryGetSpan is not implemented)
    /// </summary>
    static int SlowCopyTo<TEnumerator, T>(ref TEnumerator source, Span<T> dest)
        where TEnumerator : struct, IValueEnumerator<T>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        var i = 0;
        while (source.TryGetNext(out var current))
        {
            dest[i++] = current;
        }
        return i;
    }

    static int UnsafeSlowCopyTo<TEnumerator, T>(ref TEnumerator source, ref T dest)
        where TEnumerator : struct, IValueEnumerator<T>
#if NET9_0_OR_GREATER
    , allows ref struct
#endif
    {
        var i = 0;
        while (source.TryGetNext(out var current))
        {
            Unsafe.Add(ref dest, i) = current;
            i++;
        }
        return i;
    }
}
