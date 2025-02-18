using Cysharp.Linq.Internal;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Cysharp.Linq
{
    public readonly struct DescendantsEnumerable<T> : IStructEnumerable<T, DescendantsEnumerable<T>.Enumerator>
       where T : class // T is Transform or GameObject
    {
        readonly Transform origin;
        readonly bool withSelf;

        internal DescendantsEnumerable(Transform origin, bool withSelf)
        {
            this.origin = origin;
            this.withSelf = withSelf;
        }

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = 0;
            return false;
        }

        public Enumerator GetEnumerator() => new(origin, withSelf);

        public OfComponentEnumerable<T, DescendantsEnumerable<T>, Enumerator, TComponent> OfComponent<TComponent>()
            where TComponent : Component
        {
            return new(this);
        }

        public struct Enumerator : IStructEnumerator<T>
        {
            readonly Transform originTransform;

            bool returnSelf;
            T current;
            RefStack<ChildrenEnumerable<Transform>.Enumerator> recursiveStack;

            internal Enumerator(Transform originTransform, bool withSelf)
            {
                this.originTransform = originTransform;
                this.returnSelf = withSelf;
                this.current = null!;
                this.recursiveStack = null!;
            }

            public T Current => current;

            public bool MoveNext()
            {
                // IsDisposed
                if (recursiveStack == RefStack<ChildrenEnumerable<Transform>.Enumerator>.DisposeSentinel)
                {
                    return false;
                }

                if (returnSelf)
                {
                    current = UnsafeConversions.ConvertTransformTo<T>(originTransform);
                    returnSelf = false;
                    return true;
                }

                // initial setup
                if (recursiveStack == null)
                {
                    // mutable struct(enumerator) must use from stack ref
                    var children = originTransform.Children().GetEnumerator();
                    recursiveStack = RefStack<ChildrenEnumerable<Transform>.Enumerator>.Rent();
                    recursiveStack.Push(children);
                }

                // traverse depth first search
                {
                    ref var enumerator = ref recursiveStack.PeekRefOrNullRef();
                    while (!Unsafe.IsNullRef(ref enumerator))
                    {
                        while (enumerator.MoveNext())
                        {
                            var value = enumerator.Current;
                            this.current = UnsafeConversions.ConvertTransformTo<T>(value); // set result

                            var childCount = value.childCount;
                            if (childCount != 0)
                            {
                                var children = value.Children().InternalGetEnumerator(childCount); // avoid duplicate call of childCount
                                recursiveStack.Push(children);
                            }

                            return true; // ok.
                        }

                        recursiveStack.Pop(); // end current iteration, Pop
                        enumerator = ref recursiveStack.PeekRefOrNullRef(); // Peek Again
                    }

                    return false;
                }
            }

            public void Dispose()
            {
                if (recursiveStack != null)
                {
                    RefStack<ChildrenEnumerable<Transform>.Enumerator>.Return(recursiveStack);
                    recursiveStack = RefStack<ChildrenEnumerable<Transform>.Enumerator>.DisposeSentinel;
                }
            }
        }
    }
}
