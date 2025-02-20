using ZLinq.Linq;

namespace ZLinq;

// enumerable is enumerator because it always copied(don't share state)
// to achives copy-cost for performance and reduce assembly size
public interface IStructEnumerable<T> : IDisposable
{
    bool TryGetNonEnumeratedCount(out int count);
    bool TryGetNext(out T current); // as MoveNext + Current

    // can't do like this(some operator needs ref field but it can't support lower target platforms)
    // ref T TryGetNext(out bool success);
}

// generic implementation of enumerator
[StructLayout(LayoutKind.Auto)]
public struct StructEnumerator<TEnumerable, T>(TEnumerable source) : IDisposable
    where TEnumerable : struct, IStructEnumerable<T>
{
    T current = default!;

    public T Current => current;

    public bool MoveNext() => source.TryGetNext(out current);

    public void Dispose() => source.Dispose();
}

public static partial class StructEnumerableExtensions
{
    public static StructEnumerator<TEnumerable, T> GetEnumerator<TEnumerable, T>(ref this TEnumerable source)
        where TEnumerable : struct, IStructEnumerable<T>
    {
        return new(source);
    }

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
    {
        try
        {
            if (source.TryGetNonEnumeratedCount(out var count))
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


    public static void CopyToList<TEnumerable, T>(this TEnumerable source, List<T> list)
        where TEnumerable : struct, IStructEnumerable<T>
    {
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
