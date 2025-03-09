namespace ZLinq.Tests.Linq;

public class AppendTest
{
    [Fact]
    public void AppendToEmpty()
    {
        var xs = Array.Empty<int>();
        var element = 42;
        
        // Compare with standard LINQ
        var expected = xs.Append(element).ToArray();
        
        // Test with ZLinq
        var actual = xs.AsValueEnumerable().Append(element).ToArray();
        
        actual.ShouldBe(expected);
    }

    [Fact]
    public void AppendToNonEmpty()
    {
        var xs = new[] { 1, 2, 3, 4, 5 };
        var element = 42;
        
        // Compare with standard LINQ
        var expected = xs.Append(element).ToArray();
        
        // Test with ZLinq
        var actual = xs.AsValueEnumerable().Append(element).ToArray();
        
        actual.ShouldBe(expected);
    }

    [Fact]
    public void TryGetNonEnumeratedCount()
    {
        var xs = new[] { 1, 2, 3, 4, 5 };
        var element = 42;
        
        var enumerable = xs.AsValueEnumerable().Append(element);
        
        // Should return true and correct count
        enumerable.TryGetNonEnumeratedCount(out var count).ShouldBeTrue();
        count.ShouldBe(xs.Length + 1);
        
        // Test with empty source
        var emptyEnumerable = Array.Empty<int>().AsValueEnumerable().Append(element);
        emptyEnumerable.TryGetNonEnumeratedCount(out var emptyCount).ShouldBeTrue();
        emptyCount.ShouldBe(1);
    }

    [Fact]
    public void TryCopyTo()
    {
        var xs = new[] { 1, 2, 3, 4, 5 };
        var element = 42;
        
        var enumerable = xs.AsValueEnumerable().Append(element);
        
        // Test with sufficient space
        var destination = new int[xs.Length + 1];
        enumerable.TryCopyTo(destination).ShouldBeTrue();
        
        var expected = xs.Append(element).ToArray();
        destination.ShouldBe(expected);
        
        // Test with insufficient space
        var smallDestination = new int[xs.Length];
        enumerable.TryCopyTo(smallDestination).ShouldBeFalse();
    }

    [Fact]
    public void IterationBehavior()
    {
        var xs = new[] { 1, 2, 3, 4, 5 };
        var element = 42;
        
        var enumerable = xs.AsValueEnumerable().Append(element);
        
        // Check iteration order
        int index = 0;
        var expected = xs.Append(element).ToArray();
        
        while (enumerable.TryGetNext(out var current))
        {
            current.ShouldBe(expected[index++]);
        }
        
        index.ShouldBe(expected.Length);
    }

    [Fact]
    public void IterationEmptySource()
    {
        var xs = Array.Empty<int>();
        var element = 42;
        
        var enumerable = xs.AsValueEnumerable().Append(element);
        
        enumerable.TryGetNext(out var current).ShouldBeTrue();
        current.ShouldBe(element);
        
        enumerable.TryGetNext(out _).ShouldBeFalse();
    }

    [Fact]
    public void ProperDisposal()
    {
        var xs = new[] { 1, 2, 3, 4, 5 };
        var element = 42;
        
        var enumerable = xs.AsValueEnumerable().Append(element);
        
        // After disposal, it should return false
        enumerable.TryGetNext(out _);  // Consume one element
        enumerable.Dispose();
        enumerable.TryGetNext(out _).ShouldBeFalse();
    }
    
    [Fact]
    public void SourceWithToIterableValueEnumerable()
    {
        var xs = new[] { 1, 2, 3, 4, 5 };
        var element = 42;
        
        // Use ToIterableValueEnumerable to avoid Span optimization
        var enumerable = xs.ToIterableValueEnumerable().Append(element);
        var expected = xs.Append(element).ToArray();
        
        enumerable.ToArray().ShouldBe(expected);
    }
}
