namespace ZLinq.Tests.Linq;

public class FirstOrDefaultTest
{
    [Fact]
    public void Empty()
    {
        var xs = new int[0];
        xs.AsValueEnumerable().FirstOrDefault().ShouldBe(default(int));
        xs.ToValueEnumerable().FirstOrDefault().ShouldBe(default(int));
    }

    [Fact]
    public void NonEmpty()
    {
        var xs = new int[] { 1, 2, 3, 4, 5 };
        xs.AsValueEnumerable().FirstOrDefault().ShouldBe(1);
        xs.ToValueEnumerable().FirstOrDefault().ShouldBe(1);
    }

    [Fact]
    public void WithDefaultValue()
    {
        var xs = new int[0];
        xs.AsValueEnumerable().FirstOrDefault(42).ShouldBe(42);
        xs.ToValueEnumerable().FirstOrDefault(42).ShouldBe(42);
    }

    [Fact]
    public void WithPredicate()
    {
        var xs = new int[] { 1, 2, 3, 4, 5 };
        xs.AsValueEnumerable().FirstOrDefault(x => x > 3).ShouldBe(4);
        xs.ToValueEnumerable().FirstOrDefault(x => x > 3).ShouldBe(4);
    }

    [Fact]
    public void WithPredicateAndDefaultValue()
    {
        var xs = new int[] { 1, 2, 3, 4, 5 };
        xs.AsValueEnumerable().FirstOrDefault(x => x > 5, 42).ShouldBe(42);
        xs.ToValueEnumerable().FirstOrDefault(x => x > 5, 42).ShouldBe(42);
    }
}
