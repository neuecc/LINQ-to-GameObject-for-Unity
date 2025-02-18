using System.Runtime.CompilerServices;
using System;

namespace ZLinq.Internal
{
    // storing struct enumerator, with pooling(stack itself is node)
    internal sealed class RefStack<T>
    {
        internal static readonly RefStack<T> DisposeSentinel = new(0);

        static RefStack<T>? Last = null;
        
        RefStack<T>? Prev = null; // pooling property

        public static RefStack<T> Rent()
        {
            if (Last == null)
            {
                return new RefStack<T>(4);
            }
            var rent = Last;
            Last = Last.Prev;
            return rent;
        }

        public static void Return(RefStack<T> stack)
        {
            stack.Reset();
            stack.Prev = Last;
            Last = stack;
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
            size = 0;
        }
    }
}
