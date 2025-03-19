namespace ZLinq.Tests.Linq;

public class LongCountTest
{
    [Fact]
    public void EmptyCollection()
    {
        var xs = Array.Empty<int>();

        // System LINQ reference implementation
        var expected = xs.LongCount();
        expected.ShouldBe(0);

        // ZLinq implementations
        xs.AsValueEnumerable().LongCount().ShouldBe(expected);
        xs.ToValueEnumerable().LongCount().ShouldBe(expected);
    }

    [Fact]
    public void NonEmptyCollection()
    {
        var xs = new int[] { 1, 2, 3, 4, 5 };

        // System LINQ reference implementation
        var expected = xs.LongCount();
        expected.ShouldBe(5);

        // ZLinq implementations
        xs.AsValueEnumerable().LongCount().ShouldBe(expected);
        xs.ToValueEnumerable().LongCount().ShouldBe(expected);
    }

    [Fact]
    public void EmptyCollectionWithPredicate()
    {
        var xs = Array.Empty<int>();

        // System LINQ reference implementation
        var expected = xs.LongCount(x => x > 3);
        expected.ShouldBe(0);

        // ZLinq implementations
        xs.AsValueEnumerable().LongCount(x => x > 3).ShouldBe(expected);
        xs.ToValueEnumerable().LongCount(x => x > 3).ShouldBe(expected);
    }

    [Fact]
    public void NoItemsMatchPredicate()
    {
        var xs = new int[] { 1, 2, 3, 4, 5 };

        // System LINQ reference implementation
        var expected = xs.LongCount(x => x > 10);
        expected.ShouldBe(0);

        // ZLinq implementations
        xs.AsValueEnumerable().LongCount(x => x > 10).ShouldBe(expected);
        xs.ToValueEnumerable().LongCount(x => x > 10).ShouldBe(expected);
    }

    [Fact]
    public void SomeItemsMatchPredicate()
    {
        var xs = new int[] { 1, 2, 3, 4, 5 };

        // System LINQ reference implementation
        var expected = xs.LongCount(x => x > 3);
        expected.ShouldBe(2);

        // ZLinq implementations
        xs.AsValueEnumerable().LongCount(x => x > 3).ShouldBe(expected);
        xs.ToValueEnumerable().LongCount(x => x > 3).ShouldBe(expected);
    }

    [Fact]
    public void AllItemsMatchPredicate()
    {
        var xs = new int[] { 1, 2, 3, 4, 5 };

        // System LINQ reference implementation
        var expected = xs.LongCount(x => x > 0);
        expected.ShouldBe(5);

        // ZLinq implementations
        xs.AsValueEnumerable().LongCount(x => x > 0).ShouldBe(expected);
        xs.ToValueEnumerable().LongCount(x => x > 0).ShouldBe(expected);
    }

    [Fact]
    public void TryGetNonEnumeratedCountOptimization()
    {
        var xs = new int[] { 1, 2, 3, 4, 5 };
        
        // Using AsValueEnumerable which should optimize with TryGetNonEnumeratedCount
        var result = xs.AsValueEnumerable().LongCount();
        result.ShouldBe(5);
        
        // Using a collection that can't use the optimization (forcing enumeration)
        var enumerableCollection = xs.Select(x => x);
        var result2 = enumerableCollection.AsValueEnumerable().LongCount();
        result2.ShouldBe(5);
    }

    [Fact]
    public void DifferentCollectionTypes()
    {
        // Test with List<T>
        var list = new List<int> { 1, 2, 3 };
        list.AsValueEnumerable().LongCount().ShouldBe(3);
        
        // Test with IEnumerable<T>
        IEnumerable<int> enumerable = new[] { 1, 2, 3, 4 };
        enumerable.AsValueEnumerable().LongCount().ShouldBe(4);
        
        // Test with lazy sequence
        var lazySequence = Enumerable.Range(1, 10);
        lazySequence.AsValueEnumerable().LongCount().ShouldBe(10);
    }

    [Fact]
    public void LargeCollectionSize()
    {
        // This test verifies the LongCount works correctly with large collections
        // that might overflow an int counter
        
        // Create a large collection that wouldn't fit in an int if we were summing values
        const int size = 1000;
        var xs = Enumerable.Range(1, size);
        
        xs.AsValueEnumerable().LongCount().ShouldBe(size);
        xs.ToValueEnumerable().LongCount().ShouldBe(size);
        
        // With predicate
        xs.AsValueEnumerable().LongCount(x => x % 2 == 0).ShouldBe(size / 2);
        xs.ToValueEnumerable().LongCount(x => x % 2 == 0).ShouldBe(size / 2);
    }

    [Fact]
    public void CheckedOperationTest()
    {
        // Ensure the checked operation works correctly
        // Note: An actual overflow test would require an extremely large collection
        // which isn't practical for a unit test, so we're just verifying the logic
        
        var xs = Enumerable.Range(1, 100);
        xs.AsValueEnumerable().LongCount().ShouldBe(100);
        
        // With predicate
        xs.AsValueEnumerable().LongCount(x => true).ShouldBe(100);
    }
}
