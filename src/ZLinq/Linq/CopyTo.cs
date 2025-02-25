namespace ZLinq;

partial class ValueEnumerableExtensions
{
    // others(Span, etc...)

    public static void CopyTo<TEnumerable, T>(this TEnumerable source, List<T> list)
        where TEnumerable : struct, IValueEnumerable<T>
#if NET9_0_OR_GREATER
    , allows ref struct
#endif
    {
#if NET8_0_OR_GREATER
        if (source.TryGetSpan(out var span))
        {
            CollectionsMarshal.SetCount(list, span.Length);
            span.CopyTo(CollectionsMarshal.AsSpan(list));
            return;
        }
        else if (source.TryGetNonEnumeratedCount(out var length))
        {
            list.EnsureCapacity(length); // grow before writing
        }
#endif

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
                    return;
                }
            }

            while (source.TryGetNext(out var item))
            {
                list.Add(item);
            }
        }
        finally
        {
            source.Dispose();
        }
    }

    public static void CopyTo<TEnumerable, T>(this TEnumerable source, Span<T> dest)
        where TEnumerable : struct, IValueEnumerable<T>
#if NET9_0_OR_GREATER
    , allows ref struct
#endif
    {
        if (source.TryGetSpan(out var src))
        {
            src.CopyTo(dest);
        }
        else
        {
            SlowCopyTo(ref source, dest);
        }
    }

    /// <summary>
    /// CopyTo but non optimized-path(use for TryGetSpan is not implemented)
    /// </summary>
    static void SlowCopyTo<TEnumerable, T>(ref TEnumerable source, Span<T> dest)
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
        }
        finally
        {
            source.Dispose();
        }
    }

    static void UnsafeSlowCopyTo<TEnumerable, T>(ref TEnumerable source, ref T dest)
        where TEnumerable : struct, IValueEnumerable<T>
#if NET9_0_OR_GREATER
    , allows ref struct
#endif
    {
        try
        {
            while (source.TryGetNext(out var current))
            {
                dest = current;
                dest = Unsafe.Add(ref dest, 1);
            }
        }
        finally
        {
            source.Dispose();
        }
    }
}
