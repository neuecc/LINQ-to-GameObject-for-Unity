#if NET8_0_OR_GREATER
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.Intrinsics;
using System.Text;

namespace ZLinq.Simd;


public interface IVectorizable
{
    void Any();
}

public static class VectorizableExtensions
{
    public static Vectorizable<T> AsVectorizable<T>(this T[] source) where T : unmanaged => new Vectorizable<T>(source);
    public static Vectorizable<T> AsVectorizable<T>(this Span<T> source) where T : unmanaged => new Vectorizable<T>(source);
    public static Vectorizable<T> AsVectorizable<T>(this ReadOnlySpan<T> source) where T : unmanaged => new Vectorizable<T>(source);
}

public enum VectorBoundaryMode
{
    /// <summary>Zero-fill elements beyond boundary</summary>
    ZeroPadding,
    /// <summary>Process final elements with overlap, when data is small, zero-padding</summary>
    OverlapOrZeroPadding,
    /// <summary>Process final elements with overlap, when data is small, throws exception</summary>
    OverlapOrThrow,
}

public ref struct Vectorizable<T>(ReadOnlySpan<T> span)
    where T : unmanaged
{
    ReadOnlySpan<T> span = span;

    // TODO: Select -> ToArray?


    public bool Any(Func<Vector<T>, bool> predicate, VectorBoundaryMode mode)
    {
        if (span.Length == 0) return false;

        // not check Vector.IsHardwareAccelerated(only uses Vector predicate), if not, uses simulation
        ref var head = ref MemoryMarshal.GetReference(span);

        // loop based official vectorize guideline: https://github.com/dotnet/runtime/blob/main/docs/coding-guidelines/vectorization-guidelines.md
        nuint elementOffset = 0;
        nuint oneVectorAwayFromEnd = (nuint)(span.Length - Vector<T>.Count);
        if (span.Length >= Vector<T>.Count)
        {
            for (; elementOffset <= oneVectorAwayFromEnd; elementOffset += (nuint)Vector<T>.Count)
            {
                var data = Vector.LoadUnsafe(ref head, elementOffset);
                if (predicate(data))
                {
                    return true;
                }
            }
        }

        return AnyProcessRemaining(span, elementOffset, oneVectorAwayFromEnd, predicate, mode);
    }

    // Check if we have remaining elements to process
    static bool AnyProcessRemaining(ReadOnlySpan<T> span, nuint elementOffset, nuint oneVectorAwayFromEnd, Func<Vector<T>, bool> predicate, VectorBoundaryMode mode)
    {
        if (elementOffset != (uint)span.Length)
        {
            Vector<T> lastVector;

            // Small span check (span.Length < Vector<T>.Count)
            if (mode == VectorBoundaryMode.OverlapOrThrow && span.Length < Vector<T>.Count)
            {
                Throws.VectorSmallOverlap<T>();
            }

            // For both ZeroPadding and OverlapOrZeroPadding with small spans, we use zero padding
            if (mode == VectorBoundaryMode.ZeroPadding || span.Length < Vector<T>.Count)
            {
                // Create zero-padded vector
                Span<T> tempSpan = stackalloc T[Vector<T>.Count];
                span.Slice((int)elementOffset).CopyTo(tempSpan);
                lastVector = new Vector<T>(tempSpan);
            }
            else // OverlapOrZeroPadding or OverlapOrThrow with sufficient elements
            {
                // Overlap - load from the last complete vector position
                lastVector = Vector.LoadUnsafe(ref MemoryMarshal.GetReference(span), oneVectorAwayFromEnd);
            }

            if (predicate(lastVector))
            {
                return true;
            }
        }

        return false;
    }

    public bool Any(Func<Vector<T>, bool> predicate1, Func<T, bool> predicate2)
    {
        if (span.Length == 0) return false;

        ref var head = ref MemoryMarshal.GetReference(span);

        nuint elementOffset = 0;
        if (Vector.IsHardwareAccelerated && span.Length >= Vector<T>.Count)
        {
            nuint oneVectorAwayFromEnd = (nuint)(span.Length - Vector<T>.Count);
            if (span.Length >= Vector<T>.Count)
            {
                for (; elementOffset <= oneVectorAwayFromEnd; elementOffset += (nuint)Vector<T>.Count)
                {
                    var data = Vector.LoadUnsafe(ref head, elementOffset);
                    if (predicate1(data))
                    {
                        return true;
                    }
                }
            }
        }

        // remaining
        while (elementOffset < (nuint)span.Length)
        {
            if (predicate2(span[(int)elementOffset]))
            {
                return true;
            }
            elementOffset++;
        }

        return false;
    }

    // TODO: make All...


    public bool Any2(Func<Vector<T>, bool> predicate1, Func<T, bool> predicate2)
    {
        ref var current = ref MemoryMarshal.GetReference(span);
        ref var end = ref Unsafe.Add(ref current, span.Length);

        if (Vector.IsHardwareAccelerated && span.Length >= Vector<T>.Count)
        {
            ref var to = ref Unsafe.Subtract(ref end, Vector<T>.Count);
            do
            {
                var data = Vector.LoadUnsafe(ref current);
                if (predicate1(data))
                {
                    return true;
                }
                current = ref Unsafe.Add(ref current, Vector<T>.Count);
            } while (!Unsafe.IsAddressGreaterThan(ref current, ref to));
        }

        // iterate rest
        while (Unsafe.IsAddressLessThan(ref current, ref end))
        {
            if (predicate2(current))
            {
                return true;
            }
            current = ref Unsafe.Add(ref current, 1);
        }

        return false;
    }
}

#endif