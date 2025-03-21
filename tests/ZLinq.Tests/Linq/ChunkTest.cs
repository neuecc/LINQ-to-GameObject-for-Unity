namespace ZLinq.Tests.Linq;

public class ChunkTest
{
    [Fact]
    public void Empty()
    {
        var xs = new int[0];

        var enumerable = xs.AsValueEnumerable().Chunk(3).Enumerator;

        var e1 = enumerable;
        e1.TryGetNonEnumeratedCount(out var nonEnumeratedCount).ShouldBe(true);
        nonEnumeratedCount.ShouldBe(0);
        TestUtil.Empty().AsValueEnumerable().TryGetNonEnumeratedCount(out var _).ShouldBeFalse();

        var e2 = enumerable;
        e2.TryGetSpan(out var span).ShouldBe(false);

        var e3 = enumerable;
        e3.TryGetNext(out var _).ShouldBeFalse();

        TestUtil.Empty().AsValueEnumerable().Enumerator.TryGetNext(out _).ShouldBeFalse();

        enumerable.Dispose();
    }

    [Fact]
    public void NonEmpty()
    {
        {
            // optimized
            var xs = new int[] { 1, 2, 3, 4, 5 };

            var enumerable = xs.AsValueEnumerable().Chunk(3);

            var e1 = enumerable;
            e1.TryGetNonEnumeratedCount(out var nonEnumeratedCount).ShouldBe(true);
            nonEnumeratedCount.ShouldBe(2);

            var e2 = enumerable;
            e2.TryGetSpan(out var span).ShouldBe(false);

            xs.AsValueEnumerable().Chunk(1).ToArray().ShouldBe([[1], [2], [3], [4], [5]]);
            xs.AsValueEnumerable().Chunk(2).ToArray().ShouldBe([[1, 2], [3, 4], [5]]);
            xs.AsValueEnumerable().Chunk(3).ToArray().ShouldBe([[1, 2, 3], [4, 5]]);
            xs.AsValueEnumerable().Chunk(4).ToArray().ShouldBe([[1, 2, 3, 4], [5]]);
            xs.AsValueEnumerable().Chunk(5).ToArray().ShouldBe([[1, 2, 3, 4, 5]]);
            xs.AsValueEnumerable().Chunk(6).ToArray().ShouldBe([[1, 2, 3, 4, 5]]);

            enumerable.Dispose();
        }
        {
            // non-optimized
            var xs = Enumerable.Range(1, 5);

            var enumerable = xs.AsValueEnumerable().Chunk(3);

            var e1 = enumerable;

#if NET8_0_OR_GREATER
            e1.TryGetNonEnumeratedCount(out var nonEnumeratedCount).ShouldBe(true);
            nonEnumeratedCount.ShouldBe(2);
#endif

            var e2 = enumerable;
            e2.TryGetSpan(out var span).ShouldBe(false);

            xs.AsValueEnumerable().Chunk(1).ToArray().ShouldBe([[1], [2], [3], [4], [5]]);
            xs.AsValueEnumerable().Chunk(2).ToArray().ShouldBe([[1, 2], [3, 4], [5]]);
            xs.AsValueEnumerable().Chunk(3).ToArray().ShouldBe([[1, 2, 3], [4, 5]]);
            xs.AsValueEnumerable().Chunk(4).ToArray().ShouldBe([[1, 2, 3, 4], [5]]);
            xs.AsValueEnumerable().Chunk(5).ToArray().ShouldBe([[1, 2, 3, 4, 5]]);
            xs.AsValueEnumerable().Chunk(6).ToArray().ShouldBe([[1, 2, 3, 4, 5]]);

            enumerable.Dispose();
        }
    }

    [Fact]
    public void TryGetNonEnumeratedCount()
    {
        var xs = new int[] { 1, 2, 3, 4, 5 }.AsValueEnumerable();

        xs.Chunk(1).TryGetNonEnumeratedCount(out var count1).ShouldBeTrue();
        count1.ShouldBe(5);

        xs.Chunk(2).TryGetNonEnumeratedCount(out var count2).ShouldBeTrue();
        count2.ShouldBe(3);

        xs.Chunk(3).TryGetNonEnumeratedCount(out var count3).ShouldBeTrue();
        count3.ShouldBe(2);

        xs.Chunk(4).TryGetNonEnumeratedCount(out var count4).ShouldBeTrue();
        count4.ShouldBe(2);

        xs.Chunk(5).TryGetNonEnumeratedCount(out var count5).ShouldBeTrue();
        count5.ShouldBe(1);

        xs.Chunk(6).TryGetNonEnumeratedCount(out var count6).ShouldBeTrue();
        count6.ShouldBe(1);

        xs.Chunk(7).TryGetNonEnumeratedCount(out var count7).ShouldBeTrue();
        count7.ShouldBe(1);
    }

    [Fact]
    public void ArgumentValidation()
    {
        var xs = new[] { 1, 2, 3, 4, 5 };

        TestUtil.Throws<ArgumentOutOfRangeException>(
            () => xs.Chunk(0),
            () => xs.AsValueEnumerable().Chunk(0));

        TestUtil.Throws<ArgumentOutOfRangeException>(
            () => xs.Chunk(-1),
            () => xs.AsValueEnumerable().Chunk(-1));

        TestUtil.NoThrow(() => xs.Chunk(1), () => xs.AsValueEnumerable().Chunk(1));
    }
}
