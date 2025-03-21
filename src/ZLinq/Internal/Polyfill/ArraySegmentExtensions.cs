#if NETSTANDARD2_0

using System;
using System.Collections.Generic;
using System.Text;

namespace ZLinq.Internal;

internal static class ArraySegmentExtensions
{
    public static T GetAt<T>(this ArraySegment<T> arraySegment, int index)
    {
        if ((uint)index >= (uint)arraySegment.Count)
        {
            Throws.ArgumentOutOfRange(nameof(index));
        }
        return arraySegment.Array[arraySegment.Offset + index];
    }

    public static IEnumerator<T> GetEnumerator<T>(this ArraySegment<T> arraySegment)
    {
        var array = arraySegment.Array;
        if (array == null)
        {
            yield break;
        }

        var offset = arraySegment.Offset;
        var count = arraySegment.Count;
        var to = offset + count;
        for (int i = offset; i < to; i++)
        {
            yield return array[i];
        }
    }
}

#endif
