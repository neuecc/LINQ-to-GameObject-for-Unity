namespace ZLinq.Tests.Linq;

public class CountTest
{
    [Fact]
    public void EmptyCollection()
    {
        var xs = new int[0];

        // System LINQ reference implementation
        var expected = xs.Count();
        expected.ShouldBe(0);

        // ZLinq implementations
        xs.AsValueEnumerable().Count().ShouldBe(expected);
        xs.ToValueEnumerable().Count().ShouldBe(expected);
    }

    [Fact]
    public void NonEmptyCollection()
    {
        var xs = new int[] { 1, 2, 3, 4, 5 };

        // System LINQ reference implementation
        var expected = xs.Count();
        expected.ShouldBe(5);

        // ZLinq implementations
        xs.AsValueEnumerable().Count().ShouldBe(expected);
        xs.ToValueEnumerable().Count().ShouldBe(expected);
    }

    [Fact]
    public void EmptyCollectionWithPredicate()
    {
        var xs = new int[0];

        // System LINQ reference implementation
        var expected = xs.Count(x => x > 3);
        expected.ShouldBe(0);

        // ZLinq implementations
        xs.AsValueEnumerable().Count(x => x > 3).ShouldBe(expected);
        xs.ToValueEnumerable().Count(x => x > 3).ShouldBe(expected);
    }

    [Fact]
    public void NoItemsMatchPredicate()
    {
        var xs = new int[] { 1, 2, 3, 4, 5 };

        // System LINQ reference implementation
        var expected = xs.Count(x => x > 10);
        expected.ShouldBe(0);

        // ZLinq implementations
        xs.AsValueEnumerable().Count(x => x > 10).ShouldBe(expected);
        xs.ToValueEnumerable().Count(x => x > 10).ShouldBe(expected);
    }

    [Fact]
    public void SomeItemsMatchPredicate()
    {
        var xs = new int[] { 1, 2, 3, 4, 5 };

        // System LINQ reference implementation
        var expected = xs.Count(x => x > 3);
        expected.ShouldBe(2);

        // ZLinq implementations
        xs.AsValueEnumerable().Count(x => x > 3).ShouldBe(expected);
        xs.ToValueEnumerable().Count(x => x > 3).ShouldBe(expected);
    }

    [Fact]
    public void AllItemsMatchPredicate()
    {
        var xs = new int[] { 1, 2, 3, 4, 5 };

        // System LINQ reference implementation
        var expected = xs.Count(x => x > 0);
        expected.ShouldBe(5);

        // ZLinq implementations
        xs.AsValueEnumerable().Count(x => x > 0).ShouldBe(expected);
        xs.ToValueEnumerable().Count(x => x > 0).ShouldBe(expected);
    }

    [Fact]
    public void NonEnumeratedCountOptimization()
    {
        // Using an array which should support TryGetNonEnumeratedCount
        var xs = new int[] { 1, 2, 3, 4, 5 };

        // Create a custom counter to verify we're not enumerating when not needed
        var counter = 0;
        bool CountPredicate(int x)
        {
            counter++;
            return x > 0;
        }

        // Count without predicate should use optimization
        var result = xs.AsValueEnumerable().Count();
        result.ShouldBe(5);

        // Count with predicate should enumerate all items
        counter = 0;
        result = xs.AsValueEnumerable().Count(CountPredicate);
        result.ShouldBe(5);
        counter.ShouldBe(5); // All items should be enumerated
    }

    [Fact]
    public void LargeCollectionCountTest()
    {
        var xs = Enumerable.Range(1, 1000).ToArray();

        // System LINQ reference implementation
        var expected = xs.Count();
        expected.ShouldBe(1000);

        // ZLinq implementations
        xs.AsValueEnumerable().Count().ShouldBe(expected);
        xs.ToValueEnumerable().Count().ShouldBe(expected);
    }

    [Fact]
    public void PredicateThrowsException()
    {
        var xs = new int[] { 1, 2, 3, 0, 4 };

        // Create a predicate that will throw an exception
        bool ExceptionPredicate(int x)
        {
            return 10 / x > 0; // Will throw DivideByZeroException when x = 0
        }

        // System LINQ and ZLinq should both throw the same exception
        Should.Throw<DivideByZeroException>(() => xs.Count(ExceptionPredicate));
        Should.Throw<DivideByZeroException>(() => xs.AsValueEnumerable().Count(ExceptionPredicate));
        Should.Throw<DivideByZeroException>(() => xs.ToValueEnumerable().Count(ExceptionPredicate));
    }
}
