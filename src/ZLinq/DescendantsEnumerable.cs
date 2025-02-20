using System.Runtime.CompilerServices;

namespace ZLinq
{
    [StructLayout(LayoutKind.Auto)]
    public struct DescendantsEnumerable<T, TTraversable, TTraverser>(TTraversable traversable, bool withSelf)
            : IStructEnumerable<T>
            where TTraversable : struct, ITraversable<T, TTraversable, TTraverser>
            where TTraverser : struct, ITraverser<T>
    {
        RefStack<ChildrenEnumerable<T, TTraversable, TTraverser>>? recursiveStack = null;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = 0;
            return false;
        }

        public bool TryGetNext(out T current)
        {
            // IsDisposed
            if (recursiveStack == RefStack<ChildrenEnumerable<T, TTraversable, TTraverser>>.DisposeSentinel)
            {
                Unsafe.SkipInit(out current);
                return false;
            }

            if (withSelf)
            {
                current = traversable.Origin;
                withSelf = false;
                return true;
            }

            // initial setup
            if (recursiveStack == null)
            {
                // mutable struct(enumerator) must use from stack ref
                var children = traversable.Children();
                recursiveStack = RefStack<ChildrenEnumerable<T, TTraversable, TTraverser>>.Rent();
                recursiveStack.Push(children);
            }

            // traverse depth first search
            {
                ref var enumerator = ref recursiveStack.PeekRefOrNullRef();
                while (!Unsafe.IsNullRef(ref enumerator))
                {
                    while (enumerator.TryGetNext(out var value))
                    {
                        current = value; // set result

                        var subTraversable = traversable.ConvertToTraversable(value);
                        if (subTraversable.HasChild)
                        {
                            var children = subTraversable.Children();
                            recursiveStack.Push(children);
                        }

                        return true; // ok.
                    }

                    enumerator.Dispose(); // dispose before pop
                    recursiveStack.Pop(); // end current iteration, Pop
                    enumerator = ref recursiveStack.PeekRefOrNullRef(); // Peek Again
                }

                Unsafe.SkipInit(out current);
                return false;
            }
        }

        public void Dispose()
        {
            if (recursiveStack != null)
            {
                RefStack<ChildrenEnumerable<T, TTraversable, TTraverser>>.Return(recursiveStack);
                recursiveStack = RefStack<ChildrenEnumerable<T, TTraversable, TTraverser>>.DisposeSentinel;
            }
        }
    }
}
