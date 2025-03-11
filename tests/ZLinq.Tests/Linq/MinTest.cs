using Xunit.Runner.Common;

namespace ZLinq.Tests.Linq;

public class MinTest
{
    // Tests for empty collections - should throw NoElements exception for value types
    [Fact]
    public void EmptyIntSequenceThrows()
    {
        var empty = Array.Empty<int>();

        TestUtil.Throws<InvalidOperationException>(
            () => empty.Min(),
            () => empty.AsValueEnumerable().Min());

        TestUtil.Throws<InvalidOperationException>(
            () => empty.Min(),
            () => empty.ToIterableValueEnumerable().Min());
    }

    [Fact]
    public void EmptyStringSequenceReturnsNull()
    {
        var empty = Array.Empty<string>();

        empty.Min().ShouldBeNull();
        empty.AsValueEnumerable().Min().ShouldBeNull();
        empty.ToIterableValueEnumerable().Min().ShouldBeNull();
    }

    // Test basic functionality with different numeric types
    [Fact]
    public void IntegerMin()
    {
        var ints = new[] { 5, 3, 8, 2, 10 };
        ints.AsValueEnumerable().Min().ShouldBe(ints.Min());
        ints.ToIterableValueEnumerable().Min().ShouldBe(ints.Min());

        var bytes = new byte[] { 5, 3, 8, 2, 10 };
        bytes.AsValueEnumerable().Min().ShouldBe(bytes.Min());
        bytes.ToIterableValueEnumerable().Min().ShouldBe(bytes.Min());

        var shorts = new short[] { 5, 3, 8, 2, 10 };
        shorts.AsValueEnumerable().Min().ShouldBe(shorts.Min());
        shorts.ToIterableValueEnumerable().Min().ShouldBe(shorts.Min());

        var longs = new long[] { 5, 3, 8, 2, 10 };
        longs.AsValueEnumerable().Min().ShouldBe(longs.Min());
        longs.ToIterableValueEnumerable().Min().ShouldBe(longs.Min());
    }

    [Fact]
    public void FloatingPointMin()
    {
        var doubles = new[] { 5.5, 3.3, 8.8, 2.2, 10.1 };
        var a = doubles.AsValueEnumerable().Min();
        var b = doubles.Min();
        a.ShouldBe(b);
        doubles.ToIterableValueEnumerable().Min().ShouldBe(doubles.Min());

        var floats = new[] { 5.5f, 3.3f, 8.8f, 2.2f, 10.1f };
        floats.AsValueEnumerable().Min().ShouldBe(floats.Min());
        floats.ToIterableValueEnumerable().Min().ShouldBe(floats.Min());

        var decimals = new[] { 5.5m, 3.3m, 8.8m, 2.2m, 10.1m };
        decimals.AsValueEnumerable().Min().ShouldBe(decimals.Min());
        decimals.ToIterableValueEnumerable().Min().ShouldBe(decimals.Min());
    }

    // Test reference types
    [Fact]
    public void StringMin()
    {
        var strings = new[] { "banana", "apple", "cherry", "date" };
        strings.AsValueEnumerable().Min().ShouldBe(strings.Min());
        strings.ToIterableValueEnumerable().Min().ShouldBe(strings.Min());
    }

    [Fact]
    public void NullHandling()
    {
        var stringsWithNull = new[] { "banana", null, "apple", "cherry", null };
        stringsWithNull.AsValueEnumerable().Min().ShouldBe(stringsWithNull.Min());
        stringsWithNull.ToIterableValueEnumerable().Min().ShouldBe(stringsWithNull.Min());

        // All nulls should return null
        var allNulls = new string?[] { null, null, null };
        allNulls.AsValueEnumerable().Min().ShouldBeNull();
        allNulls.ToIterableValueEnumerable().Min().ShouldBeNull();
    }

    // Test custom comparer
    [Fact]
    public void CustomComparer()
    {
        var strings = new[] { "APPLE", "banana", "Cherry" };
        var comparer = StringComparer.OrdinalIgnoreCase;

        // With case-insensitive comparison, "APPLE" should be the min value
        strings.AsValueEnumerable().Min(comparer).ShouldBe(strings.Min(comparer));
        strings.ToIterableValueEnumerable().Min(comparer).ShouldBe(strings.Min(comparer));
    }

    // Test edge cases
    [Fact]
    public void SingleElementCollection()
    {
        var singleInt = new[] { 42 };
        singleInt.AsValueEnumerable().Min().ShouldBe(42);
        singleInt.ToIterableValueEnumerable().Min().ShouldBe(42);

        var singleString = new[] { "hello" };
        singleString.AsValueEnumerable().Min().ShouldBe("hello");
        singleString.ToIterableValueEnumerable().Min().ShouldBe("hello");
    }

    [Fact]
    public void MixedValues()
    {
        var mixed = new[] { -5, -10, 0, 5, 10 };
        mixed.AsValueEnumerable().Min().ShouldBe(-10);
        mixed.ToIterableValueEnumerable().Min().ShouldBe(-10);
    }

    [Fact]
    public void DuplicateValues()
    {
        var duplicates = new[] { 5, 2, 5, 2, 1, 1, 5 };
        duplicates.AsValueEnumerable().Min().ShouldBe(1);
        duplicates.ToIterableValueEnumerable().Min().ShouldBe(1);
    }

#if NET8_0_OR_GREATER
    // Test SIMD optimization paths
    [Fact]
    public void LargeIntCollectionForSimdOptimization()
    {
        // Create a large array that should trigger SIMD path
        var length = 1000;
        var data = new int[length];
        var random = new Random(42); // Fixed seed for deterministic results

        for (int i = 0; i < length; i++)
        {
            data[i] = random.Next(-10000, 10000);
        }

        var expected = data.Min();
        data.AsValueEnumerable().Min().ShouldBe(expected);
    }

    [Fact]
    public void SimdPrimitiveTypes()
    {
        // Test the primitive types that have SIMD optimization
        var ints = Enumerable.Range(0, 100).Select(i => 100 - i).ToArray();
        ints.AsValueEnumerable().Min().ShouldBe(1);

        var bytes = Enumerable.Range(0, 100).Select(i => (byte)(100 - i)).ToArray();
        bytes.AsValueEnumerable().Min().ShouldBe((byte)1);

        var shorts = Enumerable.Range(0, 100).Select(i => (short)(100 - i)).ToArray();
        shorts.AsValueEnumerable().Min().ShouldBe((short)1);

        var longs = Enumerable.Range(0, 100).Select(i => (long)(100 - i)).ToArray();
        longs.AsValueEnumerable().Min().ShouldBe(1L);
    }
#endif

    // Random tests with various types
    [Fact]
    public void RandomValuesTest()
    {
        var rand = new Random(42);

        // Test with ints
        for (int i = 1; i < 100; i++)
        {
            var data = new int[i].Select(_ => rand.Next(-1000, 1000)).ToArray();
            var expected = data.Min();

            var min1 = data.AsValueEnumerable().Min();
            var min2 = data.ToIterableValueEnumerable().Min();

            min1.ShouldBe(expected);
            min2.ShouldBe(expected);
        }

        // Test with strings
        for (int i = 1; i < 100; i++)
        {
            var data = new int[i].Select(_ => rand.Next(-1000, 1000).ToString()).ToArray();
            var expected = data.Min();

            var min1 = data.AsValueEnumerable().Min();
            var min2 = data.ToIterableValueEnumerable().Min();

            min1.ShouldBe(expected);
            min2.ShouldBe(expected);
        }
    }

    // Test for specific behavior with negative infinity and NaN in doubles
    [Fact]
    public void DoubleSpecialValuesTest()
    {
        var values = new[] { 1.0, double.NaN, 3.0, double.NegativeInfinity, 2.0 };

        // NaN is not considered in Min for standard LINQ
        var expected = values.Min();
        values.AsValueEnumerable().Min().ShouldBe(expected);
        values.ToIterableValueEnumerable().Min().ShouldBe(expected);
    }
}
