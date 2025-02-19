using UnityEngine;
using ZLinq;
using System;

namespace ZLinq.Tests;

public class TestObjects
{
    [Fact]
    public void Foo()
    {
        var root = new GameObject("Root", [
            new ("Container",[
                new("C1"),
                new("C2"),
                new("Origin",[
                    new("Sphere_A"),
                    new("Sphere_B"),
                    new("Group", [
                        new("P1"),
                        new("Group"),
                        new("SphereB"),
                        new("P2"),
                    new("Sphere_A"),
                    new("Sphere_B"),
                    ])
                ]),
                new("C3"),
            new("C4")
            ])
        ]);


        var origin = root.transform.GetChild(0).GetChild(2);


        var a = new int[] { 1, 10, 100 }.AsStructEnumerable().Select(x => x * 2);
        //.ToArray2();
        // StructEnumerableExtensions.ToArray2(a);

        //var foo = root.Descendants()
        //    .ToArray<GameObject, DescendantsEnumerable<GameObject>, DescendantsEnumerable<GameObject>.Enumerator>();
    }
}

internal static class CustomExtensions
{
    // generate this overload via source generator
    internal static SelectStructEnumerable<T, ArrayStructEnumerable<T>, ArrayStructEnumerable<T>.Enumerator, TResult> Select<T, TResult>(this ArrayStructEnumerable<T> source, Func<T, TResult> selector)
    {
        return source.Select<T, ArrayStructEnumerable<T>, ArrayStructEnumerable<T>.Enumerator, TResult>(selector);
    }
}