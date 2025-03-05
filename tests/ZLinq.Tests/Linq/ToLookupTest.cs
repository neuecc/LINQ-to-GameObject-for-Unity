namespace ZLinq.Tests.Linq;

public class ToLookupTest
{
    [Fact]
    public void Empty()
    {
        var xs = new int[0];
        {
            var actual1 = xs.AsValueEnumerable().ToLookup(x => x);
            var actual2 = xs.ToIterableValueEnumerable().ToLookup(x => x);

            actual1.Count.ShouldBe(0);
            actual2.Count.ShouldBe(0);
        }
        {
            var actual1 = xs.AsValueEnumerable().ToLookup(x => x, x => x);
            var actual2 = xs.ToIterableValueEnumerable().ToLookup(x => x, x => x);

            actual1.Count.ShouldBe(0);
            actual2.Count.ShouldBe(0);
        }
    }

    [Fact]
    public void NonEmpty()
    {
        var xs = new int[] { 1, 2, 3, 4, 5 };
        {
            var actual1 = xs.AsValueEnumerable().ToLookup(x => x);
            var actual2 = xs.ToIterableValueEnumerable().ToLookup(x => x);
            
            actual1.Count.ShouldBe(xs.Length);
            actual2.Count.ShouldBe(xs.Length);
            
            foreach (var x in xs)
            {
                actual1[x].Single().ShouldBe(x);
                actual2[x].Single().ShouldBe(x);
            }
        }
        {
            var actual1 = xs.AsValueEnumerable().ToLookup(x => x, x => x * x);
            var actual2 = xs.ToIterableValueEnumerable().ToLookup(x => x, x => x * x);
            
            actual1.Count.ShouldBe(xs.Length);
            actual2.Count.ShouldBe(xs.Length);
            
            foreach (var x in xs)
            {
                actual1[x].Single().ShouldBe(x * x);
                actual2[x].Single().ShouldBe(x * x);
            }
        }
    }

    [Fact]
    public void DuplicateKeys()
    {
        var xs = new int[] { 1, 2, 2, 3, 3, 3, 4, 4, 4, 4 };
        {
            var actual1 = xs.AsValueEnumerable().ToLookup(x => x % 3);
            var actual2 = xs.ToIterableValueEnumerable().ToLookup(x => x % 3);
            
            actual1.Count.ShouldBe(3);
            actual2.Count.ShouldBe(3);
            
            actual1[0].ShouldBe(new[] { 3, 3, 3 });
            actual2[0].ShouldBe(new[] { 3, 3, 3 });
            
            actual1[1].ShouldBe(new[] { 1, 4, 4, 4, 4 });
            actual2[1].ShouldBe(new[] { 1, 4, 4, 4, 4 });
            
            actual1[2].ShouldBe(new[] { 2, 2 });
            actual2[2].ShouldBe(new[] { 2, 2 });
        }
        {
            var actual1 = xs.AsValueEnumerable().ToLookup(x => x % 3, x => x * 10);
            var actual2 = xs.ToIterableValueEnumerable().ToLookup(x => x % 3, x => x * 10);
            
            actual1.Count.ShouldBe(3);
            actual2.Count.ShouldBe(3);
            
            actual1[0].ShouldBe(new[] { 30, 30, 30 });
            actual2[0].ShouldBe(new[] { 30, 30, 30 });
            
            actual1[1].ShouldBe(new[] { 10, 40, 40, 40, 40 });
            actual2[1].ShouldBe(new[] { 10, 40, 40, 40, 40 });
            
            actual1[2].ShouldBe(new[] { 20, 20 });
            actual2[2].ShouldBe(new[] { 20, 20 });
        }
    }

    [Fact]
    public void Comparer()
    {
        var xs = new[] { "a", "A", "b", "B", "c", "C" };
        
        var lookup1 = xs.AsValueEnumerable().ToLookup(x => x, StringComparer.OrdinalIgnoreCase);
        var lookup2 = xs.AsValueEnumerable().ToLookup(x => x, x => x.ToUpper(), StringComparer.OrdinalIgnoreCase);
        
        lookup1.Count.ShouldBe(3);
        lookup2.Count.ShouldBe(3);
        
        lookup1["a"].ShouldBe(new[] { "a", "A" });
        lookup1["b"].ShouldBe(new[] { "b", "B" });
        lookup1["c"].ShouldBe(new[] { "c", "C" });
        
        lookup2["a"].ShouldBe(new[] { "A", "A" });
        lookup2["b"].ShouldBe(new[] { "B", "B" });
        lookup2["c"].ShouldBe(new[] { "C", "C" });
    }

    [Fact]
    public void NullKeys()
    {
        var xs = new string?[] { null, "a", null, "b", "c", null };
        
        var lookup1 = xs.AsValueEnumerable().ToLookup(x => x);
        var lookup2 = xs.ToIterableValueEnumerable().ToLookup(x => x);
        
        lookup1.Count.ShouldBe(4); // null, "a", "b", "c"
        lookup2.Count.ShouldBe(4);
        
        lookup1[null!].ShouldBe(new string?[] { null, null, null });
        lookup2[null!].ShouldBe(new string?[] { null, null, null });
        
        lookup1["a"].Single().ShouldBe("a");
        lookup2["a"].Single().ShouldBe("a");
    }

    [Fact]
    public void LookupContains()
    {
        var xs = new[] { 1, 2, 3, 4, 5, 1, 2 };
        var lookup = xs.AsValueEnumerable().ToLookup(x => x);
        
        lookup.Contains(1).ShouldBeTrue();
        lookup.Contains(3).ShouldBeTrue();
        lookup.Contains(6).ShouldBeFalse();
        lookup.Contains(0).ShouldBeFalse();
    }

    [Fact]
    public void LookupEnumeration()
    {
        var xs = new[] { 1, 2, 3, 4, 5, 1, 2 };
        var lookup = xs.AsValueEnumerable().ToLookup(x => x);
        
        var keys = new HashSet<int>();
        var count = 0;
        
        foreach (var group in lookup)
        {
            keys.Add(group.Key);
            count++;
        }
        
        count.ShouldBe(5);
        keys.SetEquals(new[] { 1, 2, 3, 4, 5 }).ShouldBeTrue();
    }

    [Fact]
    public void ElementSelectorTransformation()
    {
        var xs = new[] { "a", "bb", "ccc", "dddd" };
        
        var lookup = xs.AsValueEnumerable().ToLookup(x => x.Length, x => x.ToUpper());
        
        lookup.Count.ShouldBe(4);
        lookup[1].ShouldBe(new[] { "A" });
        lookup[2].ShouldBe(new[] { "BB" });
        lookup[3].ShouldBe(new[] { "CCC" });
        lookup[4].ShouldBe(new[] { "DDDD" });
    }
}
