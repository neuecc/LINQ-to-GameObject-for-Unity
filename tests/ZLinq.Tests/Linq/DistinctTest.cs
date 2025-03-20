namespace ZLinq.Tests.Linq;

public class DistinctTest
{
    [Fact]
    public void EmptySequence()
    {
        var empty = Array.Empty<int>();
        
        empty.AsValueEnumerable().Distinct().ToArray().ShouldBe(Array.Empty<int>());
        empty.ToValueEnumerable().Distinct().ToArray().ShouldBe(Array.Empty<int>());
    }

    [Fact]
    public void NoDuplicates()
    {
        var source = new[] { 1, 2, 3, 4, 5 };
        
        source.AsValueEnumerable().Distinct().ToArray().ShouldBe(source);
        source.ToValueEnumerable().Distinct().ToArray().ShouldBe(source);
    }

    [Fact]
    public void WithDuplicates()
    {
        var source = new[] { 1, 2, 3, 1, 2, 4, 5, 1 };
        var expected = new[] { 1, 2, 3, 4, 5 };
        
        source.AsValueEnumerable().Distinct().ToArray().ShouldBe(expected);
        source.ToValueEnumerable().Distinct().ToArray().ShouldBe(expected);
    }

    [Fact]
    public void ConsistentWithSystemLinq()
    {
        var source = new[] { 1, 3, 5, 7, 9, 3, 5, 7, 1, 9, 9 };
        
        var systemLinqResult = source.Distinct().ToArray();
        var zLinqResult = source.AsValueEnumerable().Distinct().ToArray();
        
        zLinqResult.ShouldBe(systemLinqResult);
    }

    [Fact]
    public void PreservesOrderOfFirstOccurrence()
    {
        var source = new[] { 3, 2, 1, 3, 2, 4, 1, 5 };
        var expected = new[] { 3, 2, 1, 4, 5 };
        
        source.AsValueEnumerable().Distinct().ToArray().ShouldBe(expected);
    }

    [Fact]
    public void WithCustomEqualityComparer()
    {
        var source = new[] { "A", "a", "B", "b", "C", "A" };
        
        // Case-sensitive (default)
        var defaultResult = source.AsValueEnumerable().Distinct().ToArray();
        defaultResult.ShouldBe(new[] { "A", "a", "B", "b", "C" });
        
        // Case-insensitive
        var caseInsensitiveResult = source.AsValueEnumerable()
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
        caseInsensitiveResult.ShouldBe(new[] { "A", "B", "C" });
        
        // Verify against System.Linq
        var systemLinqResult = source.Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
        caseInsensitiveResult.ShouldBe(systemLinqResult);
    }

    [Fact]
    public void WithCustomClass()
    {
        var p1 = new Person("John", 25);
        var p2 = new Person("Jane", 30);
        var p3 = new Person("John", 25); // Same as p1
        var p4 = new Person("Jane", 31); // Different age from p2
        
        var source = new[] { p1, p2, p3, p4 };
        
        // Default comparer uses reference equality
        var defaultResult = source.AsValueEnumerable().Distinct().ToArray();
        defaultResult.Length.ShouldBe(4); // All references are distinct
        
        // Custom comparer based on Name and Age
        var customResult = source.AsValueEnumerable()
            .Distinct(new PersonEqualityComparer())
            .ToArray();
        customResult.Length.ShouldBe(3); // p1 and p3 are considered equal
    }

    [Fact]
    public void WithNullValues()
    {
        var source = new string?[] { "A", null, "B", null, "C", "A" };
        var expected = new string?[] { "A", null, "B", "C" };
        
        source.AsValueEnumerable().Distinct().ToArray().ShouldBe(expected);
        source.ToValueEnumerable().Distinct().ToArray().ShouldBe(expected);
        
        // Verify against System.Linq
        var systemLinqResult = source.Distinct().ToArray();
        source.AsValueEnumerable().Distinct().ToArray().ShouldBe(systemLinqResult);
    }

    [Fact]
    public void ForEachIteration()
    {
        var source = new[] { 1, 2, 3, 1, 2, 4, 5, 1 };
        var expectedItems = new[] { 1, 2, 3, 4, 5 };
        
        var items = new List<int>();
        foreach (var item in source.AsValueEnumerable().Distinct())
        {
            items.Add(item);
        }
        
        items.ShouldBe(expectedItems);
    }

    [Fact]
    public void SpanOptimizationPath()
    {
        // Create a sequence that should use the TryGetNext path
        var sequence = new[] { 1, 2, 3, 1, 2, 4 };
        
        // HashSet is always used, so TryGetNonEnumeratedCount should return false
        sequence.AsValueEnumerable().Distinct()
            .TryGetNonEnumeratedCount(out var count).ShouldBeFalse();
        
        // TryGetSpan should return false since the result is dynamically built
        sequence.AsValueEnumerable().Distinct()
            .TryGetSpan(out var span).ShouldBeFalse();
    }

    [Fact]
    public void LargeDataSet()
    {
        // Test with a larger data set
        var random = new Random(42); // Fixed seed for reproducibility
        var source = Enumerable.Range(0, 1000)
            .Select(_ => random.Next(0, 500)) // Creates duplicates
            .ToArray();
        
        var systemLinqResult = source.Distinct().ToArray();
        var zLinqResult = source.AsValueEnumerable().Distinct().ToArray();
        
        zLinqResult.ShouldBe(systemLinqResult);
    }

    private class Person
    {
        public string Name { get; }
        public int Age { get; }

        public Person(string name, int age)
        {
            Name = name;
            Age = age;
        }
    }

    private class PersonEqualityComparer : IEqualityComparer<Person>
    {
        public bool Equals(Person? x, Person? y)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;
            return x.Name == y.Name && x.Age == y.Age;
        }

        public int GetHashCode(Person obj)
        {
            return HashCode.Combine(obj.Name, obj.Age);
        }
    }
}
