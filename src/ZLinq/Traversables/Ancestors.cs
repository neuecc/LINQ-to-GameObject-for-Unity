namespace ZLinq.Traversables;

[StructLayout(LayoutKind.Auto)]
public struct Ancestors<TTraverser, T>(TTraverser traverser, bool withSelf)
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

    public bool TryCopyTo(Span<T> dest) => false;

    public bool TryGetNext(out T current)
    {
        if (withSelf)
        {
            current = traverser.Origin;
            withSelf = false;
            return true;
        }

        if (traverser.TryGetParent(out var parent))
        {
            current = parent;
            var nexTTraverser = traverser.ConvertToTraverser(parent);
            traverser.Dispose();
            traverser = nexTTraverser;
            return true;
        }

        Unsafe.SkipInit(out current);
        return false;
    }

    public void Dispose()
    {
        traverser.Dispose();
    }
}
