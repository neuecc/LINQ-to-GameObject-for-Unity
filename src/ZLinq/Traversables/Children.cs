namespace ZLinq.Traversables;

[StructLayout(LayoutKind.Auto)]
public struct Children<TTraverser, T>(TTraverser traverser, bool withSelf)
    : IValueEnumerator<T>
    where TTraverser : struct, ITraverser<TTraverser, T>
{
    public bool TryGetNonEnumeratedCount(out int count)
    {
        if (traverser.TryGetChildCount(out var childCount))
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

    public bool TryCopyTo(Span<T> destination, Index offset) => false;

    public bool TryGetNext(out T current)
    {
        if (withSelf)
        {
            current = traverser.Origin;
            withSelf = false;
            return true;
        }

        return traverser.TryGetNextChild(out current);
    }

    public void Dispose()
    {
        traverser.Dispose();
    }
}
