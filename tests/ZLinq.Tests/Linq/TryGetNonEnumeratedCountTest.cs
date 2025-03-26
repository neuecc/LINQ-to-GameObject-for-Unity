namespace ZLinq.Tests.Linq;

public class TryGetNonEnumeratedCountTest
{
    [Fact]
    public void EmptyCollection()
    {
        var xs = Array.Empty<int>();

        // System LINQ doesn't have this feature directly to compare against
        
        // ZLinq implementations
        var result = xs.AsValueEnumerable().TryGetNonEnumeratedCount(out var count);
        result.ShouldBeTrue();
        count.ShouldBe(0);

        xs.ToValueEnumerable().TryGetNonEnumeratedCount(out count).ShouldBeFalse();
    }

    [Fact]
    public void NonEmptyCollection()
    {
        var xs = new[] { 1, 2, 3, 4, 5 };

        // ZLinq implementations
        var result = xs.AsValueEnumerable().TryGetNonEnumeratedCount(out var count);
        result.ShouldBeTrue();
        count.ShouldBe(5);

        xs.ToValueEnumerable().TryGetNonEnumeratedCount(out count).ShouldBeFalse();
    }
    
    [Fact]
    public void NonCountableSource()
    {
        // Use TestUtil.Empty() which returns an IEnumerable without count information
        var result = TestUtil.Empty<int>().AsValueEnumerable().TryGetNonEnumeratedCount(out var count);
        result.ShouldBeFalse();
        count.ShouldBe(0); // Default value when count cannot be determined
    }

    [Fact]
    public void ChainedOperations_PreservesCount()
    {
        var xs = new[] { 1, 2, 3, 4, 5 };

        // Concat operation preserves count when both sources have count
        var result = xs.AsValueEnumerable()
            .Concat(new[] { 6, 7, 8 }.AsValueEnumerable())
            .TryGetNonEnumeratedCount(out var count);
            
        result.ShouldBeTrue();
        count.ShouldBe(8);
    }

    [Fact]
    public void ChainedOperations_LosesCount()
    {
        var xs = new[] { 1, 2, 3, 4, 5 };

        // Where operation doesn't preserve count
        var result = xs.AsValueEnumerable()
            .Where(x => x > 0) // Filter that keeps all items but breaks count optimization
            .TryGetNonEnumeratedCount(out var count);
            
        result.ShouldBeFalse();
        count.ShouldBe(0);
    }

    [Fact]
    public void WithSelect()
    {
        var xs = new[] { 1, 2, 3, 4, 5 };

        // Select should preserve count
        var result = xs.AsValueEnumerable()
            .Select(x => x * 2)
            .TryGetNonEnumeratedCount(out var count);
            
        result.ShouldBeTrue();
        count.ShouldBe(5);
    }

    [Fact]
    public void WithChunk()
    {
        var xs = new[] { 1, 2, 3, 4, 5 };

        // Chunk has special handling for count
        var result = xs.AsValueEnumerable()
            .Chunk(2)
            .TryGetNonEnumeratedCount(out var count);
            
        result.ShouldBeTrue();
        count.ShouldBe(3); // Ceiling(5/2) = 3 chunks
    }

    [Fact]
    public void WithConcatOneEmptySide()
    {
        var xs = new[] { 1, 2, 3, 4, 5 };
        var empty = Array.Empty<int>();

        // Concat with one empty side
        var result1 = xs.AsValueEnumerable()
            .Concat(empty.AsValueEnumerable())
            .TryGetNonEnumeratedCount(out var count1);
            
        result1.ShouldBeTrue();
        count1.ShouldBe(5);

        var result2 = empty.AsValueEnumerable()
            .Concat(xs.AsValueEnumerable())
            .TryGetNonEnumeratedCount(out var count2);
            
        result2.ShouldBeTrue();
        count2.ShouldBe(5);
    }

    [Fact]
    public void WithMixedCountableAndNonCountable()
    {
        var xs = new[] { 1, 2, 3 };

        // Concat with non-countable source should fail
        var result = xs.AsValueEnumerable()
            .Concat(TestUtil.Empty<int>().AsValueEnumerable())
            .TryGetNonEnumeratedCount(out var count);
            
        result.ShouldBeFalse();
        count.ShouldBe(0);
    }
}
