namespace ZLinq.Internal;

internal static class SingleSpan
{
    internal static Span<T> Create<T>(ref T reference)
    {
#if NETSTANDARD2_0
        unsafe
        {
            return new Span<T>(Unsafe.AsPointer(ref reference), 1);
        }
#elif NETSTANDARD2_1
        return MemoryMarshal.CreateSpan(ref reference, 1);
#else // NET8_0_OR_GREATER
        return new Span<T>(ref reference);
#endif
    }
}
