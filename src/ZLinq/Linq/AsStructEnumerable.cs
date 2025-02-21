namespace ZLinq
{
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

        public static MemoryStructEnumerable<T> AsStructEnumerable<T>(this Memory<T> source)
        {
            return new(source);
        }

        public static MemoryStructEnumerable<T> AsStructEnumerable<T>(this ReadOnlyMemory<T> source)
        {
            return new(source);
        }

#if NET9_0_OR_GREATER

        public static SpanStructEnumerable<T> AsStructEnumerable<T>(this Span<T> source)
        {
            return new(source);
        }

        public static SpanStructEnumerable<T> AsStructEnumerable<T>(this ReadOnlySpan<T> source)
        {
            return new(source);
        }

#endif
    }
}

namespace ZLinq.Linq
{
    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct EnumerableStructEnumerable<T>(IEnumerable<T> source) : IStructEnumerable<T>
    {
        IEnumerator<T>? enumerator = null;

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

        public bool TryGetNext(out T current)
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

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
        }
    }

    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct ArrayStructEnumerable<T>(T[] source) : IStructEnumerable<T>
    {
        int index;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = source.Length;
            return false;
        }

        public bool TryGetNext(out T current)
        {
            if (index < source.Length)
            {
                current = source[index++];
                return true;
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
        }
    }

    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public ref struct MemoryStructEnumerable<T>(ReadOnlyMemory<T> source) : IStructEnumerable<T>
    {
#if NET9_0_OR_GREATER
        ReadOnlySpan<T> source = source.Span;
#endif

        int index;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = source.Length;
            return false;
        }

        public bool TryGetNext(out T current)
        {
            if (index < source.Length)
            {
#if NET9_0_OR_GREATER
                current = source[index++];
#else
                current = source.Span[index++];
#endif
                return true;
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
        }
    }

    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct ListStructEnumerable<T>(List<T> source) : IStructEnumerable<T>
    {
        bool isInit = false;
        List<T>.Enumerator enumerator;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = source.Count;
            return true;
        }

        public bool TryGetNext(out T current)
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

            Unsafe.SkipInit(out current);
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

#if NET9_0_OR_GREATER

    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public ref struct SpanStructEnumerable<T>(ReadOnlySpan<T> source) : IStructEnumerable<T>
    {
        ReadOnlySpan<T> source = source;
        int index;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = source.Length;
            return false;
        }

        public bool TryGetNext(out T current)
        {
            if (index < source.Length)
            {
                current = source[index++];
                return true;
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
        }
    }

#endif
}