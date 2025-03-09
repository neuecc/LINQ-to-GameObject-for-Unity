namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static GroupJoin<TEnumerable, TOuter, TInner, TKey, TResult> GroupJoin<TEnumerable, TOuter, TInner, TKey, TResult>(this TEnumerable source, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
            where TEnumerable : struct, IValueEnumerable<TOuter>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(source, inner, outerKeySelector, innerKeySelector, resultSelector, null);

        public static GroupJoin<TEnumerable, TOuter, TInner, TKey, TResult> GroupJoin<TEnumerable, TOuter, TInner, TKey, TResult>(this TEnumerable source, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
            where TEnumerable : struct, IValueEnumerable<TOuter>
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
    struct GroupJoin<TEnumerable, TOuter, TInner, TKey, TResult>(TEnumerable source, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector, IEqualityComparer<TKey>? comparer)
        : IValueEnumerable<TResult>
        where TEnumerable : struct, IValueEnumerable<TOuter>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerable source = source;

        Lookup<TKey, TInner>? innerLookup;

        public ValueEnumerator<GroupJoin<TEnumerable, TOuter, TInner, TKey, TResult>, TResult> GetEnumerator() => new(this);

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
