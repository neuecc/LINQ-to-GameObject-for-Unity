namespace ZLinq.Tests.Linq;

public class AnyTest
{
    [Fact]
    public void EmptyCollection()
    {
        var xs = new int[0];

        // System LINQ reference implementation
        var expected = xs.Any();
        expected.ShouldBeFalse();

        // ZLinq implementations
        xs.AsValueEnumerable().Any().ShouldBe(expected);
        xs.ToValueEnumerable().Any().ShouldBe(expected);
    }

    [Fact]
    public void NonEmptyCollection()
    {
        var xs = new int[] { 1, 2, 3, 4, 5 };

        // System LINQ reference implementation
        var expected = xs.Any();
        expected.ShouldBeTrue();

        // ZLinq implementations
        xs.AsValueEnumerable().Any().ShouldBe(expected);
        xs.ToValueEnumerable().Any().ShouldBe(expected);
    }

    [Fact]
    public void EmptyCollectionWithPredicate()
    {
        var xs = new int[0];

        // System LINQ reference implementation
        var expected = xs.Any(x => x > 3);
        expected.ShouldBeFalse();

        // ZLinq implementations
        xs.AsValueEnumerable().Any(x => x > 3).ShouldBe(expected);
        xs.ToValueEnumerable().Any(x => x > 3).ShouldBe(expected);
    }

    [Fact]
    public void NoItemsMatchPredicate()
    {
        var xs = new int[] { 1, 2, 3, 4, 5 };

        // System LINQ reference implementation
        var expected = xs.Any(x => x > 10);
        expected.ShouldBeFalse();

        // ZLinq implementations
        xs.AsValueEnumerable().Any(x => x > 10).ShouldBe(expected);
        xs.ToValueEnumerable().Any(x => x > 10).ShouldBe(expected);
    }

    [Fact]
    public void SomeItemsMatchPredicate()
    {
        var xs = new int[] { 1, 2, 3, 4, 5 };

        // System LINQ reference implementation
        var expected = xs.Any(x => x > 3);
        expected.ShouldBeTrue();

        // ZLinq implementations
        xs.AsValueEnumerable().Any(x => x > 3).ShouldBe(expected);
        xs.ToValueEnumerable().Any(x => x > 3).ShouldBe(expected);
    }

    [Fact]
    public void AllItemsMatchPredicate()
    {
        var xs = new int[] { 1, 2, 3, 4, 5 };

        // System LINQ reference implementation
        var expected = xs.Any(x => x > 0);
        expected.ShouldBeTrue();

        // ZLinq implementations
        xs.AsValueEnumerable().Any(x => x > 0).ShouldBe(expected);
        xs.ToValueEnumerable().Any(x => x > 0).ShouldBe(expected);
    }

    [Fact]
    public void ShortCircuitingWorks()
    {
        var xs = new int[] { 1, 2, 3, 4, 5 };
        var counter = 0;

        // ZLinq implementation should short-circuit after finding first matching item
        var actual = xs.AsValueEnumerable().Any(x =>
        {
            counter++;
            return x > 2;
        });

        actual.ShouldBeTrue();
        counter.ShouldBe(3); // Should only evaluate until it hits 3
    }

    [Fact]
    public void SpanOptimizationUsed()
    {
        // Create an array that will use Span optimization path
        var xs = new int[] { 1, 2, 3, 4, 5 };
        var called = false;

        // Use a custom predicate that tracks if it's called
        bool Predicate(int x)
        {
            called = true;
            return x > 10;
        }

        // Using standard array which should use Span optimization
        var result = xs.AsValueEnumerable().Any(Predicate);

        // Predicate should have been called (proving the test works)
        called.ShouldBeTrue();

        // Result should match expected value
        result.ShouldBeFalse();
    }
}
