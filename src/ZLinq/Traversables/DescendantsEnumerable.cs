namespace ZLinq.Traversables;

[StructLayout(LayoutKind.Auto)]
public struct DescendantsEnumerable<TTraversable, T>(TTraversable traversable, bool withSelf)
    : IValueEnumerator<T>
    where TTraversable : struct, ITraversable<TTraversable, T>
{
    RefStack<ChildrenEnumerable<TTraversable, T>>? recursiveStack = null;

    public bool TryGetNonEnumeratedCount(out int count)
    {
        count = 0;
        return false;
    }

    public bool TryGetSpan(out ReadOnlySpan<T> span)
    {
        span = default;
        return false;
    }

    public bool TryCopyTo(Span<T> dest) => false;

    public bool TryGetNext(out T current)
    {
        // IsDisposed
        if (recursiveStack == RefStack<ChildrenEnumerable<TTraversable, T>>.DisposeSentinel)
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
            var children = traversable.Children<TTraversable, T>();
            recursiveStack = RefStack<ChildrenEnumerable<TTraversable, T>>.Rent();
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
                    try
                    {
                        if (!subTraversable.TryGetHasChild(out var hasChild) || hasChild)
                        {
                            var children = subTraversable.Children<TTraversable, T>();
                            recursiveStack.Push(children);
                        }
                    }
                    finally
                    {
                        subTraversable.Dispose();
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
            RefStack<ChildrenEnumerable<TTraversable, T>>.Return(recursiveStack);
            recursiveStack = RefStack<ChildrenEnumerable<TTraversable, T>>.DisposeSentinel;
        }
    }
}
