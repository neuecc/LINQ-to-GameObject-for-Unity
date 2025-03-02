using System.Buffers;

namespace ZLinq.Internal;

// Add Only

internal struct ArrayBuilder<T>
{
    public static int MaxLength => 0X7FFFFFC7; // Array.MaxLength

    T[]? array;
    int count;

    public void Add(T value)
    {
        if (array == null)
        {
            array = ArrayPool<T>.Shared.Rent(4);
        }

        if (array.Length == count)
        {
            Grow();
        }

        array[count++] = value;
    }

    void Grow()
    {
        var newCapacity = unchecked(array!.Length * 2);
        if (unchecked((uint)newCapacity) > MaxLength) newCapacity = MaxLength;

        var newArray = ArrayPool<T>.Shared.Rent(newCapacity);

        Array.Copy(array, newArray, array.Length);
        array = newArray;

        // return
        ArrayPool<T>.Shared.Return(array, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
    }

    public T[] BuildAndClear()
    {
        if (array == null)
        {
            return Array.Empty<T>();
        }

        var result = GC.AllocateUninitializedArray<T>(count);
        Array.Copy(array, result, count);
        ArrayPool<T>.Shared.Return(array, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
        return result;
    }
}
