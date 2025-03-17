namespace ZLinq.Tests.Linq;

public class DistinctTest
{
    [Fact]
    public void Empty()
    {
        var xs = new int[0];

        var enumerable = xs.AsValueEnumerable(); // TODO: impl method like .Select(x => x);

        var e1 = enumerable;
        e1.TryGetNonEnumeratedCount(out var nonEnumeratedCount).ShouldBe(true); // TODO: true | false

        var e2 = enumerable;
        e2.TryGetSpan(out var span).ShouldBe(true); // TODO: true | false

        var e3 = enumerable;
        e3.Enumerator.TryGetNext(out var next).ShouldBeFalse();

        enumerable.Dispose();
    }

    [Fact]
    public void NonEmpty()
    {
        var xs = new int[] { 1, 2, 3, 4, 5 };

        var enumerable = xs.AsValueEnumerable(); // TODO: impl method like .Select(x => x);

        var e1 = enumerable;
        e1.TryGetNonEnumeratedCount(out var nonEnumeratedCount).ShouldBe(true); // TODO: true | false

        var e2 = enumerable;
        e2.TryGetSpan(out var span).ShouldBe(true); // TODO: true | false

        var e3 = enumerable;
        var array = e3.ToArray();
        array.ShouldBe(xs.ToArray()); // TODO: impl compare for standard array

        enumerable.Dispose();
    }

    [Fact]
    public void Empty2()
    {
        var xs = new int[0];

        var enumerable = xs.AsValueEnumerable(); // TODO: impl method like .Select(x => x);

        var e1 = enumerable;
        e1.TryGetNonEnumeratedCount(out var nonEnumeratedCount).ShouldBe(true); // TODO: true | false

        var e2 = enumerable;
        e2.TryGetSpan(out var span).ShouldBe(true); // TODO: true | false

        var e3 = enumerable;
        e3.Enumerator.TryGetNext(out var next).ShouldBeFalse();

        enumerable.Dispose();
    }

    [Fact]
    public void NonEmpty2()
    {
        var xs = new int[] { 1, 2, 3, 4, 5 };

        var enumerable = xs.AsValueEnumerable(); // TODO: impl method like .Select(x => x);

        var e1 = enumerable;
        e1.TryGetNonEnumeratedCount(out var nonEnumeratedCount).ShouldBe(true); // TODO: true | false

        var e2 = enumerable;
        e2.TryGetSpan(out var span).ShouldBe(true); // TODO: true | false

        var e3 = enumerable;
        var array = e3.ToArray();
        array.ShouldBe(xs.ToArray()); // TODO: impl compare for standard array

        enumerable.Dispose();
    }

}
