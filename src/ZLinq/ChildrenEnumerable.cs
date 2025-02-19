namespace ZLinq;

public readonly struct ChildrenEnumerable<T, TTraversable, TTraverser>(TTraversable traversable, bool withSelf)
    : IStructEnumerable<T, ChildrenEnumerable<T, TTraversable, TTraverser>.Enumerator>
    where TTraversable : struct, ITraversable<T, TTraversable, TTraverser>
    where TTraverser : struct, ITraverser<T>
{
    public bool IsNull => traversable.IsNull;

    public bool TryGetNonEnumeratedCount(out int count)
    {
        if (traversable.TryGetChildCount(out var childCount))
        {
            count = childCount + (withSelf ? 1 : 0);
            return true;
        }

        count = 0;
        return false;
    }

    public Enumerator GetEnumerator() => new(traversable, withSelf);

    public struct Enumerator(TTraversable traversable, bool withSelf) : IStructEnumerator<T>
    {
        bool returnSelf = withSelf;
        T current = default!;
        TTraverser traverser = default!;

        public bool IsNull => traversable.IsNull;
        public T Current => current;

        public bool MoveNext()
        {
            if (returnSelf)
            {
                current = traversable.Origin;
                returnSelf = false;
                return true;
            }

            if (traverser.IsNull)
            {
                traverser = traversable.GetTraverser();
            }

            return traverser.TryGetNextChild(out current);
        }

        public void Dispose()
        {
            traverser.Dispose();
        }
    }
}
