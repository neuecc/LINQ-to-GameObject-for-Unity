namespace ZLinq.Internal;

[StructLayout(LayoutKind.Sequential, Size = 1)]
internal struct BitBool
{
    byte value; // = 0b00000000;

    public bool IsZero
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => value == 0;
    }

    public bool IsBit1
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (value & 0b00000001) != 0;
    }

    public bool IsBit2
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (value & 0b00000010) != 0;
    }

    public bool IsBit3
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (value & 0b00000100) != 0;
    }

    public bool IsBit4
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (value & 0b00001000) != 0;
    }

    public bool IsBit5
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (value & 0b00010000) != 0;
    }

    public bool IsBit6
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (value & 0b00100000) != 0;
    }

    public bool IsBit7
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (value & 0b01000000) != 0;
    }

    public bool IsBit8
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (value & 0b10000000) != 0;
    }

    // Bit1
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetTrueToBit1() => value |= 0b00000001;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetFalseToBit1() => value &= 0b11111110;

    // Bit2
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetTrueToBit2() => value |= 0b00000010;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetFalseToBit2() => value &= 0b11111101;

    // Bit3
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetTrueToBit3() => value |= 0b00000100;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetFalseToBit3() => value &= 0b11111011;

    // Bit4
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetTrueToBit4() => value |= 0b00001000;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetFalseToBit4() => value &= 0b11110111;

    // Bit5
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetTrueToBit5() => value |= 0b00010000;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetFalseToBit5() => value &= 0b11101111;

    // Bit6
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetTrueToBit6() => value |= 0b00100000;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetFalseToBit6() => value &= 0b11011111;

    // Bit7
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetTrueToBit7() => value |= 0b01000000;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetFalseToBit7() => value &= 0b10111111;

    // Bit8
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetTrueToBit8() => value |= 0b10000000;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetFalseToBit8() => value &= 0b01111111;

    public override string ToString() => Convert.ToString(value, 2).PadLeft(8, '0');
}