namespace ZLinq;

[StructLayout(LayoutKind.Auto)]
public struct AncestorsEnumerable<T, TTraversable, TTraverser>(TTraversable traversable, bool withSelf)
    : IStructEnumerable<T>
    where TTraversable : struct, ITraversable<T, TTraversable, TTraverser>
    where TTraverser : struct, ITraverser<T>
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

    public bool TryGetNext(out T current)
    {
        if (withSelf)
        {
            current = traversable.Origin;
            withSelf = false;
            return true;
        }

        if (traversable.TryGetParent(out var parent))
        {
            current = parent;
            traversable = traversable.ConvertToTraversable(parent);
            return true;
        }

        Unsafe.SkipInit(out current);
        return false;
    }

    public void Dispose()
    {
    }
}
