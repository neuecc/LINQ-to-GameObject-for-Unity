namespace ZLinq;

// enumerable is enumerator because it always copied(don't share state)
// to achives copy-cost for performance and reduce assembly size
public interface IValueEnumerable<T> : IDisposable
{
    bool TryGetNext(out T current); // as MoveNext + Current

    // Optimize series
    bool TryGetNonEnumeratedCount(out int count);
    bool TryGetSpan(out ReadOnlySpan<T> span);
    bool TryCopyTo(Span<T> destination);
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

    public T Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => current;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool MoveNext() => source.TryGetNext(out current);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

    // not allows ref struct; in .NET 9, only use directly from ITraversable or simple sequence source like IEnumerable<T>.AsValueEnumerable.
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
