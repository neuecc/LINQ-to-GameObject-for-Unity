#if !NET48

using System;
using System.Linq;

namespace ZLinq.Tests.Linq;

public class SkipLastTest
{
    [Fact]
    public void SkipLast_Empty()
    {
        var empty = Array.Empty<int>();

        var expected = empty.SkipLast(5).ToArray();
        var actual1 = empty.AsValueEnumerable().SkipLast(5).ToArray();
        var actual2 = empty.ToValueEnumerable().SkipLast(5).ToArray();

        actual1.ShouldBe(expected);
        actual2.ShouldBe(expected);
    }

    [Fact]
    public void SkipLast_Zero()
    {
        var sequence = Enumerable.Range(1, 10).ToArray();

        var expected = sequence.SkipLast(0).ToArray();
        var actual1 = sequence.AsValueEnumerable().SkipLast(0).ToArray();
        var actual2 = sequence.ToValueEnumerable().SkipLast(0).ToArray();

        actual1.ShouldBe(expected); // Should be the entire sequence
        actual2.ShouldBe(expected); // Should be the entire sequence
    }

    [Fact]
    public void SkipLast_Negative()
    {
        var sequence = Enumerable.Range(1, 10).ToArray();

        var expected = sequence.SkipLast(-5).ToArray();
        var actual1 = sequence.AsValueEnumerable().SkipLast(-5).ToArray();
        var actual2 = sequence.ToValueEnumerable().SkipLast(-5).ToArray();

        actual1.ShouldBe(expected); // Should be the entire sequence
        actual2.ShouldBe(expected); // Should be the entire sequence
    }

    [Fact]
    public void SkipLast_PartialCollection()
    {
        var sequence = Enumerable.Range(1, 10).ToArray();

        var expected = sequence.SkipLast(5).ToArray();
        var actual1 = sequence.AsValueEnumerable().SkipLast(5).ToArray();
        var actual2 = sequence.ToValueEnumerable().SkipLast(5).ToArray();

        actual1.ShouldBe(expected); // Should be [1,2,3,4,5]
        actual2.ShouldBe(expected); // Should be [1,2,3,4,5]
    }

    [Fact]
    public void SkipLast_ExceedingSize()
    {
        var sequence = Enumerable.Range(1, 10).ToArray();

        var expected = sequence.SkipLast(20).ToArray();
        var actual1 = sequence.AsValueEnumerable().SkipLast(20).ToArray();
        var actual2 = sequence.ToValueEnumerable().SkipLast(20).ToArray();

        actual1.ShouldBe(expected); // Should return empty array
        actual2.ShouldBe(expected); // Should return empty array
    }

    [Fact]
    public void SkipLast_ExactSize()
    {
        var sequence = Enumerable.Range(1, 10).ToArray();

        var expected = sequence.SkipLast(10).ToArray();
        var actual1 = sequence.AsValueEnumerable().SkipLast(10).ToArray();
        var actual2 = sequence.ToValueEnumerable().SkipLast(10).ToArray();

        actual1.ShouldBe(expected); // Should return empty array
        actual2.ShouldBe(expected); // Should return empty array
    }

    [Fact]
    public void SkipLast_TryGetNonEnumeratedCount()
    {
        var sequence = Enumerable.Range(1, 10).ToArray();
        var valueEnumerable = sequence.AsValueEnumerable().SkipLast(5);

        valueEnumerable.TryGetNonEnumeratedCount(out var count).ShouldBeTrue();
        count.ShouldBe(5); // 10 - 5 = 5 elements remaining

        // Exceeding size
        var valueEnumerable2 = sequence.AsValueEnumerable().SkipLast(15);
        valueEnumerable2.TryGetNonEnumeratedCount(out var count2).ShouldBeTrue();
        count2.ShouldBe(0); // Count is larger than sequence, should be 0

        // Empty sequence
        var emptyEnumerable = Array.Empty<int>().AsValueEnumerable().SkipLast(5);
        emptyEnumerable.TryGetNonEnumeratedCount(out var count3).ShouldBeTrue();
        count3.ShouldBe(0);
    }

    [Fact]
    public void SkipLast_TryGetSpan()
    {
        // The implementation should properly slice the span to get all elements except the last few
        var sequence = Enumerable.Range(1, 10).ToArray();
        var valueEnumerable = sequence.AsValueEnumerable().SkipLast(3);

        // Test that TryGetSpan works when the underlying source supports spans
        if (valueEnumerable.TryGetSpan(out var span))
        {
            span.Length.ShouldBe(7);
            span.ToArray().ShouldBe(new[] { 1, 2, 3, 4, 5, 6, 7 });
        }

        // This test might need adjustment based on the actual behavior of the underlying
        // implementation of AsValueEnumerable, which might not always support TryGetSpan
    }

    [Fact]
    public void SkipLast_EnumerationImplementation()
    {
        var sequence = Enumerable.Range(1, 5).ToArray();
        var skipLast = sequence.ToValueEnumerable().SkipLast(2).Enumerator;

        // Test the implementation of TryGetNext which uses the buffer logic
        var result = new List<int>();
        while (skipLast.TryGetNext(out var current))
        {
            result.Add(current);
        }

        result.ShouldBe(new[] { 1, 2, 3 });
    }

    [Fact]
    public void SkipLast_LargeCollection()
    {
        var sequence = Enumerable.Range(1, 10000).ToArray();

        var expected = sequence.SkipLast(100).ToArray();
        var actual = sequence.AsValueEnumerable().SkipLast(100).ToArray();

        actual.ShouldBe(expected);
        actual.Length.ShouldBe(9900);
        actual[0].ShouldBe(1); // First element should still be 1
        actual[^1].ShouldBe(9900); // Last element should be the 9900th
    }

    [Fact]
    public void SkipLast_MultipleEnumeration()
    {
        var sequence = Enumerable.Range(1, 10).ToArray();
        var skipLast = sequence.AsValueEnumerable().SkipLast(3);

        // First enumeration
        var first = skipLast.ToArray();
        // Second enumeration
        var second = skipLast.ToArray();

        first.ShouldBe(second);
        first.ShouldBe(new[] { 1, 2, 3, 4, 5, 6, 7 });
    }

    [Fact]
    public void SkipLast_DisposalBehavior()
    {
        var disposed = false;
        var disposableCollection = new DisposableTestEnumerator<int>(
            Enumerable.Range(1, 5),
            () => disposed = true
        );

        var enumerable = disposableCollection.AsValueEnumerable().SkipLast(2);
        var array = enumerable.ToArray();
        array.ShouldBe(new[] { 1, 2, 3 });

        disposed.ShouldBeTrue();
    }

    [Fact]
    public void SkipLastTryCopyTo()
    {
        var source = new[] { 1, 2, 3, 4, 5 }.AsValueEnumerable();
        var dest = new int[5];
        source.SkipLast(1).TryCopyTo(dest, 0).ShouldBeTrue();
        dest.ShouldBe([1, 2, 3, 4, 0]);

        Array.Clear(dest);
        source.SkipLast(2).TryCopyTo(dest, 0).ShouldBeTrue();
        dest.ShouldBe([1, 2, 3, 0, 0]);

        Array.Clear(dest);
        source.SkipLast(2).TryCopyTo(dest, 1).ShouldBeTrue();
        dest.ShouldBe([2, 3, 0, 0, 0]);

        Array.Clear(dest);
        source.SkipLast(2).TryCopyTo(dest, 2).ShouldBeTrue();
        dest.ShouldBe([3, 0, 0, 0, 0]);
    }

    [Fact]
    public void SkipLastTryCopyTo2()
    {
        var source1 = new[] { 1, 2, 3, 4, 5, 6, 7, 8 };
        var source2 = new[] { 1, 2, 3, 4, 5, 6, 7, 8 }.AsValueEnumerable();

        var expected = source1.SkipLast(2).SkipLast(3).ToArray();
        source2.SkipLast(2).SkipLast(3).ToArray().ShouldBe(expected);

        var expected2 = source1.Take(7).SkipLast(2).SkipLast(3).ToArray();
        source2.Take(7).SkipLast(2).SkipLast(3).ToArray().ShouldBe(expected2);
    }

    [Fact]
    public void SkipLast_TryCopyTo_EmptyCollection()
    {
        var emptySource = Array.Empty<int>().AsValueEnumerable();
        var dest = new int[5];

        // Empty source with skip
        emptySource.SkipLast(3).TryCopyTo(dest, 0).ShouldBeFalse();
        dest.ShouldBe(new int[5]); // Destination should remain unchanged
    }

    [Fact]
    public void SkipLast_TryCopyTo_SkipCountExceedsSourceSize()
    {
        var source = new[] { 1, 2, 3 }.AsValueEnumerable();
        var dest = new int[5];

        // Skip more than available
        source.SkipLast(5).TryCopyTo(dest, 0).ShouldBeFalse();
        dest.ShouldBe(new int[5]); // Destination should remain unchanged
    }

    [Fact]
    public void SkipLast_TryCopyTo_InvalidOffset()
    {
        var source = new[] { 1, 2, 3, 4, 5 }.AsValueEnumerable();
        var dest = new int[3];

        // Negative offset (in equivalent offset form)
        source.SkipLast(2).TryCopyTo(dest, ^4).ShouldBeFalse();

        // Offset beyond remaining count
        source.SkipLast(2).TryCopyTo(dest, 3).ShouldBeFalse();
    }

    [Fact]
    public void SkipLast_TryCopyTo_DestinationTooSmall()
    {
        var source = new[] { 1, 2, 3, 4, 5, 6, 7 }.AsValueEnumerable();
        var smallDest = new int[2];

        // Partial copy with small destination
        source.SkipLast(2).TryCopyTo(smallDest, 0).ShouldBeTrue();
        smallDest.ShouldBe(new[] { 1, 2 }); // Should copy only what fits

        // With offset
        Array.Clear(smallDest);
        source.SkipLast(2).TryCopyTo(smallDest, 2).ShouldBeTrue();
        smallDest.ShouldBe(new[] { 3, 4 });
    }

    [Fact]
    public void SkipLast_TryCopyTo_FromEnd()
    {
        var source = new[] { 1, 2, 3, 4, 5 }.AsValueEnumerable();
        var dest = new int[3];

        // Using Index.FromEnd
        source.SkipLast(2).TryCopyTo(dest, ^3).ShouldBeTrue();
        dest.ShouldBe(new[] { 1, 2, 3 });

        Array.Clear(dest);
        source.SkipLast(2).TryCopyTo(dest, ^2).ShouldBeTrue();
        dest.ShouldBe(new[] { 2, 3, 0 });

        Array.Clear(dest);
        source.SkipLast(2).TryCopyTo(dest, ^1).ShouldBeTrue();
        dest.ShouldBe(new[] { 3, 0, 0 });
    }

    [Fact]
    public void SkipLast_TryCopyTo_WithZeroElementsAvailable()
    {
        var source = new[] { 1, 2, 3 }.AsValueEnumerable();
        var dest = new int[3];

        // When element count is exactly zero after calculations
        source.SkipLast(3).TryCopyTo(dest, 0).ShouldBeFalse();
        dest.ShouldBe(new int[3]); // Destination should remain unchanged
    }

    [Fact]
    public void SkipLast_TryCopyTo_WithDestinationSlicing()
    {
        var source = new[] { 1, 2, 3, 4, 5, 6 }.AsValueEnumerable();
        var dest = new int[5];

        // Ensure destination slicing works correctly
        source.SkipLast(2).TryCopyTo(dest, 1).ShouldBeTrue();
        dest.ShouldBe(new[] { 2, 3, 4, 0, 0 });

        // Test with partial destination
        Array.Clear(dest);
        var slice = dest.AsSpan(1, 3);
        source.SkipLast(3).TryCopyTo(slice, 0).ShouldBeTrue();
        dest.ShouldBe(new[] { 0, 1, 2, 3, 0 });
    }

    [Fact]
    public void SkipLast_TryCopyTo_ExactElementCount()
    {
        var source = new[] { 1, 2, 3, 4, 5 }.AsValueEnumerable();
        var dest = new int[3];

        // Destination length exactly matches remaining elements
        source.SkipLast(2).TryCopyTo(dest, 0).ShouldBeTrue();
        dest.ShouldBe(new[] { 1, 2, 3 });
    }

    [Fact]
    public void SkipLast_TryCopyTo_ZeroElementsToCopy()
    {
        var source = new[] { 1, 2, 3, 4, 5 }.AsValueEnumerable();
        var dest = new int[0];

        source.SkipLast(2).TryCopyTo(dest, 2).ShouldBeFalse();
    }

    [Fact]
    public void SkipLast_TryCopyTo_ChainedOperations()
    {
        var source = Enumerable.Range(1, 10).ToArray().AsValueEnumerable();
        var dest = new int[5];

        // Chain multiple operations
        source.Skip(2).SkipLast(3).TryCopyTo(dest, 0).ShouldBeTrue();
        dest.ShouldBe(new[] { 3, 4, 5, 6, 7 });

        // With different offset
        Array.Clear(dest);
        source.Skip(2).SkipLast(3).TryCopyTo(dest, 2).ShouldBeTrue();
        dest.ShouldBe(new[] { 5, 6, 7, 0, 0 });
    }

    [Fact]
    public void SkipLast_TryCopyTo_NonEnumerableSourceCount()
    {
        // Create a custom enumerable that doesn't implement TryGetNonEnumeratedCount
        var nonCountableSource = new[] { 1, 2, 3, 4, 5 }
            .Select(x => x) // Forces enumeration
            .AsValueEnumerable();

        var dest = new int[3];

        // Should return false since the source can't provide count without enumeration
        nonCountableSource.SkipLast(2).TryCopyTo(dest, 0).ShouldBeFalse();
        dest.ShouldBe(new int[3]); // Destination should remain unchanged
    }

    [Fact]
    public void SkipLast_TryCopyTo_BoundaryConditions()
    {
        var source = new[] { 1, 2, 3, 4, 5 }.AsValueEnumerable();
        var dest = new int[3];

        // Skip everything but one element
        source.SkipLast(4).TryCopyTo(dest, 0).ShouldBeTrue();
        dest.ShouldBe(new[] { 1, 0, 0 });

        // Skip exactly the right number to leave one element with max offset
        Array.Clear(dest);
        source.SkipLast(4).TryCopyTo(dest, 0).ShouldBeTrue();
        dest.ShouldBe(new[] { 1, 0, 0 });
    }


    // Helper class to test disposal behavior
    private class DisposableTestEnumerator<T>(IEnumerable<T> source, Action onDispose) : IEnumerable<T>
    {
        public IEnumerator<T> GetEnumerator() => new DisposableEnumerator(source.GetEnumerator(), onDispose);
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

        private class DisposableEnumerator(IEnumerator<T> enumerator, Action onDispose) : IEnumerator<T>
        {
            public T Current => enumerator.Current;
            object System.Collections.IEnumerator.Current => Current!;
            public bool MoveNext() => enumerator.MoveNext();
            public void Reset() => enumerator.Reset();
            public void Dispose()
            {
                enumerator.Dispose();
                onDispose();
            }
        }
    }
}

#endif
