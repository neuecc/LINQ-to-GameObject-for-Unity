using System;
using System.Buffers;
#if NET8_0_OR_GREATER
using System.Collections.Immutable;
using System.Numerics;
#endif

namespace ZLinq
{
    public static partial class ValueEnumerable
    {
        public static ValueEnumerable<FromEnumerable<T>, T> AsValueEnumerable<T>(this IEnumerable<T> source)
        {
            return new(new(source));
        }

        public static ValueEnumerable<FromArray<T>, T> AsValueEnumerable<T>(this T[] source)
        {
            return new(new(source));
        }

        public static ValueEnumerable<FromList<T>, T> AsValueEnumerable<T>(this List<T> source)
        {
            return new(new(source));
        }

        public static ValueEnumerable<FromMemory<T>, T> AsValueEnumerable<T>(this ArraySegment<T> source)
        {
            return new(new(source));
        }

        public static ValueEnumerable<FromMemory<T>, T> AsValueEnumerable<T>(this Memory<T> source)
        {
            return new(new(source));
        }

        public static ValueEnumerable<FromMemory<T>, T> AsValueEnumerable<T>(this ReadOnlyMemory<T> source)
        {
            return new(new(source));
        }

        public static ValueEnumerable<FromReadOnlySequence<T>, T> AsValueEnumerable<T>(this ReadOnlySequence<T> source)
        {
            return new(new(source));
        }

        // for System.Collections.Generic

        public static ValueEnumerable<FromDictionary<TKey, TValue>, KeyValuePair<TKey, TValue>> AsValueEnumerable<TKey, TValue>(this Dictionary<TKey, TValue> source)
            where TKey : notnull
        {
            return new(new(source));
        }

        public static ValueEnumerable<FromQueue<T>, T> AsValueEnumerable<T>(this Queue<T> source)
        {
            return new(new(source));
        }

        public static ValueEnumerable<FromStack<T>, T> AsValueEnumerable<T>(this Stack<T> source)
        {
            return new(new(source));
        }

        public static ValueEnumerable<FromLinkedList<T>, T> AsValueEnumerable<T>(this LinkedList<T> source)
        {
            return new(new(source));
        }

        public static ValueEnumerable<FromHashSet<T>, T> AsValueEnumerable<T>(this HashSet<T> source)
        {
            return new(new(source));
        }

#if NET8_0_OR_GREATER

        public static ValueEnumerable<FromImmutableArray<T>, T> AsValueEnumerable<T>(this ImmutableArray<T> source)
        {
            return new(new(source));
        }

#endif

#if NET9_0_OR_GREATER

        public static ValueEnumerable<FromSpan<T>, T> AsValueEnumerable<T>(this Span<T> source)
        {
            return new(new(source));
        }

        public static ValueEnumerable<FromSpan<T>, T> AsValueEnumerable<T>(this ReadOnlySpan<T> source)
        {
            return new(new(source));
        }

#endif

    }
}

namespace ZLinq.Linq
{
    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct FromEnumerable<T>(IEnumerable<T> source) : IValueEnumerator<T>
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

        public bool TryGetSpan(out ReadOnlySpan<T> span)
        {
            if (source.GetType() == typeof(T[]))
            {
                span = Unsafe.As<T[]>(source);
                return true;
            }
            else if (source.GetType() == typeof(List<T>))
            {
                span = CollectionsMarshal.AsSpan(Unsafe.As<List<T>>(source));
                return true;
            }
            else
            {
                span = default;
                return false;
            }
        }

        public bool TryCopyTo(Span<T> destination)
        {
            // TODO: length validation
            if (source.GetType() == typeof(T[]))
            {
                Unsafe.As<T[]>(source).CopyTo(destination);
                return true;
            }
            else if (source.GetType() == typeof(List<T>))
            {
                CollectionsMarshal.AsSpan(Unsafe.As<List<T>>(source)).CopyTo(destination);
                return true;
            }
            else
            {
                return false;
            }
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
            if (enumerator != null)
            {
                enumerator.Dispose();
            }
        }
    }

    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct FromArray<T>(T[] source) : IValueEnumerator<T>
    {
        int index;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = source.Length;
            return true;
        }

        public bool TryGetSpan(out ReadOnlySpan<T> span)
        {
            span = source.AsSpan();
            return true;
        }

        public bool TryCopyTo(Span<T> destination)
        {
            // TODO: length validation
            source.CopyTo(destination);
            return true;
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
#if NET9_0_OR_GREATER
    public ref
#else
    public
#endif
    struct FromMemory<T>(ReadOnlyMemory<T> source) : IValueEnumerator<T>
    {
#if NET9_0_OR_GREATER
        ReadOnlySpan<T> source = source.Span;
#endif

        int index;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = source.Length;
            return true;
        }

        public bool TryCopyTo(Span<T> destination)
        {
#if NET9_0_OR_GREATER
            var span = source;
#else
            var span = source.Span;
#endif
            // TODO: length validation
            span.CopyTo(destination);
            return true;
        }

        public bool TryGetSpan(out ReadOnlySpan<T> span)
        {
#if NET9_0_OR_GREATER
            span = source;
#else
            span = source.Span;
#endif
            return true;
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
    public struct FromList<T>(List<T> source) : IValueEnumerator<T>
    {
        bool isInit = false;
        List<T>.Enumerator enumerator;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = source.Count;
            return true;
        }

        public bool TryGetSpan(out ReadOnlySpan<T> span)
        {
            span = CollectionsMarshal.AsSpan(source);
            return true;
        }

        public bool TryCopyTo(Span<T> destination)
        {
            // TODO: length validation
            CollectionsMarshal.AsSpan(source).CopyTo(destination);
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

    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct FromDictionary<TKey, TValue>(Dictionary<TKey, TValue> source) : IValueEnumerator<KeyValuePair<TKey, TValue>>
        where TKey : notnull
    {
        bool isInit = false;
        Dictionary<TKey, TValue>.Enumerator enumerator;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = source.Count;
            return true;
        }

        public bool TryGetSpan(out ReadOnlySpan<KeyValuePair<TKey, TValue>> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(Span<KeyValuePair<TKey, TValue>> destination) => false;

        public bool TryGetNext(out KeyValuePair<TKey, TValue> current)
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

    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
#if NET9_0_OR_GREATER
    public ref
#else
    public
#endif
    struct FromReadOnlySequence<T>(ReadOnlySequence<T> source) : IValueEnumerator<T>
    {
        bool isInit = false;
        ReadOnlySequence<T>.Enumerator sequenceEnumerator;
        FromMemory<T> enumerator;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = checked((int)source.Length);
            return true;
        }

        public bool TryGetSpan(out ReadOnlySpan<T> span)
        {
            if (source.IsSingleSegment)
            {
                span = source.First.Span;
                return true;
            }

            span = default;
            return false;
        }

        public bool TryCopyTo(Span<T> destination)
        {
            if (source.IsSingleSegment)
            {
                // TODO: length validation
                var span = source.First.Span;
                span.CopyTo(destination);
                return true;
            }
            return false;
        }

        public bool TryGetNext(out T current)
        {
            if (!isInit)
            {
                isInit = true;
                sequenceEnumerator = source.GetEnumerator();
            }

        MOVE_NEXT:
            if (enumerator.TryGetNext(out current))
            {
                return true;
            }

            if (sequenceEnumerator.MoveNext())
            {
                enumerator = sequenceEnumerator.Current.AsValueEnumerable().Enumerator;
                goto MOVE_NEXT;
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
            // no needs FromMemory<T>.Dispose
        }
    }

    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct FromQueue<T>(Queue<T> source) : IValueEnumerator<T>
    {
        bool isInit;
        Queue<T>.Enumerator enumerator;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = source.Count;
            return true;
        }

        public bool TryGetSpan(out ReadOnlySpan<T> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(Span<T> destination) => false;

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

    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct FromStack<T>(Stack<T> source) : IValueEnumerator<T>
    {
        bool isInit;
        Stack<T>.Enumerator enumerator;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = source.Count;
            return true;
        }

        public bool TryGetSpan(out ReadOnlySpan<T> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(Span<T> destination) => false;

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

    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct FromLinkedList<T>(LinkedList<T> source) : IValueEnumerator<T>
    {
        bool isInit;
        LinkedList<T>.Enumerator enumerator;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = source.Count;
            return true;
        }

        public bool TryGetSpan(out ReadOnlySpan<T> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(Span<T> destination) => false;

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

    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct FromHashSet<T>(HashSet<T> source) : IValueEnumerator<T>
    {
        bool isInit;
        HashSet<T>.Enumerator enumerator;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = source.Count;
            return true;
        }

        public bool TryGetSpan(out ReadOnlySpan<T> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(Span<T> destination) => false;

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

#if NET8_0_OR_GREATER

    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct FromImmutableArray<T>(ImmutableArray<T> source) : IValueEnumerator<T>
    {
        bool isInit = false;
        ImmutableArray<T>.Enumerator enumerator;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = source.Length;
            return true;
        }

        public bool TryGetSpan(out ReadOnlySpan<T> span)
        {
            span = source.AsSpan();
            return true;
        }

        public bool TryCopyTo(Span<T> destination)
        {
            // TODO:length validation
            source.AsSpan().CopyTo(destination);
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
        }
    }

#endif

#if NET9_0_OR_GREATER

    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public ref struct FromSpan<T>(ReadOnlySpan<T> source) : IValueEnumerator<T>
    {
        ReadOnlySpan<T> source = source;
        int index;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = source.Length;
            return true;
        }

        public bool TryGetSpan(out ReadOnlySpan<T> span)
        {
            span = source;
            return true;
        }

        public bool TryCopyTo(Span<T> destination)
        {
            // TODO:length validation
            source.CopyTo(destination);
            return true;
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