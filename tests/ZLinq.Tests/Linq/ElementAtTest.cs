using System.Runtime.CompilerServices;
using Shouldly;

namespace ZLinq.Tests.Linq;

public class ElementAtTest
{
    [Fact]
    public void Empty()
    {
        var xs = new int[0];
        Should.Throw<ArgumentOutOfRangeException>(() => xs.AsValueEnumerable().ElementAt(0));
        Should.Throw<ArgumentOutOfRangeException>(() => xs.ToIterableValueEnumerable().ElementAt(0));
    }

    [Fact]
    public void NonEmpty()
    {
        var xs = new int[] { 1, 2, 3, 4, 5 };
        xs.AsValueEnumerable().ElementAt(2).ShouldBe(3);
        xs.ToIterableValueEnumerable().ElementAt(2).ShouldBe(3);
    }

    [Fact]
    public void IndexOutOfRange()
    {
        var xs = new int[] { 1, 2, 3, 4, 5 };
        Should.Throw<ArgumentOutOfRangeException>(() => xs.AsValueEnumerable().ElementAt(5));
        Should.Throw<ArgumentOutOfRangeException>(() => xs.ToIterableValueEnumerable().ElementAt(5));
    }

    [Fact]
    public void NegativeIndex()
    {
        var xs = new int[] { 1, 2, 3, 4, 5 };
        Should.Throw<ArgumentOutOfRangeException>(() => xs.AsValueEnumerable().ElementAt(-1));
        Should.Throw<ArgumentOutOfRangeException>(() => xs.ToIterableValueEnumerable().ElementAt(-1));
    }

    [Fact]
    public void FromEnd()
    {
        var xs = new int[] { 1, 2, 3, 4, 5 };
        xs.AsValueEnumerable().ElementAt(^2).ShouldBe(4);
        xs.ToIterableValueEnumerable().ElementAt(^1).ShouldBe(5);
        xs.ToIterableValueEnumerable().ElementAt(^2).ShouldBe(4);
        xs.ToIterableValueEnumerable().ElementAt(^3).ShouldBe(3);
        xs.ToIterableValueEnumerable().ElementAt(^4).ShouldBe(2);
        xs.ToIterableValueEnumerable().ElementAt(^5).ShouldBe(1);

        Should.Throw<ArgumentOutOfRangeException>(() => xs.ToIterableValueEnumerable().ElementAt(^6));

        // large q
        Enumerable.Range(0, 1000).AsValueEnumerable().ElementAt(^100).ShouldBe(900);
    }
}
