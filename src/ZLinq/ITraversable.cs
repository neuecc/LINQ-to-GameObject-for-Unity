namespace ZLinq;

public interface ITraversable<T, TTraversable>
    where TTraversable : ITraversable<T, TTraversable>
{
    T Origin { get; }
    T GetParent();
    int GetChildCount();
    T GetChild(int index);
    int GetSiblingIndex(int siblingIndex);

    TTraversable GetNextTraversable(T next); // static...

    // use interface method instead of extension method(for better type inference)
    ChildrenEnumerable<T, TTraversable> Children();
}

public interface ITraversableEnumerable<T, TTraversable, TEnumerator> : IStructEnumerable<T, TEnumerator>
    where TTraversable : ITraversable<T, TTraversable>
    where TEnumerator : struct, IStructEnumerator<T>
{
    TTraversable Traversable { get; }
}