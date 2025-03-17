namespace ZLinq.Tests.Linq;

public class ToHashSetTest
{
    // pattern
    // Span = Array
    // EnumeatedCount Only = Select
    // Iterator

    [Fact]
    public void Empty()
    {
        var xs = new int[0];

        var actual1 = xs.AsValueEnumerable().ToHashSet();
        var actual2 = xs.AsValueEnumerable().Select(x => x).ToHashSet();
        var actual3 = xs.ToValueEnumerable().ToHashSet();

        actual1.Count.ShouldBe(0);
        actual2.Count.ShouldBe(0);
        actual3.Count.ShouldBe(0);
    }

    [Fact]
    public void NonEmpty()
    {
        var xs = new int[] { 1, 2, 3, 4, 5 };

        var actual1 = xs.AsValueEnumerable().ToHashSet();
        var actual2 = xs.AsValueEnumerable().Select(x => x).ToHashSet();
        var actual3 = xs.ToValueEnumerable().ToHashSet();

        Assert.Equal(xs.Length, actual1.Count);
        Assert.Equal(xs.Length, actual2.Count);
        Assert.Equal(xs.Length, actual3.Count);

        foreach (var x in xs)
        {
            Assert.Contains(x, actual1);
            Assert.Contains(x, actual2);
            Assert.Contains(x, actual3);
        }
    }

    [Fact]
    public void Comparer()
    {
        string?[] xs = ["foo", "bar", "baz"];

        var hashSet = xs.AsValueEnumerable().ToHashSet(StringComparer.OrdinalIgnoreCase);

        hashSet.Count.ShouldBe(3);
        hashSet.Contains("foO").ShouldBeTrue();
        hashSet.Contains("BaR").ShouldBeTrue();
        hashSet.Contains("Baz").ShouldBeTrue();
    }

    [Fact]
    public void DuplicateElements()
    {
        var xs = new int[] { 1, 2, 2, 3, 3, 3 };

        var actual = xs.AsValueEnumerable().ToHashSet();

        actual.Count.ShouldBe(3);
        actual.Contains(1).ShouldBeTrue();
        actual.Contains(2).ShouldBeTrue();
        actual.Contains(3).ShouldBeTrue();
    }
}
