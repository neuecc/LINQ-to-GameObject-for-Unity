namespace ZLinq.Internal;

internal static class SingleSpan
{
    internal static Span<T> Create<T>(
#if !NETSTANDARD2_0
        ref T reference
#endif
    )
    {
#if NETSTANDARD2_0
        var array = SingleArray<T>.Array;
        if (array == null)
        {
            array = SingleArray<T>.Array = new T[1];
        }
        return array.AsSpan(); // need to Clear after used to prevent memory leak
#elif NETSTANDARD2_1
        return MemoryMarshal.CreateSpan(ref reference, 1);
#else // NET8_0_OR_GREATER
        return new Span<T>(ref reference);
#endif
    }

#if NETSTANDARD2_0
    internal static class SingleArray<T>
    {
        [ThreadStatic]
        public static T[]? Array;
    }
#endif
}
