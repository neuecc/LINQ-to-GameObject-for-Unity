namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static GroupJoin<TEnumerator, TOuter, TInner, TKey, TResult> GroupJoin<TEnumerator, TOuter, TInner, TKey, TResult>(in this ValueEnumerable<TEnumerator, TSource> source, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
            where TEnumerator : struct, IValueEnumerator<TOuter>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(source, inner, outerKeySelector, innerKeySelector, resultSelector, null);

        public static GroupJoin<TEnumerator, TOuter, TInner, TKey, TResult> GroupJoin<TEnumerator, TOuter, TInner, TKey, TResult>(in this ValueEnumerable<TEnumerator, TSource> source, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
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
    struct GroupJoin<TEnumerator, TOuter, TInner, TKey, TResult>(TEnumerator source, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector, IEqualityComparer<TKey>? comparer)
        : IValueEnumerator<TResult>
        where TEnumerator : struct, IValueEnumerator<TOuter>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerator source = source;

        Lookup<TKey, TInner>? innerLookup;

        public ValueEnumerator<GroupJoin<TEnumerator, TOuter, TInner, TKey, TResult>, TResult> GetEnumerator() => new(this);

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
            if (innerLookup == null)
            {
                innerLookup = Lookup.CreateForJoin(inner.AsValueEnumerable(), innerKeySelector, comparer);
            }

            if (innerLookup.Count == 0)
            {
                goto END;
            }

            while (source.TryGetNext(out var value))
            {
                var key = outerKeySelector(value);
                // Enumerable.GroupJoin allows null unlike Join
                var group = innerLookup.GetGroup(key);
                if (group != null)
                {
                    current = resultSelector(value, group);
                    return true;
                }
            }

        END:
            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
            source.Dispose();
        }
    }
}
