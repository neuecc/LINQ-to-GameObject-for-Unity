#pragma warning disable

using System.Buffers;
using System.Collections;
using System.Diagnostics;

namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static ILookup<TKey, TSource> ToLookup<TEnumerator, TSource, TKey>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TKey> keySelector)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            return ToLookup(source, keySelector, null);
        }

        public static ILookup<TKey, TSource> ToLookup<TEnumerator, TSource, TKey>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            ArgumentNullException.ThrowIfNull(keySelector);

            var lookupBuilder = new LookupBuilder<TKey, TSource>(comparer ?? EqualityComparer<TKey>.Default);
            using (var enumerator = source.Enumerator)
            {
                if (enumerator.TryGetSpan(out var span))
                {
                    foreach (var item in span)
                    {
                        lookupBuilder.Add(keySelector(item), item);
                    }
                }
                else
                {
                    while (enumerator.TryGetNext(out var item))
                    {
                        lookupBuilder.Add(keySelector(item), item);
                    }
                }

                return lookupBuilder.BuildAndClear();
            }
        }

        public static ILookup<TKey, TElement> ToLookup<TEnumerator, TSource, TKey, TElement>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            return ToLookup(source, keySelector, elementSelector, null!);
        }

        public static ILookup<TKey, TElement> ToLookup<TEnumerator, TSource, TKey, TElement>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey>? comparer)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            ArgumentNullException.ThrowIfNull(keySelector);
            ArgumentNullException.ThrowIfNull(elementSelector);

            var lookupBuilder = new LookupBuilder<TKey, TElement>(comparer ?? EqualityComparer<TKey>.Default);
            using (var enumerator = source.Enumerator)
            {
                if (enumerator.TryGetSpan(out var span))
                {
                    foreach (var item in span)
                    {
                        lookupBuilder.Add(keySelector(item), elementSelector(item));
                    }
                }
                else
                {
                    while (enumerator.TryGetNext(out var item))
                    {
                        lookupBuilder.Add(keySelector(item), elementSelector(item));
                    }
                }

                return lookupBuilder.BuildAndClear();
            }
        }
    }
}

namespace ZLinq.Linq
{
    internal static class Lookup
    {
        // Enumerable.Join operation ignores null key
        public static Lookup<TKey, TSource> CreateForJoin<TEnumerator, TSource, TKey>(ref TEnumerator source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            var lookupBuilder = new LookupBuilder<TKey, TSource>(comparer ?? EqualityComparer<TKey>.Default);
            if (source.TryGetSpan(out var span))
            {
                foreach (var item in span)
                {
                    var key = keySelector(item);
                    if (key is not null)
                    {
                        lookupBuilder.Add(key, item);
                    }
                }
            }
            else
            {
                while (source.TryGetNext(out var item))
                {
                    var key = keySelector(item);
                    if (key is not null)
                    {
                        lookupBuilder.Add(key, item);
                    }
                }
            }

            return lookupBuilder.BuildAndClear();
        }
    }

    [StructLayout(LayoutKind.Auto)]
    internal struct LookupBuilder<TKey, TElement>
    {
        const int MinimumSize = 16; // minimum arraypool size
        const double LoadFactor = 0.72;

        readonly IEqualityComparer<TKey> comparer;

        Grouping<TKey, TElement>[]? buckets;
        int bucketsLength;

        Grouping<TKey, TElement>? last;
        int groupCount;

        public LookupBuilder(IEqualityComparer<TKey>? comparer)
        {
            this.last = null;
            this.groupCount = 0;
            this.comparer = comparer ?? EqualityComparer<TKey>.Default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        int GetBucketIndex(uint hashCode)
        {
            Debug.Assert(buckets is not null);
            return (int)(hashCode & (bucketsLength - 1));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        uint InternalGetHashCode(TKey key)
        {
            // allows null
            return (uint)((key is null) ? 0 : comparer.GetHashCode(key) & 0x7FFFFFFF);
        }

        public void Add(TKey key, TElement value)
        {
            if (buckets == null)
            {
                buckets = ArrayPool<Grouping<TKey, TElement>>.Shared.Rent(MinimumSize);
                bucketsLength = MinimumSize;
            }

            var hash = InternalGetHashCode(key);
            var index = GetBucketIndex(hash);
            ref var bucket = ref buckets[index];

            if (bucket == null) // new slot
            {
                if (groupCount != 0)
                {
                    if ((double)groupCount / bucketsLength > LoadFactor)
                    {
                        ResizeAndRehash();
                        bucket = ref buckets[GetBucketIndex(hash)]!;
                    }
                }

                bucket = new Grouping<TKey, TElement>(key, hash) { value };
                groupCount++;
                if (last != null)
                {
                    bucket.NextGroupInAddOrder = last.NextGroupInAddOrder;
                    last.NextGroupInAddOrder = bucket;
                }
                else
                {
                    bucket.NextGroupInAddOrder = bucket;
                }
                last = bucket;
            }
            else
            {
                if (comparer.Equals(bucket.Key, key))
                {
                    bucket.Add(value);
                    return; // added existing group
                }

                // conflict, try next chain
                var previous = bucket;
                var next = bucket.NextGroupInSameHashCode;
                while (next != null)
                {
                    if (comparer.Equals(next.Key, key))
                    {
                        next.Add(value);
                        return; // added existing group
                    }
                    previous = next;
                    next = next.NextGroupInSameHashCode;
                }

                // create new and chain
                var newGroup = new Grouping<TKey, TElement>(key, hash) { value };
                previous.NextGroupInSameHashCode = newGroup;

                groupCount++;
                if (last != null)
                {
                    newGroup.NextGroupInAddOrder = last.NextGroupInAddOrder;
                    last.NextGroupInAddOrder = newGroup;
                }
                else
                {
                    newGroup.NextGroupInAddOrder = newGroup;
                }
                last = newGroup;
            }
        }

        public Lookup<TKey, TElement> BuildAndClear()
        {
            if (groupCount == 0 || buckets is null)
            {
                return Lookup<TKey, TElement>.Empty;
            }

            var groups = buckets.AsSpan(0, bucketsLength).ToArray();
            ArrayPool<Grouping<TKey, TElement>>.Shared.Return(buckets, clearArray: true);
            return new Lookup<TKey, TElement>(groups, last, groupCount, comparer);
        }

        // GroupBy only needs root group.
        internal Grouping<TKey, TElement>? GetRootGroupAndClear()
        {
            if (groupCount == 0 || buckets is null)
            {
                return null;
            }

            foreach (var item in buckets)
            {
                item?.Freeze();
            }
            ArrayPool<Grouping<TKey, TElement>>.Shared.Return(buckets, clearArray: true);

            return last?.NextGroupInAddOrder; // as first.
        }

        void ResizeAndRehash()
        {
            Debug.Assert(buckets is not null);
            if (last is null) return;
            var group = last.NextGroupInAddOrder; // as first.
            if (group is null)
            {
                return;
            }

            var newSize = System.Numerics.BitOperations.RoundUpToPowerOf2((uint)(bucketsLength * 2));
            ArrayPool<Grouping<TKey, TElement>>.Shared.Return(buckets, clearArray: true);

            var newBuckets = ArrayPool<Grouping<TKey, TElement>>.Shared.Rent((int)newSize);
            buckets = newBuckets;
            bucketsLength = (int)newSize;

            var first = group;
            do
            {
                group.NextGroupInSameHashCode = null;
                ref var bucket = ref newBuckets[GetBucketIndex(group.HashCode)];
                if (bucket == null)
                {
                    bucket = group;
                }
                else
                {
                    var next = bucket;
                    while (true)
                    {
                        if (next.NextGroupInSameHashCode == null)
                        {
                            next.NextGroupInSameHashCode = group;
                            break;
                        }
                        next = next.NextGroupInSameHashCode;
                    }
                }

                group = group.NextGroupInAddOrder;
            } while (group != null && group != first);

            buckets = newBuckets;
        }
    }

    // .NET ILookup implements ICollection and it is public
    public sealed class Lookup<TKey, TElement> : ILookup<TKey, TElement>, ICollection<IGrouping<TKey, TElement>>, IReadOnlyCollection<IGrouping<TKey, TElement>>
    {
        internal static readonly Lookup<TKey, TElement> Empty = new Lookup<TKey, TElement>();

        readonly Grouping<TKey, TElement>?[]? groups;
        readonly Grouping<TKey, TElement>? last;
        readonly int count;
        readonly IEqualityComparer<TKey> comparer;

        Lookup() // for empty
        {
            this.groups = null;
            this.last = null;
            this.count = 0;
            this.comparer = null!;
        }

        internal Lookup(Grouping<TKey, TElement>[]? groupings, Grouping<TKey, TElement>? last, int count, IEqualityComparer<TKey> comparer)
        {
            if (groupings == null)
            {
                this.groups = null;
                this.last = null;
                this.count = 0;
                this.comparer = comparer;
                return;
            }

            foreach (var item in groupings)
            {
                item?.Freeze();
            }

            this.groups = groupings;
            this.count = count;
            this.last = last;
            this.comparer = comparer;
        }

        public IEnumerable<TElement> this[TKey key]
        {
            get
            {
                var group = GetGroup(key);
                return (group is null)
                    ? Array.Empty<TElement>()
                    : group;
            }
        }

        public int Count => count;

        // Lookup method

        public IEnumerable<TResult> ApplyResultSelector<TResult>(Func<TKey, IEnumerable<TElement>, TResult> resultSelector)
        {
            ArgumentNullException.ThrowIfNull(resultSelector);

            if (last is null) yield break;
            var group = last.NextGroupInAddOrder; // as first.
            if (group is null) yield break;

            var first = group;
            do
            {
                yield return resultSelector(group.Key, group);
                group = group.NextGroupInAddOrder;
            } while (group != null && group != first);
        }

        public bool Contains(TKey key)
        {
            return GetGroup(key) is not null;
        }

        internal Grouping<TKey, TElement>? GetGroup(TKey key)
        {
            if (groups is null) return null;

            var hashCode = InternalGetHashCode(key);
            var index = GetBucketIndex((uint)hashCode);

            var group = groups[index];
            while (group is not null)
            {
                if (comparer.Equals(group.Key, key))
                {
                    return group;
                }
                else
                {
                    group = group.NextGroupInSameHashCode;
                }
            }

            return null;
        }

        public IEnumerator<IGrouping<TKey, TElement>> GetEnumerator()
        {
            if (last is null) yield break;

            var group = last.NextGroupInAddOrder; // as first.
            if (group is null) yield break;

            var first = group;
            do
            {
                yield return group;
                group = group.NextGroupInAddOrder;
            } while (group != null && group != first);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        int GetBucketIndex(uint hashCode)
        {
            var buckets = this.groups;
            Debug.Assert(buckets is not null);
            return (int)(hashCode & (buckets.Length - 1));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        uint InternalGetHashCode(TKey key)
        {
            // allows null.
            return (uint)((key is null) ? 0 : comparer.GetHashCode(key) & 0x7FFFFFFF);
        }

        // ICollection

        bool ICollection<IGrouping<TKey, TElement>>.IsReadOnly => true;

        void ICollection<IGrouping<TKey, TElement>>.Add(IGrouping<TKey, TElement> item) => throw new NotSupportedException();
        bool ICollection<IGrouping<TKey, TElement>>.Remove(IGrouping<TKey, TElement> item) => throw new NotSupportedException();
        void ICollection<IGrouping<TKey, TElement>>.Clear() => throw new NotSupportedException();

        bool ICollection<IGrouping<TKey, TElement>>.Contains(IGrouping<TKey, TElement> item)
        {
            var group = GetGroup(item.Key);
            if (group != null && group == item)
            {
                return true;
            }
            return false;
        }

        void ICollection<IGrouping<TKey, TElement>>.CopyTo(IGrouping<TKey, TElement>[] array, int arrayIndex)
        {
            ArgumentNullException.ThrowIfNull(array);
            if (arrayIndex < 0) throw new ArgumentOutOfRangeException(nameof(arrayIndex), "Non-negative number required.");
            if (arrayIndex > array.Length) throw new ArgumentOutOfRangeException(nameof(arrayIndex), "Index was out of range. Must be non-negative and less than the size of the collection.");
            if (array.Length - arrayIndex < Count)  throw new ArgumentOutOfRangeException(nameof(arrayIndex), "Destination array is not long enough to copy all the items in the collection. Check array index and length.");

            if (last is null) return;

            var group = last.NextGroupInAddOrder; // as first.
            if (group is null) return;

            var first = group;
            do
            {
                array[arrayIndex] = group;
                ++arrayIndex;

                group = group.NextGroupInAddOrder;
            } while (group != null && group != first);
        }
    }

    [DebuggerDisplay("Key = {Key}, ({Count})")]
    internal sealed class Grouping<TKey, TElement>(TKey key, uint hashCode) : IGrouping<TKey, TElement>, IList<TElement>, IReadOnlyList<TElement>
    {
        public TKey Key => key;
        public uint HashCode => hashCode;
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        ArrayBuilder<TElement> elements; // in consturction phase as builder, after feezed it represents T[]

        public int Count => elements.Count;

        // mutable in construction, but readonly in public surface

        public Grouping<TKey, TElement>? NextGroupInAddOrder;  // to gurantees add order
        public Grouping<TKey, TElement>? NextGroupInSameHashCode; // linked-list node for chaining

        public void Add(TElement value)
        {
            elements.Add(value);
        }

        public void Freeze()
        {
            elements.Freeze();
        }

        // we needs IList implementation for Sytem.Linq internal optimization usage

        public bool IsReadOnly => true;

        public TElement this[int index]
        {
#if NETSTANDARD2_0
            get => elements.Array.GetAt(index);
#else
            get => elements.Array[index];
#endif
            set => throw new NotSupportedException();
        }

        public IEnumerator<TElement> GetEnumerator()
        {
            return elements.Array.GetEnumerator();
        }

        public int IndexOf(TElement item)
        {
            return ((IList<TElement>)elements.Array).IndexOf(item);
        }

        public void Insert(int index, TElement item)
        {
            throw new NotSupportedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        void ICollection<TElement>.Add(TElement item)
        {
            throw new NotSupportedException();
        }

        public void Clear()
        {
            throw new NotSupportedException();
        }

        public bool Contains(TElement item)
        {
            return ((ICollection<TElement>)elements.Array).Contains(item);
        }

        public void CopyTo(TElement[] array, int arrayIndex)
        {
            ((ICollection<TElement>)elements.Array).CopyTo(array, arrayIndex);
        }

        public bool Remove(TElement item)
        {
            throw new NotSupportedException();
        }
    }
}
