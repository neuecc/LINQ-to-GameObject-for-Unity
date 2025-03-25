namespace ZLinq;

partial class ValueEnumerableExtensions
{
    /// <summary>
    /// Unlike the semantics of normal CopyTo, this allows the destination to be smaller than the source.
    /// Returns the number of elements copied.
    /// </summary>
    public static int CopyTo<TEnumerator, T>(this ValueEnumerable<TEnumerator, T> source, Span<T> dest)
        where TEnumerator : struct, IValueEnumerator<T>
#if NET9_0_OR_GREATER
    , allows ref struct
#endif
    {
        using var enumerator = source.Enumerator;

        if (enumerator.TryGetNonEnumeratedCount(out var count))
        {
            if (enumerator.TryCopyTo(dest, 0))
            {
                return Math.Min(count, dest.Length);
            }
        }

        var i = 0;
        while (enumerator.TryGetNext(out var current))
        {
            dest[i++] = current;
            if (i == dest.Length)
            {
                return i;
            }
        }

        return i;
    }

    /// <summary>
    /// List is cleared and then filled with the elements of the source. Destination size is list.Count.
    /// </summary>
    public static void CopyTo<TEnumerator, T>(this ValueEnumerable<TEnumerator, T> source, List<T> list)
        where TEnumerator : struct, IValueEnumerator<T>
#if NET9_0_OR_GREATER
    , allows ref struct
#endif
    {
        ArgumentNullException.ThrowIfNull(list);

        list.Clear(); // clear before fill.

        using var enumerator = source.Enumerator;

        if (enumerator.TryGetNonEnumeratedCount(out var count))
        {
#if NET8_0_OR_GREATER
            CollectionsMarshal.SetCount(list, count); // expand internal T[] buffer
#else
            if (list.Capacity < count)
            {
                list.Capacity = count; // Grow only buffer is smaller.
            }
            CollectionsMarshal.UnsafeSetCount(list, count); // only set count
#endif

            var span = CollectionsMarshal.AsSpan(list);
            if (enumerator.TryCopyTo(span, 0))
            {
                return;
            }

            var i = 0;
            while (enumerator.TryGetNext(out var current))
            {
                span[i++] = current;
            }
        }
        else
        {
            while (enumerator.TryGetNext(out var item))
            {
                list.Add(item);
            }
        }
    }
}
