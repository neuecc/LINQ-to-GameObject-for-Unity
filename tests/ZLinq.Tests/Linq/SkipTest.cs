using System;
using System.Linq;

namespace ZLinq.Tests.Linq;

public class SkipTest
{
    [Fact]
    public void Skip_Empty()
    {
        var empty = Array.Empty<int>();

        var expected = empty.Skip(5).ToArray();
        var actual1 = empty.AsValueEnumerable().Skip(5).ToArray();
        var actual2 = empty.ToValueEnumerable().Skip(5).ToArray();

        actual1.ShouldBe(expected);
        actual2.ShouldBe(expected);
    }

    [Fact]
    public void Skip_Zero()
    {
        var sequence = Enumerable.Range(1, 10).ToArray();

        var expected = sequence.Skip(0).ToArray();
        var actual1 = sequence.AsValueEnumerable().Skip(0).ToArray();
        var actual2 = sequence.ToValueEnumerable().Skip(0).ToArray();

        actual1.ShouldBe(expected); // Should return all elements
        actual2.ShouldBe(expected); // Should return all elements
    }

    [Fact]
    public void Skip_Negative()
    {
        var sequence = Enumerable.Range(1, 10).ToArray();

        var expected = sequence.Skip(-5).ToArray();
        var actual1 = sequence.AsValueEnumerable().Skip(-5).ToArray();
        var actual2 = sequence.ToValueEnumerable().Skip(-5).ToArray();

        actual1.ShouldBe(expected); // Should return all elements
        actual2.ShouldBe(expected); // Should return all elements
    }

    [Fact]
    public void Skip_PartialCollection()
    {
        var sequence = Enumerable.Range(1, 10).ToArray();

        var expected = sequence.Skip(5).ToArray();
        var actual1 = sequence.AsValueEnumerable().Skip(5).ToArray();
        var actual2 = sequence.ToValueEnumerable().Skip(5).ToArray();

        actual1.ShouldBe(expected); // Should be [6,7,8,9,10]
        actual2.ShouldBe(expected); // Should be [6,7,8,9,10]
    }

    [Fact]
    public void Skip_ExceedingSize()
    {
        var sequence = Enumerable.Range(1, 10).ToArray();

        var expected = sequence.Skip(20).ToArray();
        var actual1 = sequence.AsValueEnumerable().Skip(20).ToArray();
        var actual2 = sequence.ToValueEnumerable().Skip(20).ToArray();

        actual1.ShouldBe(expected); // Should return empty array
        actual2.ShouldBe(expected); // Should return empty array
    }

    [Fact]
    public void Skip_ExactSize()
    {
        var sequence = Enumerable.Range(1, 10).ToArray();

        var expected = sequence.Skip(10).ToArray();
        var actual1 = sequence.AsValueEnumerable().Skip(10).ToArray();
        var actual2 = sequence.ToValueEnumerable().Skip(10).ToArray();

        actual1.ShouldBe(expected); // Should return empty array
        actual2.ShouldBe(expected); // Should return empty array
    }

    [Fact]
    public void Skip_TryGetNonEnumeratedCount()
    {
        var sequence = Enumerable.Range(1, 10).ToArray();
        var skipOperation = sequence.AsValueEnumerable().Skip(3);

        bool result = skipOperation.TryGetNonEnumeratedCount(out int count);

        result.ShouldBeTrue();
        count.ShouldBe(7); // 10 - 3 = 7 elements remaining
    }

    [Fact]
    public void Skip_TryGetNonEnumeratedCount_LargerThanCollection()
    {
        var sequence = Enumerable.Range(1, 10).ToArray();
        var skipOperation = sequence.AsValueEnumerable().Skip(20);

        bool result = skipOperation.TryGetNonEnumeratedCount(out int count);

        result.ShouldBeTrue();
        count.ShouldBe(0); // Skip count > collection size, should be 0
    }

    [Fact]
    public void Skip_TryGetSpan()
    {
        var sequence = Enumerable.Range(1, 10).ToArray();
        var skipOperation = sequence.AsValueEnumerable().Skip(5);

        bool result = skipOperation.TryGetSpan(out var span);

        result.ShouldBeTrue();
        span.Length.ShouldBe(5);
        span.ToArray().ShouldBe(new[] { 6, 7, 8, 9, 10 });
    }

    [Fact]
    public void Skip_TryGetSpan_SkipAll()
    {
        var sequence = Enumerable.Range(1, 10).ToArray();
        var skipOperation = sequence.AsValueEnumerable().Skip(10);

        bool result = skipOperation.TryGetSpan(out var span);

        result.ShouldBeTrue();
        span.Length.ShouldBe(0); // Should have no elements
    }

    [Fact]
    public void Skip_TryGetNext()
    {
        var sequence = Enumerable.Range(1, 5).ToArray();
        var skipOperation = sequence.AsValueEnumerable().Skip(2);

        // Manual enumeration to test TryGetNext
        using var enumerator = skipOperation.Enumerator;

        // Should get first element after skipping (element at index 2)
        enumerator.TryGetNext(out var first).ShouldBeTrue();
        first.ShouldBe(3);

        // Should get second element after skipping (element at index 3)
        enumerator.TryGetNext(out var second).ShouldBeTrue();
        second.ShouldBe(4);

        // Should get third element after skipping (element at index 4)
        enumerator.TryGetNext(out var third).ShouldBeTrue();
        third.ShouldBe(5);

        // Should return false once end is reached
        enumerator.TryGetNext(out _).ShouldBeFalse();
    }

    [Fact]
    public void Skip_TryGetNext_SkipAll()
    {
        var sequence = Enumerable.Range(1, 5).ToArray();
        var skipOperation = sequence.AsValueEnumerable().Skip(5); // Skip all elements

        using var enumerator = skipOperation.Enumerator;

        // Should return false since all elements were skipped
        enumerator.TryGetNext(out _).ShouldBeFalse();
    }
}
