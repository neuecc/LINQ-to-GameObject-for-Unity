using UnityEngine;
using ZLinq;
using System;
using System.Linq;
using ZLinq.Unity;
using System.Collections;
using System.Collections.Generic;

namespace ZLinq.Tests;

public class TraversableTest
{
    [Fact]
    public void BasicHierarchyTest()
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
                        new("Sphere_B"),
                        new("P2"),
                    ]),
                    new("Sphere_A"),
                    new("Sphere_B"),
                ]),
                new("C3"),
            new("C4")
            ])
        ]);


        var origin = root.transform.GetChild(0).GetChild(2);

        origin.Ancestors().AsEnumerable().Select(x => x.gameObject.name).ShouldBe(["Container", "Root"]);
        origin.Children().AsEnumerable().Select(x => x.gameObject.name).ShouldBe(["Sphere_A", "Sphere_B", "Group", "Sphere_A", "Sphere_B"]);
        origin.Descendants().AsEnumerable().Select(x => x.gameObject.name).ShouldBe(["Sphere_A", "Sphere_B", "Group", "P1", "Group", "Sphere_B", "P2", "Sphere_A", "Sphere_B"]);
        origin.BeforeSelf().AsEnumerable().Select(x => x.gameObject.name).ShouldBe(["C1", "C2"]);
        origin.AfterSelf().AsEnumerable().Select(x => x.gameObject.name).ShouldBe(["C3", "C4"]);
    }
}

internal static class CustomExtensions
{
    public static IEnumerable<Transform> AsEnumerable(this ChildrenEnumerable<Transform, TransformTraversable, TransformTraverser> source) => source.AsEnumerable<Transform, ChildrenEnumerable<Transform, TransformTraversable, TransformTraverser>, ChildrenEnumerable<Transform, TransformTraversable, TransformTraverser>.Enumerator>();
    public static IEnumerable<Transform> AsEnumerable(this DescendantsEnumerable<Transform, TransformTraversable, TransformTraverser> source) => source.AsEnumerable<Transform, DescendantsEnumerable<Transform, TransformTraversable, TransformTraverser>, DescendantsEnumerable<Transform, TransformTraversable, TransformTraverser>.Enumerator>();
    public static IEnumerable<Transform> AsEnumerable(this AncestorsEnumerable<Transform, TransformTraversable, TransformTraverser> source) => source.AsEnumerable<Transform, AncestorsEnumerable<Transform, TransformTraversable, TransformTraverser>, AncestorsEnumerable<Transform, TransformTraversable, TransformTraverser>.Enumerator>();
    public static IEnumerable<Transform> AsEnumerable(this BeforeSelfEnumerable<Transform, TransformTraversable, TransformTraverser> source) => source.AsEnumerable<Transform, BeforeSelfEnumerable<Transform, TransformTraversable, TransformTraverser>, BeforeSelfEnumerable<Transform, TransformTraversable, TransformTraverser>.Enumerator>();
    public static IEnumerable<Transform> AsEnumerable(this AfterSelfEnumerable<Transform, TransformTraversable, TransformTraverser> source) => source.AsEnumerable<Transform, AfterSelfEnumerable<Transform, TransformTraversable, TransformTraverser>, AfterSelfEnumerable<Transform, TransformTraversable, TransformTraverser>.Enumerator>();
}

public class NanikaComponent : Component
{

}