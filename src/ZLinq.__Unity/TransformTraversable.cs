using UnityEngine;

namespace ZLinq.Unity
{
    public static class TransformExtensions
    {
        public static TransformTraversable AsTraversable(this Transform origin) => new(origin);

        public static ChildrenEnumerable<Transform, TransformTraversable, TransformTraverser> Children(this Transform origin) => origin.AsTraversable().Children();
        public static ChildrenEnumerable<Transform, TransformTraversable, TransformTraverser> ChildrenAndSelf(this Transform origin) => origin.AsTraversable().ChildrenAndSelf();
        public static DescendantsEnumerable<Transform, TransformTraversable, TransformTraverser> Descendants(this Transform origin) => origin.AsTraversable().Descendants();
        public static DescendantsEnumerable<Transform, TransformTraversable, TransformTraverser> DescendantsAndSelf(this Transform origin) => origin.AsTraversable().DescendantsAndSelf();
        public static AncestorsEnumerable<Transform, TransformTraversable, TransformTraverser> Ancestors(this Transform origin) => origin.AsTraversable().Ancestors();
        public static AncestorsEnumerable<Transform, TransformTraversable, TransformTraverser> AncestorsAndSelf(this Transform origin) => origin.AsTraversable().AncestorsAndSelf();
        public static BeforeSelfEnumerable<Transform, TransformTraversable, TransformTraverser> BeforeSelf(this Transform origin) => origin.AsTraversable().BeforeSelf();
        public static BeforeSelfEnumerable<Transform, TransformTraversable, TransformTraverser> BeforeSelfAndSelf(this Transform origin) => origin.AsTraversable().BeforeSelfAndSelf();
        public static AfterSelfEnumerable<Transform, TransformTraversable, TransformTraverser> AfterSelf(this Transform origin) => origin.AsTraversable().AfterSelf();
        public static AfterSelfEnumerable<Transform, TransformTraversable, TransformTraverser> AfterSelfAndSelf(this Transform origin) => origin.AsTraversable().AfterSelfAndSelf();

        public static OfComponentTransformEnumerable<ChildrenEnumerable<Transform, TransformTraversable, TransformTraverser>, ChildrenEnumerable<Transform, TransformTraversable, TransformTraverser>.Enumerator, TComponent>
            OfComponent<TComponent>(this ChildrenEnumerable<Transform, TransformTraversable, TransformTraverser> source)
            where TComponent : Component
        {
            return new(source);
        }

        public static OfComponentTransformEnumerable<DescendantsEnumerable<Transform, TransformTraversable, TransformTraverser>, DescendantsEnumerable<Transform, TransformTraversable, TransformTraverser>.Enumerator, TComponent>
            OfComponent<TComponent>(this DescendantsEnumerable<Transform, TransformTraversable, TransformTraverser> source)
            where TComponent : Component
        {
            return new(source);
        }

        public static OfComponentTransformEnumerable<AncestorsEnumerable<Transform, TransformTraversable, TransformTraverser>, AncestorsEnumerable<Transform, TransformTraversable, TransformTraverser>.Enumerator, TComponent>
            OfComponent<TComponent>(this AncestorsEnumerable<Transform, TransformTraversable, TransformTraverser> source)
            where TComponent : Component
        {
            return new(source);
        }

        public static OfComponentTransformEnumerable<BeforeSelfEnumerable<Transform, TransformTraversable, TransformTraverser>, BeforeSelfEnumerable<Transform, TransformTraversable, TransformTraverser>.Enumerator, TComponent>
            OfComponent<TComponent>(this BeforeSelfEnumerable<Transform, TransformTraversable, TransformTraverser> source)
            where TComponent : Component
        {
            return new(source);
        }

        public static OfComponentTransformEnumerable<AfterSelfEnumerable<Transform, TransformTraversable, TransformTraverser>, AfterSelfEnumerable<Transform, TransformTraversable, TransformTraverser>.Enumerator, TComponent>
            OfComponent<TComponent>(this AfterSelfEnumerable<Transform, TransformTraversable, TransformTraverser> source)
            where TComponent : Component
        {
            return new(source);
        }
    }

    public readonly struct TransformTraversable : ITraversable<Transform, TransformTraversable, TransformTraverser>
    {
        readonly Transform transform;

        public TransformTraversable(Transform origin)
        {
            this.transform = origin;
        }

        public bool IsNull => transform is null; // don't use `==`.

        public Transform Origin => transform;
        public bool HasChild => transform.childCount != 0;

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

        public TransformTraverser GetTraverser()
        {
            return new(transform);
        }

        public TransformTraversable ConvertToTraversable(Transform next)
        {
            return new(next);
        }

        // Queries

        public ChildrenEnumerable<Transform, TransformTraversable, TransformTraverser> Children() => new(this, withSelf: false);
        public ChildrenEnumerable<Transform, TransformTraversable, TransformTraverser> ChildrenAndSelf() => new(this, withSelf: true);
        public DescendantsEnumerable<Transform, TransformTraversable, TransformTraverser> Descendants() => new(this, withSelf: false);
        public DescendantsEnumerable<Transform, TransformTraversable, TransformTraverser> DescendantsAndSelf() => new(this, withSelf: true);
        public AncestorsEnumerable<Transform, TransformTraversable, TransformTraverser> Ancestors() => new(this, withSelf: false);
        public AncestorsEnumerable<Transform, TransformTraversable, TransformTraverser> AncestorsAndSelf() => new(this, withSelf: true);
        public BeforeSelfEnumerable<Transform, TransformTraversable, TransformTraverser> BeforeSelf() => new(this, withSelf: false);
        public BeforeSelfEnumerable<Transform, TransformTraversable, TransformTraverser> BeforeSelfAndSelf() => new(this, withSelf: true);
        public AfterSelfEnumerable<Transform, TransformTraversable, TransformTraverser> AfterSelf() => new(this, withSelf: false);
        public AfterSelfEnumerable<Transform, TransformTraversable, TransformTraverser> AfterSelfAndSelf() => new(this, withSelf: true);
    }

    public struct TransformTraverser : ITraverser<Transform>
    {
        static readonly object CalledTryGetNextChild = new object();
        static readonly object ParentNotFound = new object();

        readonly Transform transform;
        object? initializedState; // CalledTryGetNext or Parent(for sibling operations)
        int childCount; // self childCount(TryGetNextChild) or parent childCount(TryGetSibling)
        int index;

        public bool IsNull => transform is null; // don't use `==`.

        public TransformTraverser(Transform transform)
        {
            this.transform = transform;
            this.initializedState = null;
            this.childCount = 0;
            this.index = 0;
        }

        public void Dispose()
        {
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
    }
}
