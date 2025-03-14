using ZLinq.Linq;

namespace ZLinq.Tests.Linq;

public class ToArrayTest
{
    // 4 pattern,
    // Span = Array
    // EnumeratedCount + TryCopyTo = Range
    // EnumeatedCount Only = Select
    // Iterator

    [Fact]
    public void Empty()
    {
        var xs = new int[0];

        xs.AsValueEnumerable().ToArray().ShouldBeEmpty();
        ValueEnumerable.Range(0, 0).ToArray().ShouldBeEmpty();
        xs.AsValueEnumerable().Select(x => x).ToArray().ShouldBeEmpty();
        xs.ToValueEnumerable().ToArray().ShouldBeEmpty();
    }

    [Fact]
    public void NonEmpty()
    {
        var xs = new int[] { 1, 2, 3, 4, 5 };

        xs.AsValueEnumerable().ToArray().ShouldBe(xs);
        ValueEnumerable.Range(1, 5).ToArray().ShouldBe(xs);
        xs.AsValueEnumerable().Select(x => x).ToArray().ShouldBe(xs);
        xs.ToValueEnumerable().ToArray().ShouldBe(xs);
    }
}
