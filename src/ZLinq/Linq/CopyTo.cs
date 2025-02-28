namespace ZLinq;

partial class ValueEnumerableExtensions
{
    // others(Span, etc...)

    public static int CopyTo<TEnumerable, T>(this TEnumerable source, List<T> list)
        where TEnumerable : struct, IValueEnumerable<T>
#if NET9_0_OR_GREATER
    , allows ref struct
#endif
    {
        if (source.TryGetSpan(out var span))
        {
#if NET8_0_OR_GREATER
            CollectionsMarshal.SetCount(list, span.Length);
            span.CopyTo(CollectionsMarshal.AsSpan(list));
            return span.Length;

#else
            // NETSTANDARD2_1 has no SetCount(Count + Grow)
            // only use no needs Grow
            var listSpan = CollectionsMarshal.UnsafeAsRawSpan(list);
            if (span.Length < listSpan.Length)
            {
                if (list.Count < span.Length)
                {
                    CollectionsMarshal.UnsafeSetCount(list, span.Length);
                }
                span.CopyTo(listSpan);
                return span.Length;
            }
#endif
        }

        if (source.TryGetNonEnumeratedCount(out var length))
        {
#if NET8_0_OR_GREATER
            CollectionsMarshal.SetCount(list, length);
            var listSpan = CollectionsMarshal.AsSpan(list);
            var i = 0;
            while (source.TryGetNext(out var current))
            {
                listSpan[i++] = current;
            }
            return i;
#else
            var listSpan = CollectionsMarshal.AsSpan(list);
            if (span.Length < listSpan.Length)
            {
                if (list.Count < span.Length)
                {
                    CollectionsMarshal.UnsafeSetCount(list, span.Length);
                }

                var i = 0;
                while (source.TryGetNext(out var current))
                {
                    listSpan[i++] = current;
                }
                return i;
            }
#endif
        }

        {
            var i = 0;
            var count = list.Count;
            try
            {
                while (i < count)
                {
                    if (source.TryGetNext(out var item))
                    {
                        list[i++] = item;
                    }
                    else
                    {
                        return i;
                    }
                }

                while (source.TryGetNext(out var item))
                {
                    i++;
                    list.Add(item);
                }
                return i;
            }
            finally
            {
                source.Dispose();
            }
        }
    }

    public static int CopyTo<TEnumerable, T>(this TEnumerable source, Span<T> dest)
        where TEnumerable : struct, IValueEnumerable<T>
#if NET9_0_OR_GREATER
    , allows ref struct
#endif
    {
        if (source.TryGetSpan(out var src))
        {
            src.CopyTo(dest);
            return src.Length;
        }
        else
        {
            return SlowCopyTo(ref source, dest);
        }
    }

    /// <summary>
    /// CopyTo but non optimized-path(use for TryGetSpan is not implemented)
    /// </summary>
    static int SlowCopyTo<TEnumerable, T>(ref TEnumerable source, Span<T> dest)
        where TEnumerable : struct, IValueEnumerable<T>
#if NET9_0_OR_GREATER
    , allows ref struct
#endif
    {
        try
        {
            var i = 0;
            while (source.TryGetNext(out var current))
            {
                dest[i++] = current;
            }
            return i;
        }
        finally
        {
            source.Dispose();
        }
    }

    static int UnsafeSlowCopyTo<TEnumerable, T>(ref TEnumerable source, ref T dest)
        where TEnumerable : struct, IValueEnumerable<T>
#if NET9_0_OR_GREATER
    , allows ref struct
#endif
    {
        try
        {
            var i = 0;
            while (source.TryGetNext(out var current))
            {
                Unsafe.Add(ref dest, i) = current;
                i++;
            }
            return i;
        }
        finally
        {
            source.Dispose();
        }
    }
}
