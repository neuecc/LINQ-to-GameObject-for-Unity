namespace ZLinq;

// enumerable is enumerator because it always copied(don't share state)
// to achives copy-cost for performance and reduce assembly size
public interface IStructEnumerable<T> : IDisposable
{
    bool TryGetNonEnumeratedCount(out int count);
    bool TryGetSpan(out ReadOnlySpan<T> span);
    bool TryGetNext(out T current); // as MoveNext + Current

    // can't do like this(some operator needs ref field but it can't support lower target platforms)
    // ref T TryGetNext(out bool success);
}

// generic implementation of enumerator
[StructLayout(LayoutKind.Auto)]
public ref struct StructEnumerator<TEnumerable, T>(TEnumerable source) : IDisposable
    where TEnumerable : struct, IStructEnumerable<T>
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

public static partial class StructEnumerableExtensions
{
    public static StructEnumerator<TEnumerable, T> GetEnumerator<TEnumerable, T>(this TEnumerable source)
        where TEnumerable : struct, IStructEnumerable<T>
#if NET9_0_OR_GREATER
    , allows ref struct
#endif
    {
        return new(source);
    }

    // not allows ref struct; in .NET 9, only use directly from ITraversable(or similar) sequence
    public static IEnumerable<T> AsEnumerable<TEnumerable, T>(this TEnumerable source)
        where TEnumerable : struct, IStructEnumerable<T>
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

    public static T[] ToArray<TEnumerable, T>(this TEnumerable source)
        where TEnumerable : struct, IStructEnumerable<T>
#if NET9_0_OR_GREATER
    , allows ref struct
#endif
    {
        try
        {
            if (source.TryGetSpan(out var span))
            {
                return span.ToArray();
            }
            else if (source.TryGetNonEnumeratedCount(out var count))
            {
                var i = 0;
                var array = new T[count];

                while (source.TryGetNext(out var item))
                {
                    array[i++] = item;
                }

                return array;
            }
            else
            {
                var arrayBuilder = new SegmentedArrayBuilder<T>();
                try
                {
                    while (source.TryGetNext(out var item))
                    {
                        arrayBuilder.Add(item);
                    }

                    return arrayBuilder.ToArray();
                }
                finally
                {
                    arrayBuilder.Dispose();
                }
            }
        }
        finally
        {
            source.Dispose();
        }
    }

    // TODO: CopyInto
    public static void CopyToList<TEnumerable, T>(this TEnumerable source, List<T> list)
        where TEnumerable : struct, IStructEnumerable<T>
#if NET9_0_OR_GREATER
    , allows ref struct
#endif
    {
        // TODO: CollectionSmarshal.SetCount
        var i = 0;
        var count = list.Count;
        try
        {
            while (i < count)
            {
                if (source.TryGetNext(out var item))
                {
                    list[i++] = item;
                }
                else
                {
                    return;
                }
            }

            while (source.TryGetNext(out var item))
            {
                list.Add(item);
            }
        }
        finally
        {
            source.Dispose();
        }
    }
}
