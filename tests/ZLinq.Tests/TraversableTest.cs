//using UnityEngine;
//using ZLinq.Traversables;

//namespace ZLinq.Tests;

//public class TraversableTest
//{
//    [Fact]
//    public void BasicHierarchyTest()
//    {
//        var root = new GameObject("Root", [
//            new ("Container",[
//                new("C1"),
//                new("C2"),
//                new("Origin",[
//                    new("Sphere_A"),
//                    new("Sphere_B"),
//                    new("Group", [
//                        new("P1"),
//                        new("Group"),
//                        new("Sphere_B"),
//                        new("P2"),
//                    ]),
//                    new("Sphere_A"),
//                    new("Sphere_B"),
//                ]),
//                new("C3"),
//            new("C4")
//            ])
//        ]);


//        var origin = root.transform.GetChild(0).GetChild(2);

//        origin.Ancestors().AsEnumerable().Select(x => x.gameObject.name).ShouldBe(["Container", "Root"]);
//        origin.Children().AsEnumerable().Select(x => x.gameObject.name).ShouldBe(["Sphere_A", "Sphere_B", "Group", "Sphere_A", "Sphere_B"]);
//        origin.Descendants().AsEnumerable().Select(x => x.gameObject.name).ShouldBe(["Sphere_A", "Sphere_B", "Group", "P1", "Group", "Sphere_B", "P2", "Sphere_A", "Sphere_B"]);
//        origin.BeforeSelf().AsEnumerable().Select(x => x.gameObject.name).ShouldBe(["C1", "C2"]);
//        origin.AfterSelf().AsEnumerable().Select(x => x.gameObject.name).ShouldBe(["C3", "C4"]);


//    }
//}

//internal static class CustomExtensions
//{
//    public static IEnumerable<Transform> AsEnumerable(this ChildrenEnumerable<TransformTraversable, Transform> source) => source.AsEnumerable<ChildrenEnumerable<TransformTraversable, Transform>, Transform>();
//    public static IEnumerable<Transform> AsEnumerable(this DescendantsEnumerable<TransformTraversable, Transform> source) => source.AsEnumerable<DescendantsEnumerable<TransformTraversable, Transform>, Transform>();
//    public static IEnumerable<Transform> AsEnumerable(this AncestorsEnumerable<TransformTraversable, Transform> source) => source.AsEnumerable<AncestorsEnumerable<TransformTraversable, Transform>, Transform>();
//    public static IEnumerable<Transform> AsEnumerable(this BeforeSelfEnumerable<TransformTraversable, Transform> source) => source.AsEnumerable<BeforeSelfEnumerable<TransformTraversable, Transform>, Transform>();
//    public static IEnumerable<Transform> AsEnumerable(this AfterSelfEnumerable<TransformTraversable, Transform> source) => source.AsEnumerable<AfterSelfEnumerable<TransformTraversable, Transform>, Transform>();
//}

//public class NanikaComponent : Component
//{

//}