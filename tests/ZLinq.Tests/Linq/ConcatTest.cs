using System;
using System.Collections.Generic;
using System.Linq;
using Shouldly;
using Xunit;

namespace ZLinq.Tests.Linq;

public class ConcatTest
{
    #region Basic Concat Tests

    [Fact]
    public void Concat_TwoValueEnumerables_ShouldProduceConcatenatedCollection()
    {
        // Arrange
        var first = new[] { 1, 2, 3 };
        var second = new[] { 4, 5, 6 };

        // Act - Standard LINQ as reference
        var expected = first.Concat(second).ToArray();

        // Act - ZLinq
        var actual = first
            .AsValueEnumerable()
            .Concat(second.AsValueEnumerable())
            .ToArray();

        // Assert
        actual.ShouldBe(expected);
    }

    [Fact]
    public void Concat_ValueEnumerableWithIEnumerable_ShouldProduceConcatenatedCollection()
    {
        // Arrange
        var first = new[] { 1, 2, 3 };
        var second = new List<int> { 4, 5, 6 };

        // Act - Standard LINQ as reference
        var expected = first.Concat(second).ToArray();

        // Act - ZLinq
        var actual = first
            .AsValueEnumerable()
            .Concat(second)
            .ToArray();

        // Assert
        actual.ShouldBe(expected);
    }

    [Fact]
    public void Concat_EmptyFirst_ShouldReturnOnlySecond()
    {
        // Arrange
        var first = Array.Empty<int>();
        var second = new[] { 4, 5, 6 };

        // Act - Standard LINQ as reference
        var expected = first.Concat(second).ToArray();

        // Act - ZLinq
        var actual = first
            .AsValueEnumerable()
            .Concat(second.AsValueEnumerable())
            .ToArray();

        // Assert
        actual.ShouldBe(second);
        actual.ShouldBe(expected);
    }

    [Fact]
    public void Concat_EmptySecond_ShouldReturnOnlyFirst()
    {
        // Arrange
        var first = new[] { 1, 2, 3 };
        var second = Array.Empty<int>();

        // Act - Standard LINQ as reference
        var expected = first.Concat(second).ToArray();

        // Act - ZLinq
        var actual = first
            .AsValueEnumerable()
            .Concat(second.AsValueEnumerable())
            .ToArray();

        // Assert
        actual.ShouldBe(first);
        actual.ShouldBe(expected);
    }

    [Fact]
    public void Concat_BothEmpty_ShouldReturnEmpty()
    {
        // Arrange
        var first = Array.Empty<int>();
        var second = Array.Empty<int>();

        // Act - Standard LINQ as reference
        var expected = first.Concat(second).ToArray();

        // Act - ZLinq
        var actual = first
            .AsValueEnumerable()
            .Concat(second.AsValueEnumerable())
            .ToArray();

        // Assert
        actual.ShouldBeEmpty();
        actual.ShouldBe(expected);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void Concat_WithNullSecondIEnumerable_ShouldThrowArgumentNullException()
    {
        // Arrange
        var first = new[] { 1, 2, 3 };
        IEnumerable<int>? second = null;

        // Act & Assert
        Should.Throw<ArgumentNullException>(() =>
        {
            first.AsValueEnumerable().Concat(second).ToArray();
        });
    }

    [Fact]
    public void Concat_WithDifferentTypes_ShouldWork()
    {
        // Arrange
        var first = new[] { "a", "b", "c" };
        var second = new[] { "d", "e", "f" };

        // Act - Standard LINQ as reference
        var expected = first.Concat(second).ToArray();

        // Act - ZLinq
        var actual = first
            .AsValueEnumerable()
            .Concat(second.AsValueEnumerable())
            .ToArray();

        // Assert
        actual.ShouldBe(expected);
    }

    #endregion

    #region Optimization Tests

    [Fact]
    public void Concat_TryGetNonEnumeratedCount_ShouldReturnCorrectCount()
    {
        // Arrange
        var first = new[] { 1, 2, 3 };
        var second = new[] { 4, 5, 6 };
        
        var zlinq = first
            .AsValueEnumerable()
            .Concat(second.AsValueEnumerable());
        
        // Get internal enumerator to test TryGetNonEnumeratedCount directly
        var enumerator = zlinq.Enumerator;
        
        // Act
        var success = enumerator.TryGetNonEnumeratedCount(out var count);
        
        // Assert
        success.ShouldBeTrue();
        count.ShouldBe(6); // 3 + 3
    }

    [Fact]
    public void Concat_TryGetSpan_ShouldReturnFalse()
    {
        // Arrange
        var first = new[] { 1, 2, 3 };
        var second = new[] { 4, 5, 6 };
        
        var zlinq = first
            .AsValueEnumerable()
            .Concat(second.AsValueEnumerable());
        
        // Get internal enumerator to test TryGetSpan directly
        var enumerator = zlinq.Enumerator;
        
        // Act
        var success = enumerator.TryGetSpan(out var span);
        
        // Assert
        success.ShouldBeFalse();
        span.IsEmpty.ShouldBeTrue();
    }

    #endregion

    #region Enumeration Tests

    [Fact]
    public void Concat_EnumerationOrder_ShouldBeFirstThenSecond()
    {
        // Arrange
        var first = new[] { 1, 2, 3 };
        var second = new[] { 4, 5, 6 };
        
        // Act - Manually enumerate through the concatenated collection
        var concat = first.AsValueEnumerable().Concat(second.AsValueEnumerable());
        var enumerator = concat.Enumerator;
        var results = new List<int>();
        
        while (enumerator.TryGetNext(out var item))
        {
            results.Add(item);
        }
        
        // Assert
        results.Count.ShouldBe(6);
        results[0].ShouldBe(1);
        results[1].ShouldBe(2);
        results[2].ShouldBe(3);
        results[3].ShouldBe(4);
        results[4].ShouldBe(5);
        results[5].ShouldBe(6);
    }

    [Fact]
    public void Concat_MultipleOperations_ShouldWorkCorrectly()
    {
        // Arrange
        var first = new[] { 1, 2, 3 };
        var second = new[] { 4, 5 };
        var third = new[] { 6, 7, 8 };
        
        // Act - Chaining multiple concat operations
        var result = first
            .AsValueEnumerable()
            .Concat(second.AsValueEnumerable())
            .Concat(third.AsValueEnumerable())
            .ToArray();
        
        // Act - Standard LINQ as reference
        var expected = first.Concat(second).Concat(third).ToArray();
        
        // Assert
        result.Length.ShouldBe(8);
        result.ShouldBe(expected);
    }

    #endregion
}
