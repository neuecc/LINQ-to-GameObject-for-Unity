#if !NET48

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZLinq.Tests.Linq;

public class TakeRangeTest(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void TakeRange()
    {
        var sequence = Enumerable.Range(1, 10).ToArray(); // 1,2,3,4,5,6,7,8,9,10

        // 1. Full inclusive range: start..end
        TestRange(sequence, 1..10, "1..10 (start..end)");

        // 2. Partial range from start: start..
        TestRange(sequence, 3.., "3.. (start..)");

        // 3. Partial range to end: ..end
        TestRange(sequence, ..7, "..7 (..end)");

        // 4. Full range omitting bounds: ..
        TestRange(sequence, .., ".. (..)");

        // === FromEnd Patterns ===

        // 5. Range using ^operator: start..^end
        TestRange(sequence, 2..^2, "2..^2 (start..^end)");

        // 6. Range using ^operator for both: ^start..^end
        TestRange(sequence, ^5..^1, "^5..^1 (^start..^end)");

        // 7. Partial FromEnd: ..^end
        TestRange(sequence, ..^3, "..^3 (..^end)");

        // 8. Partial FromEnd: ^start..
        TestRange(sequence, ^7.., "^7.. (^start..)");

        // === Special Cases ===

        // 9. End index equals collection length
        TestRange(sequence, 0..10, "0..10 (end equals length)");

        // 10. Start equals end (empty range)
        TestRange(sequence, 5..5, "5..5 (start equals end)");

        // 11. Start after end (invalid range)
        TestRange(sequence, 7..3, "7..3 (start > end)");

        // 12. Negative indices
        // Not applicable in C# Range

        // 13. Out of bounds indices
        TestRange(sequence, 0..11, "0..11 (end > length)");

        // === Edge Cases with FromEnd ===

        // 14. FromEnd larger than collection
        TestRange(sequence, ^12..^5, "^12..^5 (^start > length)");

        // 15. FromEnd equals 0
        TestRange(sequence, 5..^0, "5..^0 (^end = 0)");

        // 16. FromEnd indices resulting in invalid range
        TestRange(sequence, ^3..^7, "^3..^7 (FromEnd results in start > end)");
    }

    [Fact]
    public void TakeRange_EmptySource()
    {
        var empty = Array.Empty<int>();

        TestRange(empty, 0..5, "Empty source with range");
        TestRange(empty, ^5..^0, "Empty source with from-end range");
    }

    [Fact]
    public void TakeRange_TryGetNonEnumeratedCount()
    {
        var sequence = Enumerable.Range(1, 10).ToArray();
        var rangeOperation = sequence.AsValueEnumerable().Take(2..7);

        bool result = rangeOperation.TryGetNonEnumeratedCount(out int count);

        result.ShouldBeTrue();
        count.ShouldBe(5); // Should be end - start = 7 - 2 = 5
    }

    [Fact]
    public void TakeRange_TryGetSpan()
    {
        var sequence = Enumerable.Range(1, 10).ToArray();
        var rangeOperation = sequence.AsValueEnumerable().Take(2..7);

        bool result = rangeOperation.TryGetSpan(out var span);

        result.ShouldBeTrue();
        span.Length.ShouldBe(5);
        span.ToArray().ShouldBe(new[] { 3, 4, 5, 6, 7 });
    }

    [Fact]
    public void TakeRange_TryCopyTo_Basic()
    {
        var sequence = Enumerable.Range(1, 10).ToArray();
        var takeOperation = sequence.AsValueEnumerable().Take(3..8); // Take items 4,5,6,7,8

        Span<int> destination = new int[5];
        bool result = takeOperation.TryCopyTo(destination);

        result.ShouldBeTrue();
        destination.ToArray().ShouldBe(new[] { 4, 5, 6, 7, 8 });
    }

    [Fact]
    public void TakeRange_TryCopyTo_WithOffset()
    {
        var sequence = Enumerable.Range(1, 10).ToArray();
        var takeOperation = sequence.AsValueEnumerable().Take(3..8); // Take items 4,5,6,7,8

        Span<int> destination = new int[3];
        bool result = takeOperation.TryCopyTo(destination, 2); // Start from the third element (6)

        result.ShouldBeTrue();
        destination.ToArray().ShouldBe(new[] { 6, 7, 8 });
    }

    [Fact]
    public void TakeRange_TryCopyTo_FromEnd()
    {
        var sequence = Enumerable.Range(1, 10).ToArray();
        var takeOperation = sequence.AsValueEnumerable().Take(3..8); // Take items 4,5,6,7,8

        Span<int> destination = new int[3];
        bool result = takeOperation.TryCopyTo(destination, ^3); // Last 3 elements (6,7,8)

        result.ShouldBeTrue();
        destination.ToArray().ShouldBe(new[] { 6, 7, 8 });
    }

    [Fact]
    public void TakeRange_TryCopyTo_SmallDestination()
    {
        var sequence = Enumerable.Range(1, 10).ToArray();
        var takeOperation = sequence.AsValueEnumerable().Take(3..8); // Take items 4,5,6,7,8

        Span<int> smallDestination = new int[2];
        bool result = takeOperation.TryCopyTo(smallDestination);

        result.ShouldBeTrue();
        smallDestination.ToArray().ShouldBe(new[] { 4, 5 }); // Should only copy first 2 items
    }

    [Fact]
    public void TakeRange_TryCopyTo_EffectiveRemainsZero()
    {
        var sequence = Enumerable.Range(1, 10).ToArray();
        // Set skipIndex >= totalCount to make effectiveRemains zero
        var takeOperation = sequence.AsValueEnumerable().Take(10..15); // Beyond array bounds

        Span<int> destination = new int[5];
        destination.Fill(-1);
        bool result = takeOperation.TryCopyTo(destination);

        result.ShouldBeFalse(); // Should fail as effectiveRemains <= 0
        destination.ToArray().ShouldBe(new[] { -1, -1, -1, -1, -1 }); // Destination unchanged
    }

    [Fact]
    public void TakeRange_TryCopyTo_OutOfRangeOffset()
    {
        var sequence = Enumerable.Range(1, 10).ToArray();
        var takeOperation = sequence.AsValueEnumerable().Take(3..8); // Take items 4,5,6,7,8

        Span<int> destination = new int[5];
        destination.Fill(-1);

        // Try with offset beyond the range
        bool result = takeOperation.TryCopyTo(destination, 10);

        result.ShouldBeFalse(); // Should fail as offset is out of range
        destination.ToArray().ShouldBe(new[] { -1, -1, -1, -1, -1 }); // Destination unchanged
    }

    [Fact]
    public void TakeRange_TryCopyTo_EmptyRange()
    {
        var sequence = Enumerable.Range(1, 10).ToArray();
        var takeOperation = sequence.AsValueEnumerable().Take(5..5); // Empty range

        Span<int> destination = new int[5];
        destination.Fill(-1);
        bool result = takeOperation.TryCopyTo(destination);

        result.ShouldBeFalse(); // Should fail as the range is empty
        destination.ToArray().ShouldBe(new[] { -1, -1, -1, -1, -1 }); // Destination unchanged
    }

    [Fact]
    public void TakeRange_TryCopyTo_EndToEndOffsets()
    {
        var sequence = Enumerable.Range(1, 10).ToArray();

        // Test various range combinations
        var tests = new[]
        {
        (Range: 0..5, ExpectedResult: new[] { 1, 2, 3, 4, 5 }),
        (Range: 5..10, ExpectedResult: new[] { 6, 7, 8, 9, 10 }),
        (Range: 0..^0, ExpectedResult: new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }),
        (Range: ^5..^0, ExpectedResult: new[] { 6, 7, 8, 9, 10 }),
        (Range: 0..^5, ExpectedResult: new[] { 1, 2, 3, 4, 5 })
    };

        foreach (var test in tests)
        {
            Span<int> destination = new int[test.ExpectedResult.Length];
            var takeOperation = sequence.AsValueEnumerable().Take(test.Range);

            bool result = takeOperation.TryCopyTo(destination);

            result.ShouldBeTrue();
            destination.ToArray().ShouldBe(test.ExpectedResult);
        }
    }

    [Fact]
    public void TakeRange_TryCopyTo_NoNonEnumeratedCount()
    {
        // Create a source that doesn't support TryGetNonEnumeratedCount
        var nonCountableSource = new[] { 1, 2, 3, 4, 5 }
            .AsValueEnumerable()
            .Where(x => true); // Where operation doesn't support non-enumerated count

        var takeOperation = nonCountableSource.Take(1..4);

        Span<int> destination = new int[5];
        destination.Fill(-1);
        bool result = takeOperation.TryCopyTo(destination);

        result.ShouldBeFalse(); // Should fail as we need count information
        destination.ToArray().ShouldBe(new[] { -1, -1, -1, -1, -1 }); // Destination unchanged
    }

#if !NET48


    void TestRange(int[] sequence, Range range, string description)
    {
        try
        {
            var expected = sequence.Take(range).ToArray();
            var actual1 = sequence.AsValueEnumerable().Take(range).ToArray();
            var actual2 = sequence.ToValueEnumerable().Take(range).ToArray();

            actual1.ShouldBe(expected);
            actual2.ShouldBe(expected);
        }
        catch (Exception ex)
        {
            testOutputHelper.WriteLine($"{description}: Error - {ex.Message}");
            throw;
        }
    }

#endif
}

#endif
