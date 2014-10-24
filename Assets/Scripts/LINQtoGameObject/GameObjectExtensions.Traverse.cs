using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Unity.Linq
{
    // Inspired from LINQ to XML.
    // Reference: http://msdn.microsoft.com/en-us/library/system.xml.linq.xelement.aspx
    // (element -> GameObject / elements -> GameObjects / node -> hierarchy / XName -> name)
    public static partial class GameObjectExtensions
    {
        // Traverse Game Objects, based on Axis(Parent, Children, Ancestors/Descendants, ObjectsBeforeSelf/ObjectsBeforeAfter)

        /// <summary>Gets the parent GameObject of this GameObject. If this GameObject has no parent, returns null.</summary>
        public static GameObject Parent(this GameObject origin)
        {
            if (origin == null) return null;

            var parentTransform = origin.transform.parent;
            if (parentTransform == null) return null;

            return parentTransform.gameObject;
        }

        public static IEnumerable<GameObject> Children(this GameObject origin)
        {
            return ChildrenCore(origin, nameFilter: null, withSelf: false);
        }

        public static IEnumerable<GameObject> Children(this GameObject origin, string name)
        {
            return ChildrenCore(origin, nameFilter: name, withSelf: false);
        }

        public static IEnumerable<GameObject> ChildrenAndSelf(this GameObject origin)
        {
            return ChildrenCore(origin, nameFilter: null, withSelf: true);
        }

        public static IEnumerable<GameObject> ChildrenAndSelf(this GameObject origin, string name)
        {
            return ChildrenCore(origin, nameFilter: name, withSelf: true);
        }

        static IEnumerable<GameObject> ChildrenCore(this GameObject origin, string nameFilter, bool withSelf)
        {
            if (origin == null) yield break;
            if (withSelf && (nameFilter == null || origin.name == nameFilter))
            {
                yield return origin;
            }

            foreach (Transform child in origin.transform)
            {
                if (nameFilter == null || child.name == nameFilter)
                {
                    yield return child.gameObject;
                }
            }
        }

        /// <summary>Returns a collection of the ancestor GameObjects of this hierarchy.</summary>
        public static IEnumerable<GameObject> Ancestors(this GameObject origin)
        {
            return AncestorsCore(origin, nameFilter: null, withSelf: false);
        }

        /// <summary>Returns a filtered collection of the ancestor elements of this node. Only elements that have a matching XName are included in the collection.</summary>
        public static IEnumerable<GameObject> Ancestors(this GameObject origin, string name)
        {
            return AncestorsCore(origin, nameFilter: null, withSelf: false);
        }

        /// <summary>Returns a collection of elements that contain this element, and the ancestors of this element.</summary>
        public static IEnumerable<GameObject> AncestorsAndSelf(this GameObject origin)
        {
            return AncestorsCore(origin, nameFilter: null, withSelf: true);
        }

        /// <summary>Returns a filtered collection of elements that contain this element, and the ancestors of this element. Only elements that have a matching XName are included in the collection.</summary>
        public static IEnumerable<GameObject> AncestorsAndSelf(this GameObject origin, string name)
        {
            return AncestorsCore(origin, nameFilter: name, withSelf: true);
        }

        static IEnumerable<GameObject> AncestorsCore(GameObject origin, string nameFilter, bool withSelf)
        {
            if (origin == null) yield break;
            if (withSelf && (nameFilter == null || origin.name == nameFilter))
            {
                yield return origin;
            }

            var parentTransform = origin.transform.parent;
            while (parentTransform != null)
            {
                if (nameFilter == null || parentTransform.name == nameFilter)
                {
                    yield return parentTransform.gameObject;
                }
                parentTransform = parentTransform.parent;
            }
        }

        /// <summary>Returns a collection of the descendant elements for this document or element, in document order.</summary>
        public static IEnumerable<GameObject> Descendants(this GameObject origin)
        {
            return DescendantsCore(origin, nameFilter: null, withSelf: false);
        }

        /// <summary>Returns a collection of the descendant elements for this document or element, in document order. Only elements that have a matching XName are included in the collection.</summary>
        public static IEnumerable<GameObject> Descendants(this GameObject origin, string name)
        {
            return DescendantsCore(origin, nameFilter: name, withSelf: false);
        }

        /// <summary>Returns a collection of elements that contain this element, and all descendant elements of this element, in document order.</summary>
        public static IEnumerable<GameObject> DescendantsAndSelf(this GameObject origin)
        {
            return DescendantsCore(origin, nameFilter: null, withSelf: true);
        }

        /// <summary>Returns a filtered collection of elements that contain this element, and all descendant elements of this element, in document order. Only elements that have a matching XName are included in the collection.</summary>
        public static IEnumerable<GameObject> DescendantsAndSelf(this GameObject origin, string name)
        {
            return DescendantsCore(origin, nameFilter: name, withSelf: true);
        }

        static IEnumerable<GameObject> DescendantsCore(this GameObject origin, string nameFilter, bool withSelf)
        {
            if (origin == null) yield break;
            if (withSelf && (nameFilter == null || origin.name == nameFilter))
            {
                yield return origin;
            }

            foreach (Transform item in origin.transform)
            {
                foreach (var child in DescendantsCore(item.gameObject, nameFilter, withSelf: true))
                {
                    if (nameFilter == null || child.name == nameFilter)
                    {
                        yield return child.gameObject;
                    }
                }
            }
        }

        public static IEnumerable<GameObject> ObjectsBeforeSelf(this GameObject origin)
        {
            return ObjectsBeforeSelfCore(origin, nameFilter: null, withSelf: false);
        }

        public static IEnumerable<GameObject> ObjectsBeforeSelf(this GameObject origin, string name)
        {
            return ObjectsBeforeSelfCore(origin, nameFilter: name, withSelf: false);
        }

        public static IEnumerable<GameObject> ObjectsBeforeSelfAndSelf(this GameObject origin)
        {
            return ObjectsBeforeSelfCore(origin, nameFilter: null, withSelf: true);
        }

        public static IEnumerable<GameObject> ObjectsBeforeSelfAndSelf(this GameObject origin, string name)
        {
            return ObjectsBeforeSelfCore(origin, nameFilter: name, withSelf: true);
        }

        static IEnumerable<GameObject> ObjectsBeforeSelfCore(this GameObject origin, string nameFilter, bool withSelf)
        {
            if (origin == null) yield break;

            var parent = origin.transform.parent;
            if (parent == null) goto RETURN_SELF;

            foreach (Transform item in parent.transform)
            {
                if (item == origin) goto RETURN_SELF;

                if (nameFilter == null || item.name == nameFilter)
                {
                    yield return item.gameObject;
                }
            }

        RETURN_SELF:
            if (withSelf && (nameFilter == null || origin.name == nameFilter))
            {
                yield return origin;
            }
        }

        public static IEnumerable<GameObject> ObjectsAfterSelf(this GameObject origin)
        {
            return ObjectsAfterSelfCore(origin, nameFilter: null, withSelf: false);
        }

        public static IEnumerable<GameObject> ObjectsAfterSelf(this GameObject origin, string name)
        {
            return ObjectsAfterSelfCore(origin, nameFilter: name, withSelf: false);
        }

        public static IEnumerable<GameObject> ObjectsAfterSelfAndSelf(this GameObject origin)
        {
            return ObjectsAfterSelfCore(origin, nameFilter: null, withSelf: true);
        }

        public static IEnumerable<GameObject> ObjectsAfterSelfAndSelf(this GameObject origin, string name)
        {
            return ObjectsAfterSelfCore(origin, nameFilter: name, withSelf: true);
        }

        static IEnumerable<GameObject> ObjectsAfterSelfCore(this GameObject origin, string nameFilter, bool withSelf)
        {
            if (origin == null) yield break;
            if (withSelf && (nameFilter == null || origin.name == nameFilter))
            {
                yield return origin;
            }

            var parent = origin.transform.parent;
            if (parent == null) yield break;

            var index = origin.transform.GetSiblingIndex() + 1;
            var parentTransform = parent.transform;
            var count = parentTransform.childCount;

            while (index < count)
            {
                var target = parentTransform.GetChild(index).gameObject;
                if (nameFilter == null || target.name == nameFilter)
                {
                    yield return target;
                }

                index++;
            }
        }
    }
}