using ZLinq.Traversables;

namespace ZLinq;

// like IValueEnumerable, ITraversable as enumerable-enumerator so must implement as `struct` to copy state naturally.
public interface ITraversable<TTraversable, T> : IDisposable
    where TTraversable : struct, ITraversable<TTraversable, T> // self
{
    T Origin { get; }
    TTraversable ConvertToTraversable(T next); // for Descendants
    bool TryGetHasChild(out bool hasChild); // optional: optimize use for Descendants
    bool TryGetChildCount(out int count);   // optional: optimize use for Children
    bool TryGetParent(out T parent); // for Ancestors
    bool TryGetNextChild(out T child); // for Children | Descendants
    bool TryGetNextSibling(out T next); // for AfterSelf
    bool TryGetPreviousSibling(out T previous); // BeforeSelf
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