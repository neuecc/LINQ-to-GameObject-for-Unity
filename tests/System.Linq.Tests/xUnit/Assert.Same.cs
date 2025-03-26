using System.Runtime.CompilerServices;
using ZLinq;
using ZLinq.Linq;

namespace ZLinq.Tests;


/// <summary>
/// Assert.Same/NotSame rerating test is not supported.
/// </summary>
public static partial class Assert
{
    internal static void Same<TEnumerator, T>(
        ValueEnumerable<TEnumerator, T> expected,
        ValueEnumerable<TEnumerator, T> actual)
        where TEnumerator : struct, IValueEnumerator<T>
    {
        throw new NotSupportedException();
    }

    internal static void Same<TEnumerator, T>(
        IEnumerable<T> expected,
        ValueEnumerable<TEnumerator, T> actual)
        where TEnumerator : struct, IValueEnumerator<T>
    {
        throw new NotSupportedException();
    }

    internal static void Same(
        ValueEnumerable<FromEnumerable<int>, int> expected,
        ValueEnumerable<OfType<FromEnumerable<int>, int, int>, int> actual)
    {
        throw new NotSupportedException();
    }

    internal static void Same(
        ValueEnumerable<Select<FromEnumerable<int>, int, int>, int> e,
        ValueEnumerable<OfType<Select<FromEnumerable<int>, int, int>, int, int>, int> valueEnumerable)
    {
        throw new NotImplementedException();
    }
}
