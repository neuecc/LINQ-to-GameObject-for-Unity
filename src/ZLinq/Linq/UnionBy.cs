namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static UnionBy<TEnumerable, TSource, TKey> UnionBy<TEnumerable, TSource, TKey>(this TEnumerable source, IEnumerable<TSource> second, Func<TSource, TKey> keySelector)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(source, second, keySelector, null);

        public static UnionBy<TEnumerable, TSource, TKey> UnionBy<TEnumerable, TSource, TKey>(this TEnumerable source, IEnumerable<TSource> second, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(source, second, keySelector, comparer);

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
    struct UnionBy<TEnumerable, TSource, TKey>(TEnumerable source, IEnumerable<TSource> second, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer)
        : IValueEnumerable<TSource>
        where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerable source = source;
        HashSet<TKey>? set;
        IEnumerator<TSource>? secondEnumerator;
        byte state = 0;

        public ValueEnumerator<UnionBy<TEnumerable, TSource, TKey>, TSource> GetEnumerator() => new(this);

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = 0;
            return false;
        }

        public bool TryGetSpan(out ReadOnlySpan<TSource> span)
        {
            span = default;
            return false;
        }

        public bool TryCopyTo(Span<TSource> dest) => false;

        public bool TryGetNext(out TSource current)
        {
            if (state == 0)
            {
                set = new HashSet<TKey>(comparer ?? EqualityComparer<TKey>.Default);
                state = 1;
            }

            if (state == 1)
            {
                while (source.TryGetNext(out var value) && set!.Add(keySelector(value)))
                {
                    current = value;
                    return true;
                }
                state = 2;
            }

            if (state == 2)
            {
                if (secondEnumerator is null)
                {
                    secondEnumerator = second.GetEnumerator();
                }

                while (secondEnumerator.MoveNext())
                {
                    var v = secondEnumerator.Current;
                    if (set!.Add(keySelector(v)))
                    {
                        current = v;
                        return true;
                    }
                }

                state = 3;
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
            state = 3;
            secondEnumerator?.Dispose();
            source.Dispose();
        }
    }

}
