namespace ZLinq.Internal;

// Intending to use internal, but making it public so it can be used as a helper when creating own operators.
public static class EnumeratorHelper
{
    public static bool TryGetSliceRange(int sourceLength, Index offset, int destinationLength, out int start, out int count)
    {
        var sourceOffset = offset.GetOffset(sourceLength);
        if (unchecked((uint)sourceOffset) < sourceLength)
        {
            start = sourceOffset;
            count = Math.Min(sourceLength - sourceOffset, destinationLength);
            return true;
        }

        start = 0;
        count = 0;
        return false;
    }

    public static bool TryGetSlice<T>(ReadOnlySpan<T> source, Index offset, int destinationLength, out ReadOnlySpan<T> slice)
    {
        var sourceOffset = offset.GetOffset(source.Length);
        if (unchecked((uint)sourceOffset) < source.Length) // zero length is not allowed.
        {
            var count = Math.Min(source.Length - sourceOffset, destinationLength);
            slice = source.Slice(sourceOffset, count);
            return true;
        }

        slice = default;
        return false;
    }

    public static bool TryConsumeGetAt<TEnumerator, T>(ref TEnumerator enumerator, Index offset, out T value)
        where TEnumerator : struct, IValueEnumerator<T>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        if (offset.IsFromEnd)
        {
            if (offset.Value == 1)
            {
                return TryConsumeGetLast(ref enumerator, out value);
            }
            else
            {
                return TryConsumeGetFromLast(ref enumerator, offset.Value, out value);
            }
        }
        else
        {
            if (offset.Value == 0)
            {
                return TryConsumeGetFirst(ref enumerator, out value);
            }
            else
            {
                return TryConsumeGetAt(ref enumerator, offset.Value, out value);
            }
        }
    }

    static bool TryConsumeGetFirst<TEnumerator, T>(ref TEnumerator enumerator, out T first)
       where TEnumerator : struct, IValueEnumerator<T>
#if NET9_0_OR_GREATER
       , allows ref struct
#endif
    {
        if (enumerator.TryGetNext(out var item))
        {
            first = item;
            return true;
        }

        first = default!;
        return false;
    }

    static bool TryConsumeGetAt<TEnumerator, T>(ref TEnumerator enumerator, int index, out T value)
       where TEnumerator : struct, IValueEnumerator<T>
#if NET9_0_OR_GREATER
       , allows ref struct
#endif
    {
        var i = 0;
        while (enumerator.TryGetNext(out var item))
        {
            if (i++ == index)
            {
                value = item;
                return true;
            }
        }
        value = default!;
        return false;
    }

    static bool TryConsumeGetLast<TEnumerator, T>(ref TEnumerator enumerator, out T last)
       where TEnumerator : struct, IValueEnumerator<T>
#if NET9_0_OR_GREATER
       , allows ref struct
#endif
    {
        if (enumerator.TryGetNext(out var item))
        {
            while (enumerator.TryGetNext(out var next))
            {
                item = next;
            }
            last = item;
            return true;
        }

        last = default!;
        return false;
    }

    static bool TryConsumeGetFromLast<TEnumerator, T>(ref TEnumerator enumerator, int indexFromEnd, out T value)
       where TEnumerator : struct, IValueEnumerator<T>
#if NET9_0_OR_GREATER
       , allows ref struct
#endif
    {
        if (indexFromEnd == 0)
        {
            value = default!;
            return false;
        }

        using var q = new ValueQueue<T>(4);
        while (enumerator.TryGetNext(out var current))
        {
            if (q.Count == indexFromEnd)
            {
                q.Dequeue();
            }
            q.Enqueue(current);
        }

        if (q.Count == indexFromEnd)
        {
            value = q.Dequeue();
            return true;
        }

        value = default!;
        return false;
    }
}
