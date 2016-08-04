using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.Linq
{
    public struct ChildrenEnumerable : IEnumerable<GameObject>
    {
        readonly GameObject origin;
        readonly Transform originTransform;
        readonly string nameFilter;
        readonly bool withSelf;

        public ChildrenEnumerable(GameObject origin, string nameFilter, bool withSelf)
        {
            this.origin = origin;
            this.originTransform = origin.transform;
            this.nameFilter = nameFilter;
            this.withSelf = withSelf;
        }

        public Enumerator GetEnumerator()
        {
            // check GameObject is destroyed only on GetEnumerator timing
            return (origin == null)
                ? new Enumerator(origin, originTransform, nameFilter, withSelf, true)
                : new Enumerator(origin, originTransform, nameFilter, withSelf, false);
        }

        IEnumerator<GameObject> IEnumerable<GameObject>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public struct Enumerator : IEnumerator<GameObject>
        {
            readonly string nameFilter;
            readonly int childCount; // childCount is fixed when GetEnumerator is called.
            readonly GameObject origin;
            readonly Transform originTransform;
            bool withSelf;
            bool empty;
            int currentIndex;
            GameObject current;

            public Enumerator(GameObject origin, Transform originTransform, string nameFilter, bool withSelf, bool empty)
            {
                this.origin = origin;
                this.originTransform = originTransform;
                this.nameFilter = nameFilter;
                this.withSelf = withSelf;
                this.childCount = originTransform.childCount;
                this.currentIndex = -1;
                this.empty = empty;
                this.current = null;
            }

            public bool MoveNext()
            {
                if (empty) return false;

                if (withSelf && (nameFilter == null || originTransform.name == nameFilter))
                {
                    current = origin.gameObject;
                    withSelf = false;
                    return true;
                }

                currentIndex++;
                while (currentIndex < childCount)
                {
                    var child = originTransform.GetChild(currentIndex);

                    if (nameFilter == null || child.name == nameFilter)
                    {
                        current = child.gameObject;
                        return true;
                    }
                    else
                    {
                        currentIndex++;
                    }
                }

                return false;
            }

            public GameObject Current { get { return current; } }
            object IEnumerator.Current { get { return current; } }
            public void Dispose() { }
            public void Reset() { throw new NotSupportedException(); }
        }
    }



    // Inspired from LINQ to XML.
    // Reference: http://msdn.microsoft.com/en-us/library/system.xml.linq.xelement.aspx
    public static partial class GameObjectExtensions
    {
        // Traverse Game Objects, based on Axis(Parent, Child, Children, Ancestors/Descendants, BeforeSelf/ObjectsBeforeAfter)

        /// <summary>Gets the parent GameObject of this GameObject. If this GameObject has no parent, returns null.</summary>
        public static GameObject Parent(this GameObject origin)
        {
            if (origin == null) return null;

            var parentTransform = origin.transform.parent;
            if (parentTransform == null) return null;

            return parentTransform.gameObject;
        }

        /// <summary>Gets the first child GameObject with the specified name. If there is no GameObject with the speficided name, returns null.</summary>
        public static GameObject Child(this GameObject origin, string name)
        {
            if (origin == null) return null;

            var child = origin.transform.FindChild(name);
            if (child == null) return null;
            return child.gameObject;
        }

        /// <summary>Returns a collection of the child GameObjects.</summary>
        public static ChildrenEnumerable Children(this GameObject origin)
        {
            return new ChildrenEnumerable(origin, null, false);
        }

        /// <summary>Returns a filtered collection of the child GameObjects. Only GameObjects that have a matching name are included in the collection.</summary>
        public static ChildrenEnumerable Children(this GameObject origin, string name)
        {
            return new ChildrenEnumerable(origin, name, false);
        }

        /// <summary>Returns a collection of GameObjects that contain this GameObject, and the child GameObjects.</summary>
        public static ChildrenEnumerable ChildrenAndSelf(this GameObject origin)
        {
            return new ChildrenEnumerable(origin, null, true);
        }

        /// <summary>Returns a filtered collection of GameObjects that contain this GameObject, and the child GameObjects. Only GameObjects that have a matching name are included in the collection.</summary>
        public static ChildrenEnumerable ChildrenAndSelf(this GameObject origin, string name)
        {
            return new ChildrenEnumerable(origin, name, true);
        }

        /// <summary>Returns a collection of the ancestor GameObjects of this GameObject.</summary>
        public static IEnumerable<GameObject> Ancestors(this GameObject origin)
        {
            return AncestorsCore(origin, nameFilter: null, withSelf: false);
        }

        /// <summary>Returns a filtered collection of the ancestor GameObjects of this GameObject. Only GameObjects that have a matching name are included in the collection.</summary>
        public static IEnumerable<GameObject> Ancestors(this GameObject origin, string name)
        {
            return AncestorsCore(origin, nameFilter: null, withSelf: false);
        }

        /// <summary>Returns a collection of GameObjects that contain this element, and the ancestors of this GameObject.</summary>
        public static IEnumerable<GameObject> AncestorsAndSelf(this GameObject origin)
        {
            return AncestorsCore(origin, nameFilter: null, withSelf: true);
        }

        /// <summary>Returns a filtered collection of GameObjects that contain this element, and the ancestors of this GameObject. Only GameObjects that have a matching name are included in the collection.</summary>
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

        /// <summary>Returns a collection of the descendant GameObjects.</summary>
        public static IEnumerable<GameObject> Descendants(this GameObject origin)
        {
            return DescendantsCore(origin, nameFilter: null, withSelf: false);
        }

        /// <summary>Returns a filtered collection of the descendant GameObjects. Only GameObjects that have a matching name are included in the collection.</summary>
        public static IEnumerable<GameObject> Descendants(this GameObject origin, string name)
        {
            return DescendantsCore(origin, nameFilter: name, withSelf: false);
        }


        /// <summary>Returns a collection of GameObjects that contain this GameObject, and all descendant GameObjects of this GameObject.</summary>
        public static IEnumerable<GameObject> DescendantsAndSelf(this GameObject origin)
        {
            return DescendantsCore(origin, nameFilter: null, withSelf: true);
        }

        /// <summary>Returns a filtered collection of GameObjects that contain this GameObject, and all descendant GameObjects of this GameObject. Only GameObjects that have a matching name are included in the collection.</summary>       
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

            var originTransform = origin.transform;
            var childCount = originTransform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                var item = originTransform.GetChild(i);
                foreach (var child in DescendantsCore(item.gameObject, nameFilter, withSelf: true))
                {
                    if (nameFilter == null || child.name == nameFilter)
                    {
                        yield return child.gameObject;
                    }
                }
            }
        }

        /// <summary>Returns a collection of the sibling GameObjects before this GameObject.</summary>
        public static IEnumerable<GameObject> BeforeSelf(this GameObject origin)
        {
            return BeforeSelfCore(origin, nameFilter: null, withSelf: false);
        }

        /// <summary>Returns a filtered collection of the sibling GameObjects before this GameObject. Only GameObjects that have a matching name are included in the collection.</summary>       
        public static IEnumerable<GameObject> BeforeSelf(this GameObject origin, string name)
        {
            return BeforeSelfCore(origin, nameFilter: name, withSelf: false);
        }

        /// <summary>Returns a collection of GameObjects that contain this GameObject, and the sibling GameObjects before this GameObject.</summary>
        public static IEnumerable<GameObject> BeforeSelfAndSelf(this GameObject origin)
        {
            return BeforeSelfCore(origin, nameFilter: null, withSelf: true);
        }

        /// <summary>Returns a filtered collection of GameObjects that contain this GameObject, and the sibling GameObjects before this GameObject. Only GameObjects that have a matching name are included in the collection.</summary>       
        public static IEnumerable<GameObject> BeforeSelfAndSelf(this GameObject origin, string name)
        {
            return BeforeSelfCore(origin, nameFilter: name, withSelf: true);
        }

        static IEnumerable<GameObject> BeforeSelfCore(this GameObject origin, string nameFilter, bool withSelf)
        {
            if (origin == null) yield break;

            var parent = origin.transform.parent;
            if (parent == null) goto RETURN_SELF;

            var parentTransform = parent.transform;
            var childCount = parentTransform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                var item = parentTransform.GetChild(i);

                var go = item.gameObject;
                if (go == origin)
                {
                    goto RETURN_SELF;
                }

                if (nameFilter == null || item.name == nameFilter)
                {
                    yield return go;
                }
            }

            RETURN_SELF:
            if (withSelf && (nameFilter == null || origin.name == nameFilter))
            {
                yield return origin;
            }
        }

        /// <summary>Returns a collection of the sibling GameObjects after this GameObject.</summary>
        public static IEnumerable<GameObject> AfterSelf(this GameObject origin)
        {
            return AfterSelfCore(origin, nameFilter: null, withSelf: false);
        }

        /// <summary>Returns a filtered collection of the sibling GameObjects after this GameObject. Only GameObjects that have a matching name are included in the collection.</summary>       
        public static IEnumerable<GameObject> AfterSelf(this GameObject origin, string name)
        {
            return AfterSelfCore(origin, nameFilter: name, withSelf: false);
        }

        /// <summary>Returns a collection of GameObjects that contain this GameObject, and the sibling GameObjects after this GameObject.</summary>
        public static IEnumerable<GameObject> AfterSelfAndSelf(this GameObject origin)
        {
            return AfterSelfCore(origin, nameFilter: null, withSelf: true);
        }

        /// <summary>Returns a filtered collection of GameObjects that contain this GameObject, and the sibling GameObjects after this GameObject. Only GameObjects that have a matching name are included in the collection.</summary>       
        public static IEnumerable<GameObject> AfterSelfAndSelf(this GameObject origin, string name)
        {
            return AfterSelfCore(origin, nameFilter: name, withSelf: true);
        }

        static IEnumerable<GameObject> AfterSelfCore(this GameObject origin, string nameFilter, bool withSelf)
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