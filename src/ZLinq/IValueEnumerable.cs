namespace ZLinq;

// enumerable is enumerator because it always copied(don't share state)
// to achives copy-cost for performance and reduce assembly size
public interface IValueEnumerable<T> : IDisposable
{
    bool TryGetNonEnumeratedCount(out int count);
    bool TryGetSpan(out ReadOnlySpan<T> span);
    bool TryGetNext(out T current); // as MoveNext + Current

    // can't do like this(some operator needs ref field but it can't support lower target platforms)
    // ref T TryGetNext(out bool success);
}

// generic implementation of enumerator
[StructLayout(LayoutKind.Auto)]
#if NET9_0_OR_GREATER
public ref
#else
public
#endif
struct ValueEnumerator<TEnumerable, T>(TEnumerable source) : IDisposable
    where TEnumerable : struct, IValueEnumerable<T>
#if NET9_0_OR_GREATER
    , allows ref struct
#endif
{
    TEnumerable source = source;

    T current = default!;

    public T Current => current;

    public bool MoveNext() => source.TryGetNext(out current);

    public void Dispose() => source.Dispose();
}

public static partial class ValueEnumerableExtensions
{
    public static ValueEnumerator<TEnumerable, T> GetEnumerator<TEnumerable, T>(this TEnumerable source)
        where TEnumerable : struct, IValueEnumerable<T>
#if NET9_0_OR_GREATER
    , allows ref struct
#endif
    {
        return new(source);
    }

    // not allows ref struct; in .NET 9, only use directly from ITraversable(or similar) sequence
    public static IEnumerable<T> AsEnumerable<TEnumerable, T>(this TEnumerable source)
        where TEnumerable : struct, IValueEnumerable<T>
    {
        try
        {
            while (source.TryGetNext(out var current))
            {
                yield return current;
            }
        }
        finally
        {
            source.Dispose();
        }
    }
}
