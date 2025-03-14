﻿using System;
using System.Numerics;

// System.Linq, int32 Average is specialized
// other than same as Sum, supports  int/long/float/double/decimal and nullable + selector
// ZLinq doesn't support theres but supports INumber.
// returns only double

namespace ZLinq;

partial class ValueEnumerableExtensions
{
    // System.Linq returns float -> float, decimal -> decimal, others(int, long, double) -> double
    // Due to limitations with overloads, generics, and where constraints, the return value is restricted to double only.
    public static double Average<TEnumerable, TSource>(this TEnumerable source)
        where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
        where TSource : struct
#if NET8_0_OR_GREATER
        , INumber<TSource>
#endif
    {
#if NET8_0_OR_GREATER
        using (source)
        {
            if (source.TryGetSpan(out var span))
            {
                if (span.Length == 0)
                {
                    Throws.NoElements();
                }

                if (typeof(TSource) == typeof(int))
                {
                    return AverageIntSimd(UnsafeSpanBitCast<TSource, int>(span));
                }
                else
                {
                    var sum = SumSpan(span);
                    return double.CreateChecked(sum) / (double)span.Length;
                }
            }
            else
            {
                if (!source.TryGetNext(out var sum)) // store first value
                {
                    Throws.NoElements();
                }

                long count = 1;
                while (source.TryGetNext(out var current))
                {
                    checked { sum += TSource.CreateChecked(current); }
                    count++;
                }

                return double.CreateChecked(sum) / (double)count;
            }
        }
#else
        if (typeof(TSource) == typeof(float)) // float is hand-written
        {
            using (source)
            {
                if (!source.TryGetNext(out var current))
                {
                    Throws.NoElements();
                }

                double sum = (double)Unsafe.As<TSource, float>(ref current); // calc as double
                long count = 1;
                while (source.TryGetNext(out current))
                {
                    checked { sum += (double)Unsafe.As<TSource, float>(ref current); }
                    count++;
                }

                return sum / (double)count;
            }
        }
        #region generate from FileGen.Commands.Average
        else if (typeof(TSource) == typeof(byte))
        {
            using (source)
            {
                if (!source.TryGetNext(out var current))
                {
                    Throws.NoElements();
                }

                byte sum = Unsafe.As<TSource, byte>(ref current);
                long count = 1;
                while (source.TryGetNext(out current))
                {
                    checked { sum += Unsafe.As<TSource, byte>(ref current); }
                    count++;
                }

                return (double)sum / (double)count;
            }
        }
        else if (typeof(TSource) == typeof(sbyte))
        {
            using (source)
            {
                if (!source.TryGetNext(out var current))
                {
                    Throws.NoElements();
                }

                sbyte sum = Unsafe.As<TSource, sbyte>(ref current);
                long count = 1;
                while (source.TryGetNext(out current))
                {
                    checked { sum += Unsafe.As<TSource, sbyte>(ref current); }
                    count++;
                }

                return (double)sum / (double)count;
            }
        }
        else if (typeof(TSource) == typeof(short))
        {
            using (source)
            {
                if (!source.TryGetNext(out var current))
                {
                    Throws.NoElements();
                }

                short sum = Unsafe.As<TSource, short>(ref current);
                long count = 1;
                while (source.TryGetNext(out current))
                {
                    checked { sum += Unsafe.As<TSource, short>(ref current); }
                    count++;
                }

                return (double)sum / (double)count;
            }
        }
        else if (typeof(TSource) == typeof(ushort))
        {
            using (source)
            {
                if (!source.TryGetNext(out var current))
                {
                    Throws.NoElements();
                }

                ushort sum = Unsafe.As<TSource, ushort>(ref current);
                long count = 1;
                while (source.TryGetNext(out current))
                {
                    checked { sum += Unsafe.As<TSource, ushort>(ref current); }
                    count++;
                }

                return (double)sum / (double)count;
            }
        }
        else if (typeof(TSource) == typeof(int))
        {
            using (source)
            {
                if (!source.TryGetNext(out var current))
                {
                    Throws.NoElements();
                }

                int sum = Unsafe.As<TSource, int>(ref current);
                long count = 1;
                while (source.TryGetNext(out current))
                {
                    checked { sum += Unsafe.As<TSource, int>(ref current); }
                    count++;
                }

                return (double)sum / (double)count;
            }
        }
        else if (typeof(TSource) == typeof(uint))
        {
            using (source)
            {
                if (!source.TryGetNext(out var current))
                {
                    Throws.NoElements();
                }

                uint sum = Unsafe.As<TSource, uint>(ref current);
                long count = 1;
                while (source.TryGetNext(out current))
                {
                    checked { sum += Unsafe.As<TSource, uint>(ref current); }
                    count++;
                }

                return (double)sum / (double)count;
            }
        }
        else if (typeof(TSource) == typeof(long))
        {
            using (source)
            {
                if (!source.TryGetNext(out var current))
                {
                    Throws.NoElements();
                }

                long sum = Unsafe.As<TSource, long>(ref current);
                long count = 1;
                while (source.TryGetNext(out current))
                {
                    checked { sum += Unsafe.As<TSource, long>(ref current); }
                    count++;
                }

                return (double)sum / (double)count;
            }
        }
        else if (typeof(TSource) == typeof(ulong))
        {
            using (source)
            {
                if (!source.TryGetNext(out var current))
                {
                    Throws.NoElements();
                }

                ulong sum = Unsafe.As<TSource, ulong>(ref current);
                long count = 1;
                while (source.TryGetNext(out current))
                {
                    checked { sum += Unsafe.As<TSource, ulong>(ref current); }
                    count++;
                }

                return (double)sum / (double)count;
            }
        }
        else if (typeof(TSource) == typeof(double))
        {
            using (source)
            {
                if (!source.TryGetNext(out var current))
                {
                    Throws.NoElements();
                }

                double sum = Unsafe.As<TSource, double>(ref current);
                long count = 1;
                while (source.TryGetNext(out current))
                {
                    checked { sum += Unsafe.As<TSource, double>(ref current); }
                    count++;
                }

                return (double)sum / (double)count;
            }
        }
        else if (typeof(TSource) == typeof(decimal))
        {
            using (source)
            {
                if (!source.TryGetNext(out var current))
                {
                    Throws.NoElements();
                }

                decimal sum = Unsafe.As<TSource, decimal>(ref current);
                long count = 1;
                while (source.TryGetNext(out current))
                {
                    checked { sum += Unsafe.As<TSource, decimal>(ref current); }
                    count++;
                }

                return (double)sum / (double)count;
            }
        }
        else if (typeof(TSource) == typeof(nint))
        {
            using (source)
            {
                if (!source.TryGetNext(out var current))
                {
                    Throws.NoElements();
                }

                nint sum = Unsafe.As<TSource, nint>(ref current);
                long count = 1;
                while (source.TryGetNext(out current))
                {
                    checked { sum += Unsafe.As<TSource, nint>(ref current); }
                    count++;
                }

                return (double)sum / (double)count;
            }
        }
        else if (typeof(TSource) == typeof(nuint))
        {
            using (source)
            {
                if (!source.TryGetNext(out var current))
                {
                    Throws.NoElements();
                }

                nuint sum = Unsafe.As<TSource, nuint>(ref current);
                long count = 1;
                while (source.TryGetNext(out current))
                {
                    checked { sum += Unsafe.As<TSource, nuint>(ref current); }
                    count++;
                }

                return (double)sum / (double)count;
            }
        }
        #endregion
        else
        {
            Throws.NotSupportedType(typeof(TSource));
            return default!;
        }
#endif
    }

#if NET8_0_OR_GREATER

    public static double AverageIntSimd(ReadOnlySpan<int> span)
    {
        // based on SimdSumNumberUnchecked<T>
        // int[int.MaxValue] { int.MaxValue... }.Sum() is lower than long.MaxValue
        // so Sum as long without overflow check is safe.

        ref var current = ref MemoryMarshal.GetReference(span);
        ref var end = ref Unsafe.Add(ref current, span.Length);
        ref var to = ref Unsafe.Subtract(ref end, Vector<int>.Count);

        var sum = 0L;

        if (Vector.IsHardwareAccelerated && span.Length >= Vector<int>.Count)
        {
            var vectorSum = Vector<long>.Zero; // <0, 0, 0, 0> : Vector<long>
            do
            {
                var data = Vector.LoadUnsafe(ref current);     // <1, 2, 3, 4, 5, 6, 7, 8>   : Vector<int>
                Vector.Widen(data, out var low, out var high); // <1, 2, 3, 4>, <5, 6, 7, 8> : Vector<long>
                vectorSum += low;  // add low  <1, 2, 3, 4>
                vectorSum += high; // and high <6, 8, 10, 12>
                current = ref Unsafe.Add(ref current, Vector<int>.Count);
            } while (!Unsafe.IsAddressGreaterThan(ref current, ref to)); // (current <= to) -> !(current > to) 

            sum = Vector.Sum(vectorSum);
        }

        // fill rest
        while (Unsafe.IsAddressLessThan(ref current, ref end))
        {
            unchecked { sum += current; } // use unchecked
            current = ref Unsafe.Add(ref current, 1);
        }

        return (double)sum / span.Length;
    }

#endif
}