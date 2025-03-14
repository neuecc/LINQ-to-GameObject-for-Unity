using System.Runtime.CompilerServices;

namespace ZLinq.Internal;

// storing struct enumerator, with pooling(stack itself is node)
internal sealed class RefStack<T> where T : IDisposable
{
    internal static readonly RefStack<T> DisposeSentinel = new(0);
    static object gate = new object(); // TODO:change lock-free

    static RefStack<T>? Last = null;

    RefStack<T>? Prev = null; // pooling property

    public static RefStack<T> Rent()
    {
        lock (gate)
        {
            if (Last == null)
            {
                return new RefStack<T>(4);
            }
            var rent = Last;
            Last = Last.Prev;
            return rent;
        }
    }

    public static void Return(RefStack<T> stack)
    {
        stack.Reset();
        lock (gate)
        {
            stack.Prev = Last;
            Last = stack;
        }
    }

    // ---

    T[] array;
    int size = 0;

    RefStack(int initialSize)
    {
        array = initialSize == 0 ? Array.Empty<T>() : new T[initialSize];
        size = 0;
    }

    public void Push(T value)
    {
        if (size == array.Length)
        {
            Array.Resize(ref array, array.Length * 2); // I don't care if the stack is not deep enough to overflow.
        }
        array[size++] = value;
    }

    public void Pop()
    {
        size--;
    }

    public ref T PeekRefOrNullRef()
    {
        if (size == 0)
        {
            return ref Unsafe.NullRef<T>();
        }
        return ref array[size - 1];
    }

    public void Reset()
    {
        for (int i = 0; i < size; i++)
        {
            array[i].Dispose();
        }

        size = 0;
    }
}
