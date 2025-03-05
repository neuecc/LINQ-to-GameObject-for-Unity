// TODO: todo
//namespace ZLinq.Tests.Linq;

//public class ToDictionaryTest
//{
//    [Fact]
//    public void Empty()
//    {
//        var xs = new int[0];
//        var actual = xs.AsValueEnumerable().ToDictionary(x => x);
//        Assert.Empty(actual);
//    }

//    [Fact]
//    public void NonEmpty()
//    {
//        var xs = new int[] { 1, 2, 3, 4, 5 };
//        var actual = xs.AsValueEnumerable().ToDictionary(x => x);
//        Assert.Equal(xs.Length, actual.Count);
//        foreach (var x in xs)
//        {
//            Assert.Equal(x, actual[x]);
//        }
//    }
//}
