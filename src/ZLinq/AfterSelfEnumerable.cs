namespace ZLinq;

[StructLayout(LayoutKind.Auto)]
public struct AfterSelfEnumerable<T, TTraversable, TTraverser>(TTraversable traversable, bool withSelf)
    : IStructEnumerable<T>
    where TTraversable : struct, ITraversable<T, TTraversable, TTraverser>
    where TTraverser : struct, ITraverser<T>
{
    TTraverser traverser = default!;

    public bool TryGetNonEnumeratedCount(out int count)
    {
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

        return traverser.TryGetNextSibling(out current);
    }

    public void Dispose()
    {
        traverser.Dispose();
    }
}
