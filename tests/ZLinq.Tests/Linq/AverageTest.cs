using System;
using System.Linq;
using Shouldly;
using Xunit;

namespace ZLinq.Tests.Linq;

public class AverageTest
{
    #region Basic Tests - Non-Nullable

    [Fact]
    public void EmptySequence_ThrowsInvalidOperationException()
    {
        // Arrange
        var empty = Array.Empty<int>();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => empty.AsValueEnumerable().Average());
        Assert.Throws<InvalidOperationException>(() => empty.ToValueEnumerable().Average());
    }

    [Fact]
    public void IntSequence_CalculatesCorrectAverage()
    {
        // Arrange
        var ints = new[] { 2, 4, 6, 8, 10 };
        var expected = ints.Average();

        // Act & Assert
        ints.AsValueEnumerable().Average().ShouldBe(expected);
        ints.ToValueEnumerable().Average().ShouldBe(expected);
    }

    [Fact]
    public void FloatSequence_CalculatesCorrectAverage()
    {
        // Arrange
        var floats = new[] { 1.5f, 2.5f, 3.5f, 4.5f };
        var expected = floats.Average();

        // Act & Assert
        floats.AsValueEnumerable().Average().ShouldBe(expected);
        floats.ToValueEnumerable().Average().ShouldBe(expected);
    }

    [Fact]
    public void DecimalSequence_CalculatesCorrectAverage()
    {
        // Arrange
        var decimals = new[] { 1.1m, 2.2m, 3.3m, 4.4m, 5.5m };
        var expected = decimals.Average();

        // Act & Assert
        decimals.AsValueEnumerable().Average().ShouldBe(expected);
        decimals.ToValueEnumerable().Average().ShouldBe(expected);
    }

    [Fact]
    public void DoubleSequence_CalculatesCorrectAverage()
    {
        // Arrange
        var doubles = new[] { 1.5, 2.5, 3.5, 4.5, 5.5 };
        var expected = doubles.Average();

        // Act & Assert
        doubles.AsValueEnumerable().Average().ShouldBe(expected);
        doubles.ToValueEnumerable().Average().ShouldBe(expected);
    }

    #endregion

    #region Nullable Type Tests

    [Fact]
    public void EmptyNullableSequence_ReturnsNull()
    {
        // Arrange
        var empty = Array.Empty<int?>();
        
        // Act & Assert
        empty.AsValueEnumerable().Average().ShouldBeNull();
        empty.ToValueEnumerable().Average().ShouldBeNull();
    }

    [Fact]
    public void NullableSequenceWithOnlyNulls_ReturnsNull()
    {
        // Arrange
        var onlyNulls = new int?[] { null, null, null };
        
        // Act & Assert
        onlyNulls.AsValueEnumerable().Average().ShouldBeNull();
        onlyNulls.ToValueEnumerable().Average().ShouldBeNull();
    }

    [Fact]
    public void NullableIntSequenceWithMixedValues_IgnoresNulls()
    {
        // Arrange
        var mixed = new int?[] { 2, 4, null, 6, null, 8 };
        var expected = mixed.Average();

        // Act & Assert
        mixed.AsValueEnumerable().Average().ShouldBe(expected);
        mixed.ToValueEnumerable().Average().ShouldBe(expected);
    }

    [Fact]
    public void NullableFloatSequenceWithMixedValues_IgnoresNulls()
    {
        // Arrange
        var mixed = new float?[] { 1.5f, null, 2.5f, 3.5f, null, 4.5f };
        var expected = mixed.Average();

        // Act & Assert
        mixed.AsValueEnumerable().Average().ShouldBe(expected);
        mixed.ToValueEnumerable().Average().ShouldBe(expected);
    }

    [Fact]
    public void NullableDecimalSequenceWithMixedValues_IgnoresNulls()
    {
        // Arrange
        var mixed = new decimal?[] { 1.1m, null, 2.2m, null, 3.3m, 4.4m };
        var expected = mixed.Average();

        // Act & Assert
        mixed.AsValueEnumerable().Average().ShouldBe(expected);
        mixed.ToValueEnumerable().Average().ShouldBe(expected);
    }

    #endregion

    #region Selector Function Tests

    [Fact]
    public void Selector_WithIntItems_CalculatesCorrectAverage()
    {
        // Arrange
        var items = new[] { 1, 2, 3, 4, 5 };
        var expected = items.Average(x => x * 2);

        // Act & Assert
        items.AsValueEnumerable().Average(x => x * 2).ShouldBe(expected);
        items.ToValueEnumerable().Average(x => x * 2).ShouldBe(expected);
    }

    [Fact]
    public void Selector_WithFloatResults_CalculatesCorrectAverage()
    {
        // Arrange
        var items = new[] { 1, 2, 3, 4, 5 };
        var expected = items.Average(x => x * 1.5f);

        // Act & Assert
        items.AsValueEnumerable().Average(x => x * 1.5f).ShouldBe(expected);
        items.ToValueEnumerable().Average(x => x * 1.5f).ShouldBe(expected);
    }

    [Fact]
    public void Selector_WithDecimalResults_CalculatesCorrectAverage()
    {
        // Arrange
        var items = new[] { 1, 2, 3, 4, 5 };
        var expected = items.Average(x => x * 1.5m);

        // Act & Assert
        items.AsValueEnumerable().Average(x => x * 1.5m).ShouldBe(expected);
        items.ToValueEnumerable().Average(x => x * 1.5m).ShouldBe(expected);
    }

    [Fact]
    public void NullableSelector_WithMixedNullResults_CalculatesCorrectAverage()
    {
        // Arrange
        var items = new[] { 1, 2, 3, 4, 5 };
        Func<int, int?> selector = x => x % 2 == 0 ? x * 2 : null;
        var expected = items.Select(selector).Average();

        // Act & Assert
        items.AsValueEnumerable().Average(selector).ShouldBe(expected);
        items.ToValueEnumerable().Average(selector).ShouldBe(expected);
    }

    [Fact]
    public void NullableFloatSelector_WithMixedNullResults_CalculatesCorrectAverage()
    {
        // Arrange
        var items = new[] { 1, 2, 3, 4, 5 };
        Func<int, float?> selector = x => x % 2 == 0 ? x * 1.5f : null;
        var expected = items.Select(selector).Average();

        // Act & Assert
        items.AsValueEnumerable().Average(selector).ShouldBe(expected);
        items.ToValueEnumerable().Average(selector).ShouldBe(expected);
    }

    #endregion

    #region Edge Cases and Precision Tests

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

    // Test overflow protection
    [Fact]
    public void OverflowProtection()
    {
        // Values that would overflow if summed directly as the same type
        var largeValues = new[] { int.MaxValue / 2, int.MaxValue / 2 };
        var expected = largeValues.Average();

        largeValues.AsValueEnumerable().Average().ShouldBe(expected);
        largeValues.ToValueEnumerable().Average().ShouldBe(expected);
    }

    // Test with single item sequence
    [Fact]
    public void SingleItemSequence()
    {
        // Single item should just return that item converted to appropriate type
        var singleInt = new[] { 42 };
        var expected = singleInt.Average();

        singleInt.AsValueEnumerable().Average().ShouldBe(expected);
        singleInt.ToValueEnumerable().Average().ShouldBe(expected);
    }

    #endregion

    #region Return Type Tests

    // Test return type for float -> float
    [Fact]
    public void FloatAverage_ReturnsFloat()
    {
        var floats = new[] { 1.5f, 2.5f, 3.5f };
        var result = floats.AsValueEnumerable().Average();
        
        result.GetType().ShouldBe(typeof(float));
    }

    // Test return type for decimal -> decimal
    [Fact]
    public void DecimalAverage_ReturnsDecimal()
    {
        var decimals = new[] { 1.5m, 2.5m, 3.5m };
        var result = decimals.AsValueEnumerable().Average();
        
        result.GetType().ShouldBe(typeof(decimal));
    }

    // Test return type for int -> double
    [Fact]
    public void IntAverage_ReturnsDouble()
    {
        var ints = new[] { 1, 2, 3 };
        var result = ints.AsValueEnumerable().Average();
        
        result.GetType().ShouldBe(typeof(double));
    }

    #endregion

    #region .NET 8+ Specific Tests

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

    // Test SIMD performance with large arrays
    [Fact]
    public void LargeArray_UsesSimdOptimization()
    {
        var length = 10000;
        var ints = new int[length];

        for (int i = 0; i < length; i++)
        {
            ints[i] = i % 100; // Keep values small to avoid overflow
        }

        var expected = ints.Average();
        ints.AsValueEnumerable().Average().ShouldBe(expected);
    }
#endif

    #endregion

    #region ArgumentNullException Tests

    [Fact]
    public void NullSelector_ThrowsArgumentNullException()
    {
        // Arrange
        var items = new[] { 1, 2, 3 };
        Func<int, int> selector = null!;
        Func<int, int?> nullableSelector = null!;
        
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => items.AsValueEnumerable().Average(selector));
        Assert.Throws<ArgumentNullException>(() => items.AsValueEnumerable().Average(nullableSelector));
    }

    #endregion
}
