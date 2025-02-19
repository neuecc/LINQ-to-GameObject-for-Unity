namespace ZLinq;

public interface ITraversable<T, TTraversable, TTraverser>
    where TTraversable : struct, ITraversable<T, TTraversable, TTraverser>
    where TTraverser : struct, ITraverser<T>
{
    bool IsNull { get; }

    T Origin { get; }
    bool HasChild { get; }
    bool TryGetParent(out T parent);
    bool TryGetChildCount(out int count);

    // TODO:...
    // int GetSiblingIndex(int siblingIndex);

    TTraverser GetTraverser();
    TTraversable ConvertToTraversable(T next);


    // use interface method instead of extension method(for better type inference)
    ChildrenEnumerable<T, TTraversable, TTraverser> Children();
}

public interface ITraverser<T> : IDisposable
{
    bool IsNull { get; }
    bool TryGetNextChild(out T child);

}


// TODO:
//public interface ITraversableEnumerable<T, TTraversable, TEnumerator> : IStructEnumerable<T, TEnumerator>
//    where TTraversable : ITraversable<T, TTraversable>
//    where TEnumerator : struct, IStructEnumerator<T>
//{
//    TTraversable Traversable { get; }
//}