using UnityEngine;

namespace ZLinq.Unity
{
    // TODO:GameObjectTraversable

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

        public ChildrenEnumerable<Transform, TransformTraversable, TransformTraverser> Children()
        {
            return new(this, withSelf: false);
        }
    }

    public struct TransformTraverser : ITraverser<Transform>
    {
        readonly Transform transform;
        readonly int childCound;
        int index;

        public bool IsNull => transform is null; // don't use `==`.

        public TransformTraverser(Transform transform)
        {
            this.transform = transform;
            this.childCound = transform.childCount; // get childCount on init.
            this.index = 0;
        }

        public void Dispose()
        {
        }

        public bool TryGetNextChild(out Transform child)
        {
            if (index < childCound)
            {
                child = transform.GetChild(index++);
                return true;
            }

            child = default!;
            return false;
        }
    }
}
