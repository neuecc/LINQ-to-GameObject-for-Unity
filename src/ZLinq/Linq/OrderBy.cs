using System;
using System.Buffers;

// TODO: descending

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
    // OrderBy has ThenBy/ThenByDescending instance methods.

    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
#if NET9_0_OR_GREATER
    public ref
#else
    public
#endif
    struct OrderBy<TEnumerable, TSource, TKey>(TEnumerable source, Func<TSource, TKey> keySelector, IComparer<TKey>? comparer, IOrderByComparable<TSource>? parent)
        : IValueEnumerable<TSource>
        where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerable source = source;
        OrderByComparable<TSource, TKey> comparable = new(keySelector, comparer, parent);

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


#if NET8_0_OR_GREATER
                // TODO: for netstandard, create polyfill
                indexMap.AsSpan().Sort(dest, comparer);
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

        public OrderBy<TEnumerable, TSource, TSecondKey> ThenBy<TSecondKey>(Func<TSource, TSecondKey> keySelector, IComparer<TSecondKey>? comparer = null)
        {
            return new OrderBy<TEnumerable, TSource, TSecondKey>(source, keySelector, comparer, this.comparable);
        }
    }

    public interface IOrderByComparable<TSource>
    {
        IOrderByComparer GetComparer(ReadOnlySpan<TSource> source, IOrderByComparer? childComparer);
    }

    public interface IOrderByComparer : IComparer<int>, IDisposable
    {
    }

    internal sealed class OrderByComparable<TSource, TKey>(Func<TSource, TKey> keySelector, IComparer<TKey>? comparer, IOrderByComparable<TSource>? parent) : IOrderByComparable<TSource>
    {
        IComparer<TKey> comparer = comparer ?? Comparer<TKey>.Default;

        public IOrderByComparer GetComparer(ReadOnlySpan<TSource> source, IOrderByComparer? childComparer)
        {
            var nextComparer = new OrderByComparer<TSource, TKey>(source, keySelector, comparer, childComparer);
            return parent?.GetComparer(source, nextComparer) ?? nextComparer;
        }
    }

    internal sealed class OrderByComparer<TSource, TKey> : IOrderByComparer
    {
        TKey[] keys = default!; // must call Initialize before Compare
        IComparer<TKey> comparer;
        IOrderByComparer? childComparer;

        public OrderByComparer(ReadOnlySpan<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey> comparer, IOrderByComparer? childComparer)
        {
            var tempArray = ArrayPool<TKey>.Shared.Rent(source.Length);
            for (var i = 0; i < source.Length; i++)
            {
                tempArray[i] = keySelector(source[i]);
            }
            this.keys = tempArray; // Compare range is [0, source.Length).
            this.comparer = comparer;
            this.childComparer = childComparer;
        }

        public int Compare(int x, int y)
        {
            var result = comparer.Compare(keys[x], keys[y]);
            if (result != 0) return result;

            result = childComparer?.Compare(x, y) ?? 0;
            if (result != 0) return result;

            // finally compare index to ensure stable sort
            return (x == y) ? 0
                : (x > y) ? 1
                : -1;
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
