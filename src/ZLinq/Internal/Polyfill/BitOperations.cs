#if NETSTANDARD2_0 || NETSTANDARD2_1 || NET6_0

using System;
using System.Collections.Generic;
using System.Text;

namespace System.Numerics;

// borrowed from dotnet/runtime

public static class BitOperations
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint RoundUpToPowerOf2(uint value)
    {
        // Based on https://graphics.stanford.edu/~seander/bithacks.html#RoundUpPowerOf2
        --value;
        value |= value >> 1;
        value |= value >> 2;
        value |= value >> 4;
        value |= value >> 8;
        value |= value >> 16;
        return value + 1;
    }

    private static ReadOnlySpan<byte> Log2DeBruijn => // 32
    [
        00, 09, 01, 10, 13, 21, 02, 29,
            11, 14, 16, 18, 22, 25, 03, 30,
            08, 12, 20, 28, 15, 17, 24, 07,
            19, 27, 23, 06, 26, 05, 04, 31
    ];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Log2(uint value)
    {
        value |= 1;
        return Log2SoftwareFallback(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int Log2SoftwareFallback(uint value)
    {
        value |= value >> 01;
        value |= value >> 02;
        value |= value >> 04;
        value |= value >> 08;
        value |= value >> 16;

        return Unsafe.AddByteOffset(
            ref MemoryMarshal.GetReference(Log2DeBruijn),
            (IntPtr)(int)((value * 0x07C4ACDDu) >> 27));
    }
}


#endif
