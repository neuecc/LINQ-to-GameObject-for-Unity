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

    TTraverser GetTraverser();
    TTraversable ConvertToTraversable(T next);

    // use interface method instead of extension method(for better type inference)

    ChildrenEnumerable<T, TTraversable, TTraverser> Children();
    ChildrenEnumerable<T, TTraversable, TTraverser> ChildrenAndSelf();

    // TODO: Func<Transform, bool> descendIntoChildren
    DescendantsEnumerable<T, TTraversable, TTraverser> Descendants();
    DescendantsEnumerable<T, TTraversable, TTraverser> DescendantsAndSelf();
    AncestorsEnumerable<T, TTraversable, TTraverser> Ancestors();
    AncestorsEnumerable<T, TTraversable, TTraverser> AncestorsAndSelf();
    BeforeSelfEnumerable<T, TTraversable, TTraverser> BeforeSelf();
    BeforeSelfEnumerable<T, TTraversable, TTraverser> BeforeSelfAndSelf();
    AfterSelfEnumerable<T, TTraversable, TTraverser> AfterSelf();
    AfterSelfEnumerable<T, TTraversable, TTraverser> AfterSelfAndSelf();
}

public interface ITraverser<T> : IDisposable
{
    bool IsNull { get; }
    bool TryGetNextChild(out T child);
    bool TryGetNextSibling(out T next);
    bool TryGetPreviousSibling(out T previous);
}