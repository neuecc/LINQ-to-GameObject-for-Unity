namespace ZLinq.Tests.Linq;

public class ToDictionaryTest
{
    // pattern
    // Span = Array
    // EnumeatedCount Only = Select
    // Iterator

    [Fact]
    public void Empty()
    {
        var xs = new int[0];
        {
            var actual1 = xs.AsValueEnumerable().ToDictionary(x => x);
            var actual2 = xs.AsValueEnumerable().Select(x => x).ToDictionary(x => x);
            var actual3 = xs.ToValueEnumerable().ToDictionary(x => x);

            actual1.Count.ShouldBe(0);
            actual2.Count.ShouldBe(0);
            actual3.Count.ShouldBe(0);
        }
        {
            var actual1 = xs.AsValueEnumerable().ToDictionary(x => x, x => x);
            var actual2 = xs.AsValueEnumerable().Select(x => x).ToDictionary(x => x, x => x);
            var actual3 = xs.ToValueEnumerable().ToDictionary(x => x, x => x);

            actual1.Count.ShouldBe(0);
            actual2.Count.ShouldBe(0);
            actual3.Count.ShouldBe(0);
        }
    }

    [Fact]
    public void NonEmpty()
    {
        var xs = new int[] { 1, 2, 3, 4, 5 };
        {
            var actual1 = xs.AsValueEnumerable().ToDictionary(x => x);
            var actual2 = xs.AsValueEnumerable().Select(x => x).ToDictionary(x => x);
            var actual3 = xs.ToValueEnumerable().ToDictionary(x => x);
            Assert.Equal(xs.Length, actual1.Count);
            Assert.Equal(xs.Length, actual2.Count);
            Assert.Equal(xs.Length, actual3.Count);
            foreach (var x in xs)
            {
                Assert.Equal(x, actual1[x]);
                Assert.Equal(x, actual2[x]);
                Assert.Equal(x, actual3[x]);
            }
        }
        {
            var actual1 = xs.AsValueEnumerable().ToDictionary(x => x, x => x * x);
            var actual2 = xs.AsValueEnumerable().Select(x => x).ToDictionary(x => x, x => x * x);
            var actual3 = xs.ToValueEnumerable().ToDictionary(x => x, x => x * x);
            Assert.Equal(xs.Length, actual1.Count);
            Assert.Equal(xs.Length, actual2.Count);
            Assert.Equal(xs.Length, actual3.Count);
            foreach (var x in xs)
            {
                Assert.Equal(x * x, actual1[x]);
                Assert.Equal(x * x, actual2[x]);
                Assert.Equal(x * x, actual3[x]);
            }
        }
    }

    [Fact]
    public void Comparer()
    {
        (string, int)[] xs = [("foo", 100), ("bar", 200), ("baz", 300)];

        var dict = xs.AsValueEnumerable().ToDictionary(x => x.Item1, x => x.Item2, StringComparer.OrdinalIgnoreCase);

        dict["foO"].ShouldBe(100);
        dict["BaR"].ShouldBe(200);
        dict["Baz"].ShouldBe(300);
    }
}
