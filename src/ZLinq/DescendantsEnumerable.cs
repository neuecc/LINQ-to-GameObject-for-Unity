using System.Runtime.CompilerServices;

namespace ZLinq
{
    public readonly struct DescendantsEnumerable<T, TTraversable>(TTraversable traversable, bool withSelf) : IStructEnumerable<T, DescendantsEnumerator<T, TTraversable>>
        where TTraversable : ITraversable<T, TTraversable>
    {
        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = 0;
            return false;
        }

        public DescendantsEnumerator<T, TTraversable> GetEnumerator() => new(traversable, withSelf);
    }

    public struct DescendantsEnumerator<T, TTraversable>(TTraversable traversable, bool withSelf) : IStructEnumerator<T>
        where TTraversable : ITraversable<T, TTraversable>
    {
        bool returnSelf = withSelf;
        T current = default!;
        RefStack<ChildrenEnumerator<T, TTraversable>>? recursiveStack = null;

        public T Current => current;

        public bool MoveNext()
        {
            // IsDisposed
            if (recursiveStack == RefStack<ChildrenEnumerator<T, TTraversable>>.DisposeSentinel)
            {
                return false;
            }

            if (returnSelf)
            {
                current = traversable.Origin;
                returnSelf = false;
                return true;
            }

            // initial setup
            if (recursiveStack == null)
            {
                // mutable struct(enumerator) must use from stack ref
                var children = traversable.Children().GetEnumerator();
                recursiveStack = RefStack<ChildrenEnumerator<T, TTraversable>>.Rent();
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
                        current = value; // set result

                        var subTraversable = traversable.GetNextTraversable(value);
                        var childCount = subTraversable.GetChildCount();
                        if (childCount != 0)
                        {
                            var children = subTraversable.Children().InternalGetEnumerator(childCount); // avoid duplicate call of childCount
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
                RefStack<ChildrenEnumerator<T, TTraversable>>.Return(recursiveStack);
                recursiveStack = RefStack<ChildrenEnumerator<T, TTraversable>>.DisposeSentinel;
            }
        }
    }
}
