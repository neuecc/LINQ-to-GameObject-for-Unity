using System.Runtime.InteropServices;
using UnityEngine;

namespace ZLinq
{
    public static class TransformExtensions
    {
        // TODO: Func<Transform, bool> descendIntoChildren

        public static TransformTraversable AsTraversable(this Transform origin) => new(origin);

        // type inference helper

        public static ChildrenEnumerable<TransformTraversable, Transform> Children(this TransformTraversable traversable) => traversable.Children<TransformTraversable, Transform>();
        public static ChildrenEnumerable<TransformTraversable, Transform> ChildrenAndSelf(this TransformTraversable traversable) => traversable.ChildrenAndSelf<TransformTraversable, Transform>();
        public static DescendantsEnumerable<TransformTraversable, Transform> Descendants(this TransformTraversable traversable) => traversable.Descendants<TransformTraversable, Transform>();
        public static DescendantsEnumerable<TransformTraversable, Transform> DescendantsAndSelf(this TransformTraversable traversable) => traversable.DescendantsAndSelf<TransformTraversable, Transform>();
        public static AncestorsEnumerable<TransformTraversable, Transform> Ancestors(this TransformTraversable traversable) => traversable.Ancestors<TransformTraversable, Transform>();
        public static AncestorsEnumerable<TransformTraversable, Transform> AncestorsAndSelf(this TransformTraversable traversable) => traversable.AncestorsAndSelf<TransformTraversable, Transform>();
        public static BeforeSelfEnumerable<TransformTraversable, Transform> BeforeSelf(this TransformTraversable traversable) => traversable.BeforeSelf<TransformTraversable, Transform>();
        public static BeforeSelfEnumerable<TransformTraversable, Transform> BeforeSelfAndSelf(this TransformTraversable traversable) => traversable.BeforeSelfAndSelf<TransformTraversable, Transform>();
        public static AfterSelfEnumerable<TransformTraversable, Transform> AfterSelf(this TransformTraversable traversable) => traversable.AfterSelf<TransformTraversable, Transform>();
        public static AfterSelfEnumerable<TransformTraversable, Transform> AfterSelfAndSelf(this TransformTraversable traversable) => traversable.AfterSelfAndSelf<TransformTraversable, Transform>();

        public static StructEnumerator<ChildrenEnumerable<TransformTraversable, Transform>, Transform> GetEnumerator(this ChildrenEnumerable<TransformTraversable, Transform> source) => new(source);
        public static StructEnumerator<DescendantsEnumerable<TransformTraversable, Transform>, Transform> GetEnumerator(this DescendantsEnumerable<TransformTraversable, Transform> source) => new(source);
        public static StructEnumerator<AncestorsEnumerable<TransformTraversable, Transform>, Transform> GetEnumerator(this AncestorsEnumerable<TransformTraversable, Transform> source) => new(source);
        public static StructEnumerator<BeforeSelfEnumerable<TransformTraversable, Transform>, Transform> GetEnumerator(this BeforeSelfEnumerable<TransformTraversable, Transform> source) => new(source);
        public static StructEnumerator<AfterSelfEnumerable<TransformTraversable, Transform>, Transform> GetEnumerator(this AfterSelfEnumerable<TransformTraversable, Transform> source) => new(source);

        // direct shortcut

        public static ChildrenEnumerable<TransformTraversable, Transform> Children(this Transform origin) => origin.AsTraversable().Children();
        public static ChildrenEnumerable<TransformTraversable, Transform> ChildrenAndSelf(this Transform origin) => origin.AsTraversable().ChildrenAndSelf();
        public static DescendantsEnumerable<TransformTraversable, Transform> Descendants(this Transform origin) => origin.AsTraversable().Descendants();
        public static DescendantsEnumerable<TransformTraversable, Transform> DescendantsAndSelf(this Transform origin) => origin.AsTraversable().DescendantsAndSelf();
        public static AncestorsEnumerable<TransformTraversable, Transform> Ancestors(this Transform origin) => origin.AsTraversable().Ancestors();
        public static AncestorsEnumerable<TransformTraversable, Transform> AncestorsAndSelf(this Transform origin) => origin.AsTraversable().AncestorsAndSelf();
        public static BeforeSelfEnumerable<TransformTraversable, Transform> BeforeSelf(this Transform origin) => origin.AsTraversable().BeforeSelf();
        public static BeforeSelfEnumerable<TransformTraversable, Transform> BeforeSelfAndSelf(this Transform origin) => origin.AsTraversable().BeforeSelfAndSelf();
        public static AfterSelfEnumerable<TransformTraversable, Transform> AfterSelf(this Transform origin) => origin.AsTraversable().AfterSelf();
        public static AfterSelfEnumerable<TransformTraversable, Transform> AfterSelfAndSelf(this Transform origin) => origin.AsTraversable().AfterSelfAndSelf();

        // OfComponent

        public static OfComponentTransformEnumerable<ChildrenEnumerable<TransformTraversable, Transform>, Component> OfComponent<TComponent>(this ChildrenEnumerable<TransformTraversable, Transform> source)
            where TComponent : Component => new(source);

        public static OfComponentTransformEnumerable<DescendantsEnumerable<TransformTraversable, Transform>, Component> OfComponent<TComponent>(this DescendantsEnumerable<TransformTraversable, Transform> source)
            where TComponent : Component => new(source);

        public static OfComponentTransformEnumerable<AncestorsEnumerable<TransformTraversable, Transform>, Component> OfComponent<TComponent>(this AncestorsEnumerable<TransformTraversable, Transform> source)
            where TComponent : Component => new(source);

        public static OfComponentTransformEnumerable<BeforeSelfEnumerable<TransformTraversable, Transform>, Component> OfComponent<TComponent>(this BeforeSelfEnumerable<TransformTraversable, Transform> source)
            where TComponent : Component => new(source);

        public static OfComponentTransformEnumerable<AfterSelfEnumerable<TransformTraversable, Transform>, Component> OfComponent<TComponent>(this AfterSelfEnumerable<TransformTraversable, Transform> source)
            where TComponent : Component => new(source);
    }

    [StructLayout(LayoutKind.Auto)]
    public struct TransformTraversable : ITraversable<TransformTraversable, Transform>
    {
        static readonly object CalledTryGetNextChild = new object();
        static readonly object ParentNotFound = new object();

        readonly Transform transform;
        object? initializedState; // CalledTryGetNext or Parent(for sibling operations)
        int childCount; // self childCount(TryGetNextChild) or parent childCount(TryGetSibling)
        int index;

        public TransformTraversable(Transform origin)
        {
            this.transform = origin;
            this.initializedState = null;
            this.childCount = 0;
            this.index = 0;
        }

        public Transform Origin => transform;
        public TransformTraversable ConvertToTraversable(Transform next) => new(next);

        public bool TryGetParent(out Transform parent)
        {
            var tp = transform.parent;
            if (tp != null)
            {
                parent = tp;
                return true;
            }

            parent = default!;
            return false;
        }

        public bool TryGetChildCount(out int count)
        {
            count = transform.childCount;
            return true;
        }

        public bool TryGetHasChild(out bool hasChild)
        {
            hasChild = transform.childCount != 0;
            return true;
        }

        public bool TryGetNextChild(out Transform child)
        {
            if (initializedState == null)
            {
                initializedState = CalledTryGetNextChild;
                childCount = transform.childCount;
            }

            if (index < childCount)
            {
                child = transform.GetChild(index++);
                return true;
            }

            child = default!;
            return false;
        }

        public bool TryGetNextSibling(out Transform next)
        {
            if (initializedState == null)
            {
                var tp = transform.parent;
                if (tp == null)
                {
                    initializedState = ParentNotFound;
                    next = default!;
                    return false;
                }

                // cache parent and childCount
                initializedState = tp;
                childCount = tp.childCount; // parent's childCount
                index = transform.GetSiblingIndex() + 1;
            }
            else if (initializedState == ParentNotFound)
            {
                next = default!;
                return false;
            }

            var parent = (Transform)initializedState;
            if (index < childCount)
            {
                next = parent.GetChild(index++);
                return true;
            }

            next = default!;
            return false;
        }

        public bool TryGetPreviousSibling(out Transform previous)
        {
            if (initializedState == null)
            {
                var tp = transform.parent;
                if (tp == null)
                {
                    initializedState = ParentNotFound;
                    previous = default!;
                    return false;
                }

                initializedState = tp;
                childCount = transform.GetSiblingIndex(); // not childCount but means `to`
                index = 0; // 0 to siblingIndex
            }
            else if (initializedState == ParentNotFound)
            {
                previous = default!;
                return false;
            }

            var parent = (Transform)initializedState;
            if (index < childCount)
            {
                previous = parent.GetChild(index++);
                return true;
            }

            previous = default!;
            return false;
        }

        public void Dispose()
        {
        }
    }
}
