namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static ValueEnumerable<GroupBy<TEnumerator, TSource, TKey>, IGrouping<TKey, TSource>> GroupBy<TEnumerator, TSource, TKey>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TKey> keySelector)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(new(source.Enumerator, keySelector, null!));

        public static ValueEnumerable<GroupBy<TEnumerator, TSource, TKey>, IGrouping<TKey, TSource>> GroupBy<TEnumerator, TSource, TKey>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
                    , allows ref struct
#endif
            => new(new(source.Enumerator, keySelector, comparer));

        public static ValueEnumerable<GroupBy2<TEnumerator, TSource, TKey, TElement>, IGrouping<TKey, TElement>> GroupBy<TEnumerator, TSource, TKey, TElement>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
                    , allows ref struct
#endif
            => new(new(source.Enumerator, keySelector, elementSelector, null));

        public static ValueEnumerable<GroupBy2<TEnumerator, TSource, TKey, TElement>, IGrouping<TKey, TElement>> GroupBy<TEnumerator, TSource, TKey, TElement>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
                    , allows ref struct
#endif
            => new(new(source.Enumerator, keySelector, elementSelector, comparer));

        public static ValueEnumerable<GroupBy3<TEnumerator, TSource, TKey, TResult>, TResult> GroupBy<TEnumerator, TSource, TKey, TResult>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TKey> keySelector, Func<TKey, IEnumerable<TSource>, TResult> resultSelector)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
                    , allows ref struct
#endif
            => new(new(source.Enumerator, keySelector, resultSelector, null));

        public static ValueEnumerable<GroupBy3<TEnumerator, TSource, TKey, TResult>, TResult> GroupBy<TEnumerator, TSource, TKey, TResult>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TKey> keySelector, Func<TKey, IEnumerable<TSource>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
                    , allows ref struct
#endif
            => new(new(source.Enumerator, keySelector, resultSelector, comparer));

        public static ValueEnumerable<GroupBy4<TEnumerator, TSource, TKey, TElement, TResult>, TResult> GroupBy<TEnumerator, TSource, TKey, TElement, TResult>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
                    , allows ref struct
#endif
            => new(new(source.Enumerator, keySelector, elementSelector, resultSelector, null));

        public static ValueEnumerable<GroupBy4<TEnumerator, TSource, TKey, TElement, TResult>, TResult> GroupBy<TEnumerator, TSource, TKey, TElement, TResult>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
            where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
                    , allows ref struct
#endif
            => new(new(source.Enumerator, keySelector, elementSelector, resultSelector, comparer));

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

        public bool TryCopyTo(Span<IGrouping<TKey, TSource>> dest, int offset) => false;

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

        public bool TryCopyTo(Span<IGrouping<TKey, TElement>> destination, int offset) => false;

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

        public bool TryCopyTo(Span<TResult> destination, int offset)
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

        public bool TryCopyTo(Span<TResult> destination, int offset)
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
