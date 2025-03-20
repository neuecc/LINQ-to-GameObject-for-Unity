namespace ZLinq.Tests.Linq;

public class ContainsTest
{
    [Fact]
    public void EmptyCollectionReturnsFalse()
    {
        var empty = Array.Empty<int>();

        // Value should never be found in empty collection
        empty.AsValueEnumerable().Contains(1).ShouldBeFalse();
        empty.ToValueEnumerable().Contains(1).ShouldBeFalse();
    }

    [Fact]
    public void SingleElementMatchExists()
    {
        var single = new[] { 42 };

        single.AsValueEnumerable().Contains(42).ShouldBeTrue();
        single.ToValueEnumerable().Contains(42).ShouldBeTrue();
    }

    [Fact]
    public void SingleElementNoMatch()
    {
        var single = new[] { 42 };

        single.AsValueEnumerable().Contains(43).ShouldBeFalse();
        single.ToValueEnumerable().Contains(43).ShouldBeFalse();
    }

    [Fact]
    public void MultipleElementsMatchExists()
    {
        var numbers = new[] { 1, 2, 3, 4, 5 };

        numbers.AsValueEnumerable().Contains(3).ShouldBeTrue();
        numbers.ToValueEnumerable().Contains(3).ShouldBeTrue();
    }

    [Fact]
    public void MultipleElementsNoMatch()
    {
        var numbers = new[] { 1, 2, 3, 4, 5 };

        numbers.AsValueEnumerable().Contains(6).ShouldBeFalse();
        numbers.ToValueEnumerable().Contains(6).ShouldBeFalse();
    }

    [Fact]
    public void SpanOptimizationUsed()
    {
        // Here we're testing the optimization path that uses Span directly
        var numbers = new[] { 1, 2, 3, 4, 5 };

        // We can only verify the result is correct, assuming optimization is used
        numbers.AsValueEnumerable().Contains(3).ShouldBeTrue();
        numbers.AsValueEnumerable().Contains(6).ShouldBeFalse();
    }

    [Fact]
    public void EnumeratorPathWorks()
    {
        // Use TestUtil to create a non-span enumerable to force enumerator path
        var numbers = TestUtil.ToValueEnumerable(new[] { 1, 2, 3, 4, 5 });

        numbers.Contains(3).ShouldBeTrue();
        numbers.Contains(6).ShouldBeFalse();
    }

    [Fact]
    public void MatchesSystemLinqBehavior()
    {
        var numbers = new[] { 1, 3, 5, 7, 9 };

        foreach (var value in new[] { 1, 3, 5, 7, 9, 0, 2, 4, 6, 8, 10 })
        {
            var expected = numbers.Contains(value);
            var actual = numbers.AsValueEnumerable().Contains(value);

            actual.ShouldBe(expected);
        }
    }

    [Fact]
    public void WithCustomEqualityComparer()
    {
        var strings = new[] { "apple", "banana", "Cherry" };

        // Case-sensitive (default)
        strings.AsValueEnumerable().Contains("cherry").ShouldBeFalse();

        // Case-insensitive 
        strings.AsValueEnumerable().Contains("cherry", StringComparer.OrdinalIgnoreCase).ShouldBeTrue();
        strings.AsValueEnumerable().Contains("grape", StringComparer.OrdinalIgnoreCase).ShouldBeFalse();

        // Verify against System.Linq
        strings.Contains("cherry", StringComparer.OrdinalIgnoreCase)
            .ShouldBe(strings.AsValueEnumerable().Contains("cherry", StringComparer.OrdinalIgnoreCase));
    }

    [Fact]
    public void WithCustomClass()
    {
        var p1 = new Person("John", 25);
        var p2 = new Person("Jane", 30);
        var p3 = new Person("Alice", 35);

        var people = new[] { p1, p2, p3 };

        // Default equality (reference equality)
        var differentJohn = new Person("John", 25);
        people.AsValueEnumerable().Contains(differentJohn).ShouldBeFalse();

        // Custom equality comparer
        var customComparer = new PersonEqualityComparer();
        people.AsValueEnumerable().Contains(differentJohn, customComparer).ShouldBeTrue();
        people.AsValueEnumerable().Contains(new Person("Bob", 40), customComparer).ShouldBeFalse();
    }

    [Fact]
    public void NullValueSupport()
    {
        var strings = new string?[] { "a", null, "b" };

        // Contains null
        strings.AsValueEnumerable().Contains(null).ShouldBeTrue();

        // Different collection without null
        var noNulls = new string?[] { "a", "b", "c" };
        noNulls.AsValueEnumerable().Contains(null).ShouldBeFalse();
    }

    [Fact]
    public void PrimitiveTypesSupport()
    {
        // Test various primitive types

        // Integers
        new[] { 1, 2, 3 }.AsValueEnumerable().Contains(2).ShouldBeTrue();

        // Doubles
        new[] { 1.0, 2.5, 3.7 }.AsValueEnumerable().Contains(2.5).ShouldBeTrue();

        // Booleans
        new[] { true, false }.AsValueEnumerable().Contains(true).ShouldBeTrue();

        // Characters
        new[] { 'a', 'b', 'c' }.AsValueEnumerable().Contains('b').ShouldBeTrue();

        // Bytes
        new byte[] { 1, 2, 3 }.AsValueEnumerable().Contains((byte)2).ShouldBeTrue();
    }

    [Fact]
    public void WorksWithLargeCollections()
    {
        var large = Enumerable.Range(0, 10000).ToArray();

        // First item
        large.AsValueEnumerable().Contains(0).ShouldBeTrue();

        // Middle item
        large.AsValueEnumerable().Contains(5000).ShouldBeTrue();

        // Last item
        large.AsValueEnumerable().Contains(9999).ShouldBeTrue();

        // Non-existent item
        large.AsValueEnumerable().Contains(10001).ShouldBeFalse();
    }

    [Fact]
    public void NoComparerDoesDeferToCollection()
    {
        IEnumerable<string> source = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "ABC" };
        var results1 = source.Contains("abc"); // true
        var results2 = source.AsValueEnumerable().Contains("abc");
        Assert.Equal(results1, results2);
    }

    [Fact]
    public void NoComparerDoesDeferToCollection2()
    {
        HashSet<string> source = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "ABC" };
        var results1 = source.Contains("abc"); // true
        var results2 = source.AsValueEnumerable().Contains("abc");
        Assert.Equal(results1, results2);
    }

    [Fact]
    public void NoComparerDoesDeferToCollection3()
    {
        string[] source = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "ABC" }.ToArray();
        var results1 = source.Contains("abc"); // false
        var results2 = source.AsValueEnumerable().Contains("abc");
        Assert.Equal(results1, results2);
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
