namespace ZLinq.Tests.Linq;

public class ElementAtOrDefaultTest
{
    [Fact]
    public void Empty()
    {
        var xs = new int[0];
        xs.AsValueEnumerable().ElementAtOrDefault(0).ShouldBe(default(int));
        xs.ToIterableValueEnumerable().ElementAtOrDefault(0).ShouldBe(default(int));
    }

    [Fact]
    public void NonEmpty()
    {
        var xs = new int[] { 1, 2, 3, 4, 5 };
        xs.AsValueEnumerable().ElementAtOrDefault(2).ShouldBe(3);
        xs.ToIterableValueEnumerable().ElementAtOrDefault(2).ShouldBe(3);
    }

    [Fact]
    public void IndexOutOfRange()
    {
        var xs = new int[] { 1, 2, 3, 4, 5 };
        xs.AsValueEnumerable().ElementAtOrDefault(5).ShouldBe(default(int));
        xs.ToIterableValueEnumerable().ElementAtOrDefault(5).ShouldBe(default(int));
    }

    [Fact]
    public void NegativeIndex()
    {
        var xs = new int[] { 1, 2, 3, 4, 5 };
        xs.AsValueEnumerable().ElementAtOrDefault(-1).ShouldBe(default(int));
        xs.ToIterableValueEnumerable().ElementAtOrDefault(-1).ShouldBe(default(int));
    }

    [Fact]
    public void FromEnd()
    {
        var xs = new int[] { 1, 2, 3, 4, 5 };
        xs.AsValueEnumerable().ElementAtOrDefault(^2).ShouldBe(4);
        xs.ToIterableValueEnumerable().ElementAtOrDefault(^1).ShouldBe(5);
        xs.ToIterableValueEnumerable().ElementAtOrDefault(^2).ShouldBe(4);
        xs.ToIterableValueEnumerable().ElementAtOrDefault(^3).ShouldBe(3);
        xs.ToIterableValueEnumerable().ElementAtOrDefault(^4).ShouldBe(2);
        xs.ToIterableValueEnumerable().ElementAtOrDefault(^5).ShouldBe(1);

        xs.ToIterableValueEnumerable().ElementAtOrDefault(^6).ShouldBe(default(int));

        // large q
        Enumerable.Range(0, 1000).AsValueEnumerable().ElementAtOrDefault(^100).ShouldBe(900);
    }
}
