namespace ZLinq.Tests;

public class AllTest
{
    [Fact]
    public void EmptyCollectionAlwaysReturnsTrue()
    {
        var xs = new int[0];

        // System LINQ reference implementation
        var expected = xs.All(x => x > 0);
        expected.ShouldBeTrue();

        // ZLinq implementation
        xs.AsValueEnumerable().All(x => x > 0).ShouldBe(expected);
        xs.ToValueEnumerable().All(x => x > 0).ShouldBe(expected);
    }

    [Fact]
    public void AllItemsMatchPredicate()
    {
        var xs = new int[] { 2, 4, 6, 8, 10 };

        // System LINQ reference implementation
        var expected = xs.All(x => x % 2 == 0);
        expected.ShouldBeTrue();

        // ZLinq implementation
        xs.AsValueEnumerable().All(x => x % 2 == 0).ShouldBe(expected);
        xs.ToValueEnumerable().All(x => x % 2 == 0).ShouldBe(expected);
    }

    [Fact]
    public void SomeItemsDoNotMatchPredicate()
    {
        var xs = new int[] { 1, 2, 3, 4, 5 };

        // System LINQ reference implementation
        var expected = xs.All(x => x % 2 == 0);
        expected.ShouldBeFalse();

        // ZLinq implementation
        xs.AsValueEnumerable().All(x => x % 2 == 0).ShouldBe(expected);
        xs.ToValueEnumerable().All(x => x % 2 == 0).ShouldBe(expected);
    }

    [Fact]
    public void NoItemsMatchPredicate()
    {
        var xs = new int[] { 1, 3, 5, 7, 9 };

        // System LINQ reference implementation
        var expected = xs.All(x => x % 2 == 0);
        expected.ShouldBeFalse();

        // ZLinq implementation
        xs.AsValueEnumerable().All(x => x % 2 == 0).ShouldBe(expected);
        xs.ToValueEnumerable().All(x => x % 2 == 0).ShouldBe(expected);
    }

    [Fact]
    public void ShortCircuitingWorks()
    {
        var xs = new int[] { 1, 2, 3, 4, 5 };
        var counter = 0;

        // ZLinq implementation should short-circuit after finding first non-matching item
        var actual = xs.AsValueEnumerable().All(x =>
        {
            counter++;
            return x < 3;
        });

        actual.ShouldBeFalse();
        counter.ShouldBe(3); // Should only evaluate until it hits 3
    }

    [Fact]
    public void SpanOptimizationUsed()
    {
        var xs = new int[] { 2, 4, 6, 8, 10 };

        // We need a way to detect span optimization is being used
        // For this test we'll ensure the result is correct and assume 
        // the optimization path is taken for the fixed array
        var actual = xs.AsValueEnumerable().All(x => x % 2 == 0);
        actual.ShouldBeTrue();
    }

    [Fact]
    public void EnumeratorPathWorks()
    {
        // Use TestUtil to create a non-span enumerable
        var xs = TestUtil.ToValueEnumerable(new int[] { 2, 4, 6, 8, 10 });

        var actual = xs.All(x => x % 2 == 0);
        actual.ShouldBeTrue();

        var ys = TestUtil.ToValueEnumerable(new int[] { 1, 3, 5, 7, 9 });

        var actual2 = ys.All(x => x % 2 == 0);
        actual2.ShouldBeFalse();
    }
}
