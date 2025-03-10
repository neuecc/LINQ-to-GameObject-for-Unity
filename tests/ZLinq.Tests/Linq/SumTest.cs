namespace ZLinq.Tests.Linq;

public class SumTest
{
    // Tests for empty collections
    [Fact]
    public void EmptyInt()
    {
        var xs = Array.Empty<int>();

        xs.AsValueEnumerable().Sum().ShouldBe(xs.Sum());
        xs.ToIterableValueEnumerable().Sum().ShouldBe(xs.Sum());
    }

    [Fact]
    public void EmptyFloat()
    {
        var xs = Array.Empty<float>();

        xs.AsValueEnumerable().Sum().ShouldBe(xs.Sum());
        xs.ToIterableValueEnumerable().Sum().ShouldBe(xs.Sum());
    }

    [Fact]
    public void EmptyDouble()
    {
        var xs = Array.Empty<double>();

        xs.AsValueEnumerable().Sum().ShouldBe(xs.Sum());
        xs.ToIterableValueEnumerable().Sum().ShouldBe(xs.Sum());
    }

    // Tests for different numeric types
    [Fact]
    public void SumIntegers()
    {
        var ints = new[] { 1, 2, 3, 4, 5 };
        ints.AsValueEnumerable().Sum().ShouldBe(ints.Sum());
        ints.ToIterableValueEnumerable().Sum().ShouldBe(ints.Sum());

        var bytes = new byte[] { 1, 2, 3, 4, 5 };
        bytes.AsValueEnumerable().Sum().ShouldBe((byte)bytes.Sum(x => (int)x));
        bytes.ToIterableValueEnumerable().Sum().ShouldBe((byte)bytes.Sum(x => (int)x));

        var shorts = new short[] { 1, 2, 3, 4, 5 };
        shorts.AsValueEnumerable().Sum().ShouldBe((short)shorts.Sum(x => (short)x));
        shorts.ToIterableValueEnumerable().Sum().ShouldBe((short)shorts.Sum(x => (short)x));

        var longs = new long[] { 1, 2, 3, 4, 5 };
        longs.AsValueEnumerable().Sum().ShouldBe(longs.Sum());
        longs.ToIterableValueEnumerable().Sum().ShouldBe(longs.Sum());
    }

    [Fact]
    public void SumFloatingPoints()
    {
        var floats = new[] { 1.5f, 2.5f, 3.5f, 4.5f, 5.5f };
        floats.AsValueEnumerable().Sum().ShouldBe(floats.Sum());
        floats.ToIterableValueEnumerable().Sum().ShouldBe(floats.Sum());

        var doubles = new[] { 1.5, 2.5, 3.5, 4.5, 5.5 };
        doubles.AsValueEnumerable().Sum().ShouldBe(doubles.Sum());
        doubles.ToIterableValueEnumerable().Sum().ShouldBe(doubles.Sum());

        var decimals = new[] { 1.5m, 2.5m, 3.5m, 4.5m, 5.5m };
        decimals.AsValueEnumerable().Sum().ShouldBe(decimals.Sum());
        decimals.ToIterableValueEnumerable().Sum().ShouldBe(decimals.Sum());
    }

    [Fact]
    public void SumLargeCollection()
    {
        // Large enough to test SIMD path on supported platforms
        var ints = Enumerable.Range(1, 1000).ToArray();
        ints.AsValueEnumerable().Sum().ShouldBe(ints.Sum());

        var doubles = Enumerable.Range(1, 1000).Select(i => (double)i).ToArray();
        doubles.AsValueEnumerable().Sum().ShouldBe(doubles.Sum());
    }

    // Test for floating point precision
    [Fact]
    public void FloatPrecision()
    {
        // Float calculations can have precision issues
        var floats = new[] { 1e7f, 1f, -1e7f, 1f };

        // Should match standard implementation behavior
        floats.AsValueEnumerable().Sum().ShouldBe(floats.Sum());
        floats.ToIterableValueEnumerable().Sum().ShouldBe(floats.Sum());
    }

    [Fact]
    public void DoublePrecision()
    {
        var doubles = new[] { 1e15, 1, -1e15, 1 };

        doubles.AsValueEnumerable().Sum().ShouldBe(doubles.Sum());
        doubles.ToIterableValueEnumerable().Sum().ShouldBe(doubles.Sum());
    }

    // Overflow tests
    [Fact]
    public void IntOverflow()
    {
        var values = new[] { int.MaxValue, 1 };

        // Should throw overflow exception when using checked operations
        TestUtil.Throws<OverflowException>(
            () => values.Sum(),
            () => values.AsValueEnumerable().Sum());
    }

    [Fact]
    public void LongOverflow()
    {
        var values = new[] { long.MaxValue, 1L };

        // Should throw overflow exception when using checked operations
        TestUtil.Throws<OverflowException>(
            () => values.Sum(),
            () => values.AsValueEnumerable().Sum());
    }

#if NET8_0_OR_GREATER
    // Tests for SumUnchecked (available in .NET 8+)
    [Fact]
    public void SumUnchecked()
    {
        var ints = new[] { 1, 2, 3, 4, 5 };
        var expected = 15;

        ints.AsValueEnumerable().SumUnchecked().ShouldBe(expected);
        ints.ToIterableValueEnumerable().SumUnchecked().ShouldBe(expected);
    }

    [Fact]
    public void SumUncheckedWithOverflow()
    {
        var values = new[] { int.MaxValue, 1 };

        // Should not throw with unchecked operations
        // The result will be int.MinValue + some value due to overflow
        values.AsValueEnumerable().SumUnchecked().ShouldBe(unchecked(int.MaxValue + 1));
    }

    // Test for SIMD optimizations
    [Fact]
    public void SimdOptimization()
    {
        // Create array large enough to trigger SIMD path
        var length = 1000;
        var ints = new int[length];
        var expected = 0;

        for (int i = 0; i < length; i++)
        {
            ints[i] = i + 1;
            expected += i + 1;
        }

        ints.AsValueEnumerable().Sum().ShouldBe(expected);
    }

    [Fact]
    public void SimdOptimizationLong()
    {
        // Large array for longs to test SIMD path
        var length = 1000;
        var longs = new long[length];
        var expected = 0L;

        for (int i = 0; i < length; i++)
        {
            longs[i] = i + 1;
            expected += i + 1;
        }

        longs.AsValueEnumerable().Sum().ShouldBe(expected);
    }

    [Fact]
    public void SimdOptimizationDouble()
    {
        // Double uses unchecked operations
        var length = 1000;
        var doubles = new double[length];
        var expected = 0.0;

        for (int i = 0; i < length; i++)
        {
            doubles[i] = i + 0.5;
            expected += i + 0.5;
        }

        doubles.AsValueEnumerable().Sum().ShouldBe(expected);
    }
#endif

    // Edge case handling
    [Fact]
    public void SingleElementSum()
    {
        var ints = new[] { 42 };
        ints.AsValueEnumerable().Sum().ShouldBe(42);

        var doubles = new[] { 42.5 };
        doubles.AsValueEnumerable().Sum().ShouldBe(42.5);
    }

    [Fact]
    public void NegativeAndPositiveSum()
    {
        var ints = new[] { -5, -4, -3, -2, -1, 0, 1, 2, 3, 4, 5 };
        ints.AsValueEnumerable().Sum().ShouldBe(0);
        ints.ToIterableValueEnumerable().Sum().ShouldBe(0);
    }
}
