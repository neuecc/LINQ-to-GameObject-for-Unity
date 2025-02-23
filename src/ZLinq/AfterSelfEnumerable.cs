namespace ZLinq;

[StructLayout(LayoutKind.Auto)]
public struct AfterSelfEnumerable<TTraversable, T>(TTraversable traversable, bool withSelf)
    : IStructEnumerable<T>
    where TTraversable : struct, ITraversable<TTraversable, T>
{
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

    public bool TryGetNext(out T current)
    {
        if (withSelf)
        {
            current = traversable.Origin;
            withSelf = false;
            return true;
        }

        return traversable.TryGetNextSibling(out current);
    }

    public void Dispose()
    {
        traversable.Dispose();
    }
}
