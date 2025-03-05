using System.Diagnostics.CodeAnalysis;

namespace ZLinq.Internal;

internal static class Throws
{
    [DoesNotReturn]
    public static void ArgumentOutOfRangeException(string paramName) => throw new ArgumentOutOfRangeException(paramName);

    [DoesNotReturn]
    public static T ArgumentOutOfRangeException<T>(string paramName) => throw new ArgumentOutOfRangeException(paramName);

    [DoesNotReturn]
    public static void MoreThanOneElementException() => throw new InvalidOperationException("Sequence contains more than one element"); // for Single

    [DoesNotReturn]
    public static void MoreThanOneMatchException() => throw new InvalidOperationException("Sequence contains more than one matching element"); // for single with predicate

    [DoesNotReturn]
    public static T NoElementsException<T>() => throw new InvalidOperationException("Sequence contains no elements");

    [DoesNotReturn]
    public static T NoMatchException<T>() => throw new InvalidOperationException("Sequence contains no matching element"); // for first, last, single with predicate
}
