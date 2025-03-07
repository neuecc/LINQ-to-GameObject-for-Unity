namespace ZLinq
{
    partial class ValueEnumerableExtensions
    {
        public static Distinct<TEnumerable, TSource> Distinct<TEnumerable, TSource>(this TEnumerable source)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(source, null!);

        public static Distinct<TEnumerable, TSource> Distinct<TEnumerable, TSource>(this TEnumerable source, IEqualityComparer<TSource> comparer)
            where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
            => new(source, comparer);

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
    struct Distinct<TEnumerable, TSource>(TEnumerable source, IEqualityComparer<TSource>? comparer)
        : IValueEnumerable<TSource>
        where TEnumerable : struct, IValueEnumerable<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        TEnumerable source = source;
        HashSet<TSource>? set;

        public ValueEnumerator<Distinct<TEnumerable, TSource>, TSource> GetEnumerator() => new(this);

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

        public bool TryCopyTo(Span<TSource> dest)
        {
            return false;
        }

        public bool TryGetNext(out TSource current)
        {
            if (set == null)
            {
                set = new HashSet<TSource>(comparer ?? EqualityComparer<TSource>.Default);
            }

            if (source.TryGetNext(out var value) && set.Add(value))
            {
                current = value;
                return true;
            }

            Unsafe.SkipInit(out current);
            return false;
        }

        public void Dispose()
        {
            source.Dispose();
        }
    }

}
