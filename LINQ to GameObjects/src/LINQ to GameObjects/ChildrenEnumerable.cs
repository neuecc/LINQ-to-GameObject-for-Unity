using Cysharp.Linq.Internal;
using UnityEngine;

namespace Cysharp.Linq
{
    public readonly struct ChildrenEnumerable<T> : IStructEnumerable<T, ChildrenEnumerable<T>.Enumerator>
        where T : class // T is Transform or GameObject
    {
        readonly Transform origin;
        readonly bool withSelf;

        internal ChildrenEnumerable(Transform origin, bool withSelf)
        {
            this.origin = origin;
            this.withSelf = withSelf;
        }

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = origin.childCount + (withSelf ? 1 : 0);
            return true;
        }

        public Enumerator GetEnumerator() => new(origin, withSelf, origin.childCount);
        internal Enumerator InternalGetEnumerator(int childCount) => new(origin, withSelf, childCount);

        public OfComponentEnumerable<T, ChildrenEnumerable<T>, Enumerator, TComponent> OfComponent<TComponent>()
            where TComponent : Component
        {
            return new(this);
        }

        public struct Enumerator : IStructEnumerator<T>
        {
            readonly Transform originTransform;
            readonly int childCount;

            bool returnSelf;
            int currentIndex;
            T current;

            internal Enumerator(Transform originTransform, bool withSelf, int childCount)
            {
                this.originTransform = originTransform;
                this.childCount = childCount;
                this.returnSelf = withSelf;
                this.currentIndex = -1;
                this.current = null!;
            }

            public T Current => current;

            public bool MoveNext()
            {
                if (returnSelf)
                {
                    current = UnsafeConversions.ConvertTransformTo<T>(originTransform);
                    returnSelf = false;
                    return true;
                }

                currentIndex++;
                if (currentIndex < childCount)
                {
                    var childTransform = originTransform.GetChild(currentIndex);
                    current = UnsafeConversions.ConvertTransformTo<T>(childTransform);
                    return true;
                }

                return false;
            }

            public void Dispose() { }
        }
    }
}
