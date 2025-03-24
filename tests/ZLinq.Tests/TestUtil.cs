using System.Runtime.CompilerServices;
using ZLinq.Linq;

namespace ZLinq.Tests;

public static class TestUtil
{
    public static void Throws<T>(Action expectedTestCode, Action actualTestCode)
        where T : Exception
    {
        var expectedException = Assert.Throws<T>(expectedTestCode);
        var actualException = Assert.Throws<T>(actualTestCode);
        expectedException.Message.ShouldBe(actualException.Message);
    }

    public static void NoThrow(Action expectedTestCode, Action actualTestCode)
    {
        expectedTestCode();
        actualTestCode();
    }

    public static IEnumerable<int> Empty()
    {
        yield break;
    }

    public static IEnumerable<T> Empty<T>()
    {
        yield break;
    }

    // hide source type to avoid Span optimization
    public static ValueEnumerable<FromEnumerable<T>, T> ToValueEnumerable<T>(this IEnumerable<T> source)
    {
        static IEnumerable<T> Core(IEnumerable<T> source)
        {
            foreach (var item in source)
            {
                yield return item;
            }
        }

        return Core(source).AsValueEnumerable();
    }

    // direct shortcut of enumerable.enumerator

    // Enumerator is struct so this shortcut is dangerous.
    //    public static bool TryGetNext<TEnumerator, T>(this ValueEnumerable<TEnumerator, T> enumerable, out T current)
    //        where TEnumerator : struct, IValueEnumerator<T>
    //#if NET9_0_OR_GREATER
    //        , allows ref struct
    //#endif
    //    {
    //        return enumerable.Enumerator.TryGetNext(out current);
    //    }

    public static bool TryGetNonEnumeratedCount<TEnumerator, T>(this ValueEnumerable<TEnumerator, T> enumerable, out int count)
        where TEnumerator : struct, IValueEnumerator<T>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        using var e = enumerable.Enumerator;
        return e.TryGetNonEnumeratedCount(out count);
    }

    public static bool TryGetSpan<TEnumerator, T>(this ValueEnumerable<TEnumerator, T> enumerable, out ReadOnlySpan<T> span)
        where TEnumerator : struct, IValueEnumerator<T>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        using var e = enumerable.Enumerator;
        return e.TryGetSpan(out span);
    }

    public static bool TryCopyTo<TEnumerator, T>(this ValueEnumerable<TEnumerator, T> enumerable, scoped Span<T> destination, Index offset = default)
        where TEnumerator : struct, IValueEnumerator<T>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        using var e = enumerable.Enumerator;
        return e.TryCopyTo(destination, offset);
    }

    public static void Dispose<TEnumerator, T>(this ValueEnumerable<TEnumerator, T> enumerable)
        where TEnumerator : struct, IValueEnumerator<T>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        using var e = enumerable.Enumerator;
        e.Dispose();
    }

    public static ValueEnumerable<TEnumerator, T> AsValueEnumerable<TEnumerator, T>(this TEnumerator enumerable, T typeHint)
        where TEnumerator : struct, IValueEnumerator<T>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        return new ValueEnumerable<TEnumerator, T>(enumerable);
    }
}
