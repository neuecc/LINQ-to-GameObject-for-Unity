namespace ZLinq;

[StructLayout(LayoutKind.Auto)]
public struct ChildrenEnumerable<T, TTraversable, TTraverser>(TTraversable traversable, bool withSelf)
    : IStructEnumerable<T>
    where TTraversable : struct, ITraversable<T, TTraversable, TTraverser>
    where TTraverser : struct, ITraverser<T>
{
    TTraverser traverser = default!;

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

    public bool TryGetNext(out T current)
    {
        if (withSelf)
        {
            current = traversable.Origin;
            withSelf = false;
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
