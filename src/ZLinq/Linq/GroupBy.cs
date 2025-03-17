namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static GroupBy<TEnumerator, TSource, TKey> GroupBy<TEnumerator, TSource, TKey>(in this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TKey> keySelector)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(source, keySelector, null!);

        public static GroupBy<TEnumerator, TSource, TKey> GroupBy<TEnumerator, TSource, TKey>(in this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
                    , allows ref struct
#endif
            => new(source, keySelector, comparer);

        public static GroupBy2<TEnumerator, TSource, TKey, TElement> GroupBy<TEnumerator, TSource, TKey, TElement>(in this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
                    , allows ref struct
#endif
            => new(source, keySelector, elementSelector, null);

        public static GroupBy2<TEnumerator, TSource, TKey, TElement> GroupBy<TEnumerator, TSource, TKey, TElement>(in this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
                    , allows ref struct
#endif
            => new(source, keySelector, elementSelector, comparer);

        public static GroupBy3<TEnumerator, TSource, TKey, TResult> GroupBy<TEnumerator, TSource, TKey, TResult>(in this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TKey> keySelector, Func<TKey, IEnumerable<TSource>, TResult> resultSelector)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
                    , allows ref struct
#endif
            => new(source, keySelector, resultSelector, null);

        public static GroupBy3<TEnumerator, TSource, TKey, TResult> GroupBy<TEnumerator, TSource, TKey, TResult>(in this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TKey> keySelector, Func<TKey, IEnumerable<TSource>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
                    , allows ref struct
#endif
            => new(source, keySelector, resultSelector, comparer);

        public static GroupBy4<TEnumerator, TSource, TKey, TElement, TResult> GroupBy<TEnumerator, TSource, TKey, TElement, TResult>(in this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
                    , allows ref struct
#endif
            => new(source, keySelector, elementSelector, resultSelector, null);

        public static GroupBy4<TEnumerator, TSource, TKey, TElement, TResult> GroupBy<TEnumerator, TSource, TKey, TElement, TResult>(in this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
                    , allows ref struct
#endif
            => new(source, keySelector, elementSelector, resultSelector, comparer);

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
    struct GroupBy<TEnumerator, TSource, TKey>(TEnumerator source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        : IValueEnumerator<IGrouping<TKey, TSource>>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerator source = source;
        bool init;
        Grouping<TKey, TSource>? rootGrouping;
        Grouping<TKey, TSource>? currentGrouping;

        public ValueEnumerator<GroupBy<TEnumerator, TSource, TKey>, IGrouping<TKey, TSource>> GetEnumerator() => new(this);

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = 0;
            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<IGrouping<TKey, TSource>> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(Span<IGrouping<TKey, TSource>> dest) => false;

        public bool TryGetNext(out IGrouping<TKey, TSource> current)
        {
            if (!init)
            {
                init = true;
                rootGrouping = BuildRoot();
                if (rootGrouping != null)
                {
                    current = rootGrouping;
                    currentGrouping = rootGrouping;
                    return true;
                }
            }

            currentGrouping = currentGrouping?.NextGroupInAddOrder;

            if (currentGrouping == null || currentGrouping == rootGrouping)
            {
                current = default!;
                return false;
            }
            else
            {
                current = currentGrouping;
                return true;
            }
        }

        public void Dispose()
        {
            source.Dispose();
        }

        Grouping<TKey, TSource>? BuildRoot()
        {
            var lookupBuilder = new LookupBuilder<TKey, TSource>(comparer ?? EqualityComparer<TKey>.Default);

            if (source.TryGetSpan(out var span))
            {
                foreach (var item in span)
                {
                    lookupBuilder.Add(keySelector(item), item);
                }
            }
            else
            {
                using (source)
                {
                    while (source.TryGetNext(out var item))
                    {
                        lookupBuilder.Add(keySelector(item), item);
                    }
                }
            }

            return lookupBuilder.GetRootGroupAndClear();
        }
    }

    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
#if NET9_0_OR_GREATER
    public ref
#else
    public
#endif
    struct GroupBy2<TEnumerator, TSource, TKey, TElement>(TEnumerator source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey>? comparer)
        : IValueEnumerator<IGrouping<TKey, TElement>>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerator source = source;
        bool init;
        Grouping<TKey, TElement>? rootGrouping;
        Grouping<TKey, TElement>? currentGrouping;

        public ValueEnumerator<GroupBy2<TEnumerator, TSource, TKey, TElement>, IGrouping<TKey, TElement>> GetEnumerator() => new(this);

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = 0;
            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<IGrouping<TKey, TElement>> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(Span<IGrouping<TKey, TElement>> destination) => false;

        public bool TryGetNext(out IGrouping<TKey, TElement> current)
        {
            if (!init)
            {
                init = true;
                rootGrouping = BuildRoot();
                if (rootGrouping != null)
                {
                    current = rootGrouping;
                    currentGrouping = rootGrouping;
                    return true;
                }
            }

            currentGrouping = currentGrouping?.NextGroupInAddOrder;

            if (currentGrouping == null || currentGrouping == rootGrouping)
            {
                current = default!;
                return false;
            }
            else
            {
                current = currentGrouping;
                return true;
            }
        }

        public void Dispose()
        {
            source.Dispose();
        }

        Grouping<TKey, TElement>? BuildRoot()
        {
            var lookupBuilder = new LookupBuilder<TKey, TElement>(comparer ?? EqualityComparer<TKey>.Default);

            if (source.TryGetSpan(out var span))
            {
                foreach (var item in span)
                {
                    lookupBuilder.Add(keySelector(item), elementSelector(item));
                }
            }
            else
            {
                using (source)
                {
                    while (source.TryGetNext(out var item))
                    {
                        lookupBuilder.Add(keySelector(item), elementSelector(item));
                    }
                }
            }

            return lookupBuilder.GetRootGroupAndClear();
        }
    }

    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
#if NET9_0_OR_GREATER
    public ref
#else
    public
#endif
    struct GroupBy3<TEnumerator, TSource, TKey, TResult>(TEnumerator source, Func<TSource, TKey> keySelector, Func<TKey, IEnumerable<TSource>, TResult> resultSelector, IEqualityComparer<TKey>? comparer)
        : IValueEnumerator<TResult>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerator source = source;
        bool init;
        Grouping<TKey, TSource>? rootGrouping;
        Grouping<TKey, TSource>? currentGrouping;

        public ValueEnumerator<GroupBy3<TEnumerator, TSource, TKey, TResult>, TResult> GetEnumerator() => new(this);

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

        public bool TryCopyTo(Span<TResult> destination)
        {
            return false;
        }

        public bool TryGetNext(out TResult current)
        {
            if (!init)
            {
                init = true;
                rootGrouping = BuildRoot();
                if (rootGrouping != null)
                {
                    current = resultSelector(rootGrouping.Key, rootGrouping);
                    currentGrouping = rootGrouping;
                    return true;
                }
            }

            currentGrouping = currentGrouping?.NextGroupInAddOrder;

            if (currentGrouping == null || currentGrouping == rootGrouping)
            {
                current = default!;
                return false;
            }
            else
            {
                current = resultSelector(currentGrouping.Key, currentGrouping);
                return true;
            }
        }

        public void Dispose()
        {
            source.Dispose();
        }

        Grouping<TKey, TSource>? BuildRoot()
        {
            var lookupBuilder = new LookupBuilder<TKey, TSource>(comparer ?? EqualityComparer<TKey>.Default);

            if (source.TryGetSpan(out var span))
            {
                foreach (var item in span)
                {
                    lookupBuilder.Add(keySelector(item), item);
                }
            }
            else
            {
                using (source)
                {
                    while (source.TryGetNext(out var item))
                    {
                        lookupBuilder.Add(keySelector(item), item);
                    }
                }
            }

            return lookupBuilder.GetRootGroupAndClear();
        }
    }

    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
#if NET9_0_OR_GREATER
    public ref
#else
    public
#endif
    struct GroupBy4<TEnumerator, TSource, TKey, TElement, TResult>(TEnumerator source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector, IEqualityComparer<TKey>? comparer)
        : IValueEnumerator<TResult>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerator source = source;
        bool init;
        Grouping<TKey, TElement>? rootGrouping;
        Grouping<TKey, TElement>? currentGrouping;

        public ValueEnumerator<GroupBy4<TEnumerator, TSource, TKey, TElement, TResult>, TResult> GetEnumerator() => new(this);

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

        public bool TryCopyTo(Span<TResult> destination)
        {
            return false;
        }

        public bool TryGetNext(out TResult current)
        {
            if (!init)
            {
                init = true;
                rootGrouping = BuildRoot();
                if (rootGrouping != null)
                {
                    current = resultSelector(rootGrouping.Key, rootGrouping);
                    currentGrouping = rootGrouping;
                    return true;
                }
            }

            currentGrouping = currentGrouping?.NextGroupInAddOrder;

            if (currentGrouping == null || currentGrouping == rootGrouping)
            {
                current = default!;
                return false;
            }
            else
            {
                current = resultSelector(currentGrouping.Key, currentGrouping);
                return true;
            }
        }

        public void Dispose()
        {
            source.Dispose();
        }

        Grouping<TKey, TElement>? BuildRoot()
        {
            var lookupBuilder = new LookupBuilder<TKey, TElement>(comparer ?? EqualityComparer<TKey>.Default);

            if (source.TryGetSpan(out var span))
            {
                foreach (var item in span)
                {
                    lookupBuilder.Add(keySelector(item), elementSelector(item));
                }
            }
            else
            {
                using (source)
                {
                    while (source.TryGetNext(out var item))
                    {
                        lookupBuilder.Add(keySelector(item), elementSelector(item));
                    }
                }
            }

            return lookupBuilder.GetRootGroupAndClear();
        }
    }
}
