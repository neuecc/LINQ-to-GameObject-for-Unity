#nullable enable

using System.Runtime.InteropServices;
using UnityEngine;
using ZLinq.Traversables;

namespace ZLinq
{
    public static class GameObjectTraverserExtensions
    {
        public static GameObjectTraverser AsTraverser(this GameObject origin) => new(origin);

        // type inference helper

        public static ValueEnumerable<Children<GameObjectTraverser, GameObject>, GameObject> Children(this GameObjectTraverser traverser) => traverser.Children<GameObjectTraverser, GameObject>();
        public static ValueEnumerable<Children<GameObjectTraverser, GameObject>, GameObject> ChildrenAndSelf(this GameObjectTraverser traverser) => traverser.ChildrenAndSelf<GameObjectTraverser, GameObject>();
        public static ValueEnumerable<Descendants<GameObjectTraverser, GameObject>, GameObject> Descendants(this GameObjectTraverser traverser) => traverser.Descendants<GameObjectTraverser, GameObject>();
        public static ValueEnumerable<Descendants<GameObjectTraverser, GameObject>, GameObject> DescendantsAndSelf(this GameObjectTraverser traverser) => traverser.DescendantsAndSelf<GameObjectTraverser, GameObject>();
        public static ValueEnumerable<Ancestors<GameObjectTraverser, GameObject>, GameObject> Ancestors(this GameObjectTraverser traverser) => traverser.Ancestors<GameObjectTraverser, GameObject>();
        public static ValueEnumerable<Ancestors<GameObjectTraverser, GameObject>, GameObject> AncestorsAndSelf(this GameObjectTraverser traverser) => traverser.AncestorsAndSelf<GameObjectTraverser, GameObject>();
        public static ValueEnumerable<BeforeSelf<GameObjectTraverser, GameObject>, GameObject> BeforeSelf(this GameObjectTraverser traverser) => traverser.BeforeSelf<GameObjectTraverser, GameObject>();
        public static ValueEnumerable<BeforeSelf<GameObjectTraverser, GameObject>, GameObject> BeforeSelfAndSelf(this GameObjectTraverser traverser) => traverser.BeforeSelfAndSelf<GameObjectTraverser, GameObject>();
        public static ValueEnumerable<AfterSelf<GameObjectTraverser, GameObject>, GameObject> AfterSelf(this GameObjectTraverser traverser) => traverser.AfterSelf<GameObjectTraverser, GameObject>();
        public static ValueEnumerable<AfterSelf<GameObjectTraverser, GameObject>, GameObject> AfterSelfAndSelf(this GameObjectTraverser traverser) => traverser.AfterSelfAndSelf<GameObjectTraverser, GameObject>();

        // direct shortcut

        public static ValueEnumerable<Children<GameObjectTraverser, GameObject>, GameObject> Children(this GameObject origin) => origin.AsTraverser().Children();
        public static ValueEnumerable<Children<GameObjectTraverser, GameObject>, GameObject> ChildrenAndSelf(this GameObject origin) => origin.AsTraverser().ChildrenAndSelf();
        public static ValueEnumerable<Descendants<GameObjectTraverser, GameObject>, GameObject> Descendants(this GameObject origin) => origin.AsTraverser().Descendants();
        public static ValueEnumerable<Descendants<GameObjectTraverser, GameObject>, GameObject> DescendantsAndSelf(this GameObject origin) => origin.AsTraverser().DescendantsAndSelf();
        public static ValueEnumerable<Ancestors<GameObjectTraverser, GameObject>, GameObject> Ancestors(this GameObject origin) => origin.AsTraverser().Ancestors();
        public static ValueEnumerable<Ancestors<GameObjectTraverser, GameObject>, GameObject> AncestorsAndSelf(this GameObject origin) => origin.AsTraverser().AncestorsAndSelf();
        public static ValueEnumerable<BeforeSelf<GameObjectTraverser, GameObject>, GameObject> BeforeSelf(this GameObject origin) => origin.AsTraverser().BeforeSelf();
        public static ValueEnumerable<BeforeSelf<GameObjectTraverser, GameObject>, GameObject> BeforeSelfAndSelf(this GameObject origin) => origin.AsTraverser().BeforeSelfAndSelf();
        public static ValueEnumerable<AfterSelf<GameObjectTraverser, GameObject>, GameObject> AfterSelf(this GameObject origin) => origin.AsTraverser().AfterSelf();
        public static ValueEnumerable<AfterSelf<GameObjectTraverser, GameObject>, GameObject> AfterSelfAndSelf(this GameObject origin) => origin.AsTraverser().AfterSelfAndSelf();

        // OfComponent

        public static ValueEnumerable<OfComponentG<Children<GameObjectTraverser, GameObject>, TComponent>, TComponent> OfComponent<TComponent>(this ValueEnumerable<Children<GameObjectTraverser, GameObject>, GameObject> source)
            where TComponent : Component => new(new(source.Enumerator));

        public static ValueEnumerable<OfComponentG<Descendants<GameObjectTraverser, GameObject>, TComponent>, TComponent> OfComponent<TComponent>(this ValueEnumerable<Descendants<GameObjectTraverser, GameObject>, GameObject> source)
            where TComponent : Component => new(new(source.Enumerator));

        public static ValueEnumerable<OfComponentG<Ancestors<GameObjectTraverser, GameObject>, TComponent>, TComponent> OfComponent<TComponent>(this ValueEnumerable<Ancestors<GameObjectTraverser, GameObject>, GameObject> source)
            where TComponent : Component => new(new(source.Enumerator));

        public static ValueEnumerable<OfComponentG<BeforeSelf<GameObjectTraverser, GameObject>, TComponent>, TComponent> OfComponent<TComponent>(this ValueEnumerable<BeforeSelf<GameObjectTraverser, GameObject>, GameObject> source)
            where TComponent : Component => new(new(source.Enumerator));

        public static ValueEnumerable<OfComponentG<AfterSelf<GameObjectTraverser, GameObject>, TComponent>, TComponent> OfComponent<TComponent>(this ValueEnumerable<AfterSelf<GameObjectTraverser, GameObject>, GameObject> source)
            where TComponent : Component => new(new(source.Enumerator));
    }

    [StructLayout(LayoutKind.Auto)]
    public struct GameObjectTraverser : ITraverser<GameObjectTraverser, GameObject>
    {
        static readonly object CalledTryGetNextChild = new object();
        static readonly object ParentNotFound = new object();

        readonly GameObject gameObject;
        readonly Transform transform; // cache transform
        object? initializedState; // CalledTryGetNext or Parent(for sibling operations)
        int childCount; // self childCount(TryGetNextChild) or parent childCount(TryGetSibling)
        int index;

        public GameObjectTraverser(GameObject origin)
        {
            this.gameObject = origin;
            this.transform = gameObject.transform;
            this.initializedState = null;
            this.childCount = 0;
            this.index = 0;
        }

        public GameObject Origin => gameObject;
        public GameObjectTraverser ConvertToTraverser(GameObject next) => new(next);

        public bool TryGetParent(out GameObject parent)
        {
            var tp = transform.parent;
            if (tp != null)
            {
                parent = tp.gameObject;
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

        public bool TryGetNextChild(out GameObject child)
        {
            if (initializedState == null)
            {
                initializedState = CalledTryGetNextChild;
                childCount = transform.childCount;
            }

            if (index < childCount)
            {
                child = transform.GetChild(index++).gameObject;
                return true;
            }

            child = default!;
            return false;
        }

        public bool TryGetNextSibling(out GameObject next)
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
                next = parent.GetChild(index++).gameObject;
                return true;
            }

            next = default!;
            return false;
        }

        public bool TryGetPreviousSibling(out GameObject previous)
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
                previous = parent.GetChild(index++).gameObject;
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
