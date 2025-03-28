using System.Buffers;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ZLinq.Internal;

// similar as IBufferWriter style to avoid Add frequently.
[StructLayout(LayoutKind.Auto)]
internal ref struct SegmentedArrayBuilder<T>
{
    const int ArrayMaxLength = 0X7FFFFFC7;

    Span<T> currentSegment;
    int countInCurrentSegment;

    Span<T> initialBuffer;
    InlineArray27<T[]> segments; // rented buffers
    int segmentsCount;
    int countInFinishedSegments;

    public int Count => checked(countInFinishedSegments + countInCurrentSegment);

    public SegmentedArrayBuilder(Span<T> initialBuffer)
    {
        this.initialBuffer = this.currentSegment = initialBuffer;
    }

    public Span<T> GetSpan()
    {
        var span = currentSegment;
        var index = countInCurrentSegment;
        if ((uint)index < (uint)span.Length)
        {
            return span.Slice(index);
        }
        else
        {
            Expand();
            return currentSegment;
        }
    }

    public void Advance(int count)
    {
        countInCurrentSegment += count;
    }

    void Expand()
    {
        var currentSegmentLength = currentSegment.Length;
        checked { countInFinishedSegments += currentSegmentLength; }

        if (countInFinishedSegments > ArrayMaxLength)
        {
            throw new OutOfMemoryException();
        }

        var newSegmentLength = (int)Math.Min(currentSegmentLength * 2L, ArrayMaxLength);
        currentSegment =
#if NET8_0_OR_GREATER
            segments[segmentsCount]
#else
            InlineArrayMarshal.ElementRef<InlineArray27<T[]>, T[]>(ref segments, segmentsCount)
#endif
            = ArrayPool<T>.Shared.Rent(newSegmentLength);
        countInCurrentSegment = 0;
        segmentsCount++;
    }

    public void CopyToAndClear(Span<T> destination)
    {
        int segmentIndex = segmentsCount;
        if (segmentIndex != 0)
        {
            var first = initialBuffer;
            first.CopyTo(destination);
            destination = destination.Slice(first.Length);

            segmentIndex--;
            if (segmentIndex != 0) // skip for last
            {
#if NET8_0_OR_GREATER
                ReadOnlySpan<T[]> segmentSpan = ((ReadOnlySpan<T[]>)segments).Slice(0, segmentIndex);
#else
                ReadOnlySpan<T[]> segmentSpan = InlineArrayMarshal.AsSpan<InlineArray27<T[]>, T[]>(ref segments, segmentIndex);
#endif
                foreach (var array in segmentSpan)
                {
                    // copy full segments
                    var segment = array.AsSpan();
                    segment.CopyTo(destination);
                    destination = destination.Slice(segment.Length);

                    // return to pool
                    ArrayPool<T>.Shared.Return(array, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
                }
            }

            // copy current(last) buffer
#if NET8_0_OR_GREATER
            var lastSegment = segments[segmentIndex];
#else
            var lastSegment = InlineArrayMarshal.ElementRef<InlineArray27<T[]>, T[]>(ref segments, segmentIndex);
#endif
            lastSegment.AsSpan(0, countInCurrentSegment).CopyTo(destination);
            ArrayPool<T>.Shared.Return(lastSegment, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
        }
        else
        {
            // only copy initial buffer
            currentSegment.Slice(0, countInCurrentSegment).CopyTo(destination);
        }
    }
}

#if NET8_0_OR_GREATER

[InlineArray(16)]
internal struct InlineArray16<T>
{
    T item;
}

#else

[StructLayout(LayoutKind.Sequential)]
internal struct InlineArray16<T>
{
    T item0;
    T item1;
    T item2;
    T item3;
    T item4;
    T item5;
    T item6;
    T item7;
    T item8;
    T item9;
    T item10;
    T item11;
    T item12;
    T item13;
    T item14;
    T item15;
}

#endif


#if NET8_0_OR_GREATER

[InlineArray(27)]
internal struct InlineArray27<T>
{
    T item;
}

#else

[StructLayout(LayoutKind.Sequential)]
internal struct InlineArray27<T>
{
    T item0;
    T item1;
    T item2;
    T item3;
    T item4;
    T item5;
    T item6;
    T item7;
    T item8;
    T item9;
    T item10;
    T item11;
    T item12;
    T item13;
    T item14;
    T item15;
    T item16;
    T item17;
    T item18;
    T item19;
    T item20;
    T item21;
    T item22;
    T item23;
    T item24;
    T item25;
}

#endif


#if !NET8_0_OR_GREATER

internal static class InlineArrayMarshal
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Span<TElement> AsSpan<TBuffer, TElement>(ref TBuffer buffer, int length)
    {
#if !NETSTANDARD2_0
        return MemoryMarshal.CreateSpan(ref Unsafe.As<TBuffer, TElement>(ref buffer), length);
#else
        unsafe
        {
            // TODO: how implement?
            throw new NotImplementedException();
        }
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ref TElement FirstElementRef<TBuffer, TElement>(ref TBuffer buffer)
    {
        return ref Unsafe.As<TBuffer, TElement>(ref buffer);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ref TElement ElementRef<TBuffer, TElement>(ref TBuffer buffer, int index)
    {
        return ref Unsafe.Add(ref Unsafe.As<TBuffer, TElement>(ref buffer), index);
    }
}

#endif
