using System.Buffers;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace ZLinq.Internal;

[StructLayout(LayoutKind.Auto)]
internal ref struct SegmentedArrayBuilder<T> : IDisposable
{
    // Array.MaxLength = 2147483591
    T[]? array0;  // 16              total:16
    T[]? array1;  // 32              total:48
    T[]? array2;  // 64              total:112
    T[]? array3;  // 128             total:240
    T[]? array4;  // 256             total:496
    T[]? array5;  // 512             total:1008
    T[]? array6;  // 1024            total:2032
    T[]? array7;  // 2048            total:4080
    T[]? array8;  // 4096            total:8176
    T[]? array9;  // 8192            total:16368
    T[]? array10; // 16384           total:32752
    T[]? array11; // 32768           total:65520
    T[]? array12; // 65536           total:131056
    T[]? array13; // 131072          total:262128
    T[]? array14; // 262144          total:524272
    T[]? array15; // 524288          total:1048560
    T[]? array16; // 1048576         total:2097136
    T[]? array17; // 2097152         total:4194288
    T[]? array18; // 4194304         total:8388592
    T[]? array19; // 8388608         total:16777200
    T[]? array20; // 16777216        total:33554416
    T[]? array21; // 33554432        total:67108848
    T[]? array22; // 67108864        total:134217712
    T[]? array23; // 134217728       total:268435440
    T[]? array24; // 268435456       total:536870896
    T[]? array25; // 536870912       total:1073741808
    T[]? array26; // 1073741824      total:2147483632 (over)

    T[]? currentSegment;
    int indexInSegment;
    int segmentIndex;
    int count;

    public int Count => count;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(T value)
    {
        ref var segment = ref currentSegment;

        if (segment == null)
        {
            Init(value);
            return;
        }
        else if (segment.Length == indexInSegment)
        {
            SlowAdd(value);
            return;
        }

        segment[indexInSegment++] = value;
        count++;
    }

    void Init(T value)
    {
        ref var segment = ref array0;
        segment = ArrayPool<T>.Shared.Rent(16);
        currentSegment = segment;

        segment[indexInSegment++] = value;
        count++;
    }

    void SlowAdd(T value)
    {
        ref var segment = ref currentSegment;
        segmentIndex++;

        switch (segmentIndex)
        {
            case 0: { segment = ref array0; break; }
            case 1: { segment = ref array1; break; }
            case 2: { segment = ref array2; break; }
            case 3: { segment = ref array3; break; }
            case 4: { segment = ref array4; break; }
            case 5: { segment = ref array5; break; }
            case 6: { segment = ref array6; break; }
            case 7: { segment = ref array7; break; }
            case 8: { segment = ref array8; break; }
            case 9: { segment = ref array9; break; }
            case 10: { segment = ref array10; break; }
            case 11: { segment = ref array11; break; }
            case 12: { segment = ref array12; break; }
            case 13: { segment = ref array13; break; }
            case 14: { segment = ref array14; break; }
            case 15: { segment = ref array15; break; }
            case 16: { segment = ref array16; break; }
            case 17: { segment = ref array17; break; }
            case 18: { segment = ref array18; break; }
            case 19: { segment = ref array19; break; }
            case 20: { segment = ref array20; break; }
            case 21: { segment = ref array21; break; }
            case 22: { segment = ref array22; break; }
            case 23: { segment = ref array23; break; }
            case 24: { segment = ref array24; break; }
            case 25: { segment = ref array25; break; }
            case 26: { segment = ref array26; break; }
            default: break;
        }

        segment = ArrayPool<T>.Shared.Rent(indexInSegment * 2);
        indexInSegment = 0;
        currentSegment = segment;

        segment[indexInSegment++] = value;
        count++;
    }

    public void CopyTo(Span<T> dest)
    {
        if (count == 0) return;

        for (int i = 0; i <= segmentIndex; i++)
        {
            T[] segment = default!;
            switch (i)
            {
                case 0: { segment = array0!; break; }
                case 1: { segment = array1!; break; }
                case 2: { segment = array2!; break; }
                case 3: { segment = array3!; break; }
                case 4: { segment = array4!; break; }
                case 5: { segment = array5!; break; }
                case 6: { segment = array6!; break; }
                case 7: { segment = array7!; break; }
                case 8: { segment = array8!; break; }
                case 9: { segment = array9!; break; }
                case 10: { segment = array10!; break; }
                case 11: { segment = array11!; break; }
                case 12: { segment = array12!; break; }
                case 13: { segment = array13!; break; }
                case 14: { segment = array14!; break; }
                case 15: { segment = array15!; break; }
                case 16: { segment = array16!; break; }
                case 17: { segment = array17!; break; }
                case 18: { segment = array18!; break; }
                case 19: { segment = array19!; break; }
                case 20: { segment = array20!; break; }
                case 21: { segment = array21!; break; }
                case 22: { segment = array22!; break; }
                case 23: { segment = array23!; break; }
                case 24: { segment = array24!; break; }
                case 25: { segment = array25!; break; }
                case 26: { segment = array26!; break; }
                default: break;
            }

            if (segmentIndex != i)
            {
                // copy full
                segment.AsSpan().CopyTo(dest);
                dest = dest.Slice(segment.Length);
            }
            else
            {
                // last
                segment.AsSpan(0, indexInSegment).CopyTo(dest);
            }
        }
    }

    public T[] ToArray()
    {
        if (count == 0) return Array.Empty<T>();

        var array = GC.AllocateUninitializedArray<T>(count);
        CopyTo(array);
        return array;
    }

    public void Dispose()
    {
        if (currentSegment == null) return;

        for (int i = 0; i <= segmentIndex; i++)
        {
            ref T[]? segment = ref currentSegment;
            switch (segmentIndex)
            {
                case 0: { segment = ref array0; break; }
                case 1: { segment = ref array1; break; }
                case 2: { segment = ref array2; break; }
                case 3: { segment = ref array3; break; }
                case 4: { segment = ref array4; break; }
                case 5: { segment = ref array5; break; }
                case 6: { segment = ref array6; break; }
                case 7: { segment = ref array7; break; }
                case 8: { segment = ref array8; break; }
                case 9: { segment = ref array9; break; }
                case 10: { segment = ref array10; break; }
                case 11: { segment = ref array11; break; }
                case 12: { segment = ref array12; break; }
                case 13: { segment = ref array13; break; }
                case 14: { segment = ref array14; break; }
                case 15: { segment = ref array15; break; }
                case 16: { segment = ref array16; break; }
                case 17: { segment = ref array17; break; }
                case 18: { segment = ref array18; break; }
                case 19: { segment = ref array19; break; }
                case 20: { segment = ref array20; break; }
                case 21: { segment = ref array21; break; }
                case 22: { segment = ref array22; break; }
                case 23: { segment = ref array23; break; }
                case 24: { segment = ref array24; break; }
                case 25: { segment = ref array25; break; }
                case 26: { segment = ref array26; break; }
                default: break;
            }

            if (segment != null)
            {
                ArrayPool<T>.Shared.Return(segment!, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
                segment = null;
            }
        }

        currentSegment = null;
        indexInSegment = segmentIndex = count = 0;
    }
}

// similar as IBufferWriter style.
[StructLayout(LayoutKind.Auto)]
internal ref struct SegmentedArrayBuilder2<T>
{
    const int ArrayMaxLength = 0X7FFFFFC7;

    Span<T> currentSegment;
    int countInCurrentSegment;

    Span<T> initialBuffer;
    InlineArray27<T[]> segments; // rented buffers
    int segmentsCount;
    int countInFinishedSegments;

    public int Count => checked(countInFinishedSegments + countInCurrentSegment);

    public SegmentedArrayBuilder2(Span<T> initialBuffer)
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
        currentSegment = InlineArrayMarshal.ElementRef<InlineArray27<T[]>, T[]>(ref segments, segmentsCount) = ArrayPool<T>.Shared.Rent(newSegmentLength);
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
                foreach (var array in InlineArrayMarshal.AsSpan<InlineArray27<T[]>, T[]>(ref segments, segmentIndex))
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
            var lastSegment = InlineArrayMarshal.ElementRef<InlineArray27<T[]>, T[]>(ref segments, segmentIndex + 1); // ????
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

[StructLayout(LayoutKind.Auto)]
internal ref struct SegmentedArrayBuilder3<T>
{
    const int ArrayMaxLength = 0X7FFFFFC7;

    Span<T> currentSegment;
    int countInCurrentSegment;

    Span<T> initialBuffer;
    TrueInlineArray27<T[]> segments; // rented buffers
    int segmentsCount;
    int countInFinishedSegments;

    public int Count => checked(countInFinishedSegments + countInCurrentSegment);

    public SegmentedArrayBuilder3(Span<T> initialBuffer)
    {
        this.initialBuffer = this.currentSegment = initialBuffer;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void AddRange<TEnumerator>(TEnumerator source)
        where TEnumerator : struct, IValueEnumerator<T>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        Span<T> ___currentSegment = currentSegment;
        int ___countInCurrentSegment = countInCurrentSegment;

        while (source.TryGetNext(out var item))
        {
            if ((uint)___countInCurrentSegment < (uint)___currentSegment.Length)
            {
                ___currentSegment[___countInCurrentSegment] = item;
                ___countInCurrentSegment++;
            }
            else
            {
                Expand();
                ___currentSegment = currentSegment;
                ___currentSegment[0] = item;
                ___countInCurrentSegment = 1;
            }
        }

        countInCurrentSegment = ___countInCurrentSegment;
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
        currentSegment = segments[segmentsCount] = ArrayPool<T>.Shared.Rent(newSegmentLength);
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
                foreach (var array in ((ReadOnlySpan<T[]>)segments).Slice(0, segmentIndex))
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
            var lastSegment = segments[segmentIndex + 1]; // ????
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

[InlineArray(16)]
internal struct TrueInlineArray16<T>
{
    T item;
}

[InlineArray(27)]
internal struct TrueInlineArray27<T>
{
    T item;
}


#endif

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


#if NET9_0_OR_GREATER

/// <summary>Provides a helper for efficiently building arrays and lists.</summary>
/// <remarks>This is implemented as an inline array of rented arrays.</remarks>
/// <typeparam name="T">Specifies the element type of the collection being built.</typeparam>
internal ref struct DotNetSegmentedArrayBuilder<T>
{
    /// <summary>The size to use for the first segment that's stack allocated by the caller.</summary>
    /// <remarks>
    /// This value needs to be small enough that we don't need to be overly concerned about really large
    /// value types. It's not unreasonable for a method to say it has 8 locals of a T, and that's effectively
    /// what this is.
    /// </remarks>
    private const int ScratchBufferSize = 8;
    /// <summary>Minimum size to request renting from the pool.</summary>
    private const int MinimumRentSize = 16;

    /// <summary>The array of segments.</summary>
    /// <remarks><see cref="_segmentsCount"/> is how many of the segments are valid in <see cref="_segments"/>, not including <see cref="_firstSegment"/>.</remarks>
    private Arrays _segments;
    /// <summary>The scratch buffer provided by the caller.</summary>
    /// <remarks>This is treated as the initial segment, before anything in <see cref="_segments"/>.</remarks>
    private Span<T> _firstSegment;
    /// <summary>The current span. This points either to <see cref="_firstSegment"/> or to <see cref="_segments"/>[<see cref="_segmentsCount"/> - 1].</summary>
    private Span<T> _currentSegment;
    /// <summary>The count of segments in <see cref="_segments"/> that are valid.</summary>
    /// <remarks>All but the last are known to be fully populated.</remarks>
    private int _segmentsCount;
    /// <summary>The total number of elements in all but the current/last segment.</summary>
    private int _countInFinishedSegments;
    /// <summary>The number of elements in the current/last segment.</summary>
    private int _countInCurrentSegment;

    /// <summary>Initialize the builder.</summary>
    /// <param name="scratchBuffer">A buffer that can be used as part of the builder.</param>
    public DotNetSegmentedArrayBuilder(Span<T> scratchBuffer)
    {
        _currentSegment = _firstSegment = scratchBuffer;
    }

    /// <summary>Clean up the resources used by the builder.</summary>
    public void Dispose()
    {
        int segmentsCount = _segmentsCount;
        if (segmentsCount != 0)
        {
            ReturnArrays(segmentsCount);
        }
    }

    private void ReturnArrays(int segmentsCount)
    {
        Debug.Assert(segmentsCount > 0);
        ReadOnlySpan<T[]> segments = _segments;

        // We need to return all rented arrays to the pool, and if the arrays contain any references,
        // we want to clear them first so that the pool doesn't artificially root contained objects.
        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
        {
            // Return all but the last segment. All of these are full and need to be entirely cleared.
            segmentsCount--;
            foreach (T[] segment in segments.Slice(0, segmentsCount))
            {
                Array.Clear(segment);
                ArrayPool<T>.Shared.Return(segment);
            }

            // For the last segment, we can clear only what we know was used.
            T[] currentSegment = segments[segmentsCount];
            Array.Clear(currentSegment, 0, _countInCurrentSegment);
            ArrayPool<T>.Shared.Return(currentSegment);
        }
        else
        {
            // Return every rented array without clearing.
            for (int i = 0; i < segments.Length; i++)
            {
                T[] segment = segments[i];
                if (segment is null)
                {
                    break;
                }

                ArrayPool<T>.Shared.Return(segment);
            }
        }
    }

    /// <summary>Gets the number of elements in the builder.</summary>
    public readonly int Count => checked(_countInFinishedSegments + _countInCurrentSegment);

    /// <summary>Adds an item into the builder.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(T item)
    {
        Span<T> currentSegment = _currentSegment;
        int countInCurrentSegment = _countInCurrentSegment;
        if ((uint)countInCurrentSegment < (uint)currentSegment.Length)
        {
            currentSegment[countInCurrentSegment] = item;
            _countInCurrentSegment++;
        }
        else
        {
            AddSlow(item);
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void AddSlow(T item)
    {
        Expand();
        _currentSegment[0] = item;
        _countInCurrentSegment = 1;
    }

    /// <summary>Adds a collection of items into the builder.</summary>
    /// <remarks>
    /// The implementation assumes the caller has already ruled out the source being
    /// and ICollection and thus doesn't bother checking to see if it is.
    /// </remarks>
    [MethodImpl(MethodImplOptions.NoInlining)]
    public void AddNonICollectionRange(IEnumerable<T> source) =>
        AddNonICollectionRangeInlined(source);

    /// <summary>Adds a collection of items into the builder.</summary>
    /// <remarks>
    /// The implementation assumes the caller has already ruled out the source being
    /// and ICollection and thus doesn't bother checking to see if it is.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void AddNonICollectionRangeInlined(IEnumerable<T> source)
    {
        Span<T> currentSegment = _currentSegment;
        int countInCurrentSegment = _countInCurrentSegment;

        foreach (T item in source)
        {
            if ((uint)countInCurrentSegment < (uint)currentSegment.Length)
            {
                currentSegment[countInCurrentSegment] = item;
                countInCurrentSegment++;
            }
            else
            {
                Expand();
                currentSegment = _currentSegment;
                currentSegment[0] = item;
                countInCurrentSegment = 1;
            }
        }

        _countInCurrentSegment = countInCurrentSegment;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void AddNonICollectionRangeInlined<TEnumerator>(ValueEnumerable<TEnumerator, T> source)
        where TEnumerator : struct, IValueEnumerator<T>
        , allows ref struct
    {
        Span<T> currentSegment = _currentSegment;
        int countInCurrentSegment = _countInCurrentSegment;

        using (var e = source.Enumerator)
        {
            while (e.TryGetNext(out var item))
            {
                if ((uint)countInCurrentSegment < (uint)currentSegment.Length)
                {
                    currentSegment[countInCurrentSegment] = item;
                    countInCurrentSegment++;
                }
                else
                {
                    Expand();
                    currentSegment = _currentSegment;
                    currentSegment[0] = item;
                    countInCurrentSegment = 1;
                }
            }
        }

        _countInCurrentSegment = countInCurrentSegment;
    }

    /// <summary>Creates an array containing all of the elements in the builder.</summary>
    public readonly T[] ToArray()
    {
        T[] result;
        int count = Count;

        if (count != 0)
        {
            result = GC.AllocateUninitializedArray<T>(count);
            ToSpanInlined(result);
        }
        else
        {
            result = [];
        }

        return result;
    }

    /// <summary>Creates a list containing all of the elements in the builder.</summary>
    public readonly List<T> ToList()
    {
        List<T> result;
        int count = Count;

        if (count != 0)
        {
            result = new List<T>(count);

            CollectionsMarshal.SetCount(result, count);
            ToSpanInlined(CollectionsMarshal.AsSpan(result));
        }
        else
        {
            result = [];
        }

        return result;
    }

    /// <summary>Creates an array containing all of the elements in the builder.</summary>
    /// <param name="additionalLength">The number of extra elements of room to allocate in the resulting array.</param>
    public readonly T[] ToArray(int additionalLength)
    {
        T[] result;
        int count = checked(Count + additionalLength);

        if (count != 0)
        {
            result = GC.AllocateUninitializedArray<T>(count);
            ToSpanInlined(result);
        }
        else
        {
            result = [];
        }

        return result;
    }

    /// <summary>Populates the destination span with all of the elements in the builder.</summary>
    /// <param name="destination">The destination span.</param>
    [MethodImpl(MethodImplOptions.NoInlining)]
    public readonly void ToSpan(Span<T> destination) => ToSpanInlined(destination);

    /// <summary>Populates the destination span with all of the elements in the builder.</summary>
    /// <param name="destination">The destination span.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private readonly void ToSpanInlined(Span<T> destination)
    {
        int segmentsCount = _segmentsCount;
        if (segmentsCount != 0)
        {
            // Copy the first segment
            ReadOnlySpan<T> firstSegment = _firstSegment;
            firstSegment.CopyTo(destination);
            destination = destination.Slice(firstSegment.Length);

            // Copy the 0..N-1 segments
            segmentsCount--;
            if (segmentsCount != 0)
            {
                foreach (T[] arr in ((ReadOnlySpan<T[]>)_segments).Slice(0, segmentsCount))
                {
                    ReadOnlySpan<T> segment = arr;
                    segment.CopyTo(destination);
                    destination = destination.Slice(segment.Length);
                }
            }
        }

        // Copy the last segment
        _currentSegment.Slice(0, _countInCurrentSegment).CopyTo(destination);
    }

    /// <summary>Appends a new segment onto the builder.</summary>
    /// <param name="minimumRequired">The minimum amount of space to allocate in a new segment being appended.</param>
    private void Expand(int minimumRequired = MinimumRentSize)
    {
        if (minimumRequired < MinimumRentSize)
        {
            minimumRequired = MinimumRentSize;
        }

        // Update our count of the number of elements in the arrays.
        // If we know we're exceeding the maximum allowed array length, throw.
        int currentSegmentLength = _currentSegment.Length;
        checked { _countInFinishedSegments += currentSegmentLength; }
        if (_countInFinishedSegments > Array.MaxLength)
        {
            throw new OutOfMemoryException();
        }

        // Use a typical doubling algorithm to decide the length of the next array
        // and allocate it. We want to double the current array length, but if the
        // minimum required is larger than that, use the minimum required. And if
        // doubling would result in going above the max array length, only use the
        // max array length, as List<T> does.
        int newSegmentLength = (int)Math.Min(Math.Max(minimumRequired, currentSegmentLength * 2L), Array.MaxLength);
        _currentSegment = _segments[_segmentsCount] = ArrayPool<T>.Shared.Rent(newSegmentLength);
        _segmentsCount++;
    }

#pragma warning disable IDE0044 // Add readonly modifier
#pragma warning disable IDE0051 // Remove unused private members
    /// <summary>A struct to hold all of the T[]s that compose the full result set.</summary>
    /// <remarks>
    /// Starting at the minimum size of <see cref="MinimumRentSize"/>, and with a minimum of doubling
    /// on every growth, this is large enough to hold the maximum number arrays that could result
    /// until the total length has exceeded Array.MaxLength.
    /// </remarks>
    [InlineArray(27)]
    private struct Arrays
    {
        private T[] _values;
    }

    /// <summary>Provides a stack-allocatable buffer for use as an argument to the builder.</summary>
    [InlineArray(ScratchBufferSize)]
    public struct ScratchBuffer
    {
        private T _item;
    }
#pragma warning restore IDE0051
#pragma warning restore IDE0044
}


#endif
