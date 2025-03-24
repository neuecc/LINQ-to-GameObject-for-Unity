using System;
using System.Linq;

namespace ZLinq.Tests.Linq;

public class TakeTest
{
    [Fact]
    public void Take_Empty()
    {
        var empty = Array.Empty<int>();

        var expected = empty.Take(5).ToArray();
        var actual1 = empty.AsValueEnumerable().Take(5).ToArray();
        var actual2 = empty.ToValueEnumerable().Take(5).ToArray();

        actual1.ShouldBe(expected);
        actual2.ShouldBe(expected);
    }

    [Fact]
    public void Take_Zero()
    {
        var sequence = Enumerable.Range(1, 10).ToArray();

        var expected = sequence.Take(0).ToArray();
        var actual1 = sequence.AsValueEnumerable().Take(0).ToArray();
        var actual2 = sequence.ToValueEnumerable().Take(0).ToArray();

        actual1.ShouldBe(expected); // Should be empty
        actual2.ShouldBe(expected); // Should be empty
    }

    [Fact]
    public void Take_Negative()
    {
        var sequence = Enumerable.Range(1, 10).ToArray();

        var expected = sequence.Take(-5).ToArray();
        var actual1 = sequence.AsValueEnumerable().Take(-5).ToArray();
        var actual2 = sequence.ToValueEnumerable().Take(-5).ToArray();

        actual1.ShouldBe(expected); // Should be empty
        actual2.ShouldBe(expected); // Should be empty
    }

    [Fact]
    public void Take_PartialCollection()
    {
        var sequence = Enumerable.Range(1, 10).ToArray();

        var expected = sequence.Take(5).ToArray();
        var actual1 = sequence.AsValueEnumerable().Take(5).ToArray();
        var actual2 = sequence.ToValueEnumerable().Take(5).ToArray();

        actual1.ShouldBe(expected); // Should be [1,2,3,4,5]
        actual2.ShouldBe(expected); // Should be [1,2,3,4,5]
    }

    [Fact]
    public void Take_ExceedingSize()
    {
        var sequence = Enumerable.Range(1, 10).ToArray();

        var expected = sequence.Take(20).ToArray();
        var actual1 = sequence.AsValueEnumerable().Take(20).ToArray();
        var actual2 = sequence.ToValueEnumerable().Take(20).ToArray();

        actual1.ShouldBe(expected); // Should return all elements
        actual2.ShouldBe(expected); // Should return all elements
    }

    [Fact]
    public void Take_ExactSize()
    {
        var sequence = Enumerable.Range(1, 10).ToArray();

        var expected = sequence.Take(10).ToArray();
        var actual1 = sequence.AsValueEnumerable().Take(10).ToArray();
        var actual2 = sequence.ToValueEnumerable().Take(10).ToArray();

        actual1.ShouldBe(expected); // Should return all elements
        actual2.ShouldBe(expected); // Should return all elements
    }

    [Fact]
    public void Take_TryGetNonEnumeratedCount()
    {
        var sequence = Enumerable.Range(1, 10).ToArray();
        var takeOperation = sequence.AsValueEnumerable().Take(5);

        bool result = takeOperation.TryGetNonEnumeratedCount(out int count);

        result.ShouldBeTrue();
        count.ShouldBe(5);
    }

    [Fact]
    public void Take_TryGetNonEnumeratedCount_LargerThanCollection()
    {
        var sequence = Enumerable.Range(1, 10).ToArray();
        var takeOperation = sequence.AsValueEnumerable().Take(20);

        bool result = takeOperation.TryGetNonEnumeratedCount(out int count);

        result.ShouldBeTrue();
        count.ShouldBe(10); // Should return the smaller of takCount and collection size
    }

    [Fact]
    public void Take_TryGetSpan()
    {
        var sequence = Enumerable.Range(1, 10).ToArray();
        var takeOperation = sequence.AsValueEnumerable().Take(5);

        bool result = takeOperation.TryGetSpan(out var span);

        result.ShouldBeTrue();
        span.Length.ShouldBe(5);
        span.ToArray().ShouldBe(new[] { 1, 2, 3, 4, 5 });
    }

    [Fact]
    public void Take_TryCopyTo()
    {
        var sequence = Enumerable.Range(1, 10).ToArray();
        var takeOperation = sequence.AsValueEnumerable().Take(5);

        Span<int> destination = new int[5];
        bool result = takeOperation.TryCopyTo(destination);

        result.ShouldBeTrue();
        destination.ToArray().ShouldBe(new[] { 1, 2, 3, 4, 5 });
    }

    [Fact]
    public void TryCopyTo2()
    {
        var take = ValueEnumerable.Range(1, 5).Take(3);

        var dest = new int[5];

        Array.Clear(dest);
        take.TryCopyTo(dest, 0).ShouldBeTrue();
        dest.ShouldBe([1, 2, 3, 0, 0]);

        Array.Clear(dest);
        take.TryCopyTo(dest, 1).ShouldBeTrue();
        dest.ShouldBe([2, 3, 4, 0, 0]);

        Array.Clear(dest);
        take.TryCopyTo(dest, 2).ShouldBeTrue();
        dest.ShouldBe([3, 4, 5, 0, 0]);

        Array.Clear(dest);
        take.TryCopyTo(dest, 3).ShouldBeTrue();
        dest.ShouldBe([4, 5, 0, 0, 0]);

        Array.Clear(dest);
        take.TryCopyTo(dest, 4).ShouldBeTrue();
        dest.ShouldBe([5, 0, 0, 0, 0]);

        Array.Clear(dest);
        take.TryCopyTo(dest, 5).ShouldBeFalse();
    }
}
