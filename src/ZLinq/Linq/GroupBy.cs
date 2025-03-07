namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static GroupBy<TEnumerable, TSource, TKey> GroupBy<TEnumerable, TSource, TKey>(this TEnumerable source, Func<TSource, TKey> keySelector)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(source, keySelector, null!);

        public static GroupBy<TEnumerable, TSource, TKey> GroupBy<TEnumerable, TSource, TKey>(this TEnumerable source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
                    , allows ref struct
#endif
            => new(source, keySelector, comparer);

        public static GroupBy2<TEnumerable, TSource, TKey, TElement> GroupBy<TEnumerable, TSource, TKey, TElement>(this TEnumerable source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
                    , allows ref struct
#endif
            => new(source, keySelector, elementSelector, null);

        public static GroupBy2<TEnumerable, TSource, TKey, TElement> GroupBy<TEnumerable, TSource, TKey, TElement>(this TEnumerable source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
                    , allows ref struct
#endif
            => new(source, keySelector, elementSelector, comparer);

        public static GroupBy3<TEnumerable, TSource, TKey, TResult> GroupBy<TEnumerable, TSource, TKey, TResult>(this TEnumerable source, Func<TSource, TKey> keySelector, Func<TKey, IEnumerable<TSource>, TResult> resultSelector)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
                    , allows ref struct
#endif
            => new(source, keySelector, resultSelector, null);

        public static GroupBy3<TEnumerable, TSource, TKey, TResult> GroupBy<TEnumerable, TSource, TKey, TResult>(this TEnumerable source, Func<TSource, TKey> keySelector, Func<TKey, IEnumerable<TSource>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
                    , allows ref struct
#endif
            => new(source, keySelector, resultSelector, comparer);

        public static GroupBy4<TEnumerable, TSource, TKey, TElement, TResult> GroupBy<TEnumerable, TSource, TKey, TElement, TResult>(this TEnumerable source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
                    , allows ref struct
#endif
            => new(source, keySelector, elementSelector, resultSelector, null);

        public static GroupBy4<TEnumerable, TSource, TKey, TElement, TResult> GroupBy<TEnumerable, TSource, TKey, TElement, TResult>(this TEnumerable source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
            where TEnumerable : struct, IValueEnumerable<TSource>
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
    struct GroupBy<TEnumerable, TSource, TKey>(TEnumerable source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        : IValueEnumerable<IGrouping<TKey, TSource>>
        where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerable source = source;
        bool init;
        Grouping<TKey, TSource>? rootGrouping;
        Grouping<TKey, TSource>? currentGrouping;

        public ValueEnumerator<GroupBy<TEnumerable, TSource, TKey>, IGrouping<TKey, TSource>> GetEnumerator() => new(this);

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
    struct GroupBy2<TEnumerable, TSource, TKey, TElement>(TEnumerable source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey>? comparer)
        : IValueEnumerable<IGrouping<TKey, TElement>>
        where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerable source = source;

        public ValueEnumerator<GroupBy2<TEnumerable, TSource, TKey, TElement>, IGrouping<TKey, TElement>> GetEnumerator() => new(this);

        public bool TryGetNonEnumeratedCount(out int count)
        {
            throw new NotImplementedException();
            // return source.TryGetNonEnumeratedCount(count);
            // count = 0;
            // return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<IGrouping<TKey, TElement>> span)
        {
            throw new NotImplementedException();
            // span = default;
            // return false;
        }

        public bool TryCopyTo(Span<IGrouping<TKey, TElement>> destination)
        {
            throw new NotImplementedException();
        }

        public bool TryGetNext(out IGrouping<TKey, TElement> current)
        {
            throw new NotImplementedException();
            // Unsafe.SkipInit(out current);
            // return false;
        }

        public void Dispose()
        {
            source.Dispose();
        }
    }

    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
#if NET9_0_OR_GREATER
    public ref
#else
    public
#endif
    struct GroupBy3<TEnumerable, TSource, TKey, TResult>(TEnumerable source, Func<TSource, TKey> keySelector, Func<TKey, IEnumerable<TSource>, TResult> resultSelector, IEqualityComparer<TKey>? comparer)
        : IValueEnumerable<TResult>
        where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerable source = source;

        public ValueEnumerator<GroupBy3<TEnumerable, TSource, TKey, TResult>, TResult> GetEnumerator() => new(this);

        public bool TryGetNonEnumeratedCount(out int count)
        {
            throw new NotImplementedException();
            // return source.TryGetNonEnumeratedCount(count);
            // count = 0;
            // return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<TResult> span)
        {
            throw new NotImplementedException();
            // span = default;
            // return false;
        }

        public bool TryCopyTo(Span<TResult> destination)
        {
            throw new NotImplementedException();
        }

        public bool TryGetNext(out TResult current)
        {
            throw new NotImplementedException();
            // Unsafe.SkipInit(out current);
            // return false;
        }

        public void Dispose()
        {
            source.Dispose();
        }
    }

    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
#if NET9_0_OR_GREATER
    public ref
#else
    public
#endif
    struct GroupBy4<TEnumerable, TSource, TKey, TElement, TResult>(TEnumerable source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector, IEqualityComparer<TKey>? comparer)
        : IValueEnumerable<TResult>
        where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerable source = source;

        public ValueEnumerator<GroupBy4<TEnumerable, TSource, TKey, TElement, TResult>, TResult> GetEnumerator() => new(this);

        public bool TryGetNonEnumeratedCount(out int count)
        {
            throw new NotImplementedException();
            // return source.TryGetNonEnumeratedCount(count);
            // count = 0;
            // return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<TResult> span)
        {
            throw new NotImplementedException();
            // span = default;
            // return false;
        }

        public bool TryCopyTo(Span<TResult> destination)
        {
            throw new NotImplementedException();
        }

        public bool TryGetNext(out TResult current)
        {
            throw new NotImplementedException();
            // Unsafe.SkipInit(out current);
            // return false;
        }

        public void Dispose()
        {
            source.Dispose();
        }
    }

}
