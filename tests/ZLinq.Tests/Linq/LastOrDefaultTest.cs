namespace ZLinq.Tests.Linq;

public class LastOrDefaultTest
{
    [Fact]
    public void Empty()
    {
        var xs = new int[0];
        xs.AsValueEnumerable().LastOrDefault().ShouldBe(default(int));
        xs.ToValueEnumerable().LastOrDefault().ShouldBe(default(int));
    }

    [Fact]
    public void NonEmpty()
    {
        var xs = new int[] { 1, 2, 3, 4, 5 };
        xs.AsValueEnumerable().LastOrDefault().ShouldBe(5);
        xs.ToValueEnumerable().LastOrDefault().ShouldBe(5);
    }

    [Fact]
    public void SingleElement()
    {
        var xs = new int[] { 42 };
        xs.AsValueEnumerable().LastOrDefault().ShouldBe(42);
        xs.ToValueEnumerable().LastOrDefault().ShouldBe(42);
    }

    [Fact]
    public void WithDefaultValue_Empty()
    {
        var xs = new int[0];
        xs.AsValueEnumerable().LastOrDefault(42).ShouldBe(42);
        xs.ToValueEnumerable().LastOrDefault(42).ShouldBe(42);
    }

    [Fact]
    public void WithDefaultValue_NonEmpty()
    {
        var xs = new int[] { 1, 2, 3, 4, 5 };
        xs.AsValueEnumerable().LastOrDefault(42).ShouldBe(5);
        xs.ToValueEnumerable().LastOrDefault(42).ShouldBe(5);
    }

    [Fact]
    public void WithPredicate_Match()
    {
        var xs = new int[] { 1, 2, 3, 4, 5 };
        xs.AsValueEnumerable().LastOrDefault(x => x < 4).ShouldBe(3);
        xs.ToValueEnumerable().LastOrDefault(x => x < 4).ShouldBe(3);
    }

    [Fact]
    public void WithPredicate_NoMatch()
    {
        var xs = new int[] { 1, 2, 3, 4, 5 };
        xs.AsValueEnumerable().LastOrDefault(x => x > 10).ShouldBe(default(int));
        xs.ToValueEnumerable().LastOrDefault(x => x > 10).ShouldBe(default(int));
    }

    [Fact]
    public void WithPredicate_EmptyCollection()
    {
        var xs = new int[0];
        xs.AsValueEnumerable().LastOrDefault(x => x > 3).ShouldBe(default(int));
        xs.ToValueEnumerable().LastOrDefault(x => x > 3).ShouldBe(default(int));
    }

    [Fact]
    public void WithPredicateAndDefaultValue_Match()
    {
        var xs = new int[] { 1, 2, 3, 4, 5 };
        xs.AsValueEnumerable().LastOrDefault(x => x < 4, 42).ShouldBe(3);
        xs.ToValueEnumerable().LastOrDefault(x => x < 4, 42).ShouldBe(3);
    }

    [Fact]
    public void WithPredicateAndDefaultValue_NoMatch()
    {
        var xs = new int[] { 1, 2, 3, 4, 5 };
        xs.AsValueEnumerable().LastOrDefault(x => x > 10, 42).ShouldBe(42);
        xs.ToValueEnumerable().LastOrDefault(x => x > 10, 42).ShouldBe(42);
    }

    [Fact]
    public void WithPredicateAndDefaultValue_EmptyCollection()
    {
        var xs = new int[0];
        xs.AsValueEnumerable().LastOrDefault(x => x > 3, 42).ShouldBe(42);
        xs.ToValueEnumerable().LastOrDefault(x => x > 3, 42).ShouldBe(42);
    }

    [Fact]
    public void WithPredicate_MultipleMatches()
    {
        var xs = new int[] { 1, 2, 3, 3, 2, 1 };
        xs.AsValueEnumerable().LastOrDefault(x => x == 2).ShouldBe(2);
        xs.ToValueEnumerable().LastOrDefault(x => x == 2).ShouldBe(2);
        
        xs.AsValueEnumerable().LastOrDefault(x => x == 3).ShouldBe(3);
        xs.ToValueEnumerable().LastOrDefault(x => x == 3).ShouldBe(3);
    }

    [Fact]
    public void ReferenceTypeWithNull()
    {
        var xs = new string?[] { "a", "b", null, "c" };
        xs.AsValueEnumerable().LastOrDefault().ShouldBe("c");
        xs.ToValueEnumerable().LastOrDefault().ShouldBe("c");
        
        xs.AsValueEnumerable().LastOrDefault(x => x == null).ShouldBe(null);
        xs.ToValueEnumerable().LastOrDefault(x => x == null).ShouldBe(null);
    }
}
