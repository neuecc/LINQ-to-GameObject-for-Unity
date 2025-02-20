namespace ZLinq;

// like IStructEnumerable, ITraversable as enumerable-enumerator so must implement as `struct` to copy state naturally.
public interface ITraversable<TTraversable, T> : IDisposable
    where TTraversable : struct, ITraversable<TTraversable, T> // self
{
    T Origin { get; }
    bool HasChild { get; }
    TTraversable ConvertToTraversable(T next);
    bool TryGetParent(out T parent);
    bool TryGetChildCount(out int count);
    bool TryGetNextChild(out T child);
    bool TryGetNextSibling(out T next);
    bool TryGetPreviousSibling(out T previous);
}

public static class TraversableExtensions
{
    // TODO: Func<Transform, bool> descendIntoChildren

    public static ChildrenEnumerable<TTraversable, T> Children<TTraversable, T>(this TTraversable traversable)
        where TTraversable : struct, ITraversable<TTraversable, T>
    {
        return new(traversable, withSelf: false);
    }

    public static ChildrenEnumerable<TTraversable, T> ChildrenAndSelf<TTraversable, T>(this TTraversable traversable)
        where TTraversable : struct, ITraversable<TTraversable, T>
    {
        return new(traversable, withSelf: true);
    }

    public static DescendantsEnumerable<TTraversable, T> Descendants<TTraversable, T>(this TTraversable traversable)
        where TTraversable : struct, ITraversable<TTraversable, T>
    {
        return new(traversable, withSelf: false);
    }

    public static DescendantsEnumerable<TTraversable, T> DescendantsAndSelf<TTraversable, T>(this TTraversable traversable)
        where TTraversable : struct, ITraversable<TTraversable, T>
    {
        return new(traversable, withSelf: true);
    }

    public static AncestorsEnumerable<TTraversable, T> Ancestors<TTraversable, T>(this TTraversable traversable)
        where TTraversable : struct, ITraversable<TTraversable, T>
    {
        return new(traversable, withSelf: false);
    }

    public static AncestorsEnumerable<TTraversable, T> AncestorsAndSelf<TTraversable, T>(this TTraversable traversable)
        where TTraversable : struct, ITraversable<TTraversable, T>
    {
        return new(traversable, withSelf: true);
    }

    public static BeforeSelfEnumerable<TTraversable, T> BeforeSelf<TTraversable, T>(this TTraversable traversable)
        where TTraversable : struct, ITraversable<TTraversable, T>
    {
        return new(traversable, withSelf: false);
    }

    public static BeforeSelfEnumerable<TTraversable, T> BeforeSelfAndSelf<TTraversable, T>(this TTraversable traversable)
        where TTraversable : struct, ITraversable<TTraversable, T>
    {
        return new(traversable, withSelf: true);
    }

    public static AfterSelfEnumerable<TTraversable, T> AfterSelf<TTraversable, T>(this TTraversable traversable)
        where TTraversable : struct, ITraversable<TTraversable, T>
    {
        return new(traversable, withSelf: false);
    }

    public static AfterSelfEnumerable<TTraversable, T> AfterSelfAndSelf<TTraversable, T>(this TTraversable traversable)
        where TTraversable : struct, ITraversable<TTraversable, T>
    {
        return new(traversable, withSelf: true);
    }
}