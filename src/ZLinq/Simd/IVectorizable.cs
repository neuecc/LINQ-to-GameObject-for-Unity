#if NET8_0_OR_GREATER
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace ZLinq.Simd;


public interface IVectorizable
{
    void Any();
}

public static class VectorizableExtensions
{
    public static Vectorizable<T> AsVectorizable<T>(this T[] source) where T : struct => new Vectorizable<T>(source);
    public static Vectorizable<T> AsVectorizable<T>(this Span<T> source) where T : struct => new Vectorizable<T>(source);
    public static Vectorizable<T> AsVectorizable<T>(this ReadOnlySpan<T> source) where T : struct => new Vectorizable<T>(source);
}

public ref struct Vectorizable<T>(ReadOnlySpan<T> span)
    where T : struct
{
    ReadOnlySpan<T> span = span;

    // TODO: Select -> ToArray?

    public bool Any<TState>(TState state, Func<Vector<T>, TState, bool> predicate1, Func<T, bool> predicate2)
    {
        ref var current = ref MemoryMarshal.GetReference(span);
        ref var end = ref Unsafe.Add(ref current, span.Length);

        if (Vector.IsHardwareAccelerated && span.Length >= Vector<T>.Count)
        {
            ref var to = ref Unsafe.Subtract(ref end, Vector<T>.Count);
            do
            {
                var data = Vector.LoadUnsafe(ref current);
                if (predicate1(data, state))
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

    public bool All<TState>(TState state, Func<Vector<T>, TState, bool> predicate1, Func<T, bool> predicate2)
    {
        ref var current = ref MemoryMarshal.GetReference(span);
        ref var end = ref Unsafe.Add(ref current, span.Length);

        if (Vector.IsHardwareAccelerated && span.Length >= Vector<T>.Count)
        {
            ref var to = ref Unsafe.Subtract(ref end, Vector<T>.Count);
            do
            {
                var data = Vector.LoadUnsafe(ref current);
                if (!predicate1(data, state))
                {
                    return false;
                }
                current = ref Unsafe.Add(ref current, Vector<T>.Count);
            } while (!Unsafe.IsAddressGreaterThan(ref current, ref to));
        }

        // iterate rest
        while (Unsafe.IsAddressLessThan(ref current, ref end))
        {
            if (!predicate2(current))
            {
                return false;
            }
            current = ref Unsafe.Add(ref current, 1);
        }

        return true;
    }
}

#endif