namespace ZLinq.Tests.Linq;

public class SequenceEqualTest
{
    [Fact]
    public void EmptySequences()
    {
        var empty1 = Array.Empty<int>();
        var empty2 = Array.Empty<int>();

        // Both empty - should be equal
        empty1.AsValueEnumerable().SequenceEqual(empty2).ShouldBeTrue();
        empty1.AsValueEnumerable().SequenceEqual(empty2.AsValueEnumerable()).ShouldBeTrue();
        
        // Test with ToValueEnumerable variant
        empty1.ToValueEnumerable().SequenceEqual(empty2).ShouldBeTrue();
        empty1.ToValueEnumerable().SequenceEqual(empty2.AsValueEnumerable()).ShouldBeTrue();
        
        // Verify against System.Linq
        bool linqResult = empty1.SequenceEqual(empty2);
        empty1.AsValueEnumerable().SequenceEqual(empty2).ShouldBe(linqResult);
    }

    [Fact]
    public void EmptyVsNonEmpty()
    {
        var empty = Array.Empty<int>();
        var nonEmpty = new[] { 1, 2, 3 };

        // Empty vs non-empty - should be false
        empty.AsValueEnumerable().SequenceEqual(nonEmpty).ShouldBeFalse();
        nonEmpty.AsValueEnumerable().SequenceEqual(empty).ShouldBeFalse();
        
        empty.AsValueEnumerable().SequenceEqual(nonEmpty.AsValueEnumerable()).ShouldBeFalse();
        nonEmpty.AsValueEnumerable().SequenceEqual(empty.AsValueEnumerable()).ShouldBeFalse();

        // Verify against System.Linq
        var linqResult = empty.SequenceEqual(nonEmpty);
        empty.AsValueEnumerable().SequenceEqual(nonEmpty).ShouldBe(linqResult);
    }

    [Fact]
    public void EqualNonEmptySequences()
    {
        var sequence1 = new[] { 1, 2, 3, 4, 5 };
        var sequence2 = new[] { 1, 2, 3, 4, 5 };
        var sequence3 = new List<int> { 1, 2, 3, 4, 5 };

        // Arrays - should be equal
        sequence1.AsValueEnumerable().SequenceEqual(sequence2).ShouldBeTrue();
        sequence1.AsValueEnumerable().SequenceEqual(sequence2.AsValueEnumerable()).ShouldBeTrue();
        
        // Different collection types - should still be equal
        sequence1.AsValueEnumerable().SequenceEqual(sequence3).ShouldBeTrue();
        sequence1.AsValueEnumerable().SequenceEqual(sequence3.AsValueEnumerable()).ShouldBeTrue();
        
        // Verify against System.Linq
        var linqResult = sequence1.SequenceEqual(sequence3);
        sequence1.AsValueEnumerable().SequenceEqual(sequence3).ShouldBe(linqResult);
    }

    [Fact]
    public void DifferentLengthSequences()
    {
        var shorter = new[] { 1, 2, 3 };
        var longer = new[] { 1, 2, 3, 4, 5 };

        // Different lengths - should be false
        shorter.AsValueEnumerable().SequenceEqual(longer).ShouldBeFalse();
        longer.AsValueEnumerable().SequenceEqual(shorter).ShouldBeFalse();
        
        shorter.AsValueEnumerable().SequenceEqual(longer.AsValueEnumerable()).ShouldBeFalse();
        longer.AsValueEnumerable().SequenceEqual(shorter.AsValueEnumerable()).ShouldBeFalse();

        // Verify against System.Linq
        var linqResult = shorter.SequenceEqual(longer);
        shorter.AsValueEnumerable().SequenceEqual(longer).ShouldBe(linqResult);
    }

    [Fact]
    public void SameLengthDifferentValues()
    {
        var sequence1 = new[] { 1, 2, 3, 4, 5 };
        var sequence2 = new[] { 1, 2, 3, 4, 6 }; // Last element differs
        var sequence3 = new[] { 0, 2, 3, 4, 5 }; // First element differs
        var sequence4 = new[] { 1, 2, 7, 4, 5 }; // Middle element differs

        sequence1.AsValueEnumerable().SequenceEqual(sequence2).ShouldBeFalse();
        sequence1.AsValueEnumerable().SequenceEqual(sequence3).ShouldBeFalse();
        sequence1.AsValueEnumerable().SequenceEqual(sequence4).ShouldBeFalse();

        // Using ValueEnumerable overload
        sequence1.AsValueEnumerable().SequenceEqual(sequence2.AsValueEnumerable()).ShouldBeFalse();
        sequence1.AsValueEnumerable().SequenceEqual(sequence3.AsValueEnumerable()).ShouldBeFalse();
        sequence1.AsValueEnumerable().SequenceEqual(sequence4.AsValueEnumerable()).ShouldBeFalse();
    }

    [Fact]
    public void WithCustomEqualityComparer()
    {
        var sequence1 = new[] { "A", "B", "C" };
        var sequence2 = new[] { "a", "b", "c" };

        // Default - case sensitive - should be false
        sequence1.AsValueEnumerable().SequenceEqual(sequence2).ShouldBeFalse();
        sequence1.AsValueEnumerable().SequenceEqual(sequence2.AsValueEnumerable()).ShouldBeFalse();

        // Case insensitive - should be true
        sequence1.AsValueEnumerable().SequenceEqual(sequence2, StringComparer.OrdinalIgnoreCase).ShouldBeTrue();
        sequence1.AsValueEnumerable().SequenceEqual(sequence2.AsValueEnumerable(), StringComparer.OrdinalIgnoreCase).ShouldBeTrue();

        // Verify against System.Linq
        var linqResult = sequence1.SequenceEqual(sequence2, StringComparer.OrdinalIgnoreCase);
        sequence1.AsValueEnumerable().SequenceEqual(sequence2, StringComparer.OrdinalIgnoreCase).ShouldBe(linqResult);
    }

    [Fact]
    public void WithNullValues()
    {
        var sequence1 = new string?[] { "A", null, "C" };
        var sequence2 = new string?[] { "A", null, "C" };
        var sequence3 = new string?[] { "A", "B", "C" }; // No null
        var sequence4 = new string?[] { null, "A", "C" }; // Null in different position

        // Equal sequences with nulls
        sequence1.AsValueEnumerable().SequenceEqual(sequence2).ShouldBeTrue();
        sequence1.AsValueEnumerable().SequenceEqual(sequence2.AsValueEnumerable()).ShouldBeTrue();

        // Different sequences (one with null, one without)
        sequence1.AsValueEnumerable().SequenceEqual(sequence3).ShouldBeFalse();
        sequence1.AsValueEnumerable().SequenceEqual(sequence3.AsValueEnumerable()).ShouldBeFalse();

        // Different sequences (nulls in different positions)
        sequence1.AsValueEnumerable().SequenceEqual(sequence4).ShouldBeFalse();
        sequence1.AsValueEnumerable().SequenceEqual(sequence4.AsValueEnumerable()).ShouldBeFalse();

        // All nulls
        var allNulls1 = new string?[] { null, null, null };
        var allNulls2 = new string?[] { null, null, null };
        allNulls1.AsValueEnumerable().SequenceEqual(allNulls2).ShouldBeTrue();
        allNulls1.AsValueEnumerable().SequenceEqual(allNulls2.AsValueEnumerable()).ShouldBeTrue();
    }

    [Fact]
    public void WithCustomClass()
    {
        var p1 = new Person("John", 25);
        var p2 = new Person("Jane", 30);
        var p3 = new Person("Alice", 35);

        var sequence1 = new[] { p1, p2, p3 };
        var sequence2 = new[] { 
            new Person("John", 25), 
            new Person("Jane", 30), 
            new Person("Alice", 35) 
        };

        // Default equality comparer (reference equality) - should be false
        sequence1.AsValueEnumerable().SequenceEqual(sequence2).ShouldBeFalse();
        sequence1.AsValueEnumerable().SequenceEqual(sequence2.AsValueEnumerable()).ShouldBeFalse();

        // Custom equality comparer
        var comparer = new PersonEqualityComparer();
        sequence1.AsValueEnumerable().SequenceEqual(sequence2, comparer).ShouldBeTrue();
        sequence1.AsValueEnumerable().SequenceEqual(sequence2.AsValueEnumerable(), comparer).ShouldBeTrue();

        // Different values with custom comparer
        var sequence3 = new[] { 
            new Person("John", 25), 
            new Person("Jane", 31), // Different age
            new Person("Alice", 35) 
        };
        sequence1.AsValueEnumerable().SequenceEqual(sequence3, comparer).ShouldBeFalse();
        sequence1.AsValueEnumerable().SequenceEqual(sequence3.AsValueEnumerable(), comparer).ShouldBeFalse();
    }

    [Fact]
    public void EarlyTermination()
    {
        var sequence1 = new[] { 1, 2, 3, 4, 5 };
        var sequence2 = new[] { 1, 2, 9, 4, 5 }; // Difference in the middle

        // Create a counting comparer to verify early termination
        var counter = 0;
        var countingComparer = new FunctionEqualityComparer<int>(
            (x, y) => {
                counter++;
                return x == y;
            },
            obj => obj.GetHashCode());

        // Should terminate after finding the first difference
        sequence1.AsValueEnumerable().SequenceEqual(sequence2, countingComparer).ShouldBeFalse();
        counter.ShouldBe(3); // Should have compared only the first 3 elements

        // Reset counter and test with ValueEnumerable overload
        counter = 0;
        sequence1.AsValueEnumerable().SequenceEqual(sequence2.AsValueEnumerable(), countingComparer).ShouldBeFalse();
        counter.ShouldBe(3); // Should have compared only the first 3 elements
    }

    [Fact]
    public void SpanOptimizationPath()
    {
        // Using arrays should trigger the span optimization path on .NET 8+
        var sequence1 = new[] { 1, 2, 3, 4, 5 };
        var sequence2 = new[] { 1, 2, 3, 4, 5 };
        var sequence3 = new[] { 1, 2, 3, 4, 6 }; // Different

        sequence1.AsValueEnumerable().SequenceEqual(sequence2.AsValueEnumerable()).ShouldBeTrue();
        sequence1.AsValueEnumerable().SequenceEqual(sequence3.AsValueEnumerable()).ShouldBeFalse();

        // Test with non-span enumerables to force the regular enumeration path
        var nonSpan1 = TestUtil.ToValueEnumerable(sequence1);
        var nonSpan2 = TestUtil.ToValueEnumerable(sequence2);
        var nonSpan3 = TestUtil.ToValueEnumerable(sequence3);

        nonSpan1.SequenceEqual(nonSpan2).ShouldBeTrue();
        nonSpan1.SequenceEqual(nonSpan3).ShouldBeFalse();
    }

    [Fact]
    public void NonEnumeratedCountOptimization()
    {
        // Create sequences with different lengths to test count optimization
        var sequence1 = new[] { 1, 2, 3 };
        var sequence2 = new[] { 1, 2, 3, 4, 5 };

        var counter = 0;
        var countingComparer = new FunctionEqualityComparer<int>(
            (x, y) => {
                counter++;
                return x == y;
            },
            obj => obj.GetHashCode());

        // Should immediately return false due to different lengths without comparing elements
        sequence1.AsValueEnumerable().SequenceEqual(sequence2.AsValueEnumerable(), countingComparer).ShouldBeFalse();
        counter.ShouldBe(0); // Should not have compared any elements
    }

    [Fact]
    public void ConsistentWithSystemLinq()
    {
        // Test various scenarios and ensure results match System.Linq
        
        // Test 1: Empty sequences
        var empty1 = Array.Empty<int>();
        var empty2 = Array.Empty<int>();
        empty1.AsValueEnumerable().SequenceEqual(empty2).ShouldBe(empty1.SequenceEqual(empty2));
        
        // Test 2: Equal sequences
        var seq1 = new[] { 1, 2, 3, 4, 5 };
        var seq2 = new[] { 1, 2, 3, 4, 5 };
        seq1.AsValueEnumerable().SequenceEqual(seq2).ShouldBe(seq1.SequenceEqual(seq2));
        
        // Test 3: Different sequences - different lengths
        var seq3 = new[] { 1, 2, 3 };
        seq1.AsValueEnumerable().SequenceEqual(seq3).ShouldBe(seq1.SequenceEqual(seq3));
        
        // Test 4: Different sequences - same length but different values
        var seq4 = new[] { 1, 2, 3, 4, 6 };
        seq1.AsValueEnumerable().SequenceEqual(seq4).ShouldBe(seq1.SequenceEqual(seq4));
        
        // Test 5: With custom comparer
        var strings1 = new[] { "A", "B", "C" };
        var strings2 = new[] { "a", "b", "c" };
        seq1.AsValueEnumerable().SequenceEqual(seq4, EqualityComparer<int>.Default)
            .ShouldBe(seq1.SequenceEqual(seq4, EqualityComparer<int>.Default));
        strings1.AsValueEnumerable().SequenceEqual(strings2, StringComparer.OrdinalIgnoreCase)
            .ShouldBe(strings1.SequenceEqual(strings2, StringComparer.OrdinalIgnoreCase));
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

    private class FunctionEqualityComparer<T> : IEqualityComparer<T>
    {
        private readonly Func<T?, T?, bool> _equals;
        private readonly Func<T, int> _getHashCode;

        public FunctionEqualityComparer(Func<T?, T?, bool> equals, Func<T, int> getHashCode)
        {
            _equals = equals;
            _getHashCode = getHashCode;
        }

        public bool Equals(T? x, T? y) => _equals(x, y);
        public int GetHashCode(T obj) => _getHashCode(obj);
    }
}
