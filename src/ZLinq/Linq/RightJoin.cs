namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static RightJoin<TEnumerator, TOuter, TInner, TKey, TResult> RightJoin<TEnumerator, TOuter, TInner, TKey, TResult>(in this ValueEnumerable<TEnumerator, TSource> source, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter?, TInner, TResult> resultSelector)
            where TEnumerator : struct, IValueEnumerator<TOuter>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(source, inner, outerKeySelector, innerKeySelector, resultSelector, null);

        public static RightJoin<TEnumerator, TOuter, TInner, TKey, TResult> RightJoin<TEnumerator, TOuter, TInner, TKey, TResult>(in this ValueEnumerable<TEnumerator, TSource> source, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter?, TInner, TResult> resultSelector, IEqualityComparer<TKey> comparer)
            where TEnumerator : struct, IValueEnumerator<TOuter>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(source, inner, outerKeySelector, innerKeySelector, resultSelector, comparer);

    }
}

namespace ZLinq.Linq
{
    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
#if NET9_0_OR_GREATER
    public ref
#else
    public
#endif
    struct RightJoin<TEnumerator, TOuter, TInner, TKey, TResult>(TEnumerator source, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter?, TInner, TResult> resultSelector, IEqualityComparer<TKey>? comparer)
        : IValueEnumerator<TResult>
        where TEnumerator : struct, IValueEnumerator<TOuter>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerator source = source;
        FromEnumerable<TInner> innerEnumerable;

        Lookup<TKey, TOuter>? outerLookup;
        Grouping<TKey, TOuter>? currentGroup;
        int currentGroupIndex;
        TInner currentInner = default!;

        public ValueEnumerator<RightJoin<TEnumerator, TOuter, TInner, TKey, TResult>, TResult> GetEnumerator() => new(this);

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = 0;
            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<TResult> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(Span<TResult> dest) => false;

        public bool TryGetNext(out TResult current)
        {
            if (outerLookup == null)
            {
                outerLookup = Lookup.CreateForJoin(source, outerKeySelector, comparer);
                innerEnumerable = inner.AsValueEnumerable();
            }

        // iterating group
        ITERATE:
            if (currentGroup != null)
            {
                if (currentGroupIndex < currentGroup.Count)
                {
                    current = resultSelector(currentGroup[currentGroupIndex], currentInner);
                    currentGroupIndex++;
                    return true;
                }
                else
                {
                    currentGroup = null;
                }
            }

            while (innerEnumerable.TryGetNext(out var value))
            {
                var key = innerKeySelector(value);
                if (key is null)
                {
                    current = resultSelector(default, value);
                    return true;
                }
                else
                {
                    var group = outerLookup.GetGroup(key);
                    if (group != null)
                    {
                        currentInner = value;
                        currentGroup = group;
                        currentGroupIndex = 0;
                        goto ITERATE;
                    }
                }
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
            innerEnumerable.Dispose();
            source.Dispose();
        }
    }
}
