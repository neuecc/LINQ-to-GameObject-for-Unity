using System.Buffers;

namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static OrderBy<TEnumerable, TSource, TKey> OrderBy<TEnumerable, TSource, TKey>(this TEnumerable source, Func<TSource, TKey> keySelector)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
                , allows ref struct
#endif
            => new(source, keySelector, null, null, descending: false);

        public static OrderBy<TEnumerable, TSource, TKey> OrderBy<TEnumerable, TSource, TKey>(this TEnumerable source, Func<TSource, TKey> keySelector, IComparer<TKey>? comparer)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
                , allows ref struct
#endif
            => new(source, keySelector, comparer, null, descending: false);

        public static OrderBy<TEnumerable, TSource, TKey> OrderByDescending<TEnumerable, TSource, TKey>(this TEnumerable source, Func<TSource, TKey> keySelector)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
                , allows ref struct
#endif
            => new(source, keySelector, null, null, descending: true);

        public static OrderBy<TEnumerable, TSource, TKey> OrderByDescending<TEnumerable, TSource, TKey>(this TEnumerable source, Func<TSource, TKey> keySelector, IComparer<TKey>? comparer)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
                , allows ref struct
#endif
            => new(source, keySelector, comparer, null, descending: true);
    }
}

namespace ZLinq.Linq
{
    // OrderBy has ThenBy/ThenByDescending instance methods.

    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
#if NET9_0_OR_GREATER
        public ref
#else
    public
#endif
    struct OrderBy<TEnumerable, TSource, TKey>(TEnumerable source, Func<TSource, TKey> keySelector, IComparer<TKey>? comparer, IOrderByComparable<TSource>? parent, bool descending)
        : IValueEnumerable<TSource>
        where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
    {
        TEnumerable source = source;
        OrderByComparable<TSource, TKey> comparable = new(keySelector, comparer, parent, descending); // boxed

        TSource[]? sourceMap; // allocate
        int index;

        public ValueEnumerator<OrderBy<TEnumerable, TSource, TKey>, TSource> GetEnumerator()
        {
            return new(this);
        }

        public bool TryGetNonEnumeratedCount(out int count) => source.TryGetNonEnumeratedCount(out count);

        public bool TryGetSpan(out ReadOnlySpan<TSource> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(Span<TSource> destination)
        {
            if (source.TryGetNonEnumeratedCount(out var count) && source.TryCopyTo(destination.Slice(0, count)))
            {
                // QuickSort is not stable sort algorithm, OrderBy must be stable sort so always use schwartzian transform.
                var dest = destination.Slice(0, count);

                // faster(using SIMD) index map creation
                var indexMap = ValueEnumerable.Range(0, count).ToArray<FromRange, int>();

                using var comparer = comparable.GetComparer(dest, null!);
                indexMap.AsSpan().Sort(dest, comparer);

                return true;
            }
            return false;
        }

        public bool TryGetNext(out TSource current)
        {
            if (sourceMap == null)
            {
                sourceMap = source.ToArray<TEnumerable, TSource>();
                var (indexMap, size) = ValueEnumerable.Range(0, sourceMap.Length).ToArrayPool<FromRange, int>();

                using var comparer = comparable.GetComparer(sourceMap, null!);
                indexMap.AsSpan(0, size).Sort(sourceMap.AsSpan(), comparer);
                ArrayPool<int>.Shared.Return(indexMap);
            }

            if (index < sourceMap.Length)
            {
                current = sourceMap[index++];
                return true;
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
            source.Dispose();
        }

        public OrderBy<TEnumerable, TSource, TSecondKey> ThenBy<TSecondKey>(Func<TSource, TSecondKey> keySelector, IComparer<TSecondKey>? comparer = null)
        {
            return new OrderBy<TEnumerable, TSource, TSecondKey>(source, keySelector, comparer, this.comparable, descending: false);
        }

        public OrderBy<TEnumerable, TSource, TSecondKey> ThenByDescending<TSecondKey>(Func<TSource, TSecondKey> keySelector, IComparer<TSecondKey>? comparer = null)
        {
            return new OrderBy<TEnumerable, TSource, TSecondKey>(source, keySelector, comparer, this.comparable, descending: true);
        }
    }

    public interface IOrderByComparable<TSource>
    {
        IOrderByComparer GetComparer(ReadOnlySpan<TSource> source, IOrderByComparer? childComparer);
    }

    public interface IOrderByComparer : IComparer<int>, IDisposable
    {
    }

    internal sealed class OrderByComparable<TSource, TKey>(Func<TSource, TKey> keySelector, IComparer<TKey>? comparer, IOrderByComparable<TSource>? parent, bool descending) : IOrderByComparable<TSource>
    {
        IComparer<TKey> comparer = comparer ?? Comparer<TKey>.Default;

        public IOrderByComparer GetComparer(ReadOnlySpan<TSource> source, IOrderByComparer? childComparer)
        {
            var nextComparer = new OrderByComparer<TSource, TKey>(source, keySelector, comparer, childComparer, descending);
            return parent?.GetComparer(source, nextComparer) ?? nextComparer;
        }
    }

    internal sealed class OrderByComparer<TSource, TKey> : IOrderByComparer
    {
        TKey[] keys = default!; // must call Initialize before Compare
        IComparer<TKey> comparer;
        IOrderByComparer? childComparer;
        bool descending;

        public OrderByComparer(ReadOnlySpan<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer, IOrderByComparer? childComparer, bool descending)
        {
            var tempArray = ArrayPool<TKey>.Shared.Rent(source.Length);
            for (var i = 0; i < source.Length; i++)
            {
                tempArray[i] = keySelector(source[i]);
            }
            this.keys = tempArray; // Compare range is [0, source.Length).
            this.comparer = comparer;
            this.childComparer = childComparer;
            this.descending = descending;
        }

        public int Compare(int x, int y)
        {
            var result = comparer.Compare(keys[x], keys[y]);
            if (result != 0) return descending ? -result : result;

            result = childComparer?.Compare(x, y) ?? 0;
            if (result != 0) return descending ? -result : result;

            // finally compare index to ensure stable sort
            if (x == y) return 0;
            if (descending)
            {
                return (x > y) ? -1 : 1;
            }
            else
            {
                return (x > y) ? 1 : -1;
            }
        }

        public void Dispose()
        {
            if (keys != null)
            {
                ArrayPool<TKey>.Shared.Return(keys, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<TKey>());
                keys = null!;
                if (childComparer != null)
                {
                    childComparer.Dispose();
                    childComparer = null;
                }
            }
        }
    }
}
