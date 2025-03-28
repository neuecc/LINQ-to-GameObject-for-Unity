using System.Buffers;

namespace ZLinq.Tests.Linq;

public class ToArrayPoolTest
{
    // Helper to properly return arrays to the pool after tests
    private void ReturnToPool<T>((T[] Array, int Size) result)
    {
        if (result.Array != null && result.Array.Length > 0 && result.Array != Array.Empty<T>())
        {
            ArrayPool<T>.Shared.Return(result.Array);
        }
    }

#if !NET48

    [Fact]
    public void Empty_ReturnsEmptyArrayAndZeroSize()
    {
        // Empty array sources
        var emptyArray = Array.Empty<int>();
        var result1 = emptyArray.AsValueEnumerable().ToArrayPool();
        try
        {
            result1.Array.ShouldBeSameAs(Array.Empty<int>()); // net48 returns ArrayPool's internal empty so fail
            result1.Size.ShouldBe(0);
        }
        finally
        {
            ReturnToPool(result1);
        }

        // Empty Range source
        var result2 = ValueEnumerable.Range(0, 0).ToArrayPool();
        try
        {
            result2.Array.ShouldBeSameAs(Array.Empty<int>());
            result2.Size.ShouldBe(0);
        }
        finally
        {
            ReturnToPool(result2);
        }

        // Empty Selected source
        var result3 = emptyArray.AsValueEnumerable().Select(x => x).ToArrayPool();
        try
        {
            result3.Array.ShouldBeSameAs(Array.Empty<int>());
            result3.Size.ShouldBe(0);
        }
        finally
        {
            ReturnToPool(result3);
        }

        // Empty Iterator source
        var result4 = emptyArray.ToValueEnumerable().ToArrayPool();
        try
        {
            result4.Array.ShouldBeSameAs(Array.Empty<int>());
            result4.Size.ShouldBe(0);
        }
        finally
        {
            ReturnToPool(result4);
        }
    }

#endif

    [Fact]
    public void NonEmpty_WithNonEnumeratedCount_TryCopyTo()
    {
        // Sources that have TryGetNonEnumeratedCount and can TryCopyTo
        var source = new[] { 1, 2, 3, 4, 5 };
        var result = source.AsValueEnumerable().ToArrayPool();

        try
        {
            result.Size.ShouldBe(source.Length);
            for (int i = 0; i < result.Size; i++)
            {
                result.Array[i].ShouldBe(source[i]);
            }
        }
        finally
        {
            ReturnToPool(result);
        }
    }

    [Fact]
    public void NonEmpty_WithNonEnumeratedCount_NoTryCopyTo()
    {
        // Sources that have TryGetNonEnumeratedCount but can't TryCopyTo
        // To simulate this, we can use a custom wrapper that overrides TryCopyTo to return false
        var source = ValueEnumerable.Range(1, 5);
        var result = source.ToArrayPool();

        try
        {
            result.Size.ShouldBe(5);
            for (int i = 0; i < result.Size; i++)
            {
                result.Array[i].ShouldBe(i + 1);
            }
        }
        finally
        {
            ReturnToPool(result);
        }
    }

    [Fact]
    public void NonEmpty_WithoutNonEnumeratedCount()
    {
        // Sources that don't have TryGetNonEnumeratedCount
        // This uses SegmentedArrayBuilder path
        var source = new[] { 1, 2, 3, 4, 5 };
        var result = TestUtil.ToValueEnumerable(source).ToArrayPool();

        try
        {
            result.Size.ShouldBe(source.Length);
            for (int i = 0; i < result.Size; i++)
            {
                result.Array[i].ShouldBe(source[i]);
            }
        }
        finally
        {
            ReturnToPool(result);
        }
    }

    [Fact]
    public void LargeCollection()
    {
        // Test with a collection large enough to trigger multiple segments in SegmentedArrayBuilder
        var count = 10000;
        var source = Enumerable.Range(1, count).ToArray();
        var result = source.ToValueEnumerable().ToArrayPool();

        try
        {
            result.Size.ShouldBe(count);
            for (int i = 0; i < Math.Min(100, result.Size); i++) // Check at least the first 100
            {
                result.Array[i].ShouldBe(i + 1);
            }
            // Check the last item to ensure all items were copied
            result.Array[result.Size - 1].ShouldBe(count);
        }
        finally
        {
            ReturnToPool(result);
        }
    }

    [Fact]
    public void DifferentDataTypes()
    {
        // Test with different data types
        // String array
        var strings = new[] { "one", "two", "three" };
        var stringResult = strings.AsValueEnumerable().ToArrayPool();

        try
        {
            stringResult.Size.ShouldBe(strings.Length);
            for (int i = 0; i < stringResult.Size; i++)
            {
                stringResult.Array[i].ShouldBe(strings[i]);
            }
        }
        finally
        {
            ReturnToPool(stringResult);
        }

        // Custom object array
        var objects = new TestObject[]
        {
            new TestObject(1, "A"),
            new TestObject(2, "B"),
            new TestObject(3, "C")
        };
        var objectResult = objects.AsValueEnumerable().ToArrayPool();

        try
        {
            objectResult.Size.ShouldBe(objects.Length);
            for (int i = 0; i < objectResult.Size; i++)
            {
                objectResult.Array[i].ShouldBe(objects[i]);
            }
        }
        finally
        {
            ReturnToPool(objectResult);
        }
    }

    [Fact]
    public void RentedArrayHasSufficientCapacity()
    {
        // The rented array should have at least the size of the collection
        var source = new[] { 1, 2, 3, 4, 5 };
        var result = source.AsValueEnumerable().ToArrayPool();

        try
        {
            // Size equals collection size
            result.Size.ShouldBe(source.Length);

            // Array length should be at least the collection size
            // Note: ArrayPool might return a larger array than requested
            result.Array.Length.ShouldBeGreaterThanOrEqualTo(source.Length);
        }
        finally
        {
            ReturnToPool(result);
        }
    }

    [Fact]
    public void WithSelectors()
    {
        // Test with data transformations
        var source = new[] { 1, 2, 3, 4, 5 };
        var result = source.AsValueEnumerable().Select(x => x * 2).ToArrayPool();

        try
        {
            result.Size.ShouldBe(source.Length);
            for (int i = 0; i < result.Size; i++)
            {
                result.Array[i].ShouldBe(source[i] * 2);
            }
        }
        finally
        {
            ReturnToPool(result);
        }
    }

    // Helper class for testing reference types
    private class TestObject : IEquatable<TestObject>
    {
        public int Id { get; }
        public string Value { get; }

        public TestObject(int id, string value)
        {
            Id = id;
            Value = value;
        }

        public bool Equals(TestObject? other) =>
            other != null && Id == other.Id && Value == other.Value;

        public override bool Equals(object? obj) =>
            obj is TestObject other && Equals(other);

        public override int GetHashCode() =>
            HashCode.Combine(Id, Value);

        public override string ToString() => $"TestObject({Id}, {Value})";
    }
}
