﻿// TODO
//namespace ZLinq.Tests.Linq;

//public class ToLookupTest
//{
//    [Fact]
//    public void Empty()
//    {
//        var xs = new int[0];
//        var actual = xs.AsValueEnumerable().ToLookup(x => x);
//        Assert.Empty(actual);
//    }

//    [Fact]
//    public void NonEmpty()
//    {
//        var xs = new int[] { 1, 2, 3, 4, 5 };
//        var actual = xs.AsValueEnumerable().ToLookup(x => x);
//        Assert.Equal(xs.Length, actual.Count);
//        foreach (var x in xs)
//        {
//            Assert.Equal(x, actual[x].Single());
//        }
//    }
//}
