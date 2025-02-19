using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using ZLinq.Unity;

namespace ZLinq // use root namespace(not ZLinq.Unity)
{
    // GameObject and Transform extensions
    public static class GameObjectExtensions
    {
        public static TransformTraversable AsTraversable(this Transform origin) => new(origin);



        public static ChildrenEnumerable<Transform, TransformTraversable, TransformTraverser> Children(this Transform origin) => origin.AsTraversable().Children();





        //public static ChildrenEnumerable<GameObject> Children(this GameObject origin) => new(origin.transform, withSelf: false);



        //public static ChildrenEnumerable<GameObject> ChildrenAndSelf(this GameObject origin) => new(origin.transform, withSelf: true);
        //public static ChildrenEnumerable<Transform> ChildrenAndSelf(this Transform origin) => new(origin, withSelf: true);

        //public static AncestorsEnumerable Ancestors(this GameObject origin)
        //            public static AncestorsEnumerable AncestorsAndSelf(this GameObject origin)


        // TODO: Func<Transform, bool> descendIntoChildren
        //public static DescendantsEnumerable<GameObject> Descendants(this GameObject origin) => new(origin.transform, withSelf: false);
        //public static DescendantsEnumerable<Transform> Descendants(this Transform origin) => new(origin, withSelf: false);


        //           public static DescendantsEnumerable Descendants(this GameObject origin, Func<Transform, bool> descendIntoChildren = null)
        //          public static DescendantsEnumerable DescendantsAndSelf(this GameObject origin, Func<Transform, bool> descendIntoChildren = null)

        // use GetSiblingIndex
        //public static BeforeSelfEnumerable BeforeSelf(this GameObject origin)
        //public static BeforeSelfEnumerable BeforeSelfAndSelf(this GameObject origin)
        //public static AfterSelfEnumerable AfterSelf(this GameObject origin)
        //public static AfterSelfEnumerable AfterSelfAndSelf(this GameObject origin)


        // -----


    }
}
