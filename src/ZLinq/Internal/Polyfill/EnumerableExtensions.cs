#if NETSTANDARD2_0 || NETSTANDARD2_1

using System;
using System.Collections.Generic;
using System.Text;

namespace ZLinq.Internal;

internal static class EnumerableExtensions
{
    internal static bool TryGetNonEnumeratedCount<T>(this IEnumerable<T> source, out int count)
    {
        if (source is ICollection<T> c)
        {
            count = c.Count;
            return true;
        }
        else if (source is IReadOnlyCollection<T> rc)
        {
            count = rc.Count;
            return true;
        }
        count = 0;
        return false;
    }
}


#endif
