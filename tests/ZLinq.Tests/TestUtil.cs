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

    // hide source type to avoid Span optimization
    public static FromEnumerable<T> ToIterableValueEnumerable<T>(this IEnumerable<T> source)
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
}