namespace ZLinq.Tests.Linq;

public class AggregateByTest
{
    [Fact]
    public void AggregateByWithConstantSeed()
    {
        // Arrange
        var source = new[] { 1, 2, 3, 4, 5, 6 };
        var expected = new Dictionary<bool, int>
        {
            [true] = 9,  // Sum of odd numbers: 1 + 3 + 5
            [false] = 12  // Sum of even numbers: 2 + 4 + 6
        };

        // Act
        var actual = source.AsValueEnumerable()
                           .AggregateBy(
                               x => x % 2 == 1,    // Key selector: odd/even
                               0,                  // Initial seed
                               (acc, x) => acc + x, // Aggregation function
                               EqualityComparer<bool>.Default)
                           .ToDictionary(x => x.Key, x => x.Value);

        // Assert
        actual.Count.ShouldBe(expected.Count);
        actual[true].ShouldBe(expected[true]);
        actual[false].ShouldBe(expected[false]);
    }

    [Fact]
    public void AggregateByWithDynamicSeed()
    {
        // Arrange
        var source = new[] { "apple", "banana", "cherry", "date", "elderberry" };
        var expected = new Dictionary<int, string>
        {
            [4] = "date",
            [5] = "apple",
            [6] = "bananacherry",
            [10] = "elderberry"
        };

        // Act
        var actual = source.AsValueEnumerable()
                           .AggregateBy(
                               x => x.Length,                  // Key selector: string length
                               key => string.Empty,            // Seed selector: empty string for each key
                               (acc, x) => acc + x,            // Concatenate strings
                               EqualityComparer<int>.Default)
                           .ToDictionary(x => x.Key, x => x.Value);

        // Assert
        actual.Count.ShouldBe(expected.Count);
        foreach (var key in expected.Keys)
        {
            actual[key].ShouldBe(expected[key]);
        }
    }

    [Fact]
    public void AggregateByWithCustomComparer()
    {
        // Arrange
        var source = new[] { "APPLE", "apple", "BANANA", "banana", "cherry" };
        var comparer = StringComparer.OrdinalIgnoreCase;
        var expected = new Dictionary<string, int>(comparer)
        {
            ["apple"] = 2,   // "APPLE" and "apple" count as 2
            ["banana"] = 2,  // "BANANA" and "banana" count as 2
            ["cherry"] = 1   // "cherry" counts as 1
        };

        // Act
        var actual = source.AsValueEnumerable()
                           .AggregateBy(
                               x => x,                 // Key selector: the string itself
                               0,                      // Seed: start counting at 0
                               (acc, _) => acc + 1,    // Count occurrences
                               comparer)
                           .ToDictionary(x => x.Key, x => x.Value, comparer);

        // Assert
        actual.Count.ShouldBe(expected.Count);
        foreach (var key in expected.Keys)
        {
            actual[key].ShouldBe(expected[key]);
        }
    }

    [Fact]
    public void EmptySourceReturnsEmptyDictionary()
    {
        // Arrange
        var source = Array.Empty<int>();

        // Act
        var actual = source.AsValueEnumerable()
                           .AggregateBy(
                               x => x,
                               0,
                               (acc, x) => acc + x,
                               EqualityComparer<int>.Default)
                           .ToDictionary(x => x.Key, x => x.Value);

        // Assert
        actual.Count.ShouldBe(0);
    }
}
