namespace ZLinq.Tests.Linq;

public class AverageTest
{
    // Tests for empty collections - should throw NoElements exception
    [Fact]
    public void EmptySequenceThrows()
    {
        var emptyInts = Array.Empty<int>();
        var emptyDoubles = Array.Empty<double>();
        var emptyDecimals = Array.Empty<decimal>();

        TestUtil.Throws<InvalidOperationException>(
            () => emptyInts.Average(),
            () => emptyInts.AsValueEnumerable().Average());

        TestUtil.Throws<InvalidOperationException>(
            () => emptyDoubles.Average(),
            () => emptyDoubles.AsValueEnumerable().Average());

        TestUtil.Throws<InvalidOperationException>(
            () => emptyDecimals.Average(),
            () => emptyDecimals.AsValueEnumerable().Average());
    }

    // Tests for various numeric types
    [Fact]
    public void IntegerAverage()
    {
        // Test with regular integers
        var ints = new[] { 2, 4, 6, 8, 10 };
        ints.AsValueEnumerable().Average().ShouldBe(ints.Average());
        ints.ToValueEnumerable().Average().ShouldBe(ints.Average());

        // Test with bytes
        var bytes = new byte[] { 1, 2, 3, 4, 5 };
        bytes.AsValueEnumerable().Average().ShouldBe((byte)bytes.Select(x => (int)x).Average());
        bytes.ToValueEnumerable().Average().ShouldBe((byte)bytes.Select(x => (int)x).Average());

        // Test with shorts
        var shorts = new short[] { 1, 2, 3, 4, 5 };
        shorts.AsValueEnumerable().Average().ShouldBe((short)shorts.Select(x => (int)x).Average());
        shorts.ToValueEnumerable().Average().ShouldBe((short)shorts.Select(x => (int)x).Average());

        // Test with longs
        var longs = new long[] { 10, 20, 30, 40, 50 };
        longs.AsValueEnumerable().Average().ShouldBe(longs.Average());
        longs.ToValueEnumerable().Average().ShouldBe(longs.Average());
    }

    [Fact]
    public void FloatingPointAverage()
    {
        // Test with doubles
        var doubles = new[] { 1.5, 2.5, 3.5, 4.5, 5.5 };
        var a = doubles.AsValueEnumerable().Average();
        var b = doubles.Average();
        a.ShouldBe(b);

        var c = doubles.ToValueEnumerable().Average();
        c.ShouldBe(b);

        // Test with decimals
        var decimals = new[] { 1.1m, 2.2m, 3.3m, 4.4m, 5.5m };
        decimals.AsValueEnumerable().Average().ShouldBe((double)decimals.Average());
        decimals.ToValueEnumerable().Average().ShouldBe((double)decimals.Average());
    }

    // Edge case: single element collection
    [Fact]
    public void SingleElementAverage()
    {
        var singleInt = new[] { 42 };
        singleInt.AsValueEnumerable().Average().ShouldBe(42.0);
        singleInt.ToValueEnumerable().Average().ShouldBe(42.0);

        var singleDouble = new[] { 42.5 };
        singleDouble.AsValueEnumerable().Average().ShouldBe(42.5);
        singleDouble.ToValueEnumerable().Average().ShouldBe(42.5);
    }

    // Test with negative and positive numbers
    [Fact]
    public void MixedSignAverage()
    {
        var ints = new[] { -5, -3, -1, 0, 2, 4, 6 };
        var expected = ints.Average();

        ints.AsValueEnumerable().Average().ShouldBe(expected);
        ints.ToValueEnumerable().Average().ShouldBe(expected);
    }

    // Test large collections (to potentially test SIMD path)
    [Fact]
    public void LargeCollectionAverage()
    {
        // Large enough to test SIMD path on supported platforms
        var ints = Enumerable.Range(1, 1000).ToArray();
        var expected = ints.Average();

        ints.AsValueEnumerable().Average().ShouldBe(expected);

        // Also test with non-span path
        ints.ToValueEnumerable().Average().ShouldBe(expected);
    }

    // Test floating point precision issues
    [Fact]
    public void FloatingPointPrecision()
    {
        // These values can cause precision issues in floating point arithmetic
        var values = new[] { 1e7, 1.0, -1e7, 1.0 };
        var expected = values.Average();

        values.AsValueEnumerable().Average().ShouldBe(expected);
        values.ToValueEnumerable().Average().ShouldBe(expected);
    }

#if NET8_0_OR_GREATER
    // Test SIMD optimization for integers (NET 8+)
    [Fact]
    public void SimdIntegerOptimization()
    {
        var length = 1000;
        var ints = new int[length];

        for (int i = 0; i < length; i++)
        {
            ints[i] = i + 1;
        }

        var expected = ints.Average();
        ints.AsValueEnumerable().Average().ShouldBe(expected);
    }

    // Test with values that could potentially overflow during sum but not average
    [Fact]
    public void LargeValuesAverage()
    {
        // These values would overflow if summed as int, but should work as average
        var values = new[] { int.MaxValue / 3, int.MaxValue / 3, int.MaxValue / 3 };
        var expected = values.Average(); // System.Linq handles this correctly

        values.AsValueEnumerable().Average().ShouldBe(expected);
    }
#endif

    // Test with zero values (shouldn't affect average)
    [Fact]
    public void ZeroValuesAverage()
    {
        var withZeros = new[] { 5, 0, 10, 0, 15 };
        var expected = withZeros.Average();

        withZeros.AsValueEnumerable().Average().ShouldBe(expected);
        withZeros.ToValueEnumerable().Average().ShouldBe(expected);
    }

    // Test with values that result in a fractional average
    [Fact]
    public void FractionalAverage()
    {
        var oddSumInts = new[] { 1, 2, 4 }; // Sum = 7, Count = 3, Average = 2.33333...
        var expected = oddSumInts.Average();

        oddSumInts.AsValueEnumerable().Average().ShouldBe(expected);
        oddSumInts.ToValueEnumerable().Average().ShouldBe(expected);
    }

    // Test return type is always double regardless of input type
    [Fact]
    public void ReturnTypeIsDouble()
    {
        var ints = new[] { 2, 4, 6 };
        var doubles = new[] { 2.0, 4.0, 6.0 };
        var decimals = new[] { 2.0m, 4.0m, 6.0m };

        ints.AsValueEnumerable().Average().GetType().ShouldBe(typeof(double));
        doubles.AsValueEnumerable().Average().GetType().ShouldBe(typeof(double));
        decimals.AsValueEnumerable().Average().GetType().ShouldBe(typeof(double));
    }
}
