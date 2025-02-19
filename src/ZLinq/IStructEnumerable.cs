namespace ZLinq;

public interface IStructEnumerable<out T, TEnumerator>
    where TEnumerator : struct, IStructEnumerator<T> // allows ref struct(.NET 9...)
{
    bool IsNull { get; }
    bool TryGetNonEnumeratedCount(out int count);
    TEnumerator GetEnumerator();
}

public interface IStructEnumerator<out T> : IDisposable
{
    bool IsNull { get; }
    bool MoveNext();
    T Current { get; }
}

public static partial class StructEnumerableExtensions
{
    public static IEnumerable<T> AsEnumerable<T, TEnumerable, TEnumerator>(this TEnumerable source)
        where TEnumerable : struct, IStructEnumerable<T, TEnumerator>
        where TEnumerator : struct, IStructEnumerator<T>
    {
        foreach (var item in source)
        {
            yield return item;
        }
    }

    public static T[] ToArray<T, TEnumerable, TEnumerator>(this TEnumerable source)
        where TEnumerable : struct, IStructEnumerable<T, TEnumerator>
        where TEnumerator : struct, IStructEnumerator<T>
    {
        if (source.TryGetNonEnumeratedCount(out var count))
        {
            var i = 0;
            var array = new T[count];
            foreach (var item in source)
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
                foreach (var item in source)
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


    public static void CopyToList<T, TEnumerable, TEnumerator>(this TEnumerable source, List<T> list)
        where TEnumerable : struct, IStructEnumerable<T, TEnumerator>
        where TEnumerator : struct, IStructEnumerator<T>
    {
        var i = 0;
        var count = list.Count;
        var e = source.GetEnumerator();
        try
        {
            while (i < count)
            {
                if (e.MoveNext())
                {
                    list[i++] = e.Current;
                }
                else
                {
                    return;
                }
            }

            while (e.MoveNext())
            {
                list.Add(e.Current);
            }
        }
        finally
        {
            e.Dispose();
        }
    }
}
