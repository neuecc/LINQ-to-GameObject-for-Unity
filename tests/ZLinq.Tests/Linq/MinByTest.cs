namespace ZLinq.Tests.Linq;

public class MinByTest
{
    [Fact]
    public void Empty()
    {
        var xs = new int[0];

        var actual = xs.AsValueEnumerable(); // TODO:Do
    }

    [Fact]
    public void NonEmpty()
    {
        var xs = new int[] { 1, 2, 3, 4, 5 };

        var actual = xs.AsValueEnumerable(); // TODO:Do
    }

    [Fact]
    public void Empty2()
    {
        var xs = new int[0];

        var actual = xs.AsValueEnumerable(); // TODO:Do
    }

    [Fact]
    public void NonEmpty2()
    {
        var xs = new int[] { 1, 2, 3, 4, 5 };

        var actual = xs.AsValueEnumerable(); // TODO:Do
    }

}
