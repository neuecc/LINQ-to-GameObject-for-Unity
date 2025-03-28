#if !NET48

#pragma warning disable
namespace ZLinq.Tests.Linq;

public class MaxByTest
{
    // Tests for empty collections
    [Fact]
    public void EmptyIntSequenceThrows()
    {
        var empty = Array.Empty<int>();

        TestUtil.Throws<InvalidOperationException>(
            () => empty.MaxBy(x => x),
            () => empty.AsValueEnumerable().MaxBy(x => x));

        TestUtil.Throws<InvalidOperationException>(
            () => empty.MaxBy(x => x),
            () => empty.ToValueEnumerable().MaxBy(x => x));
    }

    [Fact]
    public void EmptyReferenceTypeSequenceReturnsNull()
    {
        var empty = Array.Empty<string>();

        empty.MaxBy(x => x.Length).ShouldBeNull();
        empty.AsValueEnumerable().MaxBy(x => x.Length).ShouldBeNull();
        empty.ToValueEnumerable().MaxBy(x => x.Length).ShouldBeNull();
    }

    // Test basic functionality with value types as source
    [Fact]
    public void IntegerSourceWithIntegerKey()
    {
        var ints = new[] { 5, 3, 8, 2, 10 };

        ints.AsValueEnumerable().MaxBy(x => x).ShouldBe(ints.MaxBy(x => x));
        ints.ToValueEnumerable().MaxBy(x => x).ShouldBe(ints.MaxBy(x => x));

        // Using a different key selector
        ints.AsValueEnumerable().MaxBy(x => -x).ShouldBe(ints.MaxBy(x => -x));
        ints.ToValueEnumerable().MaxBy(x => -x).ShouldBe(ints.MaxBy(x => -x));
    }

    [Fact]
    public void IntegerSourceWithStringKey()
    {
        var ints = new[] { 5, 3, 8, 2, 10 };

        // Using string length as key (all same length)
        ints.AsValueEnumerable().MaxBy(x => x.ToString()).ShouldBe(ints.MaxBy(x => x.ToString()));
        ints.ToValueEnumerable().MaxBy(x => x.ToString()).ShouldBe(ints.MaxBy(x => x.ToString()));
    }

    // Test with reference types as source
    [Fact]
    public void StringSourceWithIntegerKey()
    {
        var strings = new[] { "apple", "banana", "kiwi", "pineapple", "grape" };

        // Using string length as key
        strings.AsValueEnumerable().MaxBy(x => x.Length).ShouldBe(strings.MaxBy(x => x.Length));
        strings.ToValueEnumerable().MaxBy(x => x.Length).ShouldBe(strings.MaxBy(x => x.Length));
    }

    [Fact]
    public void StringSourceWithStringKey()
    {
        var strings = new[] { "apple", "banana", "kiwi", "pineapple", "grape" };

        // Using the string itself as key
        strings.AsValueEnumerable().MaxBy(x => x).ShouldBe(strings.MaxBy(x => x));
        strings.ToValueEnumerable().MaxBy(x => x).ShouldBe(strings.MaxBy(x => x));

        // Using first character as key
        strings.AsValueEnumerable().MaxBy(x => x.First()).ShouldBe(strings.MaxBy(x => x.First()));
        strings.ToValueEnumerable().MaxBy(x => x.First()).ShouldBe(strings.MaxBy(x => x.First()));
    }

    // Test custom comparer
    [Fact]
    public void CustomComparer()
    {
        var strings = new[] { "APPLE", "banana", "Cherry", "DATE" };
        var comparer = StringComparer.OrdinalIgnoreCase;

        // With case-insensitive comparison
        strings.AsValueEnumerable().MaxBy(x => x, comparer).ShouldBe(strings.MaxBy(x => x, comparer));
        strings.ToValueEnumerable().MaxBy(x => x, comparer).ShouldBe(strings.MaxBy(x => x, comparer));
    }

    // Test null handling
    [Fact]
    public void NullHandling()
    {
        var stringsWithNull = new[] { "banana", null, "apple", "cherry", null };

        // Key selector that handles nulls
        stringsWithNull.AsValueEnumerable().MaxBy(x => x ?? "").ShouldBe(stringsWithNull.MaxBy(x => x ?? ""));
        stringsWithNull.ToValueEnumerable().MaxBy(x => x ?? "").ShouldBe(stringsWithNull.MaxBy(x => x ?? ""));

        // Test when keys are null
        var objects = new[] {
            new { Name = "A", Value = "High" },
            new { Name = "B", Value = null as string },
            new { Name = "C", Value = "Medium" }
        };

        // Should return first non-null key's item or first item if all keys are null
        objects.AsValueEnumerable().MaxBy(x => x.Value).ShouldBe(objects.MaxBy(x => x.Value));
        objects.ToValueEnumerable().MaxBy(x => x.Value).ShouldBe(objects.MaxBy(x => x.Value));
    }

    // Test edge cases
    [Fact]
    public void SingleElementCollection()
    {
        var singleInt = new[] { 42 };
        singleInt.AsValueEnumerable().MaxBy(x => x).ShouldBe(42);
        singleInt.ToValueEnumerable().MaxBy(x => x).ShouldBe(42);

        var singleString = new[] { "hello" };
        singleString.AsValueEnumerable().MaxBy(x => x.Length).ShouldBe("hello");
        singleString.ToValueEnumerable().MaxBy(x => x.Length).ShouldBe("hello");
    }

    [Fact]
    public void TieBreaking()
    {
        // When multiple elements have the same max key, the first one should be returned
        var items = new[] {
            new { Name = "A", Value = 5 },
            new { Name = "B", Value = 10 },
            new { Name = "C", Value = 10 },  // Same value as B
            new { Name = "D", Value = 3 }
        };

        var expected = items.MaxBy(x => x.Value);
        var actual = items.AsValueEnumerable().MaxBy(x => x.Value);

        actual.ShouldBe(expected);
        actual.Name.ShouldBe("B"); // Should be "B" not "C"
    }

    [Fact]
    public void CustomType()
    {
        var people = new[] {
            new Person("Alice", 25),
            new Person("Bob", 30),
            new Person("Charlie", 22),
            new Person("David", 35)
        };

        // Find person with maximum age
        people.AsValueEnumerable().MaxBy(p => p.Age).Name.ShouldBe("David");
        people.ToValueEnumerable().MaxBy(p => p.Age).Name.ShouldBe("David");

        // Find person with name that comes last alphabetically
        people.AsValueEnumerable().MaxBy(p => p.Name).Name.ShouldBe("David");
        people.ToValueEnumerable().MaxBy(p => p.Name).Name.ShouldBe("David");
    }

    // Test with large collection to verify performance characteristics
    [Fact]
    public void LargeCollection()
    {
        var random = new Random(42);
        var data = new int[1000];

        for (int i = 0; i < data.Length; i++)
        {
            data[i] = random.Next(-10000, 10000);
        }

        var expected = data.MaxBy(x => x);
        var actual = data.AsValueEnumerable().MaxBy(x => x);

        actual.ShouldBe(expected);
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
}

#endif
