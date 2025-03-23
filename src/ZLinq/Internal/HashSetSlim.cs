#pragma warning disable 0436
using System.Buffers;
using System.Diagnostics;

namespace ZLinq.Internal;

// Most of the time we use class instead of struct because objects are kept in the heap.
// Since we're only using it for duplicate checking in set operations, having `bool Add` is sufficient.
internal sealed class HashSetSlim<T> : IDisposable
{
    const int MinimumSize = 16; // minimum arraypool size
    const double LoadFactor = 0.72;

    readonly IEqualityComparer<T> comparer;

    Entry[] entries;
    int[] buckets; // bucket is index of entries, 1-based(0 for empty).
    int bucketsLength; // power of 2
    int entryIndex;
    int resizeThreshold;

    public HashSetSlim(IEqualityComparer<T>? comparer)
    {
        this.comparer = comparer ?? EqualityComparer<T>.Default;
        this.buckets = ArrayPool<int>.Shared.Rent(MinimumSize);
        this.entries = ArrayPool<Entry>.Shared.Rent(MinimumSize);
        this.bucketsLength = MinimumSize;
        this.resizeThreshold = (int)(bucketsLength * LoadFactor);
        buckets.AsSpan().Clear(); // 0-clear.
    }

    public bool Add(T item)
    {
        var hashCode = InternalGetHashCode(item);
        ref var bucket = ref buckets[GetBucketIndex(hashCode)];
        var index = bucket - 1;

        // lookup phase
        while (index != -1)
        {
            ref var entry = ref entries[index];
            if (entry.HashCode == hashCode && comparer.Equals(entry.Value, item))
            {
                return false;
            }
            index = entry.Next;
        }

        // add phase
        if (entryIndex > resizeThreshold)
        {
            Resize();
            // Need to recalculate bucket after resize
            bucket = ref buckets[GetBucketIndex(hashCode)];
        }

        ref var newEntry = ref entries[entryIndex];
        newEntry.HashCode = hashCode;
        newEntry.Value = item;
        newEntry.Next = bucket - 1;

        bucket = entryIndex + 1;
        entryIndex++;

        return true;
    }

    void Resize()
    {
        var newSize = System.Numerics.BitOperations.RoundUpToPowerOf2((uint)entries.Length * 2);
        var newEntries = ArrayPool<Entry>.Shared.Rent((int)newSize);
        var newBuckets = ArrayPool<int>.Shared.Rent((int)newSize);
        bucketsLength = (int)newSize; // gurantees PowerOf2
        resizeThreshold = (int)(bucketsLength * LoadFactor);
        newBuckets.AsSpan().Clear(); // 0-clear.

        // Copy entries
        Array.Copy(entries, newEntries, entryIndex);

        for (int i = 0; i < entryIndex; i++)
        {
            ref var entry = ref newEntries[i];
            var bucketIndex = GetBucketIndex(entry.HashCode);

            ref var bucket = ref newBuckets[bucketIndex];
            entry.Next = bucket - 1;
            bucket = i + 1;
        }

        // return old arrays
        ArrayPool<int>.Shared.Return(buckets, clearArray: false);
        ArrayPool<Entry>.Shared.Return(entries, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<Entry>());

        // assign new arrays
        entries = newEntries;
        buckets = newBuckets;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    uint InternalGetHashCode(T key)
    {
        // allows null.
        return (uint)((key is null) ? 0 : comparer.GetHashCode(key) & 0x7FFFFFFF);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    int GetBucketIndex(uint hashCode)
    {
        return (int)(hashCode & (bucketsLength - 1));
    }

    // return to pool
    public void Dispose()
    {
        if (buckets != null)
        {
            ArrayPool<int>.Shared.Return(buckets, clearArray: false);
            buckets = null!;
        }
        if (entries != null)
        {
            ArrayPool<Entry>.Shared.Return(entries, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<Entry>());
            entries = null!;
        }
    }

    [StructLayout(LayoutKind.Auto)]
    [DebuggerDisplay("HashCode = {HashCode}, Value = {Value}, Next = {Next}")]
    struct Entry
    {
        public uint HashCode;
        public T Value;
        public int Next; // next is index of entries, -1 is end of chain
    }
}
