namespace ZLinq.Tests.Linq;

public class AggregateTest
{
    // Tests for Aggregate<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TSource, TSource> func)
    [Fact]
    public void Empty()
    {
        var xs = new int[0];

        // Both implementations should throw when the source is empty
        TestUtil.Throws<InvalidOperationException>(
            () => xs.Aggregate((x, y) => x + y),
            () => xs.AsValueEnumerable().Aggregate((x, y) => x + y)
        );

        TestUtil.Throws<InvalidOperationException>(
            () => xs.Aggregate((x, y) => x + y),
            () => xs.ToValueEnumerable().Aggregate((x, y) => x + y)
        );
    }

    [Fact]
    public void NonEmpty()
    {
        var xs = new int[] { 1, 2, 3, 4, 5 };

        // Both implementations should return the same result when aggregating
        xs.AsValueEnumerable().Aggregate((x, y) => x + y).ShouldBe(xs.Aggregate((x, y) => x + y));
        xs.ToValueEnumerable().Aggregate((x, y) => x + y).ShouldBe(xs.Aggregate((x, y) => x + y));

        // Test with different aggregation function
        xs.AsValueEnumerable().Aggregate((x, y) => x * y).ShouldBe(xs.Aggregate((x, y) => x * y));
        xs.ToValueEnumerable().Aggregate((x, y) => x * y).ShouldBe(xs.Aggregate((x, y) => x * y));
    }

    [Fact]
    public void SingleElement()
    {
        var xs = new int[] { 42 };

        // Should return the single element without calling the function
        xs.AsValueEnumerable().Aggregate((x, y) => throw new Exception("Should not be called")).ShouldBe(42);
        xs.ToValueEnumerable().Aggregate((x, y) => throw new Exception("Should not be called")).ShouldBe(42);
    }

    // Tests for Aggregate<TEnumerator, TSource, TAccumulate>(this ValueEnumerable<TEnumerator, TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func)
    [Fact]
    public void EmptyWithSeed()
    {
        var xs = new int[0];
        var seed = 10;

        // Should return the seed when source is empty
        xs.AsValueEnumerable().Aggregate(seed, (acc, x) => acc + x).ShouldBe(seed);
        xs.ToValueEnumerable().Aggregate(seed, (acc, x) => acc + x).ShouldBe(seed);
    }

    [Fact]
    public void NonEmptyWithSeed()
    {
        var xs = new int[] { 1, 2, 3, 4, 5 };
        var seed = 10;

        // Both implementations should return the same result when aggregating with seed
        xs.AsValueEnumerable().Aggregate(seed, (acc, x) => acc + x).ShouldBe(xs.Aggregate(seed, (acc, x) => acc + x));
        xs.ToValueEnumerable().Aggregate(seed, (acc, x) => acc + x).ShouldBe(xs.Aggregate(seed, (acc, x) => acc + x));

        // Test with different aggregation function
        xs.AsValueEnumerable().Aggregate(seed, (acc, x) => acc * x).ShouldBe(xs.Aggregate(seed, (acc, x) => acc * x));
        xs.ToValueEnumerable().Aggregate(seed, (acc, x) => acc * x).ShouldBe(xs.Aggregate(seed, (acc, x) => acc * x));
    }

    [Fact]
    public void WithComplexSeed()
    {
        var xs = new string[] { "Apple", "Banana", "Cherry" };
        var seed = new Dictionary<char, int>();

        // Count occurrences of first character in each string
        var expected = xs.Aggregate(
            seed,
            (dict, str) =>
            {
                if (str.Length > 0)
                {
                    var firstChar = str[0];
                    dict[firstChar] = dict.TryGetValue(firstChar, out var count) ? count + 1 : 1;
                }
                return dict;
            }
        );

        var actual = xs.AsValueEnumerable().Aggregate(
            new Dictionary<char, int>(),
            (dict, str) =>
            {
                if (str.Length > 0)
                {
                    var firstChar = str[0];
                    dict[firstChar] = dict.TryGetValue(firstChar, out var count) ? count + 1 : 1;
                }
                return dict;
            }
        );

        actual.Count.ShouldBe(expected.Count);
        foreach (var kvp in expected)
        {
            actual.ContainsKey(kvp.Key).ShouldBeTrue();
            actual[kvp.Key].ShouldBe(kvp.Value);
        }
    }

    // Tests for Aggregate<TEnumerator, TSource, TAccumulate, TResult>(this ValueEnumerable<TEnumerator, TSource> source, TAccumulate seed, 
    // Func<TAccumulate, TSource, TAccumulate> func, Func<TAccumulate, TResult> resultSelector)
    [Fact]
    public void EmptyWithSeedAndResultSelector()
    {
        var xs = new int[0];
        var seed = 10;

        // Should apply the result selector to the seed when source is empty
        xs.AsValueEnumerable().Aggregate(seed, (acc, x) => acc + x, acc => acc * 2)
            .ShouldBe(xs.Aggregate(seed, (acc, x) => acc + x, acc => acc * 2));

        xs.ToValueEnumerable().Aggregate(seed, (acc, x) => acc + x, acc => acc * 2)
            .ShouldBe(xs.Aggregate(seed, (acc, x) => acc + x, acc => acc * 2));
    }

    [Fact]
    public void NonEmptyWithSeedAndResultSelector()
    {
        var xs = new int[] { 1, 2, 3, 4, 5 };
        var seed = 10;

        // Both implementations should return the same result
        xs.AsValueEnumerable().Aggregate(seed, (acc, x) => acc + x, acc => acc * 2)
            .ShouldBe(xs.Aggregate(seed, (acc, x) => acc + x, acc => acc * 2));

        xs.ToValueEnumerable().Aggregate(seed, (acc, x) => acc + x, acc => acc * 2)
            .ShouldBe(xs.Aggregate(seed, (acc, x) => acc + x, acc => acc * 2));

        // Test with different aggregation and result selector functions
        xs.AsValueEnumerable().Aggregate(seed, (acc, x) => acc * x, acc => $"Result: {acc}")
            .ShouldBe(xs.Aggregate(seed, (acc, x) => acc * x, acc => $"Result: {acc}"));

        xs.ToValueEnumerable().Aggregate(seed, (acc, x) => acc * x, acc => $"Result: {acc}")
            .ShouldBe(xs.Aggregate(seed, (acc, x) => acc * x, acc => $"Result: {acc}"));
    }

    [Fact]
    public void WithComplexResult()
    {
        var xs = new int[] { 1, 2, 3, 4, 5 };

        // Get sum and count, then calculate average as result
        var expected = xs.Aggregate(
            (Sum: 0, Count: 0),
            (acc, x) => (Sum: acc.Sum + x, Count: acc.Count + 1),
            acc => acc.Count > 0 ? (double)acc.Sum / acc.Count : 0
        );

        var actual = xs.AsValueEnumerable().Aggregate(
            (Sum: 0, Count: 0),
            (acc, x) => (Sum: acc.Sum + x, Count: acc.Count + 1),
            acc => acc.Count > 0 ? (double)acc.Sum / acc.Count : 0
        );

        actual.ShouldBe(expected);

        actual = xs.ToValueEnumerable().Aggregate(
            (Sum: 0, Count: 0),
            (acc, x) => (Sum: acc.Sum + x, Count: acc.Count + 1),
            acc => acc.Count > 0 ? (double)acc.Sum / acc.Count : 0
        );

        actual.ShouldBe(expected);
    }
}
