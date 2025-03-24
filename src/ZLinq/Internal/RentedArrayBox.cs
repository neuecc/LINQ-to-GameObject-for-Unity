using System.Buffers;

namespace ZLinq.Internal;

// In order to manage state reliably on the heap by storing ArrayPool in struct fields, we ensure proper state management on the heap.
// This prevents accidents such as returning to the ArrayPool multiple times due to incorrect usage of the struct.
internal sealed class RentedArrayBox<T>(T[] array, int length) : IDisposable
{
    internal static readonly RentedArrayBox<T> Empty = new([], 0);

    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => length;
    }

    public Span<T> Span
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => array.AsSpan(0, length);
    }

    // not check length-range, must check Length before using this.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T UnsafeGetAt(int index)
    {
        return ref array[index];
    }

    public void Dispose()
    {
        if (array != null && length != 0)
        {
            ArrayPool<T>.Shared.Return(array, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
            array = null!;
        }
    }
}
