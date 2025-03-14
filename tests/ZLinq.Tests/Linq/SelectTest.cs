namespace ZLinq.Tests.Linq;

public class SelectTest
{
    [Fact]
    public void Test()
    {
        var xs = new int[] { 1, 2, 3, 4, 5, 6, 7, 8 };


        var foo = xs.AsValueEnumerable()
            .Select(x => x * 9999)
            .Where(x => x % 2 == 0)
            .Reverse()
            ;

        foreach (var item in foo)
        {
            
        }

    }

}
