namespace ZLinq.Tests.Linq;

public class LastTest
{
    [Fact]
    public void Empty()
    {
        var xs = new int[0];
        Assert.Throws<InvalidOperationException>(() => xs.AsValueEnumerable().Last());
        Assert.Throws<InvalidOperationException>(() => xs.ToValueEnumerable().Last());
    }

    [Fact]
    public void NonEmpty()
    {
        var xs = new int[] { 1, 2, 3, 4, 5 };
        xs.AsValueEnumerable().Last().ShouldBe(5);
        xs.ToValueEnumerable().Last().ShouldBe(5);
    }

    [Fact]
    public void SingleElement()
    {
        var xs = new int[] { 42 };
        xs.AsValueEnumerable().Last().ShouldBe(42);
        xs.ToValueEnumerable().Last().ShouldBe(42);
    }

    [Fact]
    public void WithPredicate()
    {
        var xs = new int[] { 1, 2, 3, 4, 5 };
        xs.AsValueEnumerable().Last(x => x < 4).ShouldBe(3);
        xs.ToValueEnumerable().Last(x => x < 4).ShouldBe(3);
    }

    [Fact]
    public void WithPredicate_NoMatch()
    {
        var xs = new int[] { 1, 2, 3, 4, 5 };
        Assert.Throws<InvalidOperationException>(() => xs.AsValueEnumerable().Last(x => x > 10));
        Assert.Throws<InvalidOperationException>(() => xs.ToValueEnumerable().Last(x => x > 10));
    }

    [Fact]
    public void WithPredicate_EmptyCollection()
    {
        var xs = new int[0];
        Assert.Throws<InvalidOperationException>(() => xs.AsValueEnumerable().Last(x => x > 3));
        Assert.Throws<InvalidOperationException>(() => xs.ToValueEnumerable().Last(x => x > 3));
    }

    [Fact]
    public void WithPredicate_MultipleMatches()
    {
        var xs = new int[] { 1, 2, 3, 3, 2, 1 };
        xs.AsValueEnumerable().Last(x => x == 3).ShouldBe(3);
        xs.ToValueEnumerable().Last(x => x == 3).ShouldBe(3);
        
        xs.AsValueEnumerable().Last(x => x == 2).ShouldBe(2);
        xs.ToValueEnumerable().Last(x => x == 2).ShouldBe(2);
    }

    [Fact]
    public void WithPredicate_LastMatch()
    {
        var xs = new int[] { 1, 2, 3, 4, 5 };
        xs.AsValueEnumerable().Last(x => x > 0).ShouldBe(5);
        xs.ToValueEnumerable().Last(x => x > 0).ShouldBe(5);
    }
}
