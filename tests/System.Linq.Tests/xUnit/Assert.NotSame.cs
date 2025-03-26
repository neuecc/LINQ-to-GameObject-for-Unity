using System.Runtime.CompilerServices;
using ZLinq;
using ZLinq.Linq;

namespace ZLinq.Tests;

/// <summary>
/// Assert.Same/NotSame rerating test is not supported.
/// </summary>
public static partial class Assert
{
    internal static void NotSame<TEnumerator, T>(
        ValueEnumerable<TEnumerator, T> expected,
        ValueEnumerable<TEnumerator, T> actual)
        where TEnumerator : struct, IValueEnumerator<T>
    {
        throw new NotSupportedException();
    }

    internal static void NotSame<TEnumerator, T>(
        IEnumerable<T> expected,
        ValueEnumerable<TEnumerator, T> actual)
        where TEnumerator : struct, IValueEnumerator<T>
    {
        throw new NotSupportedException();
    }

    internal static void NotSame<T>(
        ValueEnumerable<Select<FromEnumerable<T>, T, T>, T> expected,
        ValueEnumerable<OfType<Select<FromEnumerable<T>, T, T>, T, T>, T> actual)
    {
        throw new NotSupportedException();
    }

    internal static void NotSame<TEnumerator, TRange, TSelect, TOfType>(
        ValueEnumerable<Select<TEnumerator, TRange, TSelect>, TSelect> expected,
        ValueEnumerable<OfType<Select<TEnumerator, TRange, TSelect>, TSelect, TOfType>, TOfType> actual)
            where TEnumerator : struct, IValueEnumerator<TRange>
    {
        throw new NotSupportedException();
    }
}
