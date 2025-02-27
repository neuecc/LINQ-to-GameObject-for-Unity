namespace ZLinq.Tests.Linq;

public class Empty
{
    [Fact]
    public void Standard()
    {
        ValueEnumerable.Empty<int>().Select(x => x).ToArray().ShouldBe(Enumerable.Empty<int>().ToArray());
    }

    [Fact]
    public void ForEach()
    {
        var e = Enumerable.Empty<int>().GetEnumerator();
        foreach (var item in ValueEnumerable.Empty<int>())
        {
            e.MoveNext();
            item.ShouldBe(e.Current);
        }
    }
}
