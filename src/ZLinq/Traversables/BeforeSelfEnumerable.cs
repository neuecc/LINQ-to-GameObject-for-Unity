namespace ZLinq.Traversables;

[StructLayout(LayoutKind.Auto)]
public struct BeforeSelfEnumerable<TTraversable, T>(TTraversable traversable, bool withSelf)
    : IStructEnumerable<T>
    where TTraversable : struct, ITraversable<TTraversable, T>
{
    bool iterateCompleted = false;

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
        if (iterateCompleted)
        {
            Unsafe.SkipInit(out current);
            return false;
        }

        if (traversable.TryGetPreviousSibling(out current))
        {
            return true;
        }
        else
        {
            iterateCompleted = true;
            if (withSelf)
            {
                current = traversable.Origin;
                return true;
            }
        }

        Unsafe.SkipInit(out current);
        return false;
    }

    public void Dispose()
    {
        traversable.Dispose();
    }
}