using System.Runtime.CompilerServices;
using ZLinq;
using ZLinq.Linq;

namespace ZLinq.Tests;

public static partial class Assert
{
    internal static void Equal<TEnumerator, T>(
        IEnumerable<T> expected,
        ValueEnumerable<TEnumerator, T> actual)
        where TEnumerator : struct, IValueEnumerator<T>
    {
        var results = actual.ToArray();
        Xunit.Assert.Equal(expected, results);
    }

    internal static void Equals<T>(
        T[] expected,
        ValueEnumerable<Join<FromArray<T>, FromEnumerable<T>, T, T, T, T>, T> actual)
    {
        Xunit.Assert.Equal(expected, actual.ToArray());
    }

    internal static void Equal<T>(
       IEnumerable<T> expected,
       ValueEnumerable<Union<FromArray<T>, FromEnumerable<T>, T>, T> actual,
       IEqualityComparer<T> comparer)
    {
        var results2 = actual.ToArray();
        Xunit.Assert.Equal(expected, actual.ToArray(), comparer);
    }

    internal static void Equal<T>(
        ValueEnumerable<Reverse<FromEnumerable<T>, T>, T> expected,
        IEnumerable<T> actual)
    {
        Xunit.Assert.Equal(expected.ToArray(), actual);
    }

    internal static void Equal<T>(
        ValueEnumerable<Reverse<FromEnumerable<T>, T>, T> expected,
        ValueEnumerable<OrderBy<Select<Reverse<FromEnumerable<T>, T>, T, T>, T, T>, T> actual)
    {
        Xunit.Assert.Equal(expected.ToArray(), actual.ToArray());
    }

    internal static void Equal<T>(
        ValueEnumerable<SkipLast<FromEnumerable<T>, T>, T> expected,
        IEnumerable<T> actual)
    {
        Xunit.Assert.Equal(expected.ToArray(), actual.ToArray());
    }

    internal static void Equal<T>(
        ValueEnumerable<TakeLast<FromEnumerable<T>, T>, T> expected,
        IEnumerable<T> actual)
    {
        Xunit.Assert.Equal(expected.ToArray(), actual.ToArray()); ;
    }

    internal static void Equal<T>(
        ValueEnumerable<Take<OrderBy<FromArray<T>, T, T>, T>, T> expected,
        ValueEnumerable<Take<Take<OrderBy<FromArray<T>, T, T>, T>, T>, T> actual)
    {
        Xunit.Assert.Equal(expected.ToArray(), actual.ToArray());
    }

    internal static void Equal<T>(
        ValueEnumerable<Skip<FromArray<T>, T>, T> expected,
        ValueEnumerable<Skip<FromEnumerable<T>, T>, T> actual)
    {
        Xunit.Assert.Equal(expected.ToArray(), actual.ToArray());
    }

    internal static void Equal<T>(
        ValueEnumerable<Take<FromArray<T>, T>, T> expected,
        ValueEnumerable<Take<FromEnumerable<T>, T>, T> actual)
    {
        Xunit.Assert.Equal(expected.ToArray(), actual.ToArray());
    }

    internal static void Equal<T>(
        ValueEnumerable<Skip<FromArray<T>, T>, T> expected,
        ValueEnumerable<Skip<Reverse<FromArray<T>, T>, T>, T> actual)
    {
        Xunit.Assert.Equal(expected.ToArray(), actual.ToArray());
    }

    internal static void Equal<T>(
        ValueEnumerable<Take<FromArray<T>, T>, T> expected,
        ValueEnumerable<Take<Reverse<FromArray<T>, T>, T>, T> actual)
    {
        Xunit.Assert.Equal(expected.ToArray(), actual.ToArray());
    }

    internal static void Equal<T>(
        ValueEnumerable<Skip<FromArray<T>, T>, T> expected,
        ValueEnumerable<Skip<Reverse<FromEnumerable<T>, T>, T>, T> actual)
    {
        Xunit.Assert.Equal(expected.ToArray(), actual.ToArray());
    }

    internal static void Equal<T>(
        ValueEnumerable<Take<FromArray<T>, T>, T> expected,
        ValueEnumerable<Take<Reverse<FromEnumerable<T>, T>, T>, T> actual)
    {
        Xunit.Assert.Equal(expected.ToArray(), actual.ToArray());
    }

    internal static void Equal(
        ValueEnumerable<Select<FromArray<int>, int, int>, int> expected,
        IEnumerable<int> actual)
    {
        Xunit.Assert.Equal(expected.ToArray(), actual.ToArray());
    }

    internal static void Equal(
        ValueEnumerable<Select<FromEnumerable<IGrouping<string, int>>, IGrouping<string, int>, IGrouping<string, int>>, IGrouping<string, int>> expected,
        IGrouping<string, int>[] actual)
    {
        Xunit.Assert.Equal(expected.ToArray(), actual.ToArray());
    }

    internal static void Equal<T>(
        ValueEnumerable<Take<FromArray<T>, T>, T> expected,
        ValueEnumerable<Where<FromArray<T>, T>, T> actual)
    {
        Xunit.Assert.Equal(expected.ToArray(), actual.ToArray());
    }
    internal static void Equal<T>(
        ValueEnumerable<Skip<FromArray<T>, T>, T> expected,
        ValueEnumerable<Where<FromArray<T>, T>, T> actual)
    {
        Xunit.Assert.Equal(expected.ToArray(), actual.ToArray());
    }

    internal static void Equal<T>(
        ValueEnumerable<Take<FromArray<T>, T>, T> expected,
        ValueEnumerable<Where2<FromArray<T>, T>, T> actual)
    {
        Xunit.Assert.Equal(expected.ToArray(), actual.ToArray());
    }

    internal static void Equal<T>(
        ValueEnumerable<Skip<FromArray<T>, T>, T> expected,
        ValueEnumerable<Where2<FromArray<T>, T>, T> actual)
    {
        Xunit.Assert.Equal(expected.ToArray(), actual.ToArray());
    }

    internal static void Equal<T>(
        ValueEnumerable<Append<Select<FromArray<T>, T, T>, T>, T> expected,
        ValueEnumerable<Concat<Select<FromArray<T>, T, T>, FromEnumerable<T>, T>, T> actual)
    {
        Xunit.Assert.Equal(expected.ToArray(), actual.ToArray());
    }

    internal static void Equal<T>(
        ValueEnumerable<Prepend<Select<FromArray<T>, T, T>, T>, T> expected,
        ValueEnumerable<Concat<FromArray<T>, Select<FromArray<T>, T, T>, T>, T> actual)
    {
        Xunit.Assert.Equal(expected.ToArray(), actual.ToArray());
    }

    internal static void Equal<T>(
        ValueEnumerable<Append<FromEnumerable<T>, T>, T> expected,
        ValueEnumerable<Append<Append<FromEnumerable<T>, T>, T>, T> actual)
    {
        Xunit.Assert.Equal(expected.ToArray(), actual.ToArray());
    }

    internal static void Equal<T>(
        ValueEnumerable<Prepend<Select<FromList<T>, T, T>, T>, T> expected,
        ValueEnumerable<Prepend<FromList<T>, T>, T> actual)
    {
        Xunit.Assert.Equal(expected.ToArray(), actual.ToArray());
    }

    internal static void Equal<T>(
        ValueEnumerable<Chunk<FromEnumerable<T>, T>, T[]> expected,
        ValueEnumerable<Chunk<FromEnumerable<T>, T>, T[]> actual)
    {
        Xunit.Assert.Equal(expected.ToArray(), actual.ToArray());
    }

    internal static void Equal<T>(
        ValueEnumerable<Intersect<Select<FromArray<T>, T, T>, Select<FromArray<T>, T, T>, T>, T> expected,
        ValueEnumerable<Intersect<Select<FromArray<T>, T, T>, Select<FromArray<T>, T, T>, T>, T> actual)
    {
        Xunit.Assert.Equal(expected.ToArray(), actual.ToArray());
    }

    internal static void Equal<T>(
        IEnumerable<T> expected,
        ValueEnumerable<IntersectBy<FromEnumerable<T>, FromEnumerable<T>, T, T>, T> actual)
    {
        Xunit.Assert.Equal(expected.ToArray(), actual.ToArray());
    }

    internal static void Equal<TSource, TKey>(
        IEnumerable<TSource> expected,
        ValueEnumerable<IntersectBy<FromEnumerable<TSource>, FromEnumerable<TKey>, TSource, TKey>, TSource> actual)
    {
        Xunit.Assert.Equal(expected, actual.ToArray());
    }

    internal static void Equal<T>(
        IEnumerable<T> expected,
        ValueEnumerable<Intersect<FromEnumerable<T>, FromEnumerable<T>, T>, T> actual)
    {
        Xunit.Assert.Equal(expected, actual.ToArray());
    }
}
