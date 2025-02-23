namespace ZLinq.Traversables;

[StructLayout(LayoutKind.Auto)]
public struct ChildrenEnumerable<TTraversable, T>(TTraversable traversable, bool withSelf)
    : IStructEnumerable<T>
    where TTraversable : struct, ITraversable<TTraversable, T>
{
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

    public bool TryGetSpan(out ReadOnlySpan<T> span)
    {
        span = default;
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

        return traversable.TryGetNextChild(out current);
    }

    public void Dispose()
    {
        traversable.Dispose();
    }
}
