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
}
