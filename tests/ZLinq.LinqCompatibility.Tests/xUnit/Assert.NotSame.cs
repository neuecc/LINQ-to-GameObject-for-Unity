using System.Runtime.CompilerServices;
using ZLinq;
using ZLinq.Linq;

namespace ZLinq.Tests;

public static partial class Assert
{
    internal static void NotSame<TEnumerator, T>(
      ValueEnumerable<TEnumerator, T> expected,
      ValueEnumerable<TEnumerator, T> actual)
      where TEnumerator : struct, IValueEnumerator<T>
#if NET9_0_OR_GREATER
, allows ref struct
#endif
    {
        throw new NotSupportedException();
    }

    internal static void NotSame<TEnumerator, T>(
        IEnumerable<T> expected,
        ValueEnumerable<TEnumerator, T> actual)
        where TEnumerator : struct, IValueEnumerator<T>
#if NET9_0_OR_GREATER
    , allows ref struct
#endif
    {
        throw new NotSupportedException();
    }

    internal static void NotSame<TEnumerator, TRange, TSelect, TOfType>(
        ValueEnumerable<Select<TEnumerator, TRange, TSelect>, TSelect> expected,
        ValueEnumerable<OfType<Select<TEnumerator, TRange, TSelect>, TSelect, TOfType>, TOfType> actual)
            where TEnumerator : struct, IValueEnumerator<TRange>
#if NET9_0_OR_GREATER
    , allows ref struct
#endif
    {
        throw new NotSupportedException();
    }
}
