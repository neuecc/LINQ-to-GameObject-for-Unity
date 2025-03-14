namespace ZLinq.Tests.Linq;

public class CountByTest
{
    [Fact]
    public void BasicCountByTest()
    {
        // Arrange
        var source = new[] { 1, 2, 3, 4, 5, 6 };
        var expected = new Dictionary<bool, int>
        {
            [true] = 3,   // Count of odd numbers: 1, 3, 5
            [false] = 3   // Count of even numbers: 2, 4, 6
        };
        Enumerable.Range(1, 10).Index();
        // Act
        var actual = source.AsValueEnumerable()
                          .CountBy(x => x % 2 == 1)   // Key selector: odd/even
                          .ToDictionary(x => x.Key, x => x.Value);

        // Assert
        actual.Count.ShouldBe(expected.Count);
        actual[true].ShouldBe(expected[true]);
        actual[false].ShouldBe(expected[false]);
    }

    [Fact]
    public void CountByWithCustomComparer()
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
                          .CountBy(x => x, comparer)
                          .ToDictionary(x => x.Key, x => x.Value, comparer);

        // Assert
        actual.Count.ShouldBe(expected.Count);
        foreach (var key in expected.Keys)
        {
            actual[key].ShouldBe(expected[key]);
        }
    }

    [Fact]
    public void EmptySourceReturnsEmptyResult()
    {
        // Arrange
        var source = Array.Empty<int>();

        // Act
        var actual = source.AsValueEnumerable()
                          .CountBy(x => x)
                          .ToDictionary(x => x.Key, x => x.Value);

        // Assert
        actual.Count.ShouldBe(0);
    }

    [Fact]
    public void ManyDuplicateKeys()
    {
        // Arrange
        var source = new[] { 1, 2, 1, 3, 2, 1, 4, 2, 5, 1 };
        var expected = new Dictionary<int, int>
        {
            [1] = 4,  // 1 appears 4 times
            [2] = 3,  // 2 appears 3 times
            [3] = 1,  // 3 appears once
            [4] = 1,  // 4 appears once
            [5] = 1   // 5 appears once
        };

        // Act
        var actual = source.AsValueEnumerable()
                          .CountBy(x => x)
                          .ToDictionary(x => x.Key, x => x.Value);

        // Assert
        actual.Count.ShouldBe(expected.Count);
        foreach (var key in expected.Keys)
        {
            actual[key].ShouldBe(expected[key]);
        }
    }

    [Fact]
    public void ReferenceTypeKeys()
    {
        // Arrange
        var person1 = new Person { Age = 25 };
        var person2 = new Person { Age = 30 };
        var person3 = new Person { Age = 25 };

        var source = new[] { person1, person2, person3 };

        // Act
        var actual = source.AsValueEnumerable()
                          .CountBy(x => x.Age)
                          .ToDictionary(x => x.Key, x => x.Value);

        // Assert
        actual.Count.ShouldBe(2);
        actual[25].ShouldBe(2); // Two people aged 25
        actual[30].ShouldBe(1); // One person aged 30
    }

    [Fact]
    public void UseWithNonSpanOptimizableCollection()
    {
        // Arrange
        var source = new List<string> { "a", "b", "a", "c", "b", "a" };

        // Use ToIterableValueEnumerable to force the non-span code path
        var actual = source.ToValueEnumerable().CountBy(x => x)
                              .ToDictionary(x => x.Key, x => x.Value);

        // Assert
        actual.Count.ShouldBe(3);
        actual["a"].ShouldBe(3);
        actual["b"].ShouldBe(2);
        actual["c"].ShouldBe(1);
    }

    [Fact]
    public void KeySelectorThrowsException()
    {
        // Arrange
        var source = new[] { 1, 2, 3, 4, 0 }; // Will cause division by zero

        // Act & Assert
        Assert.Throws<DivideByZeroException>(() =>
        {
            var result = source.AsValueEnumerable()
                              .CountBy(x => 10 / x) // Will throw when x = 0
                              .ToDictionary(x => x.Key, x => x.Value);
        });
    }

    public class Person
    {
        public int Age { get; set; }
    }
}
