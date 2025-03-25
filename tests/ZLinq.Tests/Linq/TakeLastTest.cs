using System;
using System.Linq;

namespace ZLinq.Tests.Linq;

public class TakeLastTest
{
    [Fact]
    public void TakeLast_Empty()
    {
        var empty = Array.Empty<int>();

        var expected = empty.TakeLast(5).ToArray();
        var actual1 = empty.AsValueEnumerable().TakeLast(5).ToArray();
        var actual2 = empty.ToValueEnumerable().TakeLast(5).ToArray();

        actual1.ShouldBe(expected);
        actual2.ShouldBe(expected);
    }

    [Fact]
    public void TakeLast_Zero()
    {
        var sequence = Enumerable.Range(1, 10).ToArray();

        var expected = sequence.TakeLast(0).ToArray();
        var actual1 = sequence.AsValueEnumerable().TakeLast(0).ToArray();
        var actual2 = sequence.ToValueEnumerable().TakeLast(0).ToArray();

        actual1.ShouldBe(expected); // Should be empty
        actual2.ShouldBe(expected); // Should be empty
    }

    [Fact]
    public void TakeLast_Negative()
    {
        var sequence = Enumerable.Range(1, 10).ToArray();

        var expected = sequence.TakeLast(-5).ToArray();
        var actual1 = sequence.AsValueEnumerable().TakeLast(-5).ToArray();
        var actual2 = sequence.ToValueEnumerable().TakeLast(-5).ToArray();

        actual1.ShouldBe(expected); // Should be empty
        actual2.ShouldBe(expected); // Should be empty
    }

    [Fact]
    public void TakeLast_PartialCollection()
    {
        var sequence = Enumerable.Range(1, 10).ToArray();

        var expected = sequence.TakeLast(5).ToArray();
        var actual1 = sequence.AsValueEnumerable().TakeLast(5).ToArray();
        var actual2 = sequence.ToValueEnumerable().TakeLast(5).ToArray();

        actual1.ShouldBe(expected); // Should be [6,7,8,9,10]
        actual2.ShouldBe(expected); // Should be [6,7,8,9,10]
    }

    [Fact]
    public void TakeLast_ExceedingSize()
    {
        var sequence = Enumerable.Range(1, 10).ToArray();

        var expected = sequence.TakeLast(20).ToArray();
        var actual1 = sequence.AsValueEnumerable().TakeLast(20).ToArray();
        var actual2 = sequence.ToValueEnumerable().TakeLast(20).ToArray();

        actual1.ShouldBe(expected); // Should return all elements
        actual2.ShouldBe(expected); // Should return all elements
    }

    [Fact]
    public void TakeLast_ExactSize()
    {
        var sequence = Enumerable.Range(1, 10).ToArray();

        var expected = sequence.TakeLast(10).ToArray();
        var actual1 = sequence.AsValueEnumerable().TakeLast(10).ToArray();
        var actual2 = sequence.ToValueEnumerable().TakeLast(10).ToArray();

        actual1.ShouldBe(expected); // Should return all elements
        actual2.ShouldBe(expected); // Should return all elements
    }

    [Fact]
    public void TakeLast_TryGetNonEnumeratedCount()
    {
        var sequence = Enumerable.Range(1, 10).ToArray();
        var valueEnumerable = sequence.AsValueEnumerable().TakeLast(5);

        valueEnumerable.TryGetNonEnumeratedCount(out var count).ShouldBeTrue();
        count.ShouldBe(5);

        // Exceeding size
        var valueEnumerable2 = sequence.AsValueEnumerable().TakeLast(15);
        valueEnumerable2.TryGetNonEnumeratedCount(out var count2).ShouldBeTrue();
        count2.ShouldBe(10); // Limited to actual sequence count

        // Empty sequence
        var emptyEnumerable = Array.Empty<int>().AsValueEnumerable().TakeLast(5);
        emptyEnumerable.TryGetNonEnumeratedCount(out var count3).ShouldBeTrue();
        count3.ShouldBe(0);
    }

    [Fact]
    public void TakeLast_TryGetSpan()
    {
        // The implementation should properly slice the span to get the last elements
        var sequence = Enumerable.Range(1, 10).ToArray();
        var valueEnumerable = sequence.AsValueEnumerable().TakeLast(3);

        // Test that TryGetSpan works when the underlying source supports spans
        if (valueEnumerable.TryGetSpan(out var span))
        {
            span.Length.ShouldBe(3);
            span.ToArray().ShouldBe(new[] { 8, 9, 10 });
        }

        // This test might need adjustment based on the actual behavior of the underlying
        // implementation of AsValueEnumerable, which might not always support TryGetSpan
    }

    [Fact]
    public void TakeLast_TryCopyTo()
    {
        var sequence = Enumerable.Range(1, 10).ToArray();
        var valueEnumerable = sequence.AsValueEnumerable().TakeLast(3);

        var destination = new int[3];
        var result = valueEnumerable.TryCopyTo(destination);
        if (result)
        {
            destination.ShouldBe(new[] { 8, 9, 10 });
        }
    }

    [Fact]
    public void TakeLast_EnumerationImplementation()
    {
        var sequence = Enumerable.Range(1, 5).ToArray();
        var takeLast = sequence.ToValueEnumerable().TakeLast(3).Enumerator;

        // Test the implementation of TryGetNext which uses the Queue logic
        var result = new List<int>();
        while (takeLast.TryGetNext(out var current))
        {
            result.Add(current);
        }

        result.ShouldBe(new[] { 3, 4, 5 });
    }

    [Fact]
    public void TakeLast_LargeCollection()
    {
        var sequence = Enumerable.Range(1, 10000).ToArray();

        var expected = sequence.TakeLast(100).ToArray();
        var actual = sequence.AsValueEnumerable().TakeLast(100).ToArray();

        actual.ShouldBe(expected);
        actual.Length.ShouldBe(100);
        actual[0].ShouldBe(9901); // First element should be total - take + 1
    }

    [Fact]
    public void TakeLast_MultipleEnumeration()
    {
        var sequence = Enumerable.Range(1, 10).ToArray();
        var takeLast = sequence.AsValueEnumerable().TakeLast(3);

        // First enumeration
        var first = takeLast.ToArray();
        // Second enumeration
        var second = takeLast.ToArray();

        first.ShouldBe(second);
        first.ShouldBe(new[] { 8, 9, 10 });
    }

    [Fact]
    public void TakeLast_TryCopyTo_WithOffset()
    {
        // Arrange
        var sequence = Enumerable.Range(1, 10).ToArray();
        var valueEnumerable = sequence.AsValueEnumerable().TakeLast(5);

        // Act & Assert
        // Case 1: Start at beginning of the TakeLast result
        var destination1 = new int[3];
        var result1 = valueEnumerable.TryCopyTo(destination1, 0);
        result1.ShouldBeTrue();
        destination1.ShouldBe(new[] { 6, 7, 8 });

        // Case 2: Start at specific offset
        var destination2 = new int[3];
        var result2 = valueEnumerable.TryCopyTo(destination2, 2);
        result2.ShouldBeTrue();
        destination2.ShouldBe(new[] { 8, 9, 10 });

        // Case 3: Use Index.FromEnd
        var destination3 = new int[2];
        var result3 = valueEnumerable.TryCopyTo(destination3, ^2);
        result3.ShouldBeTrue();
        destination3.ShouldBe(new[] { 9, 10 });

        // Case 4: Destination too small for remaining elements
        var destination4 = new int[3];
        var result4 = valueEnumerable.TryCopyTo(destination4, 3);
        result4.ShouldBeTrue();
        destination4.ShouldBe(new[] { 9, 10, 0 }); // Last element is default since there are only 2 remaining elements

        // Case 5: Offset out of range (negative after conversion)
        var destination5 = new int[3];
        var result5 = valueEnumerable.TryCopyTo(destination5, ^10);
        result5.ShouldBeFalse();

        // Case 6: Offset equal to or greater than count
        var destination6 = new int[3];
        var result6 = valueEnumerable.TryCopyTo(destination6, 5);
        result6.ShouldBeFalse();

        // Case 7: Destination length zero, valid offset
        var destination7 = Array.Empty<int>();
        var result7 = valueEnumerable.TryCopyTo(destination7, 2);
        result7.ShouldBeFalse();

        // Case 8: Zero take count
        var emptyTakeLast = sequence.AsValueEnumerable().TakeLast(0);
        var destination8 = new int[3];
        var result8 = emptyTakeLast.TryCopyTo(destination8, 0);
        result8.ShouldBeFalse();
    }

    [Fact]
    public void Tes()
    {
        int[] source = [2];

        // LINQ
        source.TakeLast(1)
              .ElementAtOrDefault(3000)
              .ShouldBe(0);

        // ZLINQ
        source.AsValueEnumerable()
              .TakeLast(1)
              .ElementAtOrDefault(3000)
              .ShouldBe(0);
    }

    // Helper class to test disposal behavior
    private class DisposableTesTEnumerator<T>(IEnumerable<T> source, Action onDispose) : IEnumerable<T>
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
