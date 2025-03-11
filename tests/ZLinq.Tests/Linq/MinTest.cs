namespace ZLinq.Tests.Linq;

public class MinTest
{
    [Fact]
    public void Random()
    {
        var rand = new Random();
        // int
        for (int i = 1; i < 100; i++)
        {
            var data = new int[i].Select(_ => rand.Next()).ToArray();
            var expected = data.Min();

            var min1 = data.AsValueEnumerable().Min();
            var min2 = data.ToIterableValueEnumerable().Min();

            min1.ShouldBe(expected);
            min2.ShouldBe(expected);
        }
        // string
        for (int i = 0; i < 100; i++) // from zero
        {
            var data = new int[i].Select(_ => rand.Next().ToString()).ToArray();
            var expected = data.Min();

            var min1 = data.AsValueEnumerable().Min();
            var min2 = data.ToIterableValueEnumerable().Min();

            min1.ShouldBe(expected);
            min2.ShouldBe(expected);
        }
    }

}
