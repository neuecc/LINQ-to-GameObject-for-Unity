using System;
using Xunit;

namespace ZLinq.Tests.Linq;

public class SkipWhileTest
{
    [Fact]
    public void SkipWhile_WithPredicate_SkipsUntilPredicateFalse()
    {
        // Arrange
        var source = new[] { 1, 2, 3, 4, 5, 1, 2 }.AsValueEnumerable();

        // Act
        var result = source.SkipWhile(x => x < 4).ToArray();

        // Assert
        result.ShouldBeEquivalentTo(new[] { 4, 5, 1, 2 });
    }

    [Fact]
    public void SkipWhile_EmptySource_ReturnsEmpty()
    {
        // Arrange
        var source = Array.Empty<int>().AsValueEnumerable();

        // Act
        var result = source.SkipWhile(x => x < 4).ToArray();

        // Assert
        result.ShouldBeEmpty();
    }

    [Fact]
    public void SkipWhile_NoElementsMatchPredicate_ReturnsAllElements()
    {
        // Arrange
        var source = new[] { 5, 6, 7, 8 }.AsValueEnumerable();

        // Act
        var result = source.SkipWhile(x => x < 1).ToArray();

        // Assert
        result.ShouldBeEquivalentTo(new[] { 5, 6, 7, 8 });
    }

    [Fact]
    public void SkipWhile_AllElementsMatchPredicate_ReturnsEmpty()
    {
        // Arrange
        var source = new[] { 1, 2, 3, 4 }.AsValueEnumerable();

        // Act
        var result = source.SkipWhile(x => x < 10).ToArray();

        // Assert
        result.ShouldBeEmpty();
    }

    [Fact]
    public void SkipWhile_WithIndexPredicate_SkipsUntilPredicateFalse()
    {
        // Arrange
        var source = new[] { 10, 20, 30, 40, 50 }.AsValueEnumerable();

        // Act
        var result = source.SkipWhile((x, index) => index < 2).ToArray();

        // Assert
        result.ShouldBeEquivalentTo(new[] { 30, 40, 50 });
    }

    [Fact]
    public void SkipWhile_WithIndexPredicate_IndexIncrementsCorrectly()
    {
        // Arrange
        var source = new[] { 1, 2, 3, 4, 5 }.AsValueEnumerable();

        // Act
        var result = source.SkipWhile((x, index) => x + index < 5).ToArray();

        // Assert
        result.ShouldBeEquivalentTo(new[] { 3, 4, 5 });
    }

    [Fact]
    public void SkipWhile_MultipleEnumeration_ReturnsCorrectResults()
    {
        // Arrange
        var source = new[] { 1, 2, 3, 4, 5 }.AsValueEnumerable();
        var skipWhileQuery = source.SkipWhile(x => x < 3);

        // Act
        var firstRun = skipWhileQuery.ToArray();
        var secondRun = skipWhileQuery.ToArray();

        // Assert
        firstRun.ShouldBeEquivalentTo(new[] { 3, 4, 5 });
        secondRun.ShouldBeEquivalentTo(new[] { 3, 4, 5 });
    }

    [Fact]
    public void SkipWhile_LargeSequence_HandlesCorrectly()
    {
        // Arrange
        var source = Enumerable.Range(0, 1000).ToArray().AsValueEnumerable();

        // Act
        var result = source.SkipWhile(x => x < 500).ToArray();

        // Assert
        result.Count().ShouldBe(500);
        result.First().ShouldBe(500);
        result.Last().ShouldBe(999);
    }
}
