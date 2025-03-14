using ZLinq.Linq;

namespace ZLinq.Tests.Linq;

public class ToListTest
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

        xs.AsValueEnumerable().ToList().ShouldBeEmpty();
        ValueEnumerable.Range(0, 0).ToList().ShouldBeEmpty();
        xs.AsValueEnumerable().Select(x => x).ToList().ShouldBeEmpty();
        xs.ToValueEnumerable().ToList().ShouldBeEmpty();
    }

    [Fact]
    public void NonEmpty()
    {
        var xs = new int[] { 1, 2, 3, 4, 5 };

        xs.AsValueEnumerable().ToList().ShouldBe(xs);
        ValueEnumerable.Range(1, 5).ToList().ShouldBe(xs);
        xs.AsValueEnumerable().Select(x => x).ToList().ShouldBe(xs);
        xs.ToValueEnumerable().ToList().ShouldBe(xs);
    }
}
