using System;
using System.Collections.Generic;
using System.Linq;
using Shouldly;
using Xunit;

namespace ZLinq.Tests.Linq;

public class SelectTest
{
    #region Basic Select Tests

    [Fact]
    public void Select_WithEmptySource_ShouldReturnEmptyCollection()
    {
        // Arrange
        var source = Array.Empty<int>();

        // Act - Standard LINQ as reference
        var expected = source
            .Select(x => x * 2)
            .ToArray();

        // Act - ZLinq
        var actual = source
            .AsValueEnumerable()
            .Select(x => x * 2)
            .ToArray();

        // Assert
        actual.ShouldBeEmpty();
        actual.ShouldBe(expected);
    }

    [Fact]
    public void Select_WithNonEmptySource_ShouldTransformElements()
    {
        // Arrange
        var source = new[] { 1, 2, 3, 4, 5 };

        // Act - Standard LINQ as reference
        var expected = source
            .Select(x => x * 2)
            .ToArray();

        // Act - ZLinq
        var actual = source
            .AsValueEnumerable()
            .Select(x => x * 2)
            .ToArray();

        // Assert
        actual.Length.ShouldBe(5);
        actual.ShouldBe(expected);
    }

    [Fact]
    public void Select_WithObjectTransformation_ShouldTransformToNewType()
    {
        // Arrange
        var source = new[] { 1, 2, 3, 4, 5 };

        // Act - Standard LINQ as reference
        var expected = source
            .Select(x => $"Value: {x}")
            .ToArray();

        // Act - ZLinq
        var actual = source
            .AsValueEnumerable()
            .Select(x => $"Value: {x}")
            .ToArray();

        // Assert
        actual.Length.ShouldBe(5);
        actual.ShouldBe(expected);
    }

    [Fact]
    public void Select_WithCustomObject_ShouldTransformCorrectly()
    {
        // Arrange
        var source = new[] 
        {
            new { Id = 1, Name = "Item1" },
            new { Id = 2, Name = "Item2" },
            new { Id = 3, Name = "Item3" }
        };

        // Act - Standard LINQ as reference
        var expected = source
            .Select(x => new { x.Id, UpperName = x.Name.ToUpper() })
            .ToArray();

        // Act - ZLinq
        var actual = source
            .AsValueEnumerable()
            .Select(x => new { x.Id, UpperName = x.Name.ToUpper() })
            .ToArray();

        // Assert
        actual.Length.ShouldBe(3);
        for (int i = 0; i < actual.Length; i++)
        {
            actual[i].Id.ShouldBe(expected[i].Id);
            actual[i].UpperName.ShouldBe(expected[i].UpperName);
        }
    }

    #endregion

    #region Select With Index Tests

    [Fact]
    public void Select_WithIndex_EmptySource_ShouldReturnEmptyCollection()
    {
        // Arrange
        var source = Array.Empty<int>();

        // Act - Standard LINQ as reference
        var expected = source
            .Select((x, i) => x + i)
            .ToArray();

        // Act - ZLinq
        var actual = source
            .AsValueEnumerable()
            .Select((x, i) => x + i)
            .ToArray();

        // Assert
        actual.ShouldBeEmpty();
        actual.ShouldBe(expected);
    }

    [Fact]
    public void Select_WithIndex_ShouldTransformWithIndex()
    {
        // Arrange
        var source = new[] { 10, 20, 30, 40, 50 };

        // Act - Standard LINQ as reference
        var expected = source
            .Select((x, i) => x + i)
            .ToArray();

        // Act - ZLinq
        var actual = source
            .AsValueEnumerable()
            .Select((x, i) => x + i)
            .ToArray();

        // Assert
        actual.Length.ShouldBe(5);
        actual.ShouldBe(expected);
        actual.ShouldBe(new[] { 10, 21, 32, 43, 54 });
    }

    [Fact]
    public void Select_WithIndex_ShouldUseIndexInSelector()
    {
        // Arrange
        var source = new[] { "A", "B", "C", "D" };

        // Act - Standard LINQ as reference
        var expected = source
            .Select((x, i) => $"{i + 1}. {x}")
            .ToArray();

        // Act - ZLinq
        var actual = source
            .AsValueEnumerable()
            .Select((x, i) => $"{i + 1}. {x}")
            .ToArray();

        // Assert
        actual.Length.ShouldBe(4);
        actual.ShouldBe(expected);
        actual.ShouldBe(new[] { "1. A", "2. B", "3. C", "4. D" });
    }

    #endregion

    #region Select+Where Tests

    [Fact]
    public void SelectWhere_ShouldFilterAfterTransformation()
    {
        // Arrange
        var source = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

        // Act - Standard LINQ as reference
        var expected = source
            .Select(x => x * 2)
            .Where(x => x > 10)
            .ToArray();

        // Act - ZLinq
        var actual = source
            .AsValueEnumerable()
            .Select(x => x * 2)
            .Where(x => x > 10)
            .ToArray();

        // Assert
        actual.Length.ShouldBe(5);
        actual.ShouldBe(expected);
        actual.ShouldBe(new[] { 12, 14, 16, 18, 20 });
    }

    [Fact]
    public void SelectWhere_WithEmptySource_ShouldReturnEmptyCollection()
    {
        // Arrange
        var source = Array.Empty<int>();

        // Act - Standard LINQ as reference
        var expected = source
            .Select(x => x * 2)
            .Where(x => x > 10)
            .ToArray();

        // Act - ZLinq
        var actual = source
            .AsValueEnumerable()
            .Select(x => x * 2)
            .Where(x => x > 10)
            .ToArray();

        // Assert
        actual.ShouldBeEmpty();
        actual.ShouldBe(expected);
    }

    [Fact]
    public void SelectWhere_WithNoMatchingElements_ShouldReturnEmptyCollection()
    {
        // Arrange
        var source = new[] { 1, 2, 3, 4, 5 };

        // Act - Standard LINQ as reference
        var expected = source
            .Select(x => x * 2)
            .Where(x => x > 20)
            .ToArray();

        // Act - ZLinq
        var actual = source
            .AsValueEnumerable()
            .Select(x => x * 2)
            .Where(x => x > 20)
            .ToArray();

        // Assert
        actual.ShouldBeEmpty();
        actual.ShouldBe(expected);
    }

    [Fact]
    public void SelectWhere_ShouldApplyPredicateToTransformedElements()
    {
        // Arrange
        var source = new[] { "abc", "def", "a", "hello", "xy" };

        // Act - Standard LINQ as reference
        var expected = source
            .Select(x => x.Length)
            .Where(x => x > 2)
            .ToArray();

        // Act - ZLinq
        var actual = source
            .AsValueEnumerable()
            .Select(x => x.Length)
            .Where(x => x > 2)
            .ToArray();

        // Assert
        actual.Length.ShouldBe(expected.Length);
        actual.ShouldBe(expected);
    }

    #endregion

    #region Optimization Tests

    [Fact]
    public void Select_TryGetNonEnumeratedCount_ShouldReturnCountForStandardCollections()
    {
        // Arrange
        var source = new[] { 1, 2, 3, 4, 5 };
        var enumerable = source.AsValueEnumerable().Select(x => x * 2);
        
        // Act
        var result = enumerable.TryGetNonEnumeratedCount(out var count);
        
        // Assert
        result.ShouldBeTrue();
        count.ShouldBe(5);
    }

    [Fact]
    public void Select_TryGetSpan_ShouldReturnFalse()
    {
        // Arrange
        var source = new[] { 1, 2, 3, 4, 5 };
        var enumerable = source.AsValueEnumerable().Select(x => x * 2);
        
        // Act
        var result = enumerable.TryGetSpan(out var span);
        
        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void Select_TryCopyTo_ShouldCopyElementsForSingleElement()
    {
        // Arrange
        var source = new[] { 42 };
        var destination = new int[1];
        var enumerable = source.AsValueEnumerable().Select(x => x * 2);
        
        // Act
        var result = enumerable.TryCopyTo(destination, 0);
        
        // Assert
        result.ShouldBeTrue();
        destination[0].ShouldBe(84);
    }

    [Fact]
    public void Select_TryCopyTo_ShouldCopyElementsForSpanSource()
    {
        // Arrange
        var source = new[] { 1, 2, 3, 4, 5 };
        var destination = new int[5];
        var enumerable = source.AsValueEnumerable().Select(x => x * 2);
        
        // Act
        var result = enumerable.TryCopyTo(destination, 0);
        
        // Assert
        result.ShouldBeTrue();
        destination.ShouldBe(new[] { 2, 4, 6, 8, 10 });
    }

    #endregion

    #region Disposal Tests

    [Fact]
    public void Select_ShouldDisposeSourceEnumerator()
    {
        // Arrange
        var disposed = false;
        
        // Create a sequence that tracks disposal
        IEnumerable<int> GetTrackingSequence()
        {
            try
            {
                yield return 1;
                yield return 2;
            }
            finally
            {
                disposed = true;
            }
        }

        // Act
        var enumerable = GetTrackingSequence().AsValueEnumerable().Select(x => x * 2);
        
        // Use the enumerable and then dispose it
        using (var enumerator = enumerable.GetEnumerator())
        {
            while (enumerator.MoveNext()) { }
        }

        // Assert
        disposed.ShouldBeTrue();
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void Select_WithNullSelector_ShouldThrowArgumentNullException()
    {
        // Arrange
        var source = new[] { 1, 2, 3 };
        Func<int, int> selector = null!;
        
        // Act & Assert
        Should.Throw<ArgumentNullException>(() =>
        {
            source.AsValueEnumerable().Select(selector).ToArray();
        });
    }

    [Fact]
    public void Select_WithNullIndexSelector_ShouldThrowArgumentNullException()
    {
        // Arrange
        var source = new[] { 1, 2, 3 };
        Func<int, int, int> selector = null!;
        
        // Act & Assert
        Should.Throw<ArgumentNullException>(() =>
        {
            source.AsValueEnumerable().Select(selector).ToArray();
        });
    }

    [Fact]
    public void SelectWhere_WithNullPredicate_ShouldThrowArgumentNullException()
    {
        // Arrange
        var source = new[] { 1, 2, 3 };
        Func<int, bool> predicate = null!;
        
        // Act & Assert
        Should.Throw<ArgumentNullException>(() =>
        {
            source.AsValueEnumerable().Select(x => x * 2).Where(predicate).ToArray();
        });
    }

    #endregion

    #region Combined Operations Tests

    [Fact]
    public void Select_CombinedWithOtherOperations_ShouldWorkCorrectly()
    {
        // Arrange
        var source = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        
        // Act - Standard LINQ as reference
        var expected = source
            .Select(x => x * 3)
            .Where(x => x % 2 == 0)
            .Take(3)
            .ToArray();
            
        // Act - ZLinq
        var actual = source
            .AsValueEnumerable()
            .Select(x => x * 3)
            .Where(x => x % 2 == 0)
            .Take(3)
            .ToArray();
            
        // Assert
        actual.ShouldBe(expected);
    }

    #endregion
}
