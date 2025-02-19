using System.Runtime.CompilerServices;

namespace ZLinq
{
    public readonly struct DescendantsEnumerable<T, TTraversable, TTraverser>(TTraversable traversable, bool withSelf)
        : IStructEnumerable<T, DescendantsEnumerable<T, TTraversable, TTraverser>.Enumerator>
        where TTraversable : struct, ITraversable<T, TTraversable, TTraverser>
        where TTraverser : struct, ITraverser<T>
    {
        public bool IsNull => traversable.IsNull;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = 0;
            return false;
        }

        public Enumerator GetEnumerator() => new(traversable, withSelf);

        public struct Enumerator(TTraversable traversable, bool withSelf) : IStructEnumerator<T>
        {
            bool returnSelf = withSelf;
            T current = default!;
            RefStack<ChildrenEnumerable<T, TTraversable, TTraverser>.Enumerator>? recursiveStack = null;

            public bool IsNull => traversable.IsNull;
            public T Current => current;

            public bool MoveNext()
            {
                // IsDisposed
                if (recursiveStack == RefStack<ChildrenEnumerable<T, TTraversable, TTraverser>.Enumerator>.DisposeSentinel)
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
                    recursiveStack = RefStack<ChildrenEnumerable<T, TTraversable, TTraverser>.Enumerator>.Rent();
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

                            var subTraversable = traversable.ConvertToTraversable(value);
                            if (subTraversable.HasChild)
                            {
                                var children = subTraversable.Children().GetEnumerator();
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
                    RefStack<ChildrenEnumerable<T, TTraversable, TTraverser>.Enumerator>.Return(recursiveStack);
                    recursiveStack = RefStack<ChildrenEnumerable<T, TTraversable, TTraverser>.Enumerator>.DisposeSentinel;
                }
            }
        }
    }
}
