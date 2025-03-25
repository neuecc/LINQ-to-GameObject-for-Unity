using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using ZLinq;
using ZLinq.Linq;

namespace ZLinq.Tests;

public static partial class Assert
{
    public static void Equal<T>(T? expected, T? actual)
        => Xunit.Assert.Equal<T>(expected, actual);

    internal static void Equal<T>(T? expected, T? actual, IEqualityComparer<T>? comparer)
        => Xunit.Assert.Equal(expected, actual, comparer);

    public static void Equal<T>(IEnumerable<T>? expected, IAsyncEnumerable<T>? actual)
        => Xunit.Assert.Equal<T>(expected, actual);

    public static void Equal<T>(IEnumerable<T> expected, IEnumerable<T> actual)
        => Xunit.Assert.Equal<T>(expected, actual);

    public static void Equal<T>(IEnumerable<T> expected, IEnumerable<T> actual, IEqualityComparer<T> comparer)
        => Xunit.Assert.Equal(expected, actual, comparer);

    public static void All<T>(IEnumerable<T> collection, Action<T> action)
        => Xunit.Assert.All(collection, action);

    public static void Throws<T>(Action testCode) where T : Exception
        => Xunit.Assert.Throws<T>(testCode);

    public static void Throws<T>(string? paramName, Action testCode) where T : ArgumentException => Xunit.Assert.Throws<T>(paramName, testCode);

    public static void True(bool condition)
        => Xunit.Assert.True(condition);

    public static void False(bool condition)
        => Xunit.Assert.False(condition);

    public static void Null<T>(T value)
        => Xunit.Assert.Null(value);

    public static void NotNull<T>(T? value)
        => Xunit.Assert.NotNull(value);

    public static void Empty<T>(IEnumerable<T> collection)
        => Xunit.Assert.Empty(collection);

    public static void IsAssignableFrom<T>(T? source)
        => Xunit.Assert.IsAssignableFrom<T>(source);

    public static void IsAssignableFrom<T>(IEnumerable<T> enumerable)
        => Xunit.Assert.IsAssignableFrom<T>(enumerable);

    internal static T IsAssignableFrom<T>(IEnumerable<int> enumerable)
        => Xunit.Assert.IsAssignableFrom<T>(enumerable);

    public static void NotEmpty<TKey, TElement>(IGrouping<TKey, TElement> group)
        => throw new NotImplementedException();

    public static void Superset<T>(ISet<T> originalSet, ISet<T> hashSet)
        => Xunit.Assert.Superset(originalSet, hashSet);

    public static void Subset<T>(ISet<T> originalSet, ISet<T>? hashSet)
        => Xunit.Assert.Subset(originalSet, hashSet);

    public static void Single<T>(IEnumerable<T> collection, T? expected)
        => Xunit.Assert.Single(collection, expected);

    public static void Fail(string? message)
        => Xunit.Assert.Fail(message);

    internal static void InRange(int actual, int low, int high)
        => Xunit.Assert.InRange(actual, low, high);

    internal static void InRange(int actual, double low, double high)
        => Xunit.Assert.InRange((double)actual, low, high);

    internal static void IsType<T>(T type)
        => Xunit.Assert.IsType<T>(type);

    internal static void Contains<T>(T expected, IEnumerable<T> collection)
        => Xunit.Assert.Contains(expected, collection);

    internal static void DoesNotContain<T>(T expected, IEnumerable<T> collection)
        => Xunit.Assert.DoesNotContain(expected, collection);

    internal static void Equal(decimal expected, double actual)
        => Xunit.Assert.Equal(expected, (decimal)actual); // Compare by decimal

    internal static void Equal(long expected, double? actual)
        => Xunit.Assert.Equal(expected, actual); // Compare by double

    internal static void Equal(decimal? expected, double? actual)
        => throw new NotSupportedException("See: https://github.com/Cysharp/ZLinq/issues/74");


    public static void Same(object? expected, object? actual)
        => throw new NotSupportedException();

    public static void Same<T>(IEnumerable<T> expected, IEnumerable<T> actual)
        => throw new NotSupportedException();
}
