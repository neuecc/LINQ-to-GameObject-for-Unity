namespace ZLinq;

public readonly struct BeforeSelfEnumerable<T, TTraversable, TTraverser>(TTraversable traversable, bool withSelf)
    : IStructEnumerable<T, BeforeSelfEnumerable<T, TTraversable, TTraverser>.Enumerator>
    where TTraversable : struct, ITraversable<T, TTraversable, TTraverser>
    where TTraverser : struct, ITraverser<T>
{
    public bool IsNull => traversable.IsNull;

    public bool TryGetNonEnumeratedCount(out int count)
    {
        count = 0;
        return false;
    }

    public Enumerator GetEnumerator() => new(traversable, withSelf);

    public struct Enumerator(TTraversable traversable, bool withSelf) : IStructEnumerator<T>
    {
        bool returnSelf = withSelf;
        bool iterateCompleted = false;
        T current = default!;
        TTraverser traverser = default!;

        public bool IsNull => traversable.IsNull;
        public T Current => current;

        public bool MoveNext()
        {
            if (iterateCompleted) return false;

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
                if (returnSelf)
                {
                    current = traversable.Origin;
                    returnSelf = false;
                    return true;
                }
            }

            return false;
        }

        public void Dispose()
        {
            traverser.Dispose();
        }
    }
}