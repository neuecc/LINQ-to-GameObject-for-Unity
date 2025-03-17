#if NET8_0_OR_GREATER
using System;
using System.Buffers;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.Intrinsics;
#endif

namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static Boolean Contains<TEnumerator, TSource>(in this ValueEnumerable<TEnumerator, TSource> source, TSource value)
            where TEnumerator : struct, IValueEnumerator<TSource>
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

        public static Boolean Contains<TEnumerator, TSource>(in this ValueEnumerable<TEnumerator, TSource> source, TSource value, IEqualityComparer<TSource> comparer)
            where TEnumerator : struct, IValueEnumerator<TSource>
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
            // Generate code from FileGen.TypeOfContains
            // float, double, decimal and string are `IsBitwiseEquatable<T> == false` so don't use SIMD(but uses unroll search, it slightly faster than handwritten).
            if (typeof(T) == typeof(byte))
            {
                var span = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(source)), source.Length);
                return MemoryExtensions.Contains(span, Unsafe.As<T, byte>(ref value));
            }
            else if (typeof(T) == typeof(sbyte))
            {
                var span = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<T, sbyte>(ref MemoryMarshal.GetReference(source)), source.Length);
                return MemoryExtensions.Contains(span, Unsafe.As<T, sbyte>(ref value));
            }
            else if (typeof(T) == typeof(short))
            {
                var span = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<T, short>(ref MemoryMarshal.GetReference(source)), source.Length);
                return MemoryExtensions.Contains(span, Unsafe.As<T, short>(ref value));
            }
            else if (typeof(T) == typeof(ushort))
            {
                var span = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<T, ushort>(ref MemoryMarshal.GetReference(source)), source.Length);
                return MemoryExtensions.Contains(span, Unsafe.As<T, ushort>(ref value));
            }
            else if (typeof(T) == typeof(int))
            {
                var span = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<T, int>(ref MemoryMarshal.GetReference(source)), source.Length);
                return MemoryExtensions.Contains(span, Unsafe.As<T, int>(ref value));
            }
            else if (typeof(T) == typeof(uint))
            {
                var span = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<T, uint>(ref MemoryMarshal.GetReference(source)), source.Length);
                return MemoryExtensions.Contains(span, Unsafe.As<T, uint>(ref value));
            }
            else if (typeof(T) == typeof(long))
            {
                var span = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<T, long>(ref MemoryMarshal.GetReference(source)), source.Length);
                return MemoryExtensions.Contains(span, Unsafe.As<T, long>(ref value));
            }
            else if (typeof(T) == typeof(ulong))
            {
                var span = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<T, ulong>(ref MemoryMarshal.GetReference(source)), source.Length);
                return MemoryExtensions.Contains(span, Unsafe.As<T, ulong>(ref value));
            }
            else if (typeof(T) == typeof(float))
            {
                var span = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<T, float>(ref MemoryMarshal.GetReference(source)), source.Length);
                return MemoryExtensions.Contains(span, Unsafe.As<T, float>(ref value));
            }
            else if (typeof(T) == typeof(double))
            {
                var span = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<T, double>(ref MemoryMarshal.GetReference(source)), source.Length);
                return MemoryExtensions.Contains(span, Unsafe.As<T, double>(ref value));
            }
            else if (typeof(T) == typeof(bool))
            {
                var span = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<T, bool>(ref MemoryMarshal.GetReference(source)), source.Length);
                return MemoryExtensions.Contains(span, Unsafe.As<T, bool>(ref value));
            }
            else if (typeof(T) == typeof(char))
            {
                var span = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<T, char>(ref MemoryMarshal.GetReference(source)), source.Length);
                return MemoryExtensions.Contains(span, Unsafe.As<T, char>(ref value));
            }
            else if (typeof(T) == typeof(decimal))
            {
                var span = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<T, decimal>(ref MemoryMarshal.GetReference(source)), source.Length);
                return MemoryExtensions.Contains(span, Unsafe.As<T, decimal>(ref value));
            }
            else if (typeof(T) == typeof(nint))
            {
                var span = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<T, nint>(ref MemoryMarshal.GetReference(source)), source.Length);
                return MemoryExtensions.Contains(span, Unsafe.As<T, nint>(ref value));
            }
            else if (typeof(T) == typeof(nuint))
            {
                var span = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<T, nuint>(ref MemoryMarshal.GetReference(source)), source.Length);
                return MemoryExtensions.Contains(span, Unsafe.As<T, nuint>(ref value));
            }
            else if (typeof(T) == typeof(string))
            {
                var span = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<T, string>(ref MemoryMarshal.GetReference(source)), source.Length);
                return MemoryExtensions.Contains(span, Unsafe.As<T, string>(ref value));
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

#endif
    }
}
