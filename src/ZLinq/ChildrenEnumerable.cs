namespace ZLinq;

public readonly struct ChildrenEnumerable<T, TTraversable>(TTraversable traversable, bool withSelf)
    : ITraversableEnumerable<T, TTraversable, ChildrenEnumerator<T, TTraversable>>
    where TTraversable : ITraversable<T, TTraversable>
{
    public TTraversable Traversable => traversable;

    public bool TryGetNonEnumeratedCount(out int count)
    {
        count = traversable.GetChildCount() + (withSelf ? 1 : 0);
        return true;
    }

    public ChildrenEnumerator<T, TTraversable> GetEnumerator() => new(traversable, withSelf, traversable.GetChildCount());
    internal ChildrenEnumerator<T, TTraversable> InternalGetEnumerator(int childCount) => new(traversable, withSelf, childCount);
}

public struct ChildrenEnumerator<T, TTraversable>(TTraversable traversable, bool withSelf, int childCount) : IStructEnumerator<T>
    where TTraversable : ITraversable<T, TTraversable>
{
    bool returnSelf = withSelf;
    int currentIndex;
    T current = default!;

    public T Current => current;

    public bool MoveNext()
    {
        if (returnSelf)
        {
            current = traversable.Origin;
            returnSelf = false;
            return true;
        }

        currentIndex++;
        if (currentIndex < childCount)
        {
            current = traversable.GetChild(currentIndex);
            return true;
        }

        return false;
    }

    public void Dispose() { }
}
