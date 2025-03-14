using System.Buffers;

namespace ZLinq.Internal;

// Add Only, when Freeze(), You can keep use of this type.

internal struct ArrayBuilder<T>
{
    public static int MaxLength => 0X7FFFFFC7; // Array.MaxLength

    private T[]? array;
    int count;

    public int Count => count;
    public ArraySegment<T> Array => array == null ? [] : new(array, 0, count);

    public void Add(T value)
    {
        if (array == null)
        {
            array = ArrayPool<T>.Shared.Rent(4);
        }
        else if (array.Length == count)
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

        System.Array.Copy(array, newArray, array.Length);
        ArrayPool<T>.Shared.Return(array, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());

        array = newArray;
    }

    public T[] BuildAndClear()
    {
        if (array == null)
        {
            return System.Array.Empty<T>();
        }

        var result = GC.AllocateUninitializedArray<T>(count);
        System.Array.Copy(array, result, count);
        ArrayPool<T>.Shared.Return(array, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
        array = null;
        return result;
    }

    public void Freeze()
    {
        if (array == null) return;

        var result = GC.AllocateUninitializedArray<T>(count);
        System.Array.Copy(array, result, count);
        ArrayPool<T>.Shared.Return(array, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
        array = result;
    }
}
