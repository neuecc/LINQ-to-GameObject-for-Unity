namespace ZLinq;

public interface ITraverser<TTraverser, T> : IDisposable
    where TTraverser : struct, ITraverser<TTraverser, T> // self
{
    T Origin { get; }
    TTraverser ConvertToTraverser(T next); // for Descendants
    bool TryGetHasChild(out bool hasChild); // optional: optimize use for Descendants
    bool TryGetChildCount(out int count);   // optional: optimize use for Children
    bool TryGetParent(out T parent); // for Ancestors
    bool TryGetNextChild(out T child); // for Children | Descendants
    bool TryGetNextSibling(out T next); // for AfterSelf
    bool TryGetPreviousSibling(out T previous); // BeforeSelf
}

public static class TraverserExtensions
{
    public static ValueEnumerable<Children<TTraverser, T>, T> Children<TTraverser, T>(this TTraverser traverser)
        where TTraverser : struct, ITraverser<TTraverser, T>
    {
        return new(new(traverser, withSelf: false));
    }

    public static ValueEnumerable<Children<TTraverser, T>, T> ChildrenAndSelf<TTraverser, T>(this TTraverser traverser)
        where TTraverser : struct, ITraverser<TTraverser, T>
    {
        return new(new(traverser, withSelf: true));
    }

    public static ValueEnumerable<Descendants<TTraverser, T>, T> Descendants<TTraverser, T>(this TTraverser traverser)
        where TTraverser : struct, ITraverser<TTraverser, T>
    {
        return new(new(traverser, withSelf: false));
    }

    public static ValueEnumerable<Descendants<TTraverser, T>, T> DescendantsAndSelf<TTraverser, T>(this TTraverser traverser)
        where TTraverser : struct, ITraverser<TTraverser, T>
    {
        return new(new(traverser, withSelf: true));
    }

    public static ValueEnumerable<Ancestors<TTraverser, T>, T> Ancestors<TTraverser, T>(this TTraverser traverser)
        where TTraverser : struct, ITraverser<TTraverser, T>
    {
        return new(new(traverser, withSelf: false));
    }

    public static ValueEnumerable<Ancestors<TTraverser, T>, T> AncestorsAndSelf<TTraverser, T>(this TTraverser traverser)
        where TTraverser : struct, ITraverser<TTraverser, T>
    {
        return new(new(traverser, withSelf: true));
    }

    public static ValueEnumerable<BeforeSelf<TTraverser, T>, T> BeforeSelf<TTraverser, T>(this TTraverser traverser)
        where TTraverser : struct, ITraverser<TTraverser, T>
    {
        return new(new(traverser, withSelf: false));
    }

    public static ValueEnumerable<BeforeSelf<TTraverser, T>, T> BeforeSelfAndSelf<TTraverser, T>(this TTraverser traverser)
        where TTraverser : struct, ITraverser<TTraverser, T>
    {
        return new(new(traverser, withSelf: true));
    }

    public static ValueEnumerable<AfterSelf<TTraverser, T>, T> AfterSelf<TTraverser, T>(this TTraverser traverser)
        where TTraverser : struct, ITraverser<TTraverser, T>
    {
        return new(new(traverser, withSelf: false));
    }

    public static ValueEnumerable<AfterSelf<TTraverser, T>, T> AfterSelfAndSelf<TTraverser, T>(this TTraverser traverser)
        where TTraverser : struct, ITraverser<TTraverser, T>
    {
        return new(new(traverser, withSelf: true));
    }
}
