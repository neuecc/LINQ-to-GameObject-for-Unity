namespace ZLinq.Tests.Linq;

public class Repeat
{
    [Fact]
    public void Vectorize()
    {
        for (int i = 0; i < 100; i++)
        {
            for (int j = 0; j < 100; j++)
            {
                ValueEnumerable.Repeat(i, j).ToArray().ShouldBe(Enumerable.Repeat(i, j).ToArray());
            }
        }

        for (int i = 0; i < 100; i++)
        {
            for (int j = 0; j < 100; j++)
            {
                ValueEnumerable.Repeat(i, j).ToList().ShouldBe(Enumerable.Repeat(i, j).ToList());
            }
        }
    }

    [Fact]
    public void Standard()
    {
        for (int i = 0; i < 100; i++)
        {
            for (int j = 0; j < 100; j++)
            {
                ValueEnumerable.Repeat(i, j).Select(x => x).ToArray().ShouldBe(Enumerable.Repeat(i, j).ToArray());
            }
        }
    }

    [Fact]
    public void EmptyCount()
    {
        ValueEnumerable.Repeat(-100, 0).ToArray().ShouldBe(Array.Empty<int>());
        ValueEnumerable.Repeat(-100, 0).Select(x => x).ToArray().ShouldBe(Array.Empty<int>());
    }

    [Fact]
    public void Validation()
    {
        TestUtil.Throws<ArgumentOutOfRangeException>(
            () => Enumerable.Repeat(0, -1),
            () => ValueEnumerable.Repeat(0, -1));

        TestUtil.Throws<ArgumentOutOfRangeException>(
            () => Enumerable.Repeat(0, -10),
            () => ValueEnumerable.Repeat(0, -10));
    }

    [Fact]
    public void ForEach()
    {
        var e = Enumerable.Repeat(1, 100).GetEnumerator();
        foreach (var item in ValueEnumerable.Repeat(1, 100))
        {
            e.MoveNext();
            item.ShouldBe(e.Current);
        }
    }
}
