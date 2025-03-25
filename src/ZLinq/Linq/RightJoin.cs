namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static ValueEnumerable<RightJoin<TEnumerator, TEnumerator2, TOuter, TInner, TKey, TResult>, TResult> RightJoin<TEnumerator, TEnumerator2, TOuter, TInner, TKey, TResult>(this ValueEnumerable<TEnumerator, TOuter> source, ValueEnumerable<TEnumerator2, TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter?, TInner, TResult> resultSelector)
            where TEnumerator : struct, IValueEnumerator<TOuter>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            where TEnumerator2 : struct, IValueEnumerator<TInner>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, inner.Enumerator, Throws.IfNull(outerKeySelector), Throws.IfNull(innerKeySelector), Throws.IfNull(resultSelector), null));

        public static ValueEnumerable<RightJoin<TEnumerator, TEnumerator2, TOuter, TInner, TKey, TResult>, TResult> RightJoin<TEnumerator, TEnumerator2, TOuter, TInner, TKey, TResult>(this ValueEnumerable<TEnumerator, TOuter> source, ValueEnumerable<TEnumerator2, TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter?, TInner, TResult> resultSelector, IEqualityComparer<TKey>? comparer)
            where TEnumerator : struct, IValueEnumerator<TOuter>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            where TEnumerator2 : struct, IValueEnumerator<TInner>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, inner.Enumerator, Throws.IfNull(outerKeySelector), Throws.IfNull(innerKeySelector), Throws.IfNull(resultSelector), comparer));


        public static ValueEnumerable<RightJoin<TEnumerator, FromEnumerable<TInner>, TOuter, TInner, TKey, TResult>, TResult> RightJoin<TEnumerator, TOuter, TInner, TKey, TResult>(this ValueEnumerable<TEnumerator, TOuter> source, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter?, TInner, TResult> resultSelector)
    where TEnumerator : struct, IValueEnumerator<TOuter>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
    => new(new(source.Enumerator, Throws.IfNull(inner).AsValueEnumerable().Enumerator, Throws.IfNull(outerKeySelector), Throws.IfNull(innerKeySelector), Throws.IfNull(resultSelector), null));

        public static ValueEnumerable<RightJoin<TEnumerator, FromEnumerable<TInner>, TOuter, TInner, TKey, TResult>, TResult> RightJoin<TEnumerator, TOuter, TInner, TKey, TResult>(this ValueEnumerable<TEnumerator, TOuter> source, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter?, TInner, TResult> resultSelector, IEqualityComparer<TKey>? comparer)
            where TEnumerator : struct, IValueEnumerator<TOuter>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, Throws.IfNull(inner).AsValueEnumerable().Enumerator, Throws.IfNull(outerKeySelector), Throws.IfNull(innerKeySelector), Throws.IfNull(resultSelector), comparer));
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
    struct RightJoin<TEnumerator, TEnumerator2, TOuter, TInner, TKey, TResult>(TEnumerator source, TEnumerator2 inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter?, TInner, TResult> resultSelector, IEqualityComparer<TKey>? comparer)
        : IValueEnumerator<TResult>
            where TEnumerator : struct, IValueEnumerator<TOuter>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            where TEnumerator2 : struct, IValueEnumerator<TInner>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
    {
        TEnumerator source = source;
        TEnumerator2 inner = inner;

        Lookup<TKey, TOuter>? outerLookup;
        Grouping<TKey, TOuter>? currentGroup;
        int currentGroupIndex;
        TInner currentInner = default!;

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

        public bool TryCopyTo(Span<TResult> destination, Index offset) => false;

        public bool TryGetNext(out TResult current)
        {
            if (outerLookup == null)
            {
                try
                {
                    outerLookup = Lookup.CreateForJoin(ref source, outerKeySelector, comparer);
                }
                finally
                {
                    source.Dispose();
                }
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

            while (inner.TryGetNext(out var value))
            {
                var key = innerKeySelector(value);
                var group = outerLookup.GetGroup(key);
                if (group != null)
                {
                    currentInner = value;
                    currentGroup = group;
                    currentGroupIndex = 0;
                    goto ITERATE;
                }
                else
                {
                    // The difference with Join is how nulls are handled.
                    current = resultSelector(default, value);
                    return true;
                }
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
            inner.Dispose();
            source.Dispose();
        }
    }
}
