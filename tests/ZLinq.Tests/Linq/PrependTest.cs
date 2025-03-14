namespace ZLinq.Tests.Linq;

public class PrependTest
{
    [Fact]
    public void PrependToEmpty()
    {
        var xs = Array.Empty<int>();
        var element = 42;
        
        // Compare with standard LINQ
        var expected = xs.Prepend(element).ToArray();
        
        // Test with ZLinq
        var actual = xs.AsValueEnumerable().Prepend(element).ToArray();
        
        actual.ShouldBe(expected);
    }

    [Fact]
    public void PrependToNonEmpty()
    {
        var xs = new[] { 1, 2, 3, 4, 5 };
        var element = 42;
        
        // Compare with standard LINQ
        var expected = xs.Prepend(element).ToArray();
        
        // Test with ZLinq
        var actual = xs.AsValueEnumerable().Prepend(element).ToArray();
        
        actual.ShouldBe(expected);
    }

    [Fact]
    public void TryGetNonEnumeratedCount()
    {
        var xs = new[] { 1, 2, 3, 4, 5 };
        var element = 42;
        
        var enumerable = xs.AsValueEnumerable().Prepend(element);
        
        // Should return true and correct count
        enumerable.TryGetNonEnumeratedCount(out var count).ShouldBeTrue();
        count.ShouldBe(xs.Length + 1);
        
        // Test with empty source
        var emptyEnumerable = Array.Empty<int>().AsValueEnumerable().Prepend(element);
        emptyEnumerable.TryGetNonEnumeratedCount(out var emptyCount).ShouldBeTrue();
        emptyCount.ShouldBe(1);
    }

    [Fact]
    public void TryCopyTo()
    {
        var xs = new[] { 1, 2, 3, 4, 5 };
        var element = 42;
        
        var enumerable = xs.AsValueEnumerable().Prepend(element);
        
        // Test with sufficient space
        var destination = new int[xs.Length + 1];
        enumerable.TryCopyTo(destination).ShouldBeTrue();
        
        var expected = xs.Prepend(element).ToArray();
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
        
        var enumerable = xs.AsValueEnumerable().Prepend(element);
        
        // Check iteration order
        int index = 0;
        var expected = xs.Prepend(element).ToArray();
        
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
        
        var enumerable = xs.AsValueEnumerable().Prepend(element);
        
        enumerable.TryGetNext(out var current).ShouldBeTrue();
        current.ShouldBe(element);
        
        enumerable.TryGetNext(out _).ShouldBeFalse();
    }

    [Fact]
    public void ProperDisposal()
    {
        var xs = new[] { 1, 2, 3, 4, 5 };
        var element = 42;
        
        var enumerable = xs.AsValueEnumerable().Prepend(element);
        
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
        var expected = xs.Prepend(element).ToArray();

        xs.ToValueEnumerable().Prepend(element).ToArray().ShouldBe(expected);
    }
}
