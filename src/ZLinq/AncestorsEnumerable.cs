namespace ZLinq;

[StructLayout(LayoutKind.Auto)]
public struct AncestorsEnumerable<TTraversable, T>(TTraversable traversable, bool withSelf)
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
            var nextTraversable = traversable.ConvertToTraversable(parent);
            traversable.Dispose();
            traversable = nextTraversable;
            return true;
        }

        Unsafe.SkipInit(out current);
        return false;
    }

    public void Dispose()
    {
        traversable.Dispose();
    }
}
