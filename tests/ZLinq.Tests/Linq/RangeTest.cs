namespace ZLinq.Tests.Linq;

public class RangeTest
{
    [Fact]
    public void Vectorize()
    {
        for (int i = 0; i < 100; i++)
        {
            for (int j = 0; j < 100; j++)
            {
                ValueEnumerable.Range(i, j).ToArray().ShouldBe(Enumerable.Range(i, j).ToArray());
            }
        }

        for (int i = 0; i < 100; i++)
        {
            for (int j = 0; j < 100; j++)
            {

                ValueEnumerable.Range(i, j).ToList().ShouldBe(Enumerable.Range(i, j).ToList());
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
                ValueEnumerable.Range(i, j).Select(x => x).ToArray().ShouldBe(Enumerable.Range(i, j).ToArray());
            }
        }
    }

    [Fact]
    public void EmptyCount()
    {
        ValueEnumerable.Range(-100, 0).ToArray().ShouldBe([]);
        ValueEnumerable.Range(-100, 0).Select(x => x).ToArray().ShouldBe([]);
    }

    [Fact]
    public void Validation()
    {
        TestUtil.Throws<ArgumentOutOfRangeException>(
            () => Enumerable.Range(0, -1),
            () => ValueEnumerable.Range(0, -1));

        TestUtil.Throws<ArgumentOutOfRangeException>(
            () => Enumerable.Range(0, -10),
            () => ValueEnumerable.Range(0, -10));

        TestUtil.Throws<ArgumentOutOfRangeException>(
            () => Enumerable.Range(100, int.MaxValue - 50),
            () => ValueEnumerable.Range(100, int.MaxValue - 50));

        var xs = new int[] { 1, 2, 3, 4, 5 };
        foreach (var item in xs.AsValueEnumerable())
        {
            Console.WriteLine(item);
        }
    }

    [Fact]
    public void ForEach()
    {
        var e = Enumerable.Range(1, 100).GetEnumerator();
        foreach (var item in ValueEnumerable.Range(1, 100))
        {
            e.MoveNext();
            item.ShouldBe(e.Current);
        }
    }
}
