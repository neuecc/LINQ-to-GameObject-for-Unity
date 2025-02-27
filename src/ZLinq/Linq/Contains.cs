#if NET8_0_OR_GREATER
using System;
using System.Numerics;
using System.Runtime.Intrinsics;
#endif

namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static Boolean Contains<TEnumerable, TSource>(this TEnumerable source, TSource value)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            if (source.TryGetSpan(out var span))
            {
                // NOTE: .NET 10 can call span.Contains with comparer so no needs this hack.

#if NET8_0_OR_GREATER
                return InvokeSpanContains(span, value);
#else
                foreach (var item in span)
                {
                    if (EqualityComparer<TSource>.Default.Equals(item, value))
                    {
                        return true;
                    }
                }
                return false;
#endif
            }
            else
            {
                try
                {
                    while (source.TryGetNext(out var item))
                    {
                        if (EqualityComparer<TSource>.Default.Equals(item, value))
                        {
                            return true;
                        }
                    }
                }
                finally
                {
                    source.Dispose();
                }
                return false;
            }
        }

        public static Boolean Contains<TEnumerable, TSource>(this TEnumerable source, TSource value, IEqualityComparer<TSource> comparer)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            if (source.TryGetSpan(out var span))
            {
                foreach (var item in span)
                {
                    if (comparer.Equals(item, value))
                    {
                        return true;
                    }
                }
            }
            else
            {
                try
                {
                    while (source.TryGetNext(out var item))
                    {
                        if (comparer.Equals(item, value))
                        {
                            return true;
                        }
                    }
                }
                finally
                {
                    source.Dispose();
                }
            }

            return false;
        }

#if NET8_0_OR_GREATER

        // Hack to avoid where constraints of MemoryExtensions.Contains.
        // .NET 10 removed it so no needs this hack. https://github.com/dotnet/runtime/pull/110197
        static unsafe bool InvokeSpanContains<T>(ReadOnlySpan<T> source, T value)
        {
            // TODO: known contains...
            if (typeof(T) == typeof(int))
            {
                var span = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<T, int>(ref MemoryMarshal.GetReference(source)), source.Length);
                return MemoryExtensions.Contains(span, Unsafe.As<T, int>(ref value));
            }
            else
            {
                foreach (var item in source)
                {
                    if (EqualityComparer<T>.Default.Equals(item, value))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        // TODO: don't use?(use MemoryExtensions.Contains instead.)
        static bool ContainsCore<T>(ReadOnlySpan<T> source, T value)
           where T : struct, INumber<T>
        {
            T sum = T.Zero;

            if (!Vector128.IsHardwareAccelerated || source.Length < Vector128<T>.Count)
            {
                // Not SIMD supported or small source.
                for (int i = 0; i < source.Length; i++)
                {
                    if (source[i] == value)
                    {
                        return true;
                    }
                }
            }
            else if (!Vector256.IsHardwareAccelerated || source.Length < Vector256<T>.Count)
            {
                // Only 128bit SIMD supported or small source.
                ref var begin = ref MemoryMarshal.GetReference(source);
                ref var last = ref Unsafe.Add(ref begin, source.Length);
                ref var current = ref begin;

                var vectorValue = Vector128.Create(value);
                ref var to = ref Unsafe.Add(ref begin, source.Length - Vector128<T>.Count);
                while (Unsafe.IsAddressLessThan(ref current, ref to))
                {
                    if (Vector128.EqualsAny(Vector128.LoadUnsafe(ref current), vectorValue))
                    {
                        return true;
                    }
                    current = ref Unsafe.Add(ref current, Vector128<T>.Count);
                }
                while (Unsafe.IsAddressLessThan(ref current, ref last))
                {
                    if (current == value)
                    {
                        return true;
                    }
                    current = ref Unsafe.Add(ref current, 1);
                }
            }
            else
            {
                // 256bit SIMD supported
                ref var begin = ref MemoryMarshal.GetReference(source);
                ref var last = ref Unsafe.Add(ref begin, source.Length);
                ref var current = ref begin;

                var vectorValue = Vector256.Create(value);
                ref var to = ref Unsafe.Add(ref begin, source.Length - Vector256<T>.Count);
                while (Unsafe.IsAddressLessThan(ref current, ref to))
                {
                    if (Vector256.EqualsAny(Vector256.LoadUnsafe(ref current), vectorValue))
                    {
                        return true;
                    }
                    current = ref Unsafe.Add(ref current, Vector256<T>.Count);
                }
                while (Unsafe.IsAddressLessThan(ref current, ref last))
                {
                    if (current == value)
                    {
                        return true;
                    }
                    current = ref Unsafe.Add(ref current, 1);
                }
            }

            return false;
        }

#endif
    }
}
