#if !NET48

namespace ZLinq.Tests.Linq;

public class DistinctByTest
{
    [Fact]
    public void EmptySequence()
    {
        var empty = Array.Empty<int>();

        empty.AsValueEnumerable().DistinctBy(x => x).ToArray().ShouldBe(Array.Empty<int>());
        empty.ToValueEnumerable().DistinctBy(x => x).ToArray().ShouldBe(Array.Empty<int>());
    }

    [Fact]
    public void NoKeyDuplicates()
    {
        var source = new[] { 1, 2, 3, 4, 5 };

        source.AsValueEnumerable().DistinctBy(x => x).ToArray().ShouldBe(source);
        source.ToValueEnumerable().DistinctBy(x => x).ToArray().ShouldBe(source);
    }

    [Fact]
    public void WithKeyDuplicates()
    {
        var source = new[] { 1, 2, 3, 1, 2, 4, 5, 1 };
        var expected = new[] { 1, 2, 3, 4, 5 };

        source.AsValueEnumerable().DistinctBy(x => x).ToArray().ShouldBe(expected);
        source.ToValueEnumerable().DistinctBy(x => x).ToArray().ShouldBe(expected);
    }

    [Fact]
    public void ConsistentWithSystemLinq()
    {
        var source = new[] { 1, 3, 5, 7, 9, 3, 5, 7, 1, 9, 9 };

        var systemLinqResult = source.DistinctBy(x => x).ToArray();
        var zLinqResult = source.AsValueEnumerable().DistinctBy(x => x).ToArray();

        zLinqResult.ShouldBe(systemLinqResult);
    }

    [Fact]
    public void PreservesOrderOfFirstOccurrence()
    {
        var source = new[] { 3, 2, 1, 3, 2, 4, 1, 5 };
        var expected = new[] { 3, 2, 1, 4, 5 };

        source.AsValueEnumerable().DistinctBy(x => x).ToArray().ShouldBe(expected);
    }

    [Fact]
    public void CustomKeySelector()
    {
        var source = new[]
        {
            new Person("John", 25),
            new Person("Jane", 30),
            new Person("Bob", 25),
            new Person("Alice", 30)
        };

        // Distinct by age
        var result = source.AsValueEnumerable()
            .DistinctBy(p => p.Age)
            .ToArray();

        result.Length.ShouldBe(2);
        result[0].Name.ShouldBe("John");
        result[1].Name.ShouldBe("Jane");
    }

    [Fact]
    public void WithCustomEqualityComparer()
    {
        var source = new[] { "A", "a", "B", "b", "C", "A" };

        // Case-sensitive (default)
        var defaultResult = source.AsValueEnumerable().DistinctBy(s => s).ToArray();
        defaultResult.ShouldBe(new[] { "A", "a", "B", "b", "C" });

        // Case-insensitive
        var caseInsensitiveResult = source.AsValueEnumerable()
            .DistinctBy(s => s, StringComparer.OrdinalIgnoreCase)
            .ToArray();
        caseInsensitiveResult.ShouldBe(new[] { "A", "B", "C" });

        // Verify against System.Linq
        var systemLinqResult = source.DistinctBy(s => s, StringComparer.OrdinalIgnoreCase).ToArray();
        caseInsensitiveResult.ShouldBe(systemLinqResult);
    }

    [Fact]
    public void WithReferenceTypeKeys()
    {
        var source = new[]
        {
            new Person("John", 25),
            new Person("Jane", 30),
            new Person("Alice", 35),
            new Person("Bob", 40)
        };

        // Using name as key
        var result = source.AsValueEnumerable()
            .DistinctBy(p => p.Name.Substring(0, 1)) // First letter of name
            .ToArray();

        result.Length.ShouldBe(3); // J, A, B are the distinct first letters
    }

    [Fact]
    public void WithNullableKeys()
    {
        var source = new[]
        {
            new Person("John", 25),
            new Person("Jane", null),
            new Person("Bob", 25),
            new Person("Alice", null)
        };

        // Distinct by nullable age
        var result = source.AsValueEnumerable()
            .DistinctBy(p => p.NullableAge)
            .ToArray();

        result.Length.ShouldBe(2); // One for age 25, one for null age
    }

    [Fact]
    public void WithNullKeys()
    {
        var source = new string?[] { "A", null, "B", null, "C", "A" };
        var result = source.AsValueEnumerable().DistinctBy(s => s).ToArray();

        result.Length.ShouldBe(4); // "A", null, "B", "C"
        result.ShouldBe(new string?[] { "A", null, "B", "C" });
    }

    [Fact]
    public void ForEachIteration()
    {
        var source = new[] {
            new Person("John", 25),
            new Person("Jane", 30),
            new Person("Bob", 25),
            new Person("Alice", 30)
        };

        var items = new List<Person>();
        foreach (var item in source.AsValueEnumerable().DistinctBy(p => p.Age))
        {
            items.Add(item);
        }

        items.Count.ShouldBe(2);
        items[0].Name.ShouldBe("John");
        items[1].Name.ShouldBe("Jane");
    }

    [Fact]
    public void SpanOptimizationPath()
    {
        // Create a sequence that should use the TryGetNext path
        var sequence = new[] { 1, 2, 3, 1, 2, 4 };

        // HashSet is always used, so TryGetNonEnumeratedCount should return false
        sequence.AsValueEnumerable().DistinctBy(x => x)
            .TryGetNonEnumeratedCount(out var count).ShouldBeFalse();

        // TryGetSpan should return false since the result is dynamically built
        sequence.AsValueEnumerable().DistinctBy(x => x)
            .TryGetSpan(out var span).ShouldBeFalse();
    }

    [Fact]
    public void LargeDataSet()
    {
        // Test with a larger data set
        var random = new Random(42); // Fixed seed for reproducibility
        var source = Enumerable.Range(0, 1000)
            .Select(i => new Person($"Person{i}", random.Next(0, 50))) // Creates duplicate ages
            .ToArray();

        var systemLinqResult = source.DistinctBy(p => p.Age).Count();
        var zLinqResult = source.AsValueEnumerable().DistinctBy(p => p.Age).Count();

        zLinqResult.ShouldBe(systemLinqResult);
    }

    [Fact]
    public void ComplexKeySelector()
    {
        var source = new[]
        {
            new Person("John Doe", 25),
            new Person("Jane Smith", 30),
            new Person("John Smith", 35),
            new Person("Jane Doe", 41)
        };

        // Use tuple as composite key (first name, age parity)
        var result = source.AsValueEnumerable()
            .DistinctBy(p => (p.Name.Split(' ')[0], p.Age % 2 == 0))
            .ToArray();

        result.Length.ShouldBe(3); // John+odd, Jane+even, Jane+odd
    }

    [Fact]
    public void DistinctByFollowedByOtherOperations()
    {
        var source = new[] { 1, 2, 3, 1, 2, 4, 5, 1 };

        // DistinctBy followed by Where
        var result1 = source.AsValueEnumerable()
            .DistinctBy(x => x)
            .Where(x => x % 2 == 0)
            .ToArray();

        result1.ShouldBe(new[] { 2, 4 });

        // DistinctBy followed by Select
        var result2 = source.AsValueEnumerable()
            .DistinctBy(x => x)
            .Select(x => x * 10)
            .ToArray();

        result2.ShouldBe(new[] { 10, 20, 30, 40, 50 });
    }

    private class Person
    {
        public string Name { get; }
        public int Age { get; }
        public int? NullableAge { get; }

        public Person(string name, int age)
        {
            Name = name;
            Age = age;
            NullableAge = age;
        }

        public Person(string name, int? age)
        {
            Name = name;
            Age = age ?? 0;
            NullableAge = age;
        }
    }
}

#endif
