#pragma warning disable
namespace ZLinq.Tests.Linq;

public class MaxTest
{
    // Tests for empty collections
    [Fact]
    public void EmptyIntSequenceThrows()
    {
        var empty = Array.Empty<int>();

        TestUtil.Throws<InvalidOperationException>(
            () => empty.Max(),
            () => empty.AsValueEnumerable().Max());

        TestUtil.Throws<InvalidOperationException>(
            () => empty.Max(),
            () => empty.ToValueEnumerable().Max());
    }

    [Fact]
    public void EmptyReferenceTypeSequenceReturnsNull()
    {
        var empty = Array.Empty<string>();

        empty.Max().ShouldBeNull();
        empty.AsValueEnumerable().Max().ShouldBeNull();
        empty.ToValueEnumerable().Max().ShouldBeNull();
    }

    // Test basic functionality with different numeric types
    [Fact]
    public void IntegerSource()
    {
        var ints = new[] { 5, 3, 8, 2, 10 };

        ints.AsValueEnumerable().Max().ShouldBe(ints.Max());
        ints.ToValueEnumerable().Max().ShouldBe(ints.Max());
    }

    [Fact]
    public void DecimalSource()
    {
        var decimals = new[] { 5.5m, 3.3m, 8.8m, 2.2m, 10.1m };

        decimals.AsValueEnumerable().Max().ShouldBe(decimals.Max());
        decimals.ToValueEnumerable().Max().ShouldBe(decimals.Max());
    }

    [Fact]
    public void DoubleSource()
    {
        var doubles = new[] { 5.5, 3.3, 8.8, 2.2, 10.1 };

        doubles.AsValueEnumerable().Max().ShouldBe(doubles.Max());
        doubles.ToValueEnumerable().Max().ShouldBe(doubles.Max());
    }

    [Fact]
    public void FloatSource()
    {
        var floats = new[] { 5.5f, 3.3f, 8.8f, 2.2f, 10.1f };

        floats.AsValueEnumerable().Max().ShouldBe(floats.Max());
        floats.ToValueEnumerable().Max().ShouldBe(floats.Max());
    }

    [Fact]
    public void LongSource()
    {
        var longs = new[] { 5L, 3L, 8L, 2L, 10L };

        longs.AsValueEnumerable().Max().ShouldBe(longs.Max());
        longs.ToValueEnumerable().Max().ShouldBe(longs.Max());
    }

    // Test for reference types
    [Fact]
    public void StringSource()
    {
        var strings = new[] { "apple", "banana", "kiwi", "pineapple", "grape" };

        strings.AsValueEnumerable().Max().ShouldBe(strings.Max());
        strings.ToValueEnumerable().Max().ShouldBe(strings.Max());
    }

    // Test custom comparer
    [Fact]
    public void CustomComparer()
    {
        var strings = new[] { "APPLE", "banana", "Cherry", "DATE" };
        var comparer = StringComparer.OrdinalIgnoreCase;

        // With case-insensitive comparison
        strings.AsValueEnumerable().Max(comparer).ShouldBe(strings.Max(comparer));
        strings.ToValueEnumerable().Max(comparer).ShouldBe(strings.Max(comparer));
    }

    // Test null handling
    [Fact]
    public void NullHandling()
    {
        var stringsWithNull = new[] { "banana", null, "apple", "cherry", null };

        stringsWithNull.AsValueEnumerable().Max().ShouldBe(stringsWithNull.Max());
        stringsWithNull.ToValueEnumerable().Max().ShouldBe(stringsWithNull.Max());

        // Collection with all null values should return null
        var allNull = new string[] { null, null, null };
        allNull.AsValueEnumerable().Max().ShouldBeNull();
        allNull.ToValueEnumerable().Max().ShouldBeNull();
    }

    // Test edge cases
    [Fact]
    public void SingleElementCollection()
    {
        var singleInt = new[] { 42 };
        singleInt.AsValueEnumerable().Max().ShouldBe(42);
        singleInt.ToValueEnumerable().Max().ShouldBe(42);

        var singleString = new[] { "hello" };
        singleString.AsValueEnumerable().Max().ShouldBe("hello");
        singleString.ToValueEnumerable().Max().ShouldBe("hello");
    }

    [Fact]
    public void DuplicateMaxValues()
    {
        var items = new[] { 5, 10, 3, 10, 7 }; // 10 appears twice

        items.AsValueEnumerable().Max().ShouldBe(10);
        items.ToValueEnumerable().Max().ShouldBe(10);
    }

    // Test SIMD-supported types
    [Fact]
    public void SimdSupportedTypes()
    {
        // Byte
        var bytes = new byte[] { 5, 3, 8, 2, 10 };
        bytes.AsValueEnumerable().Max().ShouldBe(bytes.Max());

        // Short
        var shorts = new short[] { 5, 3, 8, 2, 10 };
        shorts.AsValueEnumerable().Max().ShouldBe(shorts.Max());

        // UShort
        var ushorts = new ushort[] { 5, 3, 8, 2, 10 };
        ushorts.AsValueEnumerable().Max().ShouldBe(ushorts.Max());

        // Int
        var ints = new int[] { 5, 3, 8, 2, 10 };
        ints.AsValueEnumerable().Max().ShouldBe(ints.Max());

        // UInt
        var uints = new uint[] { 5, 3, 8, 2, 10 };
        uints.AsValueEnumerable().Max().ShouldBe(uints.Max());

        // Long
        var longs = new long[] { 5, 3, 8, 2, 10 };
        longs.AsValueEnumerable().Max().ShouldBe(longs.Max());

        // ULong
        var ulongs = new ulong[] { 5, 3, 8, 2, 10 };
        ulongs.AsValueEnumerable().Max().ShouldBe(ulongs.Max());

        // Char(is not supported SIMD)
        var chars = new char[] { 'a', 'c', 'b', 'z', 'd' };
        chars.AsValueEnumerable().Max().ShouldBe(chars.Max());
    }

    // Test with larger collections to verify performance characteristics
    [Fact]
    public void LargeCollection()
    {
        var random = new Random(42);
        var data = new int[1000];

        for (int i = 0; i < data.Length; i++)
        {
            data[i] = random.Next(-10000, 10000);
        }

        var expected = data.Max();
        var actual = data.AsValueEnumerable().Max();

        actual.ShouldBe(expected);
    }

    // Test negative values
    [Fact]
    public void NegativeValues()
    {
        var negatives = new[] { -5, -3, -8, -2, -10 };

        negatives.AsValueEnumerable().Max().ShouldBe(negatives.Max());
        negatives.ToValueEnumerable().Max().ShouldBe(negatives.Max());
    }

    // Test mixed positive and negative values
    [Fact]
    public void MixedValues()
    {
        var mixed = new[] { -5, 3, -8, 2, -10 };

        mixed.AsValueEnumerable().Max().ShouldBe(mixed.Max());
        mixed.ToValueEnumerable().Max().ShouldBe(mixed.Max());
    }

    // Test custom type with default comparer
    [Fact]
    public void CustomTypeWithDefaultComparer()
    {
        var people = new[] {
            new Person("Alice", 25),
            new Person("Bob", 30),
            new Person("Charlie", 22),
            new Person("David", 35)
        };

        // Using an IComparable implementation
        var comparablePeople = new[] {
            new ComparablePerson("Alice", 25),
            new ComparablePerson("Bob", 30),
            new ComparablePerson("Charlie", 22),
            new ComparablePerson("David", 35)
        };

        comparablePeople.AsValueEnumerable().Max().Name.ShouldBe("David");
        comparablePeople.ToValueEnumerable().Max().Name.ShouldBe("David");
    }

    // Test custom type with custom comparer
    [Fact]
    public void CustomTypeWithCustomComparer()
    {
        var people = new[] {
            new Person("Alice", 25),
            new Person("Bob", 30),
            new Person("Charlie", 22),
            new Person("David", 35)
        };

        var ageComparer = new PersonAgeComparer();

        var maxByAge = people.AsValueEnumerable().Max(ageComparer);
        maxByAge.Age.ShouldBe(35);
        maxByAge.Name.ShouldBe("David");
    }

    // Non-comparable class for testing
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

    // IComparable implementation for testing
    private class ComparablePerson : IComparable<ComparablePerson>
    {
        public string Name { get; }
        public int Age { get; }

        public ComparablePerson(string name, int age)
        {
            Name = name;
            Age = age;
        }

        public int CompareTo(ComparablePerson? other)
        {
            if (other == null) return 1;
            return Age.CompareTo(other.Age);
        }
    }

    // Custom comparer for Person class
    private class PersonAgeComparer : IComparer<Person>
    {
        public int Compare(Person? x, Person? y)
        {
            if (x == null && y == null) return 0;
            if (x == null) return -1;
            if (y == null) return 1;

            return x.Age.CompareTo(y.Age);
        }
    }
}
