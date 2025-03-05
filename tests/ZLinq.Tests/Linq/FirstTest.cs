namespace ZLinq.Tests.Linq;

public class FirstTest
{
    [Fact]
    public void Empty()
    {
        var xs = new int[0];
        Assert.Throws<InvalidOperationException>(() => xs.AsValueEnumerable().First());
        Assert.Throws<InvalidOperationException>(() => xs.ToIterableValueEnumerable().First());
    }

    [Fact]
    public void NonEmpty()
    {
        var xs = new int[] { 1, 2, 3, 4, 5 };
        xs.AsValueEnumerable().First().ShouldBe(1);
        xs.ToIterableValueEnumerable().First().ShouldBe(1);
    }

    [Fact]
    public void WithPredicate()
    {
        var xs = new int[] { 1, 2, 3, 4, 5 };
        xs.AsValueEnumerable().First(x => x > 3).ShouldBe(4);
        xs.ToIterableValueEnumerable().First(x => x > 3).ShouldBe(4);
    }

    [Fact]
    public void WithPredicateAndDefaultValue()
    {
        var xs = new int[] { 1, 2, 3, 4, 5 };
        Assert.Throws<InvalidOperationException>(() => xs.AsValueEnumerable().First(x => x > 5));
        Assert.Throws<InvalidOperationException>(() => xs.ToIterableValueEnumerable().First(x => x > 5));
    }
}
