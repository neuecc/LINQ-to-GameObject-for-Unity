namespace ZLinq.Traversables;

[StructLayout(LayoutKind.Auto)]
public struct BeforeSelf<TTraverser, T>(TTraverser traverser, bool withSelf)
    : IValueEnumerator<T>
    where TTraverser : struct, ITraverser<TTraverser, T>
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

    public bool TryCopyTo(Span<T> destination, Index offset) => false;

    public bool TryGetNext(out T current)
    {
        if (iterateCompleted)
        {
            Unsafe.SkipInit(out current);
            return false;
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
                current = traverser.Origin;
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
