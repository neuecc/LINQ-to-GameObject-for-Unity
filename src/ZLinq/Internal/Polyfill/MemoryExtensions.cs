#if NETSTANDARD2_1
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Text;

namespace System;

internal static class MemoryExtensionsPolyfill
{
    public static void Sort<TKey, TValue, TComparer>(this Span<TKey> keys, Span<TValue> items, TComparer comparer)
        where TComparer : IComparer<TKey>?
    {
        if (keys.Length != items.Length) throw new ArgumentException("keys and items must be same length");

        if (keys.Length > 1)
        {
            ArraySortHelper<TKey, TValue>.Sort(keys, items, comparer);
        }
    }
}

// borrowed from dotnet/runtime

internal static class ArraySortHelper<TKey, TValue>
{
    const int IntrosortSizeThreshold = 16;

    public static void Sort(Span<TKey> keys, Span<TValue> values, IComparer<TKey>? comparer)
    {
        // Add a try block here to detect IComparers (or their
        // underlying IComparables, etc) that are bogus.
        try
        {
            IntrospectiveSort(keys, values, comparer ?? Comparer<TKey>.Default);
        }
        catch (IndexOutOfRangeException)
        {
            throw new ArgumentException("Bad comparer");
        }
        catch (Exception e)
        {
            throw new InvalidOperationException("compare failed", e);
        }
    }

    private static void SwapIfGreaterWithValues(Span<TKey> keys, Span<TValue> values, IComparer<TKey> comparer, int i, int j)
    {
        Debug.Assert(comparer != null);
        Debug.Assert(0 <= i && i < keys.Length && i < values.Length);
        Debug.Assert(0 <= j && j < keys.Length && j < values.Length);
        Debug.Assert(i != j);

        if (comparer.Compare(keys[i], keys[j]) > 0)
        {
            TKey key = keys[i];
            keys[i] = keys[j];
            keys[j] = key;

            TValue value = values[i];
            values[i] = values[j];
            values[j] = value;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Swap(Span<TKey> keys, Span<TValue> values, int i, int j)
    {
        Debug.Assert(i != j);

        TKey k = keys[i];
        keys[i] = keys[j];
        keys[j] = k;

        TValue v = values[i];
        values[i] = values[j];
        values[j] = v;
    }

    internal static void IntrospectiveSort(Span<TKey> keys, Span<TValue> values, IComparer<TKey> comparer)
    {
        Debug.Assert(comparer != null);
        Debug.Assert(keys.Length == values.Length);

        if (keys.Length > 1)
        {
            IntroSort(keys, values, 2 * (BitOperations.Log2((uint)keys.Length) + 1), comparer);
        }
    }

    private static void IntroSort(Span<TKey> keys, Span<TValue> values, int depthLimit, IComparer<TKey> comparer)
    {
        Debug.Assert(!keys.IsEmpty);
        Debug.Assert(values.Length == keys.Length);
        Debug.Assert(depthLimit >= 0);
        Debug.Assert(comparer != null);

        int partitionSize = keys.Length;
        while (partitionSize > 1)
        {
            if (partitionSize <= IntrosortSizeThreshold)
            {

                if (partitionSize == 2)
                {
                    SwapIfGreaterWithValues(keys, values, comparer, 0, 1);
                    return;
                }

                if (partitionSize == 3)
                {
                    SwapIfGreaterWithValues(keys, values, comparer, 0, 1);
                    SwapIfGreaterWithValues(keys, values, comparer, 0, 2);
                    SwapIfGreaterWithValues(keys, values, comparer, 1, 2);
                    return;
                }

                InsertionSort(keys.Slice(0, partitionSize), values.Slice(0, partitionSize), comparer);
                return;
            }

            if (depthLimit == 0)
            {
                HeapSort(keys.Slice(0, partitionSize), values.Slice(0, partitionSize), comparer);
                return;
            }
            depthLimit--;

            int p = PickPivotAndPartition(keys.Slice(0, partitionSize), values.Slice(0, partitionSize), comparer);

            // Note we've already partitioned around the pivot and do not have to move the pivot again.
            IntroSort(keys[(p + 1)..partitionSize], values[(p + 1)..partitionSize], depthLimit, comparer);
            partitionSize = p;
        }
    }

    private static int PickPivotAndPartition(Span<TKey> keys, Span<TValue> values, IComparer<TKey> comparer)
    {
        Debug.Assert(keys.Length >= IntrosortSizeThreshold);
        Debug.Assert(comparer != null);

        int hi = keys.Length - 1;

        // Compute median-of-three.  But also partition them, since we've done the comparison.
        int middle = hi >> 1;

        // Sort lo, mid and hi appropriately, then pick mid as the pivot.
        SwapIfGreaterWithValues(keys, values, comparer, 0, middle);  // swap the low with the mid point
        SwapIfGreaterWithValues(keys, values, comparer, 0, hi);   // swap the low with the high
        SwapIfGreaterWithValues(keys, values, comparer, middle, hi); // swap the middle with the high

        TKey pivot = keys[middle];
        Swap(keys, values, middle, hi - 1);
        int left = 0, right = hi - 1;  // We already partitioned lo and hi and put the pivot in hi - 1.  And we pre-increment & decrement below.

        while (left < right)
        {
            while (comparer.Compare(keys[++left], pivot) < 0) ;
            while (comparer.Compare(pivot, keys[--right]) < 0) ;

            if (left >= right)
                break;

            Swap(keys, values, left, right);
        }

        // Put pivot in the right location.
        if (left != hi - 1)
        {
            Swap(keys, values, left, hi - 1);
        }
        return left;
    }

    private static void HeapSort(Span<TKey> keys, Span<TValue> values, IComparer<TKey> comparer)
    {
        Debug.Assert(comparer != null);
        Debug.Assert(!keys.IsEmpty);

        int n = keys.Length;
        for (int i = n >> 1; i >= 1; i--)
        {
            DownHeap(keys, values, i, n, comparer);
        }

        for (int i = n; i > 1; i--)
        {
            Swap(keys, values, 0, i - 1);
            DownHeap(keys, values, 1, i - 1, comparer);
        }
    }

    private static void DownHeap(Span<TKey> keys, Span<TValue> values, int i, int n, IComparer<TKey> comparer)
    {
        Debug.Assert(comparer != null);

        TKey d = keys[i - 1];
        TValue dValue = values[i - 1];

        while (i <= n >> 1)
        {
            int child = 2 * i;
            if (child < n && comparer.Compare(keys[child - 1], keys[child]) < 0)
            {
                child++;
            }

            if (!(comparer.Compare(d, keys[child - 1]) < 0))
                break;

            keys[i - 1] = keys[child - 1];
            values[i - 1] = values[child - 1];
            i = child;
        }

        keys[i - 1] = d;
        values[i - 1] = dValue;
    }

    private static void InsertionSort(Span<TKey> keys, Span<TValue> values, IComparer<TKey> comparer)
    {
        Debug.Assert(comparer != null);

        for (int i = 0; i < keys.Length - 1; i++)
        {
            TKey t = keys[i + 1];
            TValue tValue = values[i + 1];

            int j = i;
            while (j >= 0 && comparer.Compare(t, keys[j]) < 0)
            {
                keys[j + 1] = keys[j];
                values[j + 1] = values[j];
                j--;
            }

            keys[j + 1] = t;
            values[j + 1] = tValue;
        }
    }
}

#endif