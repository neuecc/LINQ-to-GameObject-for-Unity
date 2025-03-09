namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static Join<TEnumerable, TOuter, TInner, TKey, TResult> Join<TEnumerable, TOuter, TInner, TKey, TResult>(this TEnumerable source, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
            where TEnumerable : struct, IValueEnumerable<TOuter>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(source, inner, outerKeySelector, innerKeySelector, resultSelector, null);

        public static Join<TEnumerable, TOuter, TInner, TKey, TResult> Join<TEnumerable, TOuter, TInner, TKey, TResult>(this TEnumerable source, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector, IEqualityComparer<TKey> comparer)
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
    struct Join<TEnumerable, TOuter, TInner, TKey, TResult>(TEnumerable source, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector, IEqualityComparer<TKey>? comparer)
        : IValueEnumerable<TResult>
        where TEnumerable : struct, IValueEnumerable<TOuter>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerable source = source;

        Lookup<TKey, TInner>? innerLookup;
        Grouping<TKey, TInner>? currentGroup;
        int currentGroupIndex;
        TOuter currentOuter = default!;

        public ValueEnumerator<Join<TEnumerable, TOuter, TInner, TKey, TResult>, TResult> GetEnumerator() => new(this);

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
                innerLookup = CreateInnerLookup();
            }

            if (innerLookup.Count == 0)
            {
                goto END;
            }

        // iterating group
        ITERATE:
            if (currentGroup != null)
            {
                if (currentGroupIndex < currentGroup.Count)
                {
                    current = resultSelector(currentOuter, currentGroup[currentGroupIndex]);
                    currentGroupIndex++;
                    return true;
                }
                else
                {
                    currentGroup = null;
                }
            }

            while (source.TryGetNext(out var value))
            {
                var key = outerKeySelector(value);
                if (key is not null) // Enumerable.Join ignores null keys
                {
                    var group = innerLookup.GetGroup(key);
                    if (group != null)
                    {
                        currentOuter = value;
                        currentGroup = group;
                        currentGroupIndex = 0;
                        goto ITERATE;
                    }
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

        Lookup<TKey, TInner> CreateInnerLookup()
        {
            // Join ignores null keys so remove it before insert.
            var lookupBuilder = new LookupBuilder<TKey, TInner>(comparer ?? EqualityComparer<TKey>.Default);

            using (var source = inner.AsValueEnumerable())
            {
                if (source.TryGetSpan(out var span))
                {
                    foreach (var item in span)
                    {
                        var key = innerKeySelector(item);
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
                        var key = innerKeySelector(item);
                        if (key is not null)
                        {
                            lookupBuilder.Add(key, item);
                        }
                    }
                }

                return lookupBuilder.BuildAndClear();
            }
        }
    }
}
