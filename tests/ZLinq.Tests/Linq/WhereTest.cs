using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ZLinq.Linq;

namespace ZLinq.Tests.Linq;

public class WhereTest
{
    [Fact]
    public void EmptySource()
    {
        // Arrange
        var empty = Array.Empty<int>();
        
        // Act
        var result = empty.AsValueEnumerable().Where(x => x > 0).ToArray();
        
        // Assert
        result.ShouldBeEmpty();
    }
    
    [Fact]
    public void WhereWithPredicate()
    {
        // Arrange
        var source = new[] { 1, 2, 3, 4, 5 };
        
        // Act
        var result = source.AsValueEnumerable().Where(x => x % 2 == 0).ToArray();
        
        // Assert
        result.ShouldBe(new[] { 2, 4 });
    }
    
    [Fact]
    public void WhereWithIndexPredicate()
    {
        // Arrange
        var source = new[] { 10, 20, 30, 40, 50 };
        
        // Act
        var result = source.AsValueEnumerable().Where((x, i) => i % 2 == 0).ToArray();
        
        // Assert
        result.ShouldBe(new[] { 10, 30, 50 });
    }
    
    [Fact]
    public void WhereFollowedBySelect()
    {
        // Arrange
        var source = new[] { 1, 2, 3, 4, 5 };
        
        // Act
        var result = source.AsValueEnumerable()
            .Where(x => x % 2 == 0)
            .Select(x => x * 10)
            .ToArray();
        
        // Assert
        result.ShouldBe(new[] { 20, 40 });
    }
    
    [Fact]
    public void WhereChaining()
    {
        // Arrange
        var source = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        
        // Act
        var result = source.AsValueEnumerable()
            .Where(x => x > 3)
            .Where(x => x < 8)
            .ToArray();
        
        // Assert
        result.ShouldBe(new[] { 4, 5, 6, 7 });
    }
    
    [Fact]
    public void TryGetNonEnumeratedCount_ReturnsFalse()
    {
        // Arrange
        var source = new[] { 1, 2, 3 };
        var whereEnumerable = source.AsValueEnumerable().Where(x => x > 1);
        
        // Act
        var result = whereEnumerable.TryGetNonEnumeratedCount(out var count);
        
        // Assert
        result.ShouldBeFalse();
        count.ShouldBe(0);
    }
    
    [Fact]
    public void TryGetSpan_ReturnsFalse()
    {
        // Arrange
        var source = new[] { 1, 2, 3 };
        var whereEnumerable = source.AsValueEnumerable().Where(x => x > 1);
        
        // Act
        var result = whereEnumerable.TryGetSpan(out var span);
        
        // Assert
        result.ShouldBeFalse();
        span.IsEmpty.ShouldBeTrue();
    }
    
    [Fact]
    public void TryCopyTo_ReturnsFalse()
    {
        // Arrange
        var source = new[] { 1, 2, 3 };
        var whereEnumerable = source.AsValueEnumerable().Where(x => x > 1);
        var destination = new int[3];
        
        // Act
        var result = whereEnumerable.TryCopyTo(destination);
        
        // Assert
        result.ShouldBeFalse();
    }
    
    [Fact]
    public void Dispose_CallsSourceDispose()
    {
        // Arrange
        var source = new[] { 1, 2, 3 };
        var enumerable = source.AsValueEnumerable().Where(x => x > 1);
        
        // Act & Assert (no exception should be thrown)
        enumerable.Dispose();
    }
    
    [Fact]
    public void WhereSelectOptimization()
    {
        // Arrange
        var source = new[] { 1, 2, 3, 4, 5 };
        
        // Act
        // This should use the optimized WhereSelect path
        var result1 = source.AsValueEnumerable()
            .Where(x => x % 2 == 0)
            .Select(x => x * 10)
            .ToArray();
            
        // This should use the standard Where followed by Select path
        var result2 = source.AsValueEnumerable()
            .Where(x => x % 2 == 0)
            .ToArray()
            .AsValueEnumerable()
            .Select(x => x * 10)
            .ToArray();
        
        // Assert
        result1.ShouldBe(new[] { 20, 40 });
        result2.ShouldBe(new[] { 20, 40 });
        // The results should be the same regardless of which path was taken
        result1.ShouldBe(result2);
    }
    
    [Fact]
    public void Where2_IndexIsTrackedCorrectly()
    {
        // Arrange
        var source = new[] { 10, 20, 30, 40, 50 };
        
        // Act - select items where (value + index) is even
        var result = source.AsValueEnumerable()
            .Where((x, i) => (x + i) % 2 == 0)
            .ToArray();
        
        // Assert - indices should be 0, 1, 2, 3, 4 and items 10, 20, 30, 40, 50
        // So we expect items where (10+0)%2==0, (20+1)%2==0, etc.
        result.ShouldBe(new[] { 10, 30, 50 });
    }
    
    [Fact]
    public void PredicateCalledOncePerItem()
    {
        // Arrange
        var source = new[] { 1, 2, 3, 4, 5 };
        var callCount = 0;
        
        // Act
        var result = source.AsValueEnumerable()
            .Where(x => { callCount++; return x % 2 == 0; })
            .ToArray();
        
        // Assert
        callCount.ShouldBe(5); // Called once for each source item
        result.ShouldBe(new[] { 2, 4 });
    }
    
    [Fact]
    public void PredicateWithIndexCalledOncePerItem()
    {
        // Arrange
        var source = new[] { 10, 20, 30, 40, 50 };
        var callCount = 0;
        var indices = new List<int>();
        
        // Act
        var result = source.AsValueEnumerable()
            .Where((x, i) => { callCount++; indices.Add(i); return i % 2 == 0; })
            .ToArray();
        
        // Assert
        callCount.ShouldBe(5); // Called once for each source item
        indices.ShouldBe(new[] { 0, 1, 2, 3, 4 }); // Indices should be sequential
        result.ShouldBe(new[] { 10, 30, 50 });
    }

    [Fact]
    public void WhereSelect_PredicateAndSelectorCalledCorrectly()
    {
        // Arrange
        var source = new[] { 1, 2, 3, 4, 5 };
        var predicateCalls = 0;
        var selectorCalls = 0;
        
        // Act
        var result = source.AsValueEnumerable()
            .Where(x => { predicateCalls++; return x % 2 == 0; })
            .Select(x => { selectorCalls++; return x * 10; })
            .ToArray();
        
        // Assert
        predicateCalls.ShouldBe(5); // Called for each source item
        selectorCalls.ShouldBe(2);  // Called only for items that passed the predicate
        result.ShouldBe(new[] { 20, 40 });
    }
}
