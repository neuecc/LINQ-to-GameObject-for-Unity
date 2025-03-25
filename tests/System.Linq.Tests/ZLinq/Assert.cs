using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// global namespace

public static class Assert
{
    public static void All<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.NonPublicFields | DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties | DynamicallyAccessedMemberTypes.PublicMethods)] T>(
        IEnumerable<T> collection,
        Action<T> action)
    {
        Xunit.Assert.All(collection, action);
    }

    public static void Equal<T>(T expected, T actual)
    {
        Xunit.Assert.Equal(expected, actual);
    }

    public static void Equal(int expected, int actual)
    {
        Xunit.Assert.Equal(expected, actual);
    }
    public static void Equal(bool expected, bool actual)
    {
        Xunit.Assert.Equal(expected, actual);
    }

    public static void Equal<T>(IEnumerable<T> expected, IEnumerable<T> actual)
    {
        Xunit.Assert.Equal(expected, actual);
    }

    public static void Equal<TEnumerator, T>(ValueEnumerable<TEnumerator, T> actual, IEnumerable<T> expected)
           where TEnumerator : struct, IValueEnumerator<T>
        //            , allows ref struct
    {
        Xunit.Assert.Equal(expected, actual.ToArray());
    }

    public static void Equal<TEnumerator, T>(ValueEnumerable<TEnumerator, T> actual, ValueEnumerable<TEnumerator, T> expected)
           where TEnumerator : struct, IValueEnumerator<T>
        //            , allows ref struct
    {
        Xunit.Assert.Equal(expected.ToArray(), actual.ToArray());
    }

    public static void Equal<TEnumerator, T>(IEnumerable<T> expected, ValueEnumerable<TEnumerator, T> actual)
           where TEnumerator : struct, IValueEnumerator<T>
        //            , allows ref struct
    {
        Xunit.Assert.Equal(expected, actual.ToArray());
    }

    public static void NotEqual<T>(T expected, T actual)
    {
        Xunit.Assert.NotEqual(expected, actual);
    }

    public static void NotEqual<T>(IEnumerable<T> expected, IEnumerable<T> actual)
    {
        Xunit.Assert.NotEqual(expected, actual);
    }

    public static void NotEqual<TEnumerator, T>(IEnumerable<T> expected, ValueEnumerable<TEnumerator, T> actual)
           where TEnumerator : struct, IValueEnumerator<T>
        //            , allows ref struct
    {
        Xunit.Assert.NotEqual(expected, actual.ToArray());
    }

    public static void Empty<T>(IEnumerable<T> actual)
    {
        Xunit.Assert.Empty(actual);
    }

    public static void Empty<TEnumerator, T>(ValueEnumerable<TEnumerator, T> actual)
           where TEnumerator : struct, IValueEnumerator<T>
        //            , allows ref struct
    {
        Xunit.Assert.Empty(actual.ToArray());
    }

    public static void False(bool actual)
    {
        Xunit.Assert.False(actual);
    }

    public static void True(bool actual, string msg = null)
    {
        Xunit.Assert.True(actual, msg);
    }

    public static void Throws<T>(Action test)
        where T : Exception
    {
        Xunit.Assert.Throws<T>(test);
    }

    public static void Throws<T>(string p, Func<object> test)
        where T : ArgumentException
    {
        Xunit.Assert.Throws<T>(p, test);
    }

    public static void Null<T>(T value)
    {
        Xunit.Assert.Null(value);
    }

    public static void NotNull<T>(T value)
    {
        Xunit.Assert.NotNull(value);
    }

    public static void InRange<T>(T value, T low, T high)
        where T : IComparable
    {
        Xunit.Assert.InRange(value, low, high);
    }

    public static void Same(object x, object y)
    {
        Xunit.Assert.Same(x, y);
    }

    public static void NotSame(object x, object y)
    {
        Xunit.Assert.NotSame(x, y);
    }

    public static void Fail(string msg)
    {
        Xunit.Assert.Fail(msg);
    }

    public static void All<TEnumerator, T>(ValueEnumerable<TEnumerator, T> actual, Action<T> action)
                   where TEnumerator : struct, IValueEnumerator<T>
        //            , allows ref struct
    {
        Xunit.Assert.All(actual.ToArray(), action);
    }

    public static T IsAssignableFrom<T>(object @object)
    {
        return Xunit.Assert.IsAssignableFrom<T>(@object);
    }

    public static T IsType<T>(object @object)
    {
        return Xunit.Assert.IsType<T>(@object);
    }
    public static void Single<TEnumerator, T>(ValueEnumerable<TEnumerator, T> actual, T v)
           where TEnumerator : struct, IValueEnumerator<T>
        //            , allows ref struct
    {
        Xunit.Assert.Single(actual.ToArray(), v);
    }
}
