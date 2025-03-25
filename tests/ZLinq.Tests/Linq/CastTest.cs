using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ZLinq.Linq;

namespace ZLinq.Tests.Linq;

public class CastTest
{
    [Fact]
    public void EmptySource()
    {
        // Arrange
        var empty = Array.Empty<object>();
        
        // Act
        var result = empty.AsValueEnumerable().Cast<int>().ToArray();
        
        // Assert
        result.ShouldBeEmpty();
    }
    
    [Fact]
    public void CastCompatibleTypes()
    {
        // Arrange
        var source = new object[] { 1, 2, 3, 4, 5 };
        
        // Act
        var result = source.AsValueEnumerable().Cast<int>().ToArray();
        
        // Assert
        result.ShouldBe(new[] { 1, 2, 3, 4, 5 });
    }
    
    [Fact]
    public void CastToBaseType()
    {
        // Arrange
        var source = new[] { "one", "two", "three" };
        
        // Act
        var result = source.AsValueEnumerable().Cast<object>().ToArray();
        
        // Assert
        result.ShouldBe(new object[] { "one", "two", "three" });
    }
    
    [Fact]
    public void CastThrowsForIncompatibleTypes()
    {
        // Arrange
        var source = new object[] { "one", 2, "three" }; // Mixed string and int
        
        // Act & Assert
        Should.Throw<InvalidCastException>(() =>
        {
            var enumerable = source.AsValueEnumerable().Cast<int>();
            enumerable.ToArray(); // Will throw when trying to cast "one" to int
        });
    }
    
    [Fact]
    public void CastFollowedByLinqOperations()
    {
        // Arrange
        var source = new object[] { 1, 2, 3, 4, 5 };
        
        // Act
        var result = source.AsValueEnumerable()
            .Cast<int>()
            .Where(x => x % 2 == 0)
            .Select(x => x * 10)
            .ToArray();
        
        // Assert
        result.ShouldBe(new[] { 20, 40 });
    }
    
    [Fact]
    public void TryGetNonEnumeratedCount_ReturnsSourceCount()
    {
        // Arrange
        var source = new object[] { 1, 2, 3 };
        var castEnumerable = source.AsValueEnumerable().Cast<int>();
        
        // Act
        var result = castEnumerable.TryGetNonEnumeratedCount(out var count);
        
        // Assert
        // The Cast enumerator passes through the count from its source
        result.ShouldBeTrue(); 
        count.ShouldBe(3);
    }
    
    [Fact]
    public void TryGetSpan_ReturnsFalse()
    {
        // Arrange
        var source = new object[] { 1, 2, 3 };
        var castEnumerable = source.AsValueEnumerable().Cast<int>();
        
        // Act
        var result = castEnumerable.TryGetSpan(out var span);
        
        // Assert
        result.ShouldBeFalse();
        span.IsEmpty.ShouldBeTrue();
    }
    
    [Fact]
    public void TryCopyTo_ReturnsFalse()
    {
        // Arrange
        var source = new object[] { 1, 2, 3 };
        var castEnumerable = source.AsValueEnumerable().Cast<int>();
        var destination = new int[3];
        
        // Act
        var result = castEnumerable.TryCopyTo(destination);
        
        // Assert
        result.ShouldBeFalse();
    }
    
    [Fact]
    public void Dispose_CallsSourceDispose()
    {
        // Arrange
        var source = new object[] { 1, 2, 3 };
        var enumerable = source.AsValueEnumerable().Cast<int>();
        
        // Act & Assert (no exception should be thrown)
        enumerable.Dispose();
    }
    
    [Fact]
    public void CastWithNull()
    {
        // Arrange
        var source = new object?[] { null, 1, null, 2 };
        
        // Act
        var result = source.AsValueEnumerable().Cast<int?>().ToArray();
        
        // Assert
        result.ShouldBe(new int?[] { null, 1, null, 2 });
    }
    
    [Fact]
    public void CastPreservesItemsOrder()
    {
        // Arrange
        var source = new object[] { "first", "second", "third", "fourth" };
        
        // Act
        var result = source.AsValueEnumerable().Cast<string>().ToArray();
        
        // Assert
        result.ShouldBe(new[] { "first", "second", "third", "fourth" });
    }
    
    [Fact]
    public void TryGetNext_ReturnsItemsInOrder()
    {
        // Arrange
        var source = new object[] { 1, 2, 3, 4, 5 };
        var castEnumerable = source.AsValueEnumerable().Cast<int>();
        var result = new List<int>();
        
        // Act
        using var enumerator = castEnumerable.GetEnumerator();
        while (enumerator.MoveNext())
        {
            result.Add(enumerator.Current);
        }
        
        // Assert
        result.ShouldBe(new[] { 1, 2, 3, 4, 5 });
    }
    
    [Fact]
    public void CompareCastWithOfType()
    {
        // Arrange
        var source = new object[] { "one", 2, "three", 4 }; // Mixed string and int
        
        // Act - Cast will throw, OfType will filter
        var ofTypeResult = source.AsValueEnumerable().OfType<int>().ToArray();
        
        // Assert
        ofTypeResult.ShouldBe(new[] { 2, 4 });
        
        // Act & Assert for Cast (should throw)
        Should.Throw<InvalidCastException>(() => 
        {
            source.AsValueEnumerable().Cast<int>().ToArray();
        });
    }
}
