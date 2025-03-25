using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using ZLinq;
using ZLinq.Linq;

namespace ZLinq.Tests;

public static partial class Assert
{
    internal static void Single<T>(
        ValueEnumerable<Append<FromEnumerable<T>, T>, T> collection,
        T expected)
    {
        Xunit.Assert.Single(collection.ToArray(), expected);
    }

    internal static void Single<T>(
        ValueEnumerable<Prepend<FromEnumerable<T>, T>, T> collection,
        T expected)
    {
        Xunit.Assert.Single(collection.ToArray(), expected);
    }
}
