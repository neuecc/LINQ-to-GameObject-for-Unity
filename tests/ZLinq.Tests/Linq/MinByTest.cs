namespace ZLinq.Tests.Linq;

public class MinByTest
{
    // Tests for empty collections - should throw NoElements exception for value types
    [Fact]
    public void EmptySequenceThrows()
    {
        var empty = Array.Empty<int>();

        TestUtil.Throws<InvalidOperationException>(
            () => empty.MinBy(x => x),
            () => empty.AsValueEnumerable().MinBy(x => x));

        TestUtil.Throws<InvalidOperationException>(
            () => empty.MinBy(x => x),
            () => empty.ToValueEnumerable().MinBy(x => x));
    }

    [Fact]
    public void EmptyReferenceTypeSequenceReturnsNull()
    {
        var empty = Array.Empty<string>();

        empty.MinBy(x => x.Length).ShouldBeNull();
        empty.AsValueEnumerable().MinBy(x => x.Length).ShouldBeNull();
        empty.ToValueEnumerable().MinBy(x => x.Length).ShouldBeNull();
    }

    // Test basic functionality with different numeric keys
    [Fact]
    public void MinByInteger()
    {
        var people = new[]
        {
            new Person("Alice", 30),
            new Person("Bob", 25),
            new Person("Charlie", 35),
            new Person("David", 20),
            new Person("Eve", 40)
        };

        var expected = people.MinBy(p => p.Age);
        people.AsValueEnumerable().MinBy(p => p.Age).ShouldBe(expected);
        people.ToValueEnumerable().MinBy(p => p.Age).ShouldBe(expected);
    }

    [Fact]
    public void MinByString()
    {
        var people = new[]
        {
            new Person("Charlie", 35),
            new Person("Bob", 25),
            new Person("Alice", 30),
            new Person("Eve", 40),
            new Person("David", 20)
        };

        var expected = people.MinBy(p => p.Name);
        people.AsValueEnumerable().MinBy(p => p.Name).ShouldBe(expected);
        people.ToValueEnumerable().MinBy(p => p.Name).ShouldBe(expected);
    }

    // Test different key types
    [Fact]
    public void MinByWithFloatKey()
    {
        var items = new[]
        {
            new Item("Apple", 1.5f),
            new Item("Banana", 0.5f),
            new Item("Cherry", 2.5f),
            new Item("Date", 1.0f)
        };

        var expected = items.MinBy(i => i.Weight);
        items.AsValueEnumerable().MinBy(i => i.Weight).ShouldBe(expected);
        items.ToValueEnumerable().MinBy(i => i.Weight).ShouldBe(expected);
    }

    [Fact]
    public void MinByWithCustomStructKey()
    {
        var items = new[]
        {
            new Item("Item1", new CustomKey(5, 10)),
            new Item("Item2", new CustomKey(3, 20)),
            new Item("Item3", new CustomKey(8, 30)),
            new Item("Item4", new CustomKey(2, 40))
        };

        var expected = items.MinBy(i => i.CustomKeyValue);
        items.AsValueEnumerable().MinBy(i => i.CustomKeyValue).ShouldBe(expected);
        items.ToValueEnumerable().MinBy(i => i.CustomKeyValue).ShouldBe(expected);
    }

    // Test null handling in source sequence
    [Fact]
    public void NullHandlingInSource()
    {
        var stringsWithNull = new string?[] { "banana", null, "apple", "cherry", null };

        // When source contains nulls, MinBy should ignore null values
        var expected = stringsWithNull.MinBy(s => s?.Length ?? int.MaxValue);
        stringsWithNull.AsValueEnumerable().MinBy(s => s?.Length ?? int.MaxValue).ShouldBe(expected);
        stringsWithNull.ToValueEnumerable().MinBy(s => s?.Length ?? int.MaxValue).ShouldBe(expected);
    }

    // Test null handling in key selector results
    [Fact]
    public void NullHandlingInKeys()
    {
        var items = new[]
        {
            new NullableItem("Item1", "KeyA"),
            new NullableItem("Item2", null),
            new NullableItem("Item3", "KeyB"),
            new NullableItem("Item4", null),
            new NullableItem("Item5", "KeyC")
        };

        // MinBy should skip items with null keys and return the first item with non-null minimum key
        var expected = items.MinBy(i => i.NullableKey);
        items.AsValueEnumerable().MinBy(i => i.NullableKey).ShouldBe(expected);
        items.ToValueEnumerable().MinBy(i => i.NullableKey).ShouldBe(expected);
    }

#if NET9_0_OR_GREATER

    // after .NET 7 changes sematnics
    // https://github.com/dotnet/runtime/pull/61364

    [Fact]
    public void AllNullKeys()
    {
        var items = new[]
        {
            new NullableItem("Item1", null),
            new NullableItem("Item2", null),
            new NullableItem("Item3", null)
        };

        // Should return the first item when all keys are null
        var expected = items.MinBy(i => i.NullableKey);
        var actual1 = items.AsValueEnumerable().MinBy(i => i.NullableKey);
        actual1!.Name.ShouldBe(expected!.Name);
        var actual2 = items.ToValueEnumerable().MinBy(i => i.NullableKey);
        actual2!.Name.ShouldBe(expected!.Name);
    }
#endif

    // Test custom comparer
    [Fact]
    public void CustomComparer()
    {
        var people = new[]
        {
            new Person("ALICE", 30),
            new Person("bob", 25),
            new Person("Charlie", 35)
        };

        var comparer = StringComparer.OrdinalIgnoreCase;

        // With case-insensitive comparison, "ALICE" should be the min value
        var expected = people.MinBy(p => p.Name, comparer);
        people.AsValueEnumerable().MinBy(p => p.Name, comparer).ShouldBe(expected);
        people.ToValueEnumerable().MinBy(p => p.Name, comparer).ShouldBe(expected);
    }

    // Test edge cases
    [Fact]
    public void SingleElementCollection()
    {
        var singlePerson = new[] { new Person("Alice", 30) };

        var expected = singlePerson.MinBy(p => p.Age);
        singlePerson.AsValueEnumerable().MinBy(p => p.Age).ShouldBe(expected);
        singlePerson.ToValueEnumerable().MinBy(p => p.Age).ShouldBe(expected);
    }

    [Fact]
    public void DuplicateMinimumKeys()
    {
        var people = new[]
        {
            new Person("Alice", 25),
            new Person("Bob", 30),
            new Person("Charlie", 25),  // Same minimum age as Alice
            new Person("David", 40)
        };

        // Should return the first person with the minimum age (Alice)
        var expected = people.MinBy(p => p.Age);
        people.AsValueEnumerable().MinBy(p => p.Age).ShouldBe(expected);
        people.ToValueEnumerable().MinBy(p => p.Age).ShouldBe(expected);
    }

    [Fact]
    public void ArgumentNullExceptions()
    {
        // Source is not null, but selector is null
        var collection = new[] { 1, 2, 3 };
        Func<int, int> selector = null!;

        // Should throw ArgumentNullException
        Should.Throw<ArgumentNullException>(() => collection.AsValueEnumerable().MinBy(selector));
        Should.Throw<ArgumentNullException>(() => collection.ToValueEnumerable().MinBy(selector));

        // Source is not null, selector is not null, but comparer is null - should NOT throw
        // (comparer parameter is nullable and defaults to Comparer<T>.Default when null)
        Should.NotThrow(() => collection.AsValueEnumerable().MinBy(x => x, null));
        Should.NotThrow(() => collection.ToValueEnumerable().MinBy(x => x, null));
    }

    // Test for performance with larger collections - not testing actual performance,
    // just ensuring the method works correctly with larger data sets
    [Fact]
    public void LargeCollection()
    {
        var count = 1000;
        var random = new Random(42);

        var people = new Person[count];
        for (int i = 0; i < count; i++)
        {
            people[i] = new Person($"Person{i}", random.Next(18, 80));
        }

        var expected = people.MinBy(p => p.Age);
        people.AsValueEnumerable().MinBy(p => p.Age).ShouldBe(expected);
        people.ToValueEnumerable().MinBy(p => p.Age).ShouldBe(expected);
    }

    // Helper classes for testing
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

    private class Item
    {
        public string Name { get; }
        public float Weight { get; }
        public CustomKey CustomKeyValue { get; }

        public Item(string name, float weight)
        {
            Name = name;
            Weight = weight;
        }

        public Item(string name, CustomKey customKey)
        {
            Name = name;
            CustomKeyValue = customKey;
        }
    }

    private record NullableItem
    {
        public string Name { get; }
        public string? NullableKey { get; }

        public NullableItem(string name, string? nullableKey)
        {
            Name = name;
            NullableKey = nullableKey;
        }
    }

    private struct CustomKey : IComparable<CustomKey>
    {
        public int Primary { get; }
        public int Secondary { get; }

        public CustomKey(int primary, int secondary)
        {
            Primary = primary;
            Secondary = secondary;
        }

        public int CompareTo(CustomKey other)
        {
            var primaryComparison = Primary.CompareTo(other.Primary);
            return primaryComparison != 0 ? primaryComparison : Secondary.CompareTo(other.Secondary);
        }
    }
}
