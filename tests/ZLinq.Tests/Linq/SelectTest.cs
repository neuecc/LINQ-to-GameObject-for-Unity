namespace ZLinq.Tests.Linq;

public class SelectTest
{
    [Fact]
    public void Empty()
    {
        var xs = new int[0];

        var enumerable = xs.AsValueEnumerable().Select(x => x);

        var e1 = enumerable;
        e1.TryGetNonEnumeratedCount(out var nonEnumeratedCount).ShouldBe(true); // TODO: true | false

        var e2 = enumerable;
        e2.TryGetSpan(out var span).ShouldBe(true); // TODO: true | false

        var e3 = enumerable;
        e3.TryGetNext(out var next).ShouldBeFalse();

        enumerable.Dispose();
    }

    [Fact]
    public void NonEmpty()
    {
        var xs = new int[] { 1, 2, 3, 4, 5 };

        var enumerable = xs.AsValueEnumerable().Select(x => x);

        var e1 = enumerable;
        e1.TryGetNonEnumeratedCount(out var nonEnumeratedCount).ShouldBe(true); // TODO: true | false

        var e2 = enumerable;
        e2.TryGetSpan(out var span).ShouldBe(true); // TODO: true | false

        var e3 = enumerable;
        var array = e3.ToArray();
        array.ShouldBe([1, 2, 3, 4, 5]); // TODO:

        enumerable.Dispose();
    }
}