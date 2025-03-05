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
            => new(source, keySelector, null, null);

        public static OrderBy<TEnumerable, TSource, TKey> OrderBy<TEnumerable, TSource, TKey>(this TEnumerable source, Func<TSource, TKey> keySelector, IComparer<TKey>? comparer)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(source, keySelector, comparer, null);
    }
}

namespace ZLinq.Linq
{
    // OrderBy has ThenBy/ThenByDescending methods.

    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
#if NET9_0_OR_GREATER
    public ref
#else
    public
#endif
    struct OrderBy<TEnumerable, TSource, TKey>(TEnumerable source, Func<TSource, TKey> keySelector, IComparer<TKey>? comparer, IOrderByComparer<TSource>? thenBy)
        : IValueEnumerable<TSource>
        where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerable source = source;
        OrderBySortContext<TSource, TKey> sortContext = new (keySelector, comparer ?? Comparer<TKey>.Default, null); // TODO: is thenby null?


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

                sortContext.Initialize(dest);

#if NET8_0_OR_GREATER
                // TODO: for netstandard, create polyfill
                indexMap.AsSpan().Sort(dest, sortContext);
#endif

                return true;
            }
            return false;
        }

        public bool TryGetNext(out TSource current)
        {
            throw new NotImplementedException();
            // Unsafe.SkipInit(out current);
            // return false;
        }

        public void Dispose()
        {
            source.Dispose();
        }

        // my js implementation: https://github.com/mihaifm/linq/blob/master/linq.js#L2535-L2624

        public OrderBy<TEnumerable, TSource, TSecondKey> ThenBy<TSecondKey>(Func<TSource, TSecondKey> keySelector, IComparer<TSecondKey>? comparer)
        {
            var childContext = new OrderBySortContext<TSource, TSecondKey>(keySelector, comparer ?? Comparer<TSecondKey>.Default, this);

            return new OrderBy<TEnumerable, TSource, TSecondKey>(source, keySelector, comparer, childContext);

            // throw new NotImplementedException();
        }
    }

    public interface IOrderByComparer<TSource> : IComparer<int>
    {
        void Initialize(ReadOnlySpan<TSource> source);
    }

    internal sealed class OrderBySortContext<TSource, TKey>(Func<TSource, TKey> keySelector, IComparer<TKey> comparer, IOrderByComparer<TSource>? parentComparer) : IComparer<int>, IDisposable
    {
        TKey[] keys = default!; // must call Initialize before Compare

        public void Initialize(ReadOnlySpan<TSource> source)
        {
            var tempArray = ArrayPool<TKey>.Shared.Rent(source.Length);
            for (var i = 0; i < source.Length; i++)
            {
                tempArray[i] = keySelector(source[i]);
            }
            keys = tempArray; // Compare range is [0, source.Length).

            parentComparer?.Initialize(source); // initialize to root
        }

        public int Compare(int x, int y)
        {
            // first, compare self key
            // first, compare by root

            var result = comparer.Compare(keys[x], keys[y]);
            if (result != 0) return result;

            // second, thenby key
            if (thenByComparer != null)
            {
                return thenByComparer.Compare(x, y);
            }

            // finally compare index
            return x > y ? 0 : -1; // TODO: compare
        }

        public void Dispose()
        {
            if (keys != null)
            {
                ArrayPool<TKey>.Shared.Return(keys, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<TKey>());
                keys = null!;
            }
        }
    }
}
