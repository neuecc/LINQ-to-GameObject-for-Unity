namespace ZLinq;

partial class StructEnumerableExtensions
{
    public static EnumerableStructEnumerable<T> AsStructEnumerable<T>(this IEnumerable<T> source)
    {
        return new(source);
    }

    public static ArrayStructEnumerable<T> AsStructEnumerable<T>(this T[] source)
    {
        return new(source);
    }

    public static ListStructEnumerable<T> AsStructEnumerable<T>(this List<T> source)
    {
        return new(source);
    }
}

public readonly struct EnumerableStructEnumerable<T>(IEnumerable<T> source) : IStructEnumerable<T, EnumerableStructEnumerable<T>.Enumerator>
{
    public bool IsNull => source == null;
    public Enumerator GetEnumerator() => new(source);

    public bool TryGetNonEnumeratedCount(out int count)
    {
#if NET8_0_OR_GREATER
        return source.TryGetNonEnumeratedCount(out count);
#else
        if (source is ICollection<T> c)
        {
            count = c.Count;
            return true;
        }
        else if (source is IReadOnlyCollection<T> rc)
        {
            count = rc.Count;
            return true;
        }
        count = 0;
        return false;
#endif
    }

    public struct Enumerator(IEnumerable<T> source) : IStructEnumerator<T>
    {
        T current = default!;
        IEnumerator<T>? enumerator;

        public bool IsNull => source == null;
        public T Current => current;

        public bool MoveNext()
        {
            if (enumerator == null)
            {
                enumerator = source.GetEnumerator();
            }

            if (enumerator.MoveNext())
            {
                current = enumerator.Current;
                return true;
            }

            return false;
        }

        public void Dispose()
        {
            if (enumerator != null)
            {
                enumerator.Dispose();
            }
        }
    }
}

public readonly struct ArrayStructEnumerable<T>(T[] source) : IStructEnumerable<T, ArrayStructEnumerable<T>.Enumerator>
{
    public bool IsNull => source == null;
    public Enumerator GetEnumerator() => new(source);

    public bool TryGetNonEnumeratedCount(out int count)
    {
        count = source.Length;
        return false;
    }

    public struct Enumerator(T[] source) : IStructEnumerator<T>
    {
        T current = default!;
        int index;

        public bool IsNull => source == null;
        public T Current => current;

        public bool MoveNext()
        {
            if (index < source.Length)
            {
                current = source[index++];
                return true;
            }
            return false;
        }

        public void Dispose()
        {
        }
    }
}

public readonly struct ListStructEnumerable<T>(List<T> source) : IStructEnumerable<T, ListStructEnumerable<T>.Enumerator>
{
    public bool IsNull => source == null;
    public Enumerator GetEnumerator() => new(source);

    public bool TryGetNonEnumeratedCount(out int count)
    {
        count = source.Count;
        return true;
    }

    public struct Enumerator(List<T> source) : IStructEnumerator<T>
    {
        T current = default!;
        bool isInit = false;
        List<T>.Enumerator enumerator;

        public bool IsNull => source == null;
        public T Current => current;

        public bool MoveNext()
        {
            if (!isInit)
            {
                isInit = true;
                enumerator = source.GetEnumerator();
            }

            if (enumerator.MoveNext())
            {
                current = enumerator.Current;
                return true;
            }
            return false;
        }

        public void Dispose()
        {
            if (isInit)
            {
                enumerator.Dispose();
            }
        }
    }
}
