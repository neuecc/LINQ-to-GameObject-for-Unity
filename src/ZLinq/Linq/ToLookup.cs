using System.Collections;
using System.Diagnostics;

namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static ILookup<TKey, TSource> ToLookup<TEnumerable, TSource, TKey>(this TEnumerable source, Func<TSource, TKey> keySelector)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            throw new NotImplementedException();
        }

        public static ILookup<TKey, TSource> ToLookup<TEnumerable, TSource, TKey>(this TEnumerable source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            throw new NotImplementedException();
        }

        public static ILookup<TKey, TElement> ToLookup<TEnumerable, TSource, TKey, TElement>(this TEnumerable source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            throw new NotImplementedException();
        }

        public static ILookup<TKey, TElement> ToLookup<TEnumerable, TSource, TKey, TElement>(this TEnumerable source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            if (source.TryGetSpan(out var span))
            {
                var lookupBuilder = new LookupBuilder<TKey, TElement>(comparer);

                foreach (var item in span)
                {
                    lookupBuilder.Add(keySelector(item), elementSelector(item));
                }

                return lookupBuilder.BuildAndClear();
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}


namespace ZLinq.Linq
{
    internal struct LookupBuilder<TKey, TElement>
    {
        // open addressing with quadratic-probing hash-table
        const int MinimumPrime = 7;

        GroupingBuilder<TKey, TElement>[]? buckets;
        readonly IEqualityComparer<TKey> comparer;
        int lastAddIndex;
        int groupCount;
        ulong fastModMultiplier;

        public LookupBuilder(IEqualityComparer<TKey>? comparer)
        {
            var size = MinimumPrime;
            if (IntPtr.Size == 8)
            {
                this.fastModMultiplier = HashHelpers.GetFastModMultiplier((uint)size);
            }
            this.lastAddIndex = -1;
            this.groupCount = 0;
            this.comparer = comparer ?? EqualityComparer<TKey>.Default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        int GetBucketIndex(uint hashCode)
        {
            var buckets = this.buckets;
            Debug.Assert(buckets is not null);

            if (IntPtr.Size == 8)
            {
                return (int)HashHelpers.FastMod(hashCode, (uint)buckets.Length, fastModMultiplier);
            }
            else
            {
                return (int)(hashCode % buckets.Length);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        uint InternalGetHashCode(TKey key)
        {
            return (uint)((key is null) ? 0 : comparer.GetHashCode(key) & 0x7FFFFFFF);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        int QuadraticProbe(int step)
        {
            return step * step;
        }

        public void Add(TKey key, TElement value)
        {
            if (buckets == null)
            {
                buckets = new GroupingBuilder<TKey, TElement>[MinimumPrime]; // TODO: from pool
            }

            var hash = InternalGetHashCode(key);
            var index = GetBucketIndex(hash);
            ref var bucket = ref buckets[index];

            if (bucket.IsNull) // new slot
            {
                bucket = new GroupingBuilder<TKey, TElement>(key, value);
                groupCount++;
                goto NEW_GROUP_ADD;
            }
            else
            {
                if (comparer.Equals(bucket.key, key))
                {
                    bucket.Add(value);
                    return; // add existing group chain
                }

                // conflict, try next 
                while (bucket.HasNext)
                {
                    var nextIndex = bucket.next;
                    bucket = ref buckets[nextIndex];

                    if (!bucket.IsNull && comparer.Equals(bucket.key, key))
                    {
                        bucket.Add(value);
                        return; // add existing group chain
                    }
                }

                // try to find empty slot

                // TODO: resize

                var step = 1;
                var bucketIndex = 0;
                do
                {
                    bucketIndex = (index + QuadraticProbe(step)) % buckets.Length;
                    step++;
                    if (step > buckets.Length) throw new InvalidOperationException("hashtable is full.");
                    bucket = ref buckets[bucketIndex];
                }
                while (bucket.IsNull);

                index = bucketIndex;
                goto NEW_GROUP_ADD;
            }

        NEW_GROUP_ADD:
            groupCount++;

            if (lastAddIndex != -1)
            {
                ref var previous = ref buckets[lastAddIndex];
                bucket.next = previous.next; // as first
                previous.next = index;
            }
            else
            {
                bucket.next = index; // register first
            }
        }

        public Lookup<TKey, TElement> BuildAndClear()
        {
            return new Lookup<TKey, TElement>(ref buckets, lastAddIndex, groupCount);
        }
    }

    internal struct GroupingBuilder<TKey, TElement>
    {
        ArrayBuilder<TElement> arrayBuilder;
        public TKey key;
        public int next;
        bool notNull;
        public bool HasNext => next != -1;
        public bool IsNull => !notNull;

        public GroupingBuilder(TKey key, TElement element)
        {
            this.key = key;
            this.arrayBuilder.Add(element);
            this.next = -1;
            this.notNull = true;
        }

        public void Add(TElement value)
        {
            arrayBuilder.Add(value);
        }

        public Grouping<TKey, TElement>? BuildAndClear()
        {
            if (!notNull)
            {
                return new Grouping<TKey, TElement>(key, arrayBuilder.BuildAndClear());
            }
            {
                return null;
            }
        }
    }

    internal sealed class Lookup<TKey, TElement> : ILookup<TKey, TElement>
    {
        Grouping<TKey, TElement>?[] groups;
        Grouping<TKey, TElement>? last;
        int count;

        public Lookup(ref GroupingBuilder<TKey, TElement>[]? groupingBuilders, int lastGroupingIndex, int count)
        {
            // for space efficiency, re-hash is best, however for creation performance, don't do re-hash.

            if (groupingBuilders is null)
            {
                count = 0;
                return;
            }

            var grouping = new Grouping<TKey, TElement>?[groupingBuilders.Length];
            for (int i = 0; i < grouping.Length; i++)
            {
                grouping[i++] = groupingBuilder[i].BuildAndClear();
            }

            this.values = grouping;
            this.count = count;
            this.last = grouping[lastGroupingIndex];
        }

        public IEnumerable<TElement> this[TKey key]
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public int Count => count;

        public bool Contains(TKey key)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<IGrouping<TKey, TElement>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    internal sealed class Grouping<TKey, TElement>(TKey key, TElement[] elements) : IGrouping<TKey, TElement>
    {
        public TKey Key => key;
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public int Count => elements.Length;

        public IEnumerator<TElement> GetEnumerator()
        {
            return elements.AsEnumerable().GetEnumerator();
        }
    }
}
