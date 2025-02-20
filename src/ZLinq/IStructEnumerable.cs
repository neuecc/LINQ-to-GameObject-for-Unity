namespace ZLinq;

// enumerable is enumerator because it always copied(don't share state)
// to achives copy-cost for performance and reduce assembly size
public interface IStructEnumerable<TEnumerable, T> : IDisposable
    where TEnumerable : struct, IStructEnumerable<TEnumerable, T> // allows ref struct(but needs .NET 9...)
{
    bool IsNull { get; }
    bool TryGetNonEnumeratedCount(out int count);
    bool TryGetNext(out T value); // as MoveNext + Current

    StructEnumerator<TEnumerable, T> GetEnumerator(); // for foreach
}

// generic implementation of enumerator
public struct StructEnumerator<TEnumerable, T>(TEnumerable source) : IDisposable
    where TEnumerable : struct, IStructEnumerable<TEnumerable, T>
{
    T current = default!;

    public T Current => current;

    public bool MoveNext() => source.TryGetNext(out current);

    public void Dispose() => source.Dispose();
}

public static partial class StructEnumerableExtensions
{
    public static IEnumerable<T> AsEnumerable<TEnumerable, T>(this TEnumerable source)
        where TEnumerable : struct, IStructEnumerable<TEnumerable, T>
    {
        foreach (var item in source)
        {
            yield return item;
        }
    }

    public static T[] ToArray<TEnumerable, T>(this TEnumerable source)
        where TEnumerable : struct, IStructEnumerable<TEnumerable, T>
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
        where TEnumerable : struct, IStructEnumerable<TEnumerable, T>
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
