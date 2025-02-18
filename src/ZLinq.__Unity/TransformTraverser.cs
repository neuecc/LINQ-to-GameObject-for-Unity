using UnityEngine;

namespace ZLinq.Unity
{
    // TODO:GameObjectTraversable

    public readonly struct TransformTraversable : ITraversable<Transform, TransformTraversable>
    {
        readonly Transform transform;

        public TransformTraversable(Transform origin)
        {
            this.transform = origin;
        }

        public Transform Origin => transform;

        public TransformTraversable GetNextTraversable(Transform child)
        {
            return new TransformTraversable(child);
        }

        public Transform GetChild(int index)
        {
            return transform.GetChild(index);
        }

        public int GetChildCount()
        {
            return transform.childCount;
        }

        public Transform GetParent()
        {
            return transform.parent;
        }

        public int GetSiblingIndex(int siblingIndex)
        {
            return transform.GetSiblingIndex();
        }

        public ChildrenEnumerable<Transform, TransformTraversable> Children()
        {
            return new(this, withSelf: false);
        }
    }

}
