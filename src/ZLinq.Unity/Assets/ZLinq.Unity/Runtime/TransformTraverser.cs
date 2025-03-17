#nullable enable

using System.Runtime.InteropServices;
using UnityEngine;
using ZLinq.Traversables;

namespace ZLinq
{
    public static class TransformTraverserExtensions
    {
        public static TransformTraverser AsTraverser(this Transform origin) => new(origin);

        // type inference helper

        public static ValueEnumerable<Children<TransformTraverser, Transform>, Transform> Children(this TransformTraverser traverser) => traverser.Children<TransformTraverser, Transform>();
        public static ValueEnumerable<Children<TransformTraverser, Transform>, Transform> ChildrenAndSelf(this TransformTraverser traverser) => traverser.ChildrenAndSelf<TransformTraverser, Transform>();
        public static ValueEnumerable<Descendants<TransformTraverser, Transform>, Transform> Descendants(this TransformTraverser traverser) => traverser.Descendants<TransformTraverser, Transform>();
        public static ValueEnumerable<Descendants<TransformTraverser, Transform>, Transform> DescendantsAndSelf(this TransformTraverser traverser) => traverser.DescendantsAndSelf<TransformTraverser, Transform>();
        public static ValueEnumerable<Ancestors<TransformTraverser, Transform>, Transform> Ancestors(this TransformTraverser traverser) => traverser.Ancestors<TransformTraverser, Transform>();
        public static ValueEnumerable<Ancestors<TransformTraverser, Transform>, Transform> AncestorsAndSelf(this TransformTraverser traverser) => traverser.AncestorsAndSelf<TransformTraverser, Transform>();
        public static ValueEnumerable<BeforeSelf<TransformTraverser, Transform>, Transform> BeforeSelf(this TransformTraverser traverser) => traverser.BeforeSelf<TransformTraverser, Transform>();
        public static ValueEnumerable<BeforeSelf<TransformTraverser, Transform>, Transform> BeforeSelfAndSelf(this TransformTraverser traverser) => traverser.BeforeSelfAndSelf<TransformTraverser, Transform>();
        public static ValueEnumerable<AfterSelf<TransformTraverser, Transform>, Transform> AfterSelf(this TransformTraverser traverser) => traverser.AfterSelf<TransformTraverser, Transform>();
        public static ValueEnumerable<AfterSelf<TransformTraverser, Transform>, Transform> AfterSelfAndSelf(this TransformTraverser traverser) => traverser.AfterSelfAndSelf<TransformTraverser, Transform>();

        // direct shortcut

        public static ValueEnumerable<Children<TransformTraverser, Transform>, Transform> Children(this Transform origin) => origin.AsTraverser().Children();
        public static ValueEnumerable<Children<TransformTraverser, Transform>, Transform> ChildrenAndSelf(this Transform origin) => origin.AsTraverser().ChildrenAndSelf();
        public static ValueEnumerable<Descendants<TransformTraverser, Transform>, Transform> Descendants(this Transform origin) => origin.AsTraverser().Descendants();
        public static ValueEnumerable<Descendants<TransformTraverser, Transform>, Transform> DescendantsAndSelf(this Transform origin) => origin.AsTraverser().DescendantsAndSelf();
        public static ValueEnumerable<Ancestors<TransformTraverser, Transform>, Transform> Ancestors(this Transform origin) => origin.AsTraverser().Ancestors();
        public static ValueEnumerable<Ancestors<TransformTraverser, Transform>, Transform> AncestorsAndSelf(this Transform origin) => origin.AsTraverser().AncestorsAndSelf();
        public static ValueEnumerable<BeforeSelf<TransformTraverser, Transform>, Transform> BeforeSelf(this Transform origin) => origin.AsTraverser().BeforeSelf();
        public static ValueEnumerable<BeforeSelf<TransformTraverser, Transform>, Transform> BeforeSelfAndSelf(this Transform origin) => origin.AsTraverser().BeforeSelfAndSelf();
        public static ValueEnumerable<AfterSelf<TransformTraverser, Transform>, Transform> AfterSelf(this Transform origin) => origin.AsTraverser().AfterSelf();
        public static ValueEnumerable<AfterSelf<TransformTraverser, Transform>, Transform> AfterSelfAndSelf(this Transform origin) => origin.AsTraverser().AfterSelfAndSelf();

        // OfComponent

        public static ValueEnumerable<OfComponentT<Children<TransformTraverser, Transform>, TComponent>, TComponent> OfComponent<TComponent>(this ValueEnumerable<Children<TransformTraverser, Transform>, Transform> source)
            where TComponent : Component => new(new(source.Enumerator));

        public static ValueEnumerable<OfComponentT<Descendants<TransformTraverser, Transform>, TComponent>, TComponent> OfComponent<TComponent>(this ValueEnumerable<Descendants<TransformTraverser, Transform>, Transform> source)
            where TComponent : Component => new(new(source.Enumerator));

        public static ValueEnumerable<OfComponentT<Ancestors<TransformTraverser, Transform>, TComponent>, TComponent> OfComponent<TComponent>(this ValueEnumerable<Ancestors<TransformTraverser, Transform>, Transform> source)
            where TComponent : Component => new(new(source.Enumerator));

        public static ValueEnumerable<OfComponentT<BeforeSelf<TransformTraverser, Transform>, TComponent>, TComponent> OfComponent<TComponent>(this ValueEnumerable<BeforeSelf<TransformTraverser, Transform>, Transform> source)
            where TComponent : Component => new(new(source.Enumerator));

        public static ValueEnumerable<OfComponentT<AfterSelf<TransformTraverser, Transform>, TComponent>, TComponent> OfComponent<TComponent>(this ValueEnumerable<AfterSelf<TransformTraverser, Transform>, Transform> source)
            where TComponent : Component => new(new(source.Enumerator));
    }

    [StructLayout(LayoutKind.Auto)]
    public struct TransformTraverser : ITraverser<TransformTraverser, Transform>
    {
        static readonly object CalledTryGetNextChild = new object();
        static readonly object ParentNotFound = new object();

        readonly Transform transform;
        object? initializedState; // CalledTryGetNext or Parent(for sibling operations)
        int childCount; // self childCount(TryGetNextChild) or parent childCount(TryGetSibling)
        int index;

        public TransformTraverser(Transform origin)
        {
            this.transform = origin;
            this.initializedState = null;
            this.childCount = 0;
            this.index = 0;
        }

        public Transform Origin => transform;
        public TransformTraverser ConvertToTraverser(Transform next) => new(next);

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
