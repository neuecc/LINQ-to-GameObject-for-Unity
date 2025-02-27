#if NET9_0_OR_GREATER

// TODO: is slow... remove this.

using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using System.Text;

namespace ZLinq
{
    public static class _TODO
    {
        public static VectorizedSpanValueEnumerable<T> VectorizedSelect<T>(this SpanValueEnumerable<T> source, Func<Vector<T>, Vector<T>> selector)
            where T : unmanaged
            => new(source, selector);
    }
}

namespace ZLinq.Linq
{
    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public ref struct VectorizedSpanValueEnumerable<T> : IValueEnumerable<T>
        where T : unmanaged
    {
        readonly Func<Vector<T>, Vector<T>> selector;
        readonly ref T head;
        readonly int sourceLength;

        Vector<T> currentVector;
        int index = 0;
        int indexInVector = -1;
        int indexInVectorTo = Vector<T>.Count;

        public VectorizedSpanValueEnumerable(SpanValueEnumerable<T> source, Func<Vector<T>, Vector<T>> selector)
        {
            source.TryGetSpan(out var sourceSpan); // always success
            this.head = ref MemoryMarshal.GetReference(sourceSpan);
            this.sourceLength = sourceSpan.Length;
            this.selector = selector;
        }

        public ValueEnumerator<VectorizedSpanValueEnumerable<T>, T> GetEnumerator() => new(this);

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = sourceLength;
            return true;
        }

        public bool TryGetSpan(out ReadOnlySpan<T> span)
        {
            span = default;
            return false;
        }

        public bool TryGetNext(out T current)
        {
        BEGIN:
            if (indexInVector == -2) goto END;
            if (indexInVector != -1)
            {
                current = currentVector[indexInVector++];
                if (indexInVector == indexInVectorTo)
                {
                    if (indexInVectorTo == Vector<T>.Count)
                    {
                        indexInVector = -1;
                    }
                    else
                    {
                        indexInVector = -2; // end
                    }
                }
                return true;
            }

            if (index <= (sourceLength - Vector<T>.Count))
            {
                var vector = Vector.LoadUnsafe(ref head, (nuint)index);
                currentVector = selector(vector);
                indexInVector = 0;
                index += Vector<T>.Count;
                goto BEGIN;
            }

            if (index < sourceLength)
            {
                Span<T> span = stackalloc T[Vector<T>.Count];
                MemoryMarshal.CreateReadOnlySpan(ref Unsafe.Add(ref head, index), sourceLength - index).CopyTo(span);
                var vector = new Vector<T>(span);
                currentVector = selector(vector);
                indexInVector = 0;
                indexInVectorTo = sourceLength - index;
                index = sourceLength; // load completed.
                goto BEGIN;
            }

        END:
            indexInVector = -2;
            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
        }
    }
}

#endif