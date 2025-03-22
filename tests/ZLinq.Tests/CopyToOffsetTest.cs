namespace ZLinq.Tests;

public class CopyToOffsetTest
{
    [Fact]
    public void Offset()
    {
        var fromArray = new[] { 1, 2, 3, 4, 5 }.AsValueEnumerable().Enumerator;

        // same size of destination
        var dest = new int[5];
        fromArray.TryCopyTo(dest, 0).ShouldBeTrue();
        dest.ShouldBe([1, 2, 3, 4, 5]);

        Array.Clear(dest);
        fromArray.TryCopyTo(dest, 1).ShouldBeTrue();
        dest.ShouldBe([2, 3, 4, 5, 0]);

        Array.Clear(dest);
        fromArray.TryCopyTo(dest, 2).ShouldBeTrue();
        dest.ShouldBe([3, 4, 5, 0, 0]);

        Array.Clear(dest);
        fromArray.TryCopyTo(dest, 3).ShouldBeTrue();
        dest.ShouldBe([4, 5, 0, 0, 0]);

        Array.Clear(dest);
        fromArray.TryCopyTo(dest, 4).ShouldBeTrue();
        dest.ShouldBe([5, 0, 0, 0, 0]);

        // offset is full
        Array.Clear(dest);
        fromArray.TryCopyTo(dest, 5).ShouldBeTrue();
        dest.ShouldBe([0, 0, 0, 0, 0]);

        // offset is larger than destination
        fromArray.TryCopyTo(dest, 6).ShouldBeFalse();

        // offset is negative(Index can't be negative)
        // fromArray.TryCopyTo(dest, -1).ShouldBeFalse();

        // destination is larger than source
        dest = Enumerable.Repeat(99, 10).ToArray();
        fromArray.TryCopyTo(dest, 0).ShouldBeTrue();
        dest.ToArray().ShouldBe([1, 2, 3, 4, 5, 99, 99, 99, 99, 99]);

        // destination is smaller than source
        dest = new int[3];
        fromArray.TryCopyTo(dest, 0).ShouldBeTrue();
        dest.ShouldBe([1, 2, 3]);

        // from last
        Array.Clear(dest);
        fromArray.TryCopyTo(dest, ^5).ShouldBeTrue();
        dest.ShouldBe([1, 2, 3]);

        Array.Clear(dest);
        fromArray.TryCopyTo(dest, ^4).ShouldBeTrue();
        dest.ShouldBe([2, 3, 4]);

        Array.Clear(dest);
        fromArray.TryCopyTo(dest, ^3).ShouldBeTrue();
        dest.ShouldBe([3, 4, 5]);

        Array.Clear(dest);
        fromArray.TryCopyTo(dest, ^2).ShouldBeTrue();
        dest.ShouldBe([4, 5, 0]);
        Array.Clear(dest);

        Array.Clear(dest);
        fromArray.TryCopyTo(dest, ^1).ShouldBeTrue();
        dest.ShouldBe([5, 0, 0]);

        Array.Clear(dest);
        fromArray.TryCopyTo(dest, ^0).ShouldBeTrue();
        dest.ShouldBe([0, 0, 0]);

        // from last offset is negative
        fromArray.TryCopyTo(dest, ^6).ShouldBeFalse();
    }
}
