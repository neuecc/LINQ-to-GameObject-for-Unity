namespace ZLinq;

[StructLayout(LayoutKind.Auto)]
public struct BeforeSelfEnumerable<T, TTraversable, TTraverser>(TTraversable traversable, bool withSelf)
    : IStructEnumerable<T>
    where TTraversable : struct, ITraversable<T, TTraversable, TTraverser>
    where TTraverser : struct, ITraverser<T>
{
    bool iterateCompleted = false;
    TTraverser traverser = default!;

    public bool TryGetNonEnumeratedCount(out int count)
    {
        count = 0;
        return false;
    }

    public bool TryGetNext(out T current)
    {
        if (iterateCompleted)
        {
            Unsafe.SkipInit(out current);
            return false;
        }

        if (traverser.IsNull)
        {
            traverser = traversable.GetTraverser();
        }

        if (traverser.TryGetPreviousSibling(out current))
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
        traverser.Dispose();
    }
}