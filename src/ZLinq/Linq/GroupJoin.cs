namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static GroupJoinValueEnumerable<TEnumerable, TOuter, TInner, TKey, TResult> GroupJoin<TEnumerable, TOuter, TInner, TKey, TResult>(this TEnumerable source, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
            where TEnumerable : struct, IValueEnumerable<TOuter>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(source, inner, outerKeySelector, innerKeySelector, resultSelector);

        public static GroupJoinValueEnumerable2<TEnumerable, TOuter, TInner, TKey, TResult> GroupJoin<TEnumerable, TOuter, TInner, TKey, TResult>(this TEnumerable source, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
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
    struct GroupJoinValueEnumerable<TEnumerable, TOuter, TInner, TKey, TResult>(TEnumerable source, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
        : IValueEnumerable<TResult>
        where TEnumerable : struct, IValueEnumerable<TOuter>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerable source = source;

        public ValueEnumerator<GroupJoinValueEnumerable<TEnumerable, TOuter, TInner, TKey, TResult>, TResult> GetEnumerator() => new(this);

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
    struct GroupJoinValueEnumerable2<TEnumerable, TOuter, TInner, TKey, TResult>(TEnumerable source, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
        : IValueEnumerable<TResult>
        where TEnumerable : struct, IValueEnumerable<TOuter>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerable source = source;

        public ValueEnumerator<GroupJoinValueEnumerable2<TEnumerable, TOuter, TInner, TKey, TResult>, TResult> GetEnumerator() => new(this);

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
